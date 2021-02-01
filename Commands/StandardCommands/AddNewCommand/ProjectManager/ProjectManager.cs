using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Converters;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;

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

        public static string AddFiles(params string[] files)
        {
            files[0] = @"C:\Users\HP\Desktop\editorcommand.cs";
            Assembly currentAssembly = AppDomain.CurrentDomain.Load(LIBRARY_NAME);
            AssemblyName[] assemblyNames = currentAssembly.GetReferencedAssemblies();
            List<string> referencedAssemblies = new List<string>();
            foreach (AssemblyName name in assemblyNames)
                referencedAssemblies.AddRange(GetAssemblyPath(name));
            //files = MoveFiles(files, GetPathToMove(files));
            string buildResult = Build(referencedAssemblies.ToArray(), files);
            return buildResult;
        }

        private static string GetPathToMove(string[] files)
        {
            if (files.Length > 0)
            {
                string p = Path.Combine(Assembly.GetExecutingAssembly().Location, "..\\..\\..");
                p = Path.GetFullPath(p);
                if (files.Length > 1)
                    return CreateDirectory(p);
                else if (files[0].Contains(DLL_EXTENSION))
                    return p + EXPORTED_DLLS;
                return p + EXPORTED_FILES;
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
            lastDirectory = Path.GetDirectoryName(lastDirectory);
            int o;
            int lastDir = int.TryParse(lastDirectory, out o)
                ? int.Parse(lastDirectory)
                : -1;
            lastDir++; //next number of directory;
            string newDirPath = Path.Combine(p, lastDir.ToString());
            Directory.CreateDirectory(newDirPath);
            return newDirPath;
        }

        private static List<Assembly> GetAddedAssemblies(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            List<Stream> manifestStreams = GetManifestStreams(assembly);
            List<Assembly> addedAssemblies = new List<Assembly>();
            foreach (Stream stream in manifestStreams)
                addedAssemblies.Add(Assembly.Load(ReadFully(stream)));
            return addedAssemblies;
        }

        private static List<Stream> GetManifestStreams(Assembly assembly)
        {
            string[] resourcesNames = assembly.GetManifestResourceNames();
            List<Stream> manifestStreams = new List<Stream>();
            foreach (string name in resourcesNames)
                manifestStreams.Add(assembly.GetManifestResourceStream(name));
            return manifestStreams;
        }

        private static IEnumerable<string> GetAssemblyPath(AssemblyName name)
        {
            return Assembly.Load(name)
                           .GetFiles()
                           .Select(fs => fs.Name)
                           .Where(fs => fs.Contains(DLL_EXTENSION));
        }



        private static string Build(string[] referencedAssemblies, params string[] files)
        {
            CSharpCodeProvider compiler = new CSharpCodeProvider();
            CompilerParameters parameters = GetCompilerParameters(referencedAssemblies);
            TempFileCollection tfc = new TempFileCollection(Assembly.GetEntryAssembly().Location, false);
            CompilerResults cr = new CompilerResults(tfc);
            cr = compiler.CompileAssemblyFromFile(parameters, files);
            string compileResults = GetResultString(cr);
            return compileResults;
        }

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

        private static string[] MoveFiles(string[] files, string dirPath)
        {
            for(int i = 0; i < files.Length; i++)
            {
                string temp = Path.Combine(dirPath, Path.GetFileName(files[i]));
                File.Move(files[i], temp);
                files[i] = temp;
            }
            return files;
        }

        private static CompilerParameters GetCompilerParameters(string[] referencedAssemblies)
        {
            CompilerParameters parameters = new CompilerParameters();
            parameters.OutputAssembly = "C:\\Users\\HP\\Desktop\\Commands.dll";
            foreach (string assemblyPath in referencedAssemblies)
                parameters.ReferencedAssemblies.Add(assemblyPath);
            AddStandardWPFAssemblies(parameters);
            string codeBase = typeof(System.Windows.Shell.JumpItem).Assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            parameters.ReferencedAssemblies.Add(path);
            parameters.WarningLevel = 3;
            parameters.CompilerOptions = "/target:library /optimize";
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            return parameters;
        }
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
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
