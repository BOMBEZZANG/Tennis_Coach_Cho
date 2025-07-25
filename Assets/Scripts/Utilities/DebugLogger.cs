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
        
        public static void LogSpecialistSkillData(TennisCoachCho.Data.SpecialistSkillData skill)
        {
            if (skill == null)
            {
                Debug.Log("[SPECIALIST_SKILL] SpecialistSkillData is NULL!");
                return;
            }
            
            string message = $"Specialist Field: {skill.GetFieldDisplayName()}";
            message += $" | Level: {skill.level}";
            message += $" | XP: {skill.exp}/{skill.expToNext}";
            message += $" | Field: {skill.field}";
            
            Debug.Log($"[SPECIALIST_SKILL] {message}");
        }
        
        public static void LogPerkTreeNode(TennisCoachCho.Data.PerkTreeNode perk)
        {
            if (perk == null)
            {
                Debug.Log("[PERK_TREE] PerkTreeNode is NULL!");
                return;
            }
            
            string message = $"Perk: {perk.nodeName}";
            message += $" | Unlocked: {perk.isUnlocked}";
            message += $" | Required Field: {perk.requiredField}";
            message += $" | Required Level: {perk.requiredFieldLevel}";
            message += $" | Cost: {perk.perkCost} perk points";
            message += $" | Description: {perk.description}";
            message += $" | Effect: {perk.gameplayEffect}";
            
            Debug.Log($"[PERK_TREE] {message}");
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
                Debug.Log("[DOG_APPOINTMENT] AppointmentData is NULL!");
                return;
            }
            
            string message = $"Dog Coach Appointment: {appointment.clientName} with {appointment.dogName}";
            message += $" | Time: {appointment.GetTimeString()}";
            message += $" | Location: {appointment.location}";
            message += $" | Accepted: {appointment.isAccepted}";
            message += $" | Completed: {appointment.isCompleted}";
            message += $" | Cash: ${appointment.cashReward}";
            message += $" | Player XP: {appointment.playerExpReward}";
            message += $" | Specialist Field: {appointment.primaryField}";
            message += $" | Specialist XP: {appointment.specialistExpReward}";
            message += $" | Stamina Cost: {appointment.staminaCost}";
            
            Debug.Log($"[DOG_APPOINTMENT] {message}");
        }
        
        // DOG COACH SYSTEM: Backward compatibility for old method names
        [System.Obsolete("Use LogSpecialistSkillData instead")]
        public static void LogSkillData(object skill)
        {
            Debug.LogWarning("[DEPRECATED] LogSkillData is deprecated. Use LogSpecialistSkillData for specialist skills.");
        }
        
        [System.Obsolete("Use LogPerkTreeNode instead")]
        public static void LogPerkData(object perk)
        {
            Debug.LogWarning("[DEPRECATED] LogPerkData is deprecated. Use LogPerkTreeNode for perk trees.");
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