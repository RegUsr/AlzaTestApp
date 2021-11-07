// ------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Peter Tomciak">
//   Copyright (c) 2021 by Peter Tomciak
// </copyright>
// <summary>
//   Defines the Log type.
// </summary>
// ------------------------------------------------------------------------------------------------
namespace AlzaTestApp.Helpers
{
    using NLog;
    using NLog.Common;
    using NLog.Config;
    using NLog.Targets;
    using System.Diagnostics;

    /// <summary>
    /// A small extension class for NLog (open source free logging platform).
    /// Obecně NLog nabízí několik úrovní logování, které jsou zde
    /// vyvedeny do příslušných class methods.
    /// Do log.řádků se ukládá kromě úrovně logování i název metody,
    /// ze které bylo zalogováno.
    /// </summary>
    public static class Log
    {
        #region Fields

        /// <summary>
        /// Formátovaná délka názvu logované caller metody konkrétního testCase (chars).
        /// Znaky nad definovanou délku budou ořezány.
        /// </summary>
        private static int _callerMethodLength = 20;

        /// <summary>
        /// První ze čtyř prezentačních cílů logování: barevná konzole.
        /// </summary>
        private static ColoredConsoleTarget _consoleTarget;

        /// <summary>
        /// Druhý ze čtyř prezentačních cílů logování: barevná konzole.
        /// </summary>
        private static ColoredConsoleTarget _consoleTargetPlainText;

        /// <summary>
        /// Třetí ze čtyř prezentačních cílů logování: logování do souboru.
        /// </summary>
        private static FileTarget _fileTarget;

        /// <summary>
        /// Čtvrtý ze čtyř prezentačních cílů logování: logování do souboru.
        /// </summary>
        private static FileTarget _fileTargetPlainText;

        /// <summary>
        /// Definice jména pro použitý hlavní logger.
        /// Prakticky pouze pro interní potřeby platformy NLogger a třídy Log.
        /// </summary>
        private static readonly Logger _logger = LogManager.GetLogger("MainAppLogger");

        /// <summary>
        /// Definice jména pro použitý plaintText logger.
        /// Prakticky pouze pro interní potřeby platformy NLogger a třídy Log.
        /// </summary>
        private static readonly Logger _loggerPlain = LogManager.GetLogger("PlainAppLogger");

        #endregion Fields

        #region Methods

        /// <summary>
        /// Metoda určena pro logování Debug stavu.
        /// </summary>
        /// <param name="msg">
        /// Debug zpráva určena k zalogování.
        /// </param>
        public static void Debug(string msg)
        {
            _logger.Debug("{0}{1}", GetCallerMethod(), msg);
        }

        /// <summary>
        /// Uvolnění naalokovaných systémových zdrojů.
        /// </summary>
        public static void Dispose()
        {
            _consoleTarget?.Dispose();
            _fileTarget?.Dispose();
        }

        /// <summary>
        /// Metoda určena pro logování chybového stavu.
        /// </summary>
        /// <param name="msg">
        /// Error zpráva určena k zalogování.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Error(string msg)
        {
            _logger.Error("{0}{1}", GetCallerMethod(true), PeekMessage(msg));
            return msg;
        }

        /// <summary>
        /// Metoda určena pro logování fatálního stavu, tento se považuje
        /// za závažnější, než je Error state.
        /// </summary>
        /// <param name="msg">
        /// Fatal zpráva určena k zalogování.
        /// </param>
        public static void Fatal(string msg)
        {
            _logger.Fatal("{0}{1}", GetCallerMethod(true), PeekMessage(msg));
        }

        /// <summary>
        /// Metoda určena pro logování Info stavu.
        /// </summary>
        /// <param name="msg">
        /// Informační zpráva určena k zalogování.
        /// </param>
        public static void Info(string msg)
        {
            _logger.Info("{0}{1}", GetCallerMethod(), PeekMessage(msg));
        }

        /// <summary>
        /// Metoda určena pro logování Info stavu do LoggerPlain.
        /// </summary>
        /// <param name="msg">
        /// Informační zpráva určena k zalogování.
        /// </param>
        public static void InfoPlain(string msg = "")
        {
            _loggerPlain.Info("{0}", msg);
        }

