using Command.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

//TODO: create for(init;condition;iteration) "[command]" command. DONE
//TODO: create method in CyberCommand to set none parameters error. DONE
//TODO: refactoring. DONE
//TODO: clear command. DONE.
//TODO: fix copy of readonly segments. DONE.
//TODO: add console functions to add commands with -f, -d attributes (file, directory)
//TODO: last commands (rAlt | lAlt).

namespace CyberpunkConsoleControl
{
    /// <summary>
    /// Class for executing commands
    /// </summary>
    public static class CommandsManager
    {
        #region Constants

        private const string NOT_FOUND = "Command not found.";

        #endregion

        #region Properties

        /// <summary>
        /// List with types which inherited from <see cref="ICommand"/>.
        /// </summary>
        private static List<Type> assemblyTypes { get; set; }

        #endregion

        #region Constructors

        static CommandsManager()
        {
            UpdateAssemblyTypes();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute command by its type.
        /// </summary>
        /// <param name="command">Command type.</param>
        /// <param name="commandLineText">Line to parse</param>
        /// <param name="console">Console parameter.</param>
        public static void ExecuteCommand(ICommand command, string commandLineText, CyberConsole console)
        {
            command.Action(commandLineText, console);
            console.InsertText(command.PrintInfo());
        }

        /// <summary>
        /// Find and execute command with <paramref name="commandLineText"/> lexic.
        /// </summary>
        /// <param name="commandLineText">Line to parse.</param>
        /// <param name="console">Console parameter.</param>
        public static void ExecuteCommand(string commandLineText, CyberConsole console)
        {
            if (commandLineText.Length > 0)
            {
                List<Type> commandTypes = GetCommandTypes();
                foreach (Type t in commandTypes)
                {
                    ICommand command = (ICommand)Activator.CreateInstance(t);
                    if (command.IsCommandLexic(commandLineText))
                    {
                        ExecuteCommand(command, commandLineText, console);
                        return;
                    }
                }
            }
            console.InsertText(NOT_FOUND, true);
          
        }

        /// <summary>
        /// Get types which are inherited from <see cref="ICommand"/>
        /// </summary>
        public static List<Type> GetCommandTypes()
        {
            Type commandType = typeof(ICommand);
            UpdateAssemblyTypes();
            return assemblyTypes.Where(t => commandType.IsAssignableFrom(t) && t.IsClass)
                .ToList();
        }

        /// <summary>
        /// Update types (for dynamically adding commands to console).
        /// </summary>
        public static void UpdateAssemblyTypes()
        {
            assemblyTypes = Assembly.Load("Commands")//Commands - assembly with standard console commands.
                                    .GetTypes()
                                    .ToList();
            string exportedPath = AppDomain.CurrentDomain.BaseDirectory + "\\ExportedCommands.dll";//ExportedCommands contains all added assemblies and files
            if (File.Exists(exportedPath))
            {
                Assembly exportedAssembly = Assembly.Load(File.ReadAllBytes(exportedPath));
                List<Assembly> addedAssemblies = GetAddedAssemblies(exportedPath);
                addedAssemblies.Add(exportedAssembly);
                assemblyTypes.AddRange(from assembly in addedAssemblies    //mini recursion for getting command types from embedded resources
                                       let typesArray = assembly.GetTypes()
                                       from type in typesArray
                                       select type);
            }
        }

        /// <summary>
        /// Get assemblies from embedded resources.
        /// </summary>
        /// <param name="assemblyPath">Path to assembly with resources.</param>
        private static List<Assembly> GetAddedAssemblies(string assemblyPath)
        {
            Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            List<Stream> manifestStreams = GetManifestStreams(assembly);
            List<Assembly> addedAssemblies = new List<Assembly>();
            foreach (Stream stream in manifestStreams)
                addedAssemblies.Add(Assembly.Load(ReadFully(stream)));
            return addedAssemblies;
        }

        /// <summary>
        /// Get embedded reources streams.
        /// </summary>
        /// <param name="assembly">Assembly to get streams.</param>
        private static List<Stream> GetManifestStreams(Assembly assembly)
        {
            string[] resourcesNames = assembly.GetManifestResourceNames();
            List<Stream> manifestStreams = new List<Stream>();
            foreach (string name in resourcesNames)
                manifestStreams.Add(assembly.GetManifestResourceStream(name));
            return manifestStreams;
        }

        /// <summary>
        /// Convert <see cref="Stream"/> to <see cref="byte[]"/>.
        /// </summary>
        /// <param name="input">Stream to read.</param>
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

        #endregion


    }
}
