<a href="https://aimeos.org/">
    <img src="https://raw.githubusercontent.com/typicalHuman/CyberConsole/master/resources/WIndows-Terminal-icon.png" alt="Console logo" title="CyberpunkConsole" align="right"
    height="100"/>
</a>

# CyberpunkConsole

![](https://img.shields.io/badge/.NET-v.4.6.1-green.svg?style=flat-square&logo=nuget&logoColor=blue&link=https://www.nuget.org/packages/CyberpunkConsole)

## Info
The CyberpunkConsole is based on [AvalonEdit](http://avalonedit.net/). It has several standard commands for primitive operations, but the primary feature of the console is - **dynamically changing commands composition**. You can add and delete your own commands with the **add_cmnd** and **rm_cmnd** commands. The commands build includes all *WPF* references + references to CyberpunkConsole libraries. You also can add your dlls to the project and it will be concluded in the **Module**.

### Template
```csharp
    class MyCommand : ConsoleCommand
    {
        /// <param name="commandLineText">Line to process (for getting parameters, attributes and errors if any)</param>
        /// <param name="args">First argument is CyberConsole variable.</param>
        public override void Action(string commandLineText, params object[] args)
        {
            //do some stuff here
                       Message = "This message will display on the screen after action " +
                "(you can change display logic by overriding PrintInfo()";
        }

        /// <summary>
        /// The root of the command.
        /// </summary>
        public override string Spelling { get; protected set; } = "execute_my_cmnd";

        /// <summary>
        /// Array with standard attributes which are supported by command.
        /// </summary>
        public override IAttrib[] StandardAttributes { get; protected set; }
        public override IAttrib[] CurrentAttributes { get; set; }
        /// <summary>
        /// Array with standard parameters which are supported by command. 
        /// (the difference between parameter and attribute is that attribute is constant value 
        /// and parameter is dynamic value which you can parse with the regex functions).
        /// </summary>
        public override IParameter[] StandardParameters { get; protected set; }
        public override IParameter[] Parameters { get; set; }
    }
```
 
## [Documentation](https://github.com/typicalHuman/CyberConsole/wiki)

## [Nuget](https://www.nuget.org/packages/CyberpunkConsole)