        /// <summary>
        /// Metoda určena pro logování Info Status stavu.
        /// </summary>
        /// <param name="status">
        /// Typ statusu určen k zalogování.
        /// </param>
        /// <param name="msg">
        /// Informační zpráva určena k zalogování.
        /// </param>
        public static void InfoStatus(string status, string msg)
        {
            _logger.Info("{0}{1}{2}", GetStatus(status), GetCallerMethod(), PeekMessage(msg));
        }


        /// <summary>
        /// Pokud je potřeba zalogovat konec nějaké info-logovací metody a nehodí
        /// se použít TraceMethodEnd, použije se toto.
        /// </summary>
        public static void InfoMethodEnd()
        {
            _logger.Info("{0}Method end.", GetCallerMethod());
        }

        /// <summary>
        /// Pokud je potřeba zalogovat začátek nějaké info-logovací metody a nehodí
        /// se použít TraceMethodStart, použije se toto.
        /// </summary>
        public static void InfoMethodStart()
        {
            _logger.Info("{0}Method start.", GetCallerMethod());
        }

        /// <summary>
        /// Nastavení logovacího default adresáře.
        /// </summary>
        public static void InitLogService()
        {
            Init(@"${basedir}\_Logs\${shortdate}");
        }

        /// <summary>
        /// Metoda určena pro logování Trace stavu.
        /// </summary>
        /// <param name="msg">
        /// Trasovací zpráva určena k zalogování.
        /// </param>
        public static void Trace(string msg)
        {
            _logger.Trace("{0}{1}", GetCallerTypeMethod(), PeekMessage(msg));
        }

        /// <summary>
        /// Metoda určena pro podmíněné zalogování Trace stavu.
        /// Podmínkou je vstupní unique místo, ze kterého je povoleno trace logování,
        /// pokud se volá z jiného místa v programu, než-li je předpokládáno,
        /// k logování Trace stavu nedojde.
        /// </summary>
        /// <param name="msg">
        /// Trasovací zpráva určena k zalogování.
        /// </param>
        /// <param name="allowedMethodName">
        /// Název metody, ze které se dovoleno zalogovat Trace stav.
        /// </param>
        public static void TraceConditional(string msg, string allowedMethodName)
        {
            if (GetCallerMethod(false, 3).Contains(allowedMethodName))
                _logger.Trace("{0}{1}", GetCallerMethod(), msg);
        }

        /// <summary>
        /// Pokud je potřeba zalogovat konec nějaké trasované metody, použije se toto.
        /// </summary>
        public static void TraceMethodEnd()
        {
            _logger.Trace("{0}Method end.", GetCallerTypeMethod());
        }

        /// <summary>
        /// Pokud je potřeba zalogovat začátek nějaké trasované metody, použije se toto.
        /// </summary>
        public static void TraceMethodStart()
        {
            _logger.Trace("{0}Method start.", GetCallerTypeMethod());
        }

        /// <summary>
        /// Metoda určena pro logování Warning stavu.
        /// </summary>
        /// <param name="msg">
        /// Warning zpráva určena k zalogování.
        /// </param>
        public static void Warn(string msg)
        {
            _logger.Warn("{0}{1}", GetCallerMethod(), PeekMessage(msg));
        }

        /// <summary>
        /// Součástí log.řádku je i název metody, ze které log.line pochází.
        /// </summary>
        /// <param name="showFullName">
        /// Caller metoda nebude při logování zkrácena.
        /// </param>
        /// <param name="frameNumber">
        /// Pořadové číslo framu v zásobníku.
        /// </param>
        /// <returns>
        /// Název metody ve <see cref="string"/> formátu.
        /// </returns>
        private static string GetCallerMethod(bool showFullName = false, int frameNumber = 3)
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(frameNumber);
            var method = stackFrame.GetMethod();
            var type = method.DeclaringType;

            var methodName = !showFullName
                ? $"{method.Name.Replace(".", string.Empty).PadRight(_callerMethodLength).Substring(0, _callerMethodLength)} | >> "
                : $"{method.Name.Replace(".", string.Empty)} | >> ";

            return type != null ? methodName : string.Empty;
        }


        /// <summary>
        /// Součástí log.řádku je i název metody, ze které log.line pochází.
        /// </summary>
        /// <param name="frameNumber">
        /// Pořadové číslo framu v zásobníku.
        /// </param>
        /// <returns>
        /// Název metody ve <see cref="string"/> formátu.
        /// </returns>
        private static string GetCallerTypeMethod(int frameNumber = 2)
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(frameNumber);
            var method = stackFrame.GetMethod();
            var type = method.DeclaringType;

