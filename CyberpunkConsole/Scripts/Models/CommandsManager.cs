﻿using Command.Interfaces;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CyberpunkConsole.Scripts.Models
{
    public static class CommandsManager
    {
        #region Constants

        private const string NOT_FOUND = "Command not found.";

        #endregion

        #region Properties

        private static List<Type> assemblyTypes { get; set; }

        #endregion

        #region Constructors

        static CommandsManager()
        {
            UpdateAssemblyTypes();
        }

        #endregion

        #region Methods

        public static void ExecuteCommand(ICommand command, string commandLineText)
        {
            commandLineText = Regex.Replace(commandLineText, @"\s+", " ");
            command.Action(commandLineText);
            App.ConsoleVM.InsertText(command.PrintInfo());
        }

        /// <summary>
        /// Find and execute command with <paramref name="commandLineText"/> lexic.
        /// </summary>
        public static void ExecuteCommand(string commandLineText)
        {
            commandLineText = Regex.Replace(commandLineText, @"\s+", " ");
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
                        ExecuteCommand(command, commandLineText);
                        return;
                    }
                }
            }
            App.ConsoleVM.InsertText(NOT_FOUND, true);
        }

        public static void UpdateAssemblyTypes()
        {
            assemblyTypes = Assembly.GetAssembly(typeof(CommandsManager))
                                    .GetTypes()
                                    .ToList();
        }

        #endregion


    }
}
