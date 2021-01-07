using Command.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//TODO: create for(init;condition;iteration) "[command]" command. DONE
//TODO: create method in CyberCommand to set none parameters error. DONE
//TODO: refactoring. DONE
//TODO: clear command. DONE.
//TODO: fix copy of readonly segments. DONE.
//TODO: add console functions to add commands with -f, -d, -fp attributes (file, directory, files_params)

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
                Type commandType = typeof(ICommand);
                List<Type> commandTypes = assemblyTypes.Where(t => commandType.IsAssignableFrom(t) && t.IsClass)
                    .ToList();
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
        /// Update types (for dynamically adding commands to console).
        /// </summary>
        public static void UpdateAssemblyTypes()
        {
            assemblyTypes = Assembly.Load("Commands")
                                    .GetTypes()
                                    .ToList();
        }

        #endregion


    }
}