            return type != null
                ? $"{type.Name}.{method.Name.Replace(".", string.Empty).PadRight(_callerMethodLength + type.Name.Length).Substring(0, _callerMethodLength - 7)} | >> "
                : string.Empty;
        }

        /// <summary>
        /// Součástí log.řádku je i status type PASS/FAIL.
        /// </summary>
        /// <param name="status">
        /// PASS/FAIL logovaný status výsledku test case.
        /// </param>
        /// <returns>
        /// Název metody v <see cref="string"/> formátu.
        /// </returns>
        private static string GetStatus(string status)
        {
            return $"{status.ToUpper()} | ";
        }

        /// <summary>
        /// NLog setup and configuration.
        /// </summary>
        /// <param name="logDirectory">
        /// Formát adresáře + název logovacího souboru logFile.txt.
        /// </param>
        private static void Init(string logDirectory)
        {
            // -------------------------------
            // How to configure NLog:
            // -------------------------------
            // 
            // 1) Create a LoggingConfiguration instance/object
            //      that will hold the configuration.
            //
            // 2) Create one or more targets
            //      (objects of classes inherited from Target).
            //
            // 3) Set the properties of the targets.
            //
            // 4) Define logging rules through LoggingRule objects and add
            //      the them to the configuration's LoggingRules.
            //
            // 5) Activate the configuration by assigning the configuration object
            //      to LogManager.Configuration.
            //

            var config = new LoggingConfiguration
            {
                Variables =
                {
                    ["logDataFormat"] = @"${date:format=yyyy-MM-dd | HH\:mm\:ss} | ${level:uppercase=true:padding=-5} | ${message}",

                    ["logDataPlainFormat"]
                        = @"${message}"
                }
            };

            try
            {
                // Řešení pro čtení obsahu logDirectory variable
                // ze souboru NLog.config.

                config.Variables["logDirectory"]
                    = config.LogFactory.Configuration.Variables["logDirectory"];
            }
            catch
            {
                config.Variables["logDirectory"] = logDirectory;
            }
            
            // ------------------
            // Examples:
            // ------------------
            //
            // Formát log.řádku.Např.:
            // "2018-11-09 | 08:01:21 | TRACE | ProcessConfig.ctor | >> Method start."
            //
            // Definice přístupové log.path pro logování do souboru.Např.:
            // AppDir\Logs\2018-11-09\logFile.txt"
            //

            #region INTERNAL LOGGER

            InternalLogger.LogFile = @"c:\temp\nlog-internal.log";
            InternalLogger.LogLevel = LogLevel.Off;

            #endregion

            var wordRule = new ConsoleWordHighlightingRule
            {
                // Definice barev pro část logovaného textu.
                //

                ForegroundColor = ConsoleOutputColor.Red,
                Text = "{message}"
            };

            var traceRowRule = new ConsoleRowHighlightingRule 
            {
                // Gray.Color pro logovací trace řádek.
                //

                ForegroundColor = ConsoleOutputColor.Gray,
                Condition = "level == LogLevel.Trace"
            };

            // --- [ #01 ] ---------------------------------------------
            // První ze čtyř prezentačních cílů logování:
            // Barevná konzole.
            //

            _consoleTarget = new ColoredConsoleTarget("logConsole")
            {
                // Do konzole se loguje barevně.
                RowHighlightingRules = { traceRowRule },
                WordHighlightingRules = { wordRule },
                Layout = @"${var:logDataFormat}"
            };

            config.AddTarget(_consoleTarget);

            // --- [ #02 ] ---------------------------------------------
            // Druhý ze čtyř prezentačních cílů logování:
            // Barevná konzole.
            //

            _consoleTargetPlainText = new ColoredConsoleTarget("logConsolePlain")
            {
                // Do konzole se loguje barevně.
                RowHighlightingRules = { traceRowRule },
                WordHighlightingRules = { wordRule },
                Layout = @"${var:logDataPlainFormat}"
            };

            config.AddTarget(_consoleTargetPlainText);


            // --- [ #03 ] ---------------------------------------------
            // Třetí ze čtyř prezentačních cílů logování:
            // Logování do souboru.
            //

            _fileTarget = new FileTarget("logFile")
            {
                // Do souboru se loguje ve stejném formátu jako do konzole.
                FileName = "${var:logDirectory}/logFile.txt",
                Layout = @"${var:logDataFormat}",
                DeleteOldFileOnStartup = true
            };

            config.AddTarget(_fileTarget);

            // --- [ #04 ] ---------------------------------------------
            // Čtvrtý ze čtyř prezentačních cílů logování:
            // Logování do souboru.
            //

            _fileTargetPlainText = new FileTarget("logFilePlain")
            {
                // Do souboru se loguje ve stejném formátu jako do konzole.
                FileName = "${var:logDirectory}/logFile.txt",
                Layout = @"${var:logDataPlainFormat}",
                DeleteOldFileOnStartup = false
            };

            config.AddTarget(_fileTargetPlainText);

            //
            // Přidání logLevel od-do filtru pro oba targety.
            //

            // Přidání pravidel: logLevel min, max + targety + loggerName pattern
            //

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, _consoleTarget, "MainAppLogger");
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, _fileTarget, "MainAppLogger");
            config.AddRule(LogLevel.Info, LogLevel.Info, _consoleTargetPlainText, "PlainAppLogger");
            config.AddRule(LogLevel.Info, LogLevel.Info, _fileTargetPlainText, "PlainAppLogger");

            #region Alternative way to add config rules

            /*

            var consoleTargetRule = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            var fileTargetRule = new LoggingRule("*", LogLevel.Trace, fileTarget);

            config.LoggingRules.Add(consoleTargetRule);
            config.LoggingRules.Add(fileTargetRule);

            */

            #endregion

            LogManager.ThrowExceptions = false;
            LogManager.Configuration = config;

            #region NLog external configuration file

            // The following locations will be searched when executing a stand-alone *.exe application:
            // 1) Standard application configuration file (usually applicationname.exe.config).
            // 2) Applicationname.exe.nlog in application’s directory.
            // 3) NLog.config in application’s directory (Name sensitive; using docker dotnet core).
            // 4) NLog.dll.nlog in a directory where NLog.dll is located (only if NLog isn't installed in the GAC)
            // 5) In other situations NLog can be configured programatically.

            #endregion
        }

        /// <summary>
        /// Do log.message přidá na konci věty znak dot(.), pokud tam chybí.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <returns>
        /// Log.message typu <see cref="string"/> zakončená tečkou.
        /// </returns>
        private static string PeekMessage(string msg)
        {
            return msg.EndsWith(".") || msg.EndsWith(":") ? msg : $"{msg}.";
        }

        #endregion Methods

        #region Other

        /*

        ----------------------------------------------------------------
        NLog.config file (located at solution's root directory):
        ----------------------------------------------------------------

        <?xml version="1.0" encoding="utf-8" ?>
        <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
              xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
              xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
              autoReload="true"
              throwExceptions="false"
              internalLogLevel="Off" internalLogFile="c:\temp\nlog-internal.log">

          <!-- optional, add some variables
          https://github.com/nlog/NLog/wiki/Configuration-file#variables
          -->
          <variable name="logDirectory" value="${basedir}/Logs/${shortdate}" />

          <!--
          See https://github.com/nlog/nlog/wiki/Configuration-file
          for information on customizing logging rules and outputs.
           -->
          <targets>

            <target name="logConsole" xsi:type="ColoredConsole"
                    layout="${date:format=yyyy-MM-dd | HH\:mm\:ss} | ${level:uppercase=true:padding=-5} | ${message}">
              <highlight-row condition="level == LogLevel.Trace" foregroundColor="Gray" />
              <highlight-word text="{message}" foregroundColor="Red" />
            </target>

            <target name="logFile" xsi:type="File"
                    layout="${date:format=yyyy-MM-dd | HH\:mm\:ss} | ${level:uppercase=true:padding=-5} | ${message}"
                    fileName="${logDirectory}/logFile.txt">
            </target>

          </targets>

          <rules>

            <logger name="*" minlevel="Trace" writeTo="logConsole" />
            <logger name="*" minlevel="Trace" writeTo="logFile" />

          </rules>

        </nlog>

        ----------------------------------------------------------------

        Logger.Trace("Hello, world...");
        Logger.Debug("Hello, world...");
        Logger.Info("Hello, world...");
        Logger.Warn("Hello, world...");
        Logger.Error("Hello, world...");
        Logger.Fatal("Hello, world...");

        ----------------------------------------------------------------

        */

        #endregion Other
    }
}