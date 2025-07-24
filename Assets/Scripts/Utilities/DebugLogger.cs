using System;
using System.IO;
using UnityEngine;

namespace TennisCoachCho.Utilities
{
    /// <summary>
    /// Debug logging utility that saves messages to a text file
    /// </summary>
    public static class DebugLogger
    {
        private static string logFilePath;
        private static bool isInitialized = false;
        
        static DebugLogger()
        {
            InitializeLogger();
        }
        
        private static void InitializeLogger()
        {
            if (isInitialized) return;
            
            // Create logs directory if it doesn't exist
            string logsDirectory = Path.Combine(Application.dataPath, "..", "DebugLogs");
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }
            
            // Create log file with timestamp
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logFilePath = Path.Combine(logsDirectory, $"SchedulerApp_Debug_{timestamp}.txt");
            
            // Write header
            WriteToFile($"=== SchedulerApp Debug Log Started ===");
            WriteToFile($"Time: {DateTime.Now}");
            WriteToFile($"Unity Version: {Application.unityVersion}");
            WriteToFile($"Platform: {Application.platform}");
            WriteToFile($"======================================\n");
            
            isInitialized = true;
            
            Debug.Log($"DebugLogger initialized. Log file: {logFilePath}");
        }
        
        public static void Log(string message, string category = "INFO")
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string formattedMessage = $"[{timestamp}] [{category}] {message}";
            
            // Write to Unity console
            Debug.Log(formattedMessage);
            
            // Write to file
            WriteToFile(formattedMessage);
        }
        
        public static void LogError(string message)
        {
            Log(message, "ERROR");
        }
        
        public static void LogWarning(string message)
        {
            Log(message, "WARNING");
        }
        
        public static void LogSchedulerApp(string message)
        {
            Log(message, "SCHEDULER");
        }
        
        public static void LogUIEvent(string message)
        {
            Log(message, "UI_EVENT");
        }
        
        public static void LogSkillsPerks(string message)
        {
            Log(message, "SKILLS_PERKS");
        }
        
        public static void LogSkillData(TennisCoachCho.Data.SkillTreeNode skill)
        {
            if (skill == null)
            {
                Log("SkillTreeNode is NULL!", "SKILL_DATA");
                return;
            }
            
            string message = $"Skill: {skill.nodeName}";
            message += $" | Level: {skill.level}/{skill.maxLevel}";
            message += $" | CanUpgrade: {skill.CanUpgrade()}";
            message += $" | Description: {skill.description}";
            
            Log(message, "SKILL_DATA");
        }
        
        public static void LogPerkData(TennisCoachCho.Data.PerkData perk)
        {
            if (perk == null)
            {
                Log("PerkData is NULL!", "PERK_DATA");
                return;
            }
            
            string message = $"Perk: {perk.perkName}";
            message += $" | Unlocked: {perk.isUnlocked}";
            message += $" | Description: {perk.description}";
            
            Log(message, "PERK_DATA");
        }
        
        public static void LogGameObject(GameObject obj, string context = "")
        {
            if (obj == null)
            {
                Log($"GameObject is NULL! Context: {context}", "GAMEOBJECT");
                return;
            }
            
            string message = $"GameObject: {obj.name}";
            message += $" | Active: {obj.activeInHierarchy}";
            message += $" | ActiveSelf: {obj.activeSelf}";
            message += $" | Position: {obj.transform.position}";
            message += $" | LocalPosition: {obj.transform.localPosition}";
            message += $" | Parent: {(obj.transform.parent ? obj.transform.parent.name : "None")}";
            message += $" | Children: {obj.transform.childCount}";
            
            if (!string.IsNullOrEmpty(context))
                message += $" | Context: {context}";
            
            Log(message, "GAMEOBJECT");
        }
        
        public static void LogRectTransform(RectTransform rectTransform, string context = "")
        {
            if (rectTransform == null)
            {
                Log($"RectTransform is NULL! Context: {context}", "RECTTRANSFORM");
                return;
            }
            
            string message = $"RectTransform: {rectTransform.name}";
            message += $" | AnchorMin: {rectTransform.anchorMin}";
            message += $" | AnchorMax: {rectTransform.anchorMax}";
            message += $" | SizeDelta: {rectTransform.sizeDelta}";
            message += $" | AnchoredPosition: {rectTransform.anchoredPosition}";
            message += $" | Pivot: {rectTransform.pivot}";
            message += $" | Rect: {rectTransform.rect}";
            
            if (!string.IsNullOrEmpty(context))
                message += $" | Context: {context}";
            
            Log(message, "RECTTRANSFORM");
        }
        
        public static void LogAppointmentData(TennisCoachCho.Data.AppointmentData appointment)
        {
            if (appointment == null)
            {
                Log("AppointmentData is NULL!", "APPOINTMENT");
                return;
            }
            
            string message = $"Appointment: {appointment.clientName}";
            message += $" | Time: {appointment.GetTimeString()}";
            message += $" | Location: {appointment.location}";
            message += $" | Accepted: {appointment.isAccepted}";
            message += $" | Completed: {appointment.isCompleted}";
            message += $" | Cash: ${appointment.cashReward}";
            message += $" | EXP: {appointment.expReward}";
            
            Log(message, "APPOINTMENT");
        }
        
        public static void LogSeparator(string title = "")
        {
            string separator = new string('=', 50);
            if (!string.IsNullOrEmpty(title))
            {
                WriteToFile($"\n{separator}");
                WriteToFile($"=== {title} ===");
                WriteToFile($"{separator}");
            }
            else
            {
                WriteToFile($"\n{separator}");
            }
        }
        
        private static void WriteToFile(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(message);
                    writer.Flush();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write to debug log file: {e.Message}");
            }
        }
        
        public static void FlushAndClose()
        {
            WriteToFile($"\n=== Debug Log Session Ended ===");
            WriteToFile($"Time: {DateTime.Now}");
            WriteToFile($"=====================================");
        }
        
        // Call this when the application quits
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnApplicationQuit()
        {
            FlushAndClose();
        }
    }
}