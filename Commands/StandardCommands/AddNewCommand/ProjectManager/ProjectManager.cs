using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CyberpunkConsoleControl;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Converters;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace Commands.StandardCommands.AddNewCommand.ProjectManager
{
    class ProjectManager
    {
        #region Constants

        private const string LIBRARY_NAME = "Commands";

        private const string DLL_EXTENSION = ".dll";

        private const string EXPORTED_DLLS = @"\ExportedCommands\ExportedDLLs";
        private const string EXPORTED_FILES =@"\ExportedCommands\ExportedFiles";

        #endregion

        #region Methods

        #region Public

        public static string AddFiles(params string[] files)
        {
            files = new[] { @"C:\Users\HP\Desktop\editorcommand.cs", @"C:\Users\HP\Desktop\AddType\Lib\bin\Debug\Lib.dll" };
            string[] dlls  = RemoveDLLs(files).ToArray();
            files = files.Where(file => file.Contains(DLL_EXTENSION)).ToArray();
            Assembly currentAssembly = AppDomain.CurrentDomain.Load(LIBRARY_NAME);
            AssemblyName[] assemblyNames = currentAssembly.GetReferencedAssemblies();
            List<string> referencedAssemblies = new List<string>();
            foreach (AssemblyName name in assemblyNames)
                referencedAssemblies.AddRange(GetAssemblyPath(name));
            //files = MoveFiles(files, GetPathToMove(files));
            string buildResult = Build(referencedAssemblies.ToArray(), files);


            return buildResult;
        }


        #endregion

        #region Private

        /// <summary>
        /// Get assemblies paths by its name.
        /// </summary>
        /// <param name="name">Assembly's name.</param>
        /// <returns>Enumerable of assemblies paths.</returns>
        private static IEnumerable<string> GetAssemblyPath(AssemblyName name)
        {
            return Assembly.Load(name)
                           .GetFiles()
                           .Select(fs => fs.Name)
                           .Where(fs => fs.Contains(DLL_EXTENSION));
        }

        private static IEnumerable<string> RemoveDLLs(string[] files)
        {
            List<string> _files = files.ToList();
            foreach (string file in files)
                if (file.Contains(DLL_EXTENSION))
                {
                    _files.Remove(file);
                    yield return file;
                }
            files = _files.ToArray();
        }


        #region FilesMove Methods

        /// <summary>
        /// Move <paramref name="files"/> to directory with <paramref name="dirPath"/> path.
        /// </summary>
        /// <param name="files">Files to move.</param>
        /// <param name="dirPath">Final path.</param>
        /// <returns>Array of new paths.</returns>
        private static string[] MoveFiles(string[] files, string dirPath)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string temp = Path.Combine(dirPath, Path.GetFileName(files[i]));
                File.Move(files[i], temp);
                files[i] = temp;
            }
            return files;
        }

        /// <summary>
        /// Define directory depending on files.
        /// </summary>
        /// <param name="files">Files to move.</param>
        /// <returns>Path to directory in which files will be moved.</returns>
        private static string GetPathToMove(string[] files)
        {
            if (files.Length > 0)
            {
                string p = Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\..");//for directories back; because (proj_dir\bin\debug)
                p = Path.GetFullPath(p); //combine steps back with the string path, because Path.Combine() doesn't do that.
                if (files.Length > 1)//for separating single files and logical modules
                    return CreateDirectory(p);
                else if (files[0].Contains(DLL_EXTENSION))
                    return p + EXPORTED_DLLS; //here some problem with the Path.Combine()
                return p + EXPORTED_FILES;    //method and I've used + operation;
            }
            return "No files to include.";
        }

        
        /// <summary>
        /// Create directory for multiples files.
        /// </summary>
        /// <param name="p">Default folder path (exported files path).</param>
        /// <returns>Path of new directory.</returns>
        private static string CreateDirectory(string p)
        {
            p = Path.Combine(p, EXPORTED_FILES);
            string lastDirectory = Directory.GetDirectories(p).LastOrDefault();
            lastDirectory = Path.GetDirectoryName(lastDirectory);//directory are named just by serial number
            int o;
            int lastDir = int.TryParse(lastDirectory, out o)
                ? int.Parse(lastDirectory)
                : -1;//if ExportedFiles directory has no directories or if directory name was wrong 
            lastDir++; //next number of directory;
            string newDirPath = p + $@"\{lastDir.ToString()}"; //Path.Combine() won't work in that situtation because we haven't created a directory yet.
            Directory.CreateDirectory(newDirPath);
            return newDirPath;
        }

        #endregion

        #region Build Methods

        /// <summary>
        /// Build project with includeing <paramref name="referencedAssemblies"/>.
        /// </summary>
        /// <param name="referencedAssemblies">Assemblies to include.</param>
        /// <param name="files">Files to build.</param>
        /// <returns>Compile result string.</returns>
        private static string Build(string[] referencedAssemblies, string[] dlls = default(string[]), params string[] files)
        {
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            CompilerParameters parameters = GetCompilerParameters(referencedAssemblies, dlls);
            TempFileCollection tfc = new TempFileCollection(Assembly.GetEntryAssembly().Location, false);
            CompilerResults cr = new CompilerResults(tfc);
            cr = compiler.CompileAssemblyFromFile(parameters, files);
            string compileResults = GetResultString(cr);
            return compileResults;
        }

        /// <summary>
        /// Get result string by <paramref name="cr"/>.
        /// </summary>
        /// <param name="cr">Compiler results value.</param>
        /// <returns>String equivalent of <paramref name="cr"/></returns>
        private static string GetResultString(CompilerResults cr)
        {
            if (cr.Errors.Count > 0)
            {
                string errorString = string.Empty;
                foreach (CompilerError ce in cr.Errors)
                    errorString += $"{ce.ErrorNumber}:{ce.ErrorText}\n";
                return errorString;
            }
            return string.Join(" ", cr.Output.Cast<string>().ToArray());
        }

        /// <summary>
        /// Get compiler parameters with including <paramref name="referencedAssemblies"/>.
        /// </summary>
        /// <param name="referencedAssemblies">Assemblies to include.</param>
        /// <returns><see cref="CompilerParameters"/> value.</returns>
        private static CompilerParameters GetCompilerParameters(string[] referencedAssemblies, string[] dlls = default(string[]))
        {
            CompilerParameters parameters = new CompilerParameters();
            parameters.OutputAssembly = "C:\\Users\\HP\\Desktop\\Commands.dll";
            foreach (string assemblyPath in referencedAssemblies)
                parameters.ReferencedAssemblies.Add(assemblyPath);
            foreach (string dll in dlls)
            {
                parameters.ReferencedAssemblies.Add(dll);
                parameters.EmbeddedResources.Add(dll);
            }
            AddStandardWPFAssemblies(parameters);
            parameters.WarningLevel = 3;
            parameters.CompilerOptions = "/target:library /optimize";
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            return parameters;
        }
        /// <summary>
        /// To work with <see cref="CyberConsole"/> assembly needs some wpf libraries.
        /// </summary>
        private static void AddStandardWPFAssemblies(CompilerParameters cp)
        {
            string codeBase = typeof(System.Windows.Shell.JumpItem).Assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            cp.ReferencedAssemblies.Add(path);
            codeBase = typeof(System.Windows.Media.Converters.MatrixValueSerializer).Assembly.CodeBase;
            uri = new UriBuilder(codeBase);
            path = Uri.UnescapeDataString(uri.Path);
            cp.ReferencedAssemblies.Add(path);
        }


        #endregion

        #endregion

        #endregion

   


    }

    static public class FileUtil
    {
        [StructLayout(LayoutKind.Sequential)]
        struct RM_UNIQUE_PROCESS
        {
            public int dwProcessId;
            public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
        }

        const int RmRebootReasonNone = 0;
        const int CCH_RM_MAX_APP_NAME = 255;
        const int CCH_RM_MAX_SVC_NAME = 63;

        enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct RM_PROCESS_INFO
        {
            public RM_UNIQUE_PROCESS Process;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)]
            public string strAppName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)]
            public string strServiceShortName;

            public RM_APP_TYPE ApplicationType;
            public uint AppStatus;
            public uint TSSessionId;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bRestartable;
        }

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        static extern int RmRegisterResources(uint pSessionHandle,
                                              UInt32 nFiles,
                                              string[] rgsFilenames,
                                              UInt32 nApplications,
                                              [In] RM_UNIQUE_PROCESS[] rgApplications,
                                              UInt32 nServices,
                                              string[] rgsServiceNames);

        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
        static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        [DllImport("rstrtmgr.dll")]
        static extern int RmEndSession(uint pSessionHandle);

        [DllImport("rstrtmgr.dll")]
        static extern int RmGetList(uint dwSessionHandle,
                                    out uint pnProcInfoNeeded,
                                    ref uint pnProcInfo,
                                    [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
                                    ref uint lpdwRebootReasons);

        /// <summary>
        /// Find out what process(es) have a lock on the specified file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <returns>Processes locking the file</returns>
        /// <remarks>See also:
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
        /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
        /// 
        /// </remarks>
        static public List<Process> WhoIsLocking(string path)
        {
            uint handle;
            string key = Guid.NewGuid().ToString();
            List<Process> processes = new List<Process>();

            int res = RmStartSession(out handle, 0, key);
            if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

            try
            {
                const int ERROR_MORE_DATA = 234;
                uint pnProcInfoNeeded = 0,
                     pnProcInfo = 0,
                     lpdwRebootReasons = RmRebootReasonNone;

                string[] resources = new string[] { path }; // Just checking on one resource.

                res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                if (res != 0) throw new Exception("Could not register resource.");

                //Note: there's a race condition here -- the first call to RmGetList() returns
                //      the total number of process. However, when we call RmGetList() again to get
                //      the actual processes this number may have increased.
                res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

                if (res == ERROR_MORE_DATA)
                {
                    // Create an array to store the process results
                    RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                    pnProcInfo = pnProcInfoNeeded;

                    // Get the list
                    res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                    if (res == 0)
                    {
                        processes = new List<Process>((int)pnProcInfo);

                        // Enumerate all of the results and add them to the 
                        // list to be returned
                        for (int i = 0; i < pnProcInfo; i++)
                        {
                            try
                            {
                                processes.Add(Process.GetProcessById(processInfo[i].Process.dwProcessId));
                            }
                            // catch the error -- in case the process is no longer running
                            catch (ArgumentException) { }
                        }
                    }
                    else throw new Exception("Could not list processes locking resource.");
                }
                else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
            }
            finally
            {
                RmEndSession(handle);
            }

            return processes;
        }
    }
}
