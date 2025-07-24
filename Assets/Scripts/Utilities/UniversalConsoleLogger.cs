using System;
using System.IO;
using UnityEngine;

namespace TennisCoachCho.Utilities
{
    public class UniversalConsoleLogger : MonoBehaviour
    {
        [Header("Console Logging Settings")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private bool logToFile = true;
        [SerializeField] private bool logErrors = true;
        [SerializeField] private bool logWarnings = true;
        [SerializeField] private bool logMessages = true;
        [SerializeField] private bool logExceptions = true;
        
        private string logFilePath;
        private static UniversalConsoleLogger instance;
        
        private void Awake()
        {
            Debug.Log("[UniversalConsoleLogger] Awake() called!");
            Debug.Log($"[UniversalConsoleLogger] GameObject name: {gameObject.name}");
            Debug.Log($"[UniversalConsoleLogger] instance == null: {instance == null}");
            
            // Ensure only one instance exists
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[UniversalConsoleLogger] Instance created, calling InitializeLogger...");
                InitializeLogger();
            }
            else
            {
                Debug.Log("[UniversalConsoleLogger] Duplicate instance found, destroying...");
                Destroy(gameObject);
            }
        }
        
        private void InitializeLogger()
        {
            Debug.Log($"[UniversalConsoleLogger] InitializeLogger() called!");
            Debug.Log($"[UniversalConsoleLogger] enableLogging: {enableLogging}");
            Debug.Log($"[UniversalConsoleLogger] logToFile: {logToFile}");
            
            if (!enableLogging)
            {
                Debug.Log("[UniversalConsoleLogger] Logging disabled, returning...");
                return;
            }
            
            // Create logs directory
            string logsDirectory = Path.Combine(Application.dataPath, "..", "ConsoleLogs");
            Debug.Log($"[UniversalConsoleLogger] Logs directory path: {logsDirectory}");
            Debug.Log($"[UniversalConsoleLogger] Application.dataPath: {Application.dataPath}");
            
            try
            {
                if (!Directory.Exists(logsDirectory))
                {
                    Debug.Log("[UniversalConsoleLogger] Directory doesn't exist, creating...");
                    Directory.CreateDirectory(logsDirectory);
                    Debug.Log("[UniversalConsoleLogger] Directory created successfully!");
                }
                else
                {
                    Debug.Log("[UniversalConsoleLogger] Directory already exists");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[UniversalConsoleLogger] Failed to create directory: {e.Message}");
                return;
            }
            
            // Create log file with timestamp
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logFilePath = Path.Combine(logsDirectory, $"Console_{timestamp}.txt");
            Debug.Log($"[UniversalConsoleLogger] Log file path: {logFilePath}");
            
            // Write initial header
            if (logToFile)
            {
                Debug.Log("[UniversalConsoleLogger] Writing initial header...");
                WriteToFile("=== Unity Console Log Started ===");
                WriteToFile($"Time: {DateTime.Now}");
                WriteToFile($"Unity Version: {Application.unityVersion}");
                WriteToFile($"Platform: {Application.platform}");
                WriteToFile($"Product Name: {Application.productName}");
                WriteToFile("=====================================\n");
            }
            
            // Subscribe to Unity's log callback
            Application.logMessageReceived += OnLogMessageReceived;
            Debug.Log("[UniversalConsoleLogger] Subscribed to log callback");
            
            Debug.Log($"[UniversalConsoleLogger] ✅ INITIALIZATION COMPLETE! Log file: {logFilePath}");
        }
        
        private void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (!enableLogging || !logToFile) return;
            
            // Check if we should log this type of message
            bool shouldLog = false;
            string typeString = "";
            
            switch (type)
            {
                case LogType.Error:
                    shouldLog = logErrors;
                    typeString = "ERROR";
                    break;
                case LogType.Assert:
                    shouldLog = logErrors;
                    typeString = "ASSERT";
                    break;
                case LogType.Warning:
                    shouldLog = logWarnings;
                    typeString = "WARNING";
                    break;
                case LogType.Log:
                    shouldLog = logMessages;
                    typeString = "LOG";
                    break;
                case LogType.Exception:
                    shouldLog = logExceptions;
                    typeString = "EXCEPTION";
                    break;
            }
            
            if (!shouldLog) return;
            
            // Format the log message exactly like console
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string formattedMessage = $"[{timestamp}] [{typeString}] {logString}";
            
            // Add stack trace for errors and exceptions
            if ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) && !string.IsNullOrEmpty(stackTrace))
            {
                formattedMessage += $"\nStack Trace:\n{stackTrace}";
            }
            
            WriteToFile(formattedMessage);
        }
        
        private void WriteToFile(string message)
        {
            try
            {
                // Debug: Don't use Debug.Log here to avoid infinite loop
                Console.WriteLine($"[UniversalConsoleLogger] Writing to file: {logFilePath}");
                Console.WriteLine($"[UniversalConsoleLogger] Message: {message}");
                
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(message);
                    writer.Flush();
                }
                
                Console.WriteLine("[UniversalConsoleLogger] Write successful!");
            }
            catch (Exception e)
            {
                // Avoid infinite loop by not using Debug.LogError here
                Console.WriteLine($"[UniversalConsoleLogger] Failed to write to console log file: {e.Message}");
                Console.WriteLine($"[UniversalConsoleLogger] File path was: {logFilePath}");
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from log callback
            Application.logMessageReceived -= OnLogMessageReceived;
            
            if (logToFile && !string.IsNullOrEmpty(logFilePath))
            {
                WriteToFile($"\n=== Unity Console Log Ended ===");
                WriteToFile($"Time: {DateTime.Now}");
                WriteToFile("===================================");
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && logToFile)
            {
                WriteToFile($"\n[{DateTime.Now:HH:mm:ss.fff}] [SYSTEM] Application paused");
            }
            else if (!pauseStatus && logToFile)
            {
                WriteToFile($"[{DateTime.Now:HH:mm:ss.fff}] [SYSTEM] Application resumed");
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (logToFile)
            {
                string focusStatus = hasFocus ? "gained focus" : "lost focus";
                WriteToFile($"[{DateTime.Now:HH:mm:ss.fff}] [SYSTEM] Application {focusStatus}");
            }
        }
        
        // Public methods for runtime control
        public void EnableLogging()
        {
            enableLogging = true;
            Debug.Log("UniversalConsoleLogger: Logging enabled");
        }
        
        public void DisableLogging()
        {
            enableLogging = false;
            Debug.Log("UniversalConsoleLogger: Logging disabled");
        }
        
        public void ClearLogFile()
        {
            if (logToFile && File.Exists(logFilePath))
            {
                try
                {
                    File.WriteAllText(logFilePath, "");
                    WriteToFile("=== Log file cleared ===");
                    WriteToFile($"Time: {DateTime.Now}");
                    WriteToFile("========================\n");
                    Debug.Log("Console log file cleared");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to clear log file: {e.Message}");
                }
            }
        }
        
        public string GetLogFilePath()
        {
            return logFilePath;
        }
        
        // Static access methods
        public static void Enable()
        {
            if (instance != null)
                instance.EnableLogging();
        }
        
        public static void Disable()
        {
            if (instance != null)
                instance.DisableLogging();
        }
        
        public static void Clear()
        {
            if (instance != null)
                instance.ClearLogFile();
        }
    }
}