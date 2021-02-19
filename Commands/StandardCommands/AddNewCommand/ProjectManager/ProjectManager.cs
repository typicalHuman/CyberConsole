using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CyberpunkConsoleControl;
using System.Collections.Generic;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using Newtonsoft.Json;

namespace Commands
{
    public static class ProjectManager
    {
        #region Ctor

        static ProjectManager()
        {
            modulesFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\modules.json";
            InitFromJSONData();
        }

        #endregion

        #region Constants

        private const string LIBRARY_NAME = "Commands";

        private const string DLL_EXTENSION = ".dll";
        private const string CS_EXTENSION = ".cs";

        private const string NO_ERRORS_RESULT = "No errors.";
        private const string NO_FILES_ERROR = "No files to build.";
        private const string NO_CODEFILES_ERROR = "No .cs files to build.";
        private const string ONLY_DIRS_ERROR = "Input contains path to file (-d parameter only for directories).";
        private const string NO_DIRECTORIES_ERROR = "No directories to build.";

        private const string EXPORTED_DLLS = @"\ExportedCommands\ExportedDLLs";
        private const string EXPORTED_FILES = @"\ExportedCommands\ExportedFiles";

        private const string DEFAULT_MODULE = "<default>";

        private const string FAILED_BUILD_RESULT = "FAILED!";
        private const string SUCCESS_BUILD_RESULT = "SUCCESS!";

        #endregion

        #region Properties

        #region Private

        private static List<Module> Modules { get; set; } = new List<Module>();

        private static string modulesFilePath { get; set; } = string.Empty;

        #endregion

        #endregion

        #region Methods

        #region Public

        public static List<Module> GetModules()
        {
            return Modules;
        }

        /// <summary>
        /// Build assembly with <paramref name="files"/> (including .dll files).
        /// </summary>
        /// <param name="files">C# files paths.</param>
        /// <returns>Build result.</returns>
        public static string AddFiles(params string[] files)
        {
            //files = new[] { @"C:\Users\HP\Desktop\editorcommand.cs" };
            if (files != null && files.Length > 0)
            {
                string existenceResult = CheckAllFilesExists(files);
                if (existenceResult == NO_ERRORS_RESULT)
                {
                    string[] _dlls = RemoveDLLs(files).ToArray();
                    files = files.Where(file => !file.Contains(DLL_EXTENSION)).ToArray();
                    if (files.Length > 0)
                    {
                        List<string> referencedAssemblies = GetReferencedAssemblies();
                        //files = MoveFiles(files, GetPathToMove(files));
                        string buildResult = Build(referencedAssemblies.ToArray(), _dlls, files);
                        if (buildResult.Contains(SUCCESS_BUILD_RESULT))
                        {
                            Modules.Add(new Module(files, _dlls, $"Module #{Modules.Count + 1}"));
                            SaveJSONData();
                        }
                        return buildResult;
                    }
                    return NO_CODEFILES_ERROR;
                }
                return existenceResult;
            }
            return NO_FILES_ERROR;
        }

        /// <summary>
        /// Build assembly with all files in <paramref name="directories"/>.
        /// </summary>
        /// <param name="directories">Paths to directories</param>
        /// <returns>Build result.</returns>
        public static string AddDirectories(params string[] directories)
        {
            if (directories != null && directories.Length > 0)
            {
                string checkDirsResult = CheckDirectories(directories);
                if (checkDirsResult == NO_ERRORS_RESULT)
                {
                    List<string> files = new List<string>();
                    for (int i = 0; i < directories.Length; i++)
                        files.AddRange(GetDirectoryFiles(directories[i]));
                    if (files.Count == 0)
                        return NO_FILES_ERROR;
                    return AddFiles(files.ToArray());
                }
                return checkDirsResult;
            }
            return NO_DIRECTORIES_ERROR;
        }

        #endregion

        #region Private

        #region AddDirectories Methods


        /// <summary>
        /// Get cs and dll files in directory.
        /// </summary>
        /// <param name="path">Path to directory.</param>
        private static IEnumerable<string> GetDirectoryFiles(string path)
        {
            return Directory
                   .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)//get all files
                   .Where(file => file.ToLower().EndsWith(CS_EXTENSION) || file.ToLower().EndsWith(DLL_EXTENSION));//select .cs and .dll files
        }

        /// <summary>
        /// Check: is paths contains only pointers to directories.
        /// </summary>
        /// <param name="directories">Array to check.</param>
        private static string CheckDirectories(string[] directories)
        {
            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i] == null || directories[i].Length == 0)
                    return $"Path with index {i} is null.";
                else if (directories[i].Contains('.'))//if path contain extension symbol - it means that it's not a directory.
                    return ONLY_DIRS_ERROR;
                else if (!Directory.Exists(directories[i]))
                    return $"Directory with path '{directories[i]}' doesn't exist.";
            }
            return NO_ERRORS_RESULT;
        }

        #endregion

        #region AddFilesMethods
        /// <summary>
        /// Get referenced assemblies of current assembly (.exe file).
        /// </summary>
        /// <returns>List with paths to referenced assemblies.</returns>
        private static List<string> GetReferencedAssemblies()
        {
            Assembly currentAssembly = AppDomain.CurrentDomain.Load(LIBRARY_NAME);
            AssemblyName[] assemblyNames = currentAssembly.GetReferencedAssemblies();
            List<string> referencedAssemblies = new List<string>();
            foreach (AssemblyName name in assemblyNames)
                referencedAssemblies.AddRange(GetAssemblyPath(name));
            return referencedAssemblies;
        }

        /// <summary>
        /// Check is all input files exists.
        /// </summary>
        /// <param name="files">Files to compile.</param>
        /// <returns></returns>
        private static string CheckAllFilesExists(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] == null || files[i].Length == 0)
                    return $"Path with index {i} is null.";
                else if (!File.Exists(files[i]))
                    return $"File with path '{files[i]}' doesn't exist.";
            }
            return NO_ERRORS_RESULT;
        }

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

        /// <summary>
        /// Get Enumerable with dlls.
        /// </summary>
        private static IEnumerable<string> RemoveDLLs(string[] files)
        {
            List<string> _files = files.ToList();
            foreach (string file in files)
                if (file.Contains(DLL_EXTENSION))
                {
                    _files.Remove(file);
                    yield return file;
                }
        }

        #endregion

        #region FilesCopy Methods

        /// <summary>
        /// Copy <paramref name="files"/> to directory with <paramref name="dirPath"/> path.
        /// </summary>
        /// <param name="files">Files to copy.</param>
        /// <param name="dirPath">Final path.</param>
        /// <returns>Array of new paths.</returns>
        private static string[] MoveCopy(string[] files, string dirPath)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string temp = Path.Combine(dirPath, Path.GetFileName(files[i]));
                File.Copy(files[i], temp);
                files[i] = temp;
            }
            return files;
        }

        /// <summary>
        /// Define directory depending on files.
        /// </summary>
        /// <param name="files">Files to copy.</param>
        /// <returns>Path to directory in which files will be copied.</returns>
        private static string GetPathToCopy(string[] files)
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
            files = ConcatFiles(files);
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
                errorString.Insert(errorString.Length, $"\n{FAILED_BUILD_RESULT}");
                return errorString;
            }
            string result = string.Join(" ", cr.Output.Cast<string>().ToArray());
            return result.Insert(result.Length, $"\n{SUCCESS_BUILD_RESULT}");
        }

        /// <summary>
        /// Get compiler parameters with including <paramref name="referencedAssemblies"/>.
        /// </summary>
        /// <param name="referencedAssemblies">Assemblies to include.</param>
        /// <returns><see cref="CompilerParameters"/> value.</returns>
        private static CompilerParameters GetCompilerParameters(string[] referencedAssemblies, string[] dlls = default(string[]))
        {
            CompilerParameters parameters = new CompilerParameters();
            parameters.OutputAssembly = AppDomain.CurrentDomain.BaseDirectory + "\\ExportedCommands.dll";
            AddResources(parameters, referencedAssemblies, dlls);
            parameters.WarningLevel = 3;
            parameters.CompilerOptions = "/target:library /optimize";
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            return parameters;
        }

        /// <summary>
        /// Add references and resources to <paramref name="parameters"/>.
        /// </summary>
        private static void AddResources(CompilerParameters parameters, string[] referencedAssemblies, string[] dlls)
        {
            foreach (string assemblyPath in referencedAssemblies)
                parameters.ReferencedAssemblies.Add(assemblyPath);
            if (dlls != null)
            {
                foreach (string dll in dlls)
                {
                    parameters.ReferencedAssemblies.Add(dll);
                    parameters.EmbeddedResources.Add(dll);
                }
            }
            AddStandardWPFAssemblies(parameters);
            AddPreviousDlls(parameters);
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

        /// <summary>
        /// Add dlls which were included in previous builds.
        /// </summary>
        /// <param name="parameters"></param>
        private static void AddPreviousDlls(CompilerParameters parameters)
        {
            foreach(Module m in Modules)
            {
                foreach (string path in m.DllsPaths)
                    parameters.ReferencedAssemblies.Add(path);
            }
        }

        /// <summary>
        /// Concat new files and files which were in assembly ExportedTypes.
        /// </summary>
        private static string[] ConcatFiles(string[] newFiles)
        {
            List<string> files = newFiles.ToList();
            foreach (Module m in Modules)
            {
                foreach (string path in m.FilesPaths)
                    files.Add(path);
            }
            return files.ToArray();
        }


        #endregion

        #region JSON

        private static void SaveJSONData()
        {
                using (StreamWriter sw = File.CreateText(modulesFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(sw, Modules);
                }
        }

        private static void InitFromJSONData()
        {
            if (File.Exists(modulesFilePath))
            {
                using (StreamReader file = File.OpenText(modulesFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Modules = (List<Module>)serializer.Deserialize(file, typeof(List<Module>));
                }
            }
        }

        #endregion

        #endregion

        #endregion




    }
}
