using UnityEngine;

namespace TennisCoachCho.Utilities
{
    public static class DebugLogger
    {
        // DebugLogger now only outputs to Unity console
        // UniversalConsoleLogger will capture and save all console messages
        
        public static void Log(string message, string category = "INFO")
        {
            Debug.Log($"[{category}] {message}");
        }
        
        public static void LogError(string message)
        {
            Debug.LogError($"[ERROR] {message}");
        }
        
        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[WARNING] {message}");
        }
        
        public static void LogSchedulerApp(string message)
        {
            Debug.Log($"[SCHEDULER] {message}");
        }
        
        public static void LogUIEvent(string message)
        {
            Debug.Log($"[UI_EVENT] {message}");
        }
        
        public static void LogSkillsPerks(string message)
        {
            Debug.Log($"[SKILLS_PERKS] {message}");
        }
        
        public static void LogSkillData(TennisCoachCho.Data.SkillTreeNode skill)
        {
            if (skill == null)
            {
                Debug.Log("[SKILL_DATA] SkillTreeNode is NULL!");
                return;
            }
            
            string message = $"Skill: {skill.nodeName}";
            message += $" | Level: {skill.level}/{skill.maxLevel}";
            message += $" | CanUpgrade: {skill.CanUpgrade()}";
            message += $" | Description: {skill.description}";
            
            Debug.Log($"[SKILL_DATA] {message}");
        }
        
        public static void LogPerkData(TennisCoachCho.Data.PerkData perk)
        {
            if (perk == null)
            {
                Debug.Log("[PERK_DATA] PerkData is NULL!");
                return;
            }
            
            string message = $"Perk: {perk.perkName}";
            message += $" | Unlocked: {perk.isUnlocked}";
            message += $" | Description: {perk.description}";
            
            Debug.Log($"[PERK_DATA] {message}");
        }
        
        public static void LogGameObject(GameObject obj, string context = "")
        {
            if (obj == null)
            {
                Debug.Log($"[GAMEOBJECT] GameObject is NULL! Context: {context}");
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
            
            Debug.Log($"[GAMEOBJECT] {message}");
        }
        
        public static void LogRectTransform(RectTransform rectTransform, string context = "")
        {
            if (rectTransform == null)
            {
                Debug.Log($"[RECTTRANSFORM] RectTransform is NULL! Context: {context}");
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
            
            Debug.Log($"[RECTTRANSFORM] {message}");
        }
        
        public static void LogAppointmentData(TennisCoachCho.Data.AppointmentData appointment)
        {
            if (appointment == null)
            {
                Debug.Log("[APPOINTMENT] AppointmentData is NULL!");
                return;
            }
            
            string message = $"Appointment: {appointment.clientName}";
            message += $" | Time: {appointment.GetTimeString()}";
            message += $" | Location: {appointment.location}";
            message += $" | Accepted: {appointment.isAccepted}";
            message += $" | Completed: {appointment.isCompleted}";
            message += $" | Cash: ${appointment.cashReward}";
            message += $" | EXP: {appointment.expReward}";
            
            Debug.Log($"[APPOINTMENT] {message}");
        }
        
        public static void LogSeparator(string title = "")
        {
            string separator = new string('=', 50);
            if (!string.IsNullOrEmpty(title))
            {
                Debug.Log($"\n{separator}");
                Debug.Log($"=== {title} ===");
                Debug.Log($"{separator}");
            }
            else
            {
                Debug.Log($"\n{separator}");
            }
        }
    }
}