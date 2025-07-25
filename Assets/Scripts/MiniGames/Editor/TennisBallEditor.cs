using UnityEngine;
using UnityEditor;

namespace TennisCoachCho.MiniGames
{
#if UNITY_EDITOR
    [CustomEditor(typeof(TennisBall))]
    public class TennisBallEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            TennisBall tennisBall = (TennisBall)target;
            
            // Draw the default inspector
            DrawDefaultInspector();
            
            // Add some space
            EditorGUILayout.Space(10);
            
            // Add a prominent reset button
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Reset All Settings to Defaults", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Reset Tennis Ball Settings", 
                    "This will reset all tennis ball physics and gameplay settings to their default values. Are you sure?", 
                    "Yes, Reset", "Cancel"))
                {
                    // Get the settings field using SerializedProperty
                    SerializedProperty settingsProperty = serializedObject.FindProperty("settings");
                    if (settingsProperty != null)
                    {
                        // Create a new instance with default values
                        BallPhysicsSettings defaultSettings = new BallPhysicsSettings();
                        defaultSettings.ResetToDefaults();
                        
                        // Apply the default values to each property
                        ApplyDefaultSettings(settingsProperty, defaultSettings);
                        
                        // Apply changes
                        serializedObject.ApplyModifiedProperties();
                        
                        // Mark the scene as dirty so Unity knows to save
                        EditorUtility.SetDirty(tennisBall);
                        
                        Debug.Log("Tennis Ball settings have been reset to defaults!");
                    }
                }
            }
            GUI.backgroundColor = Color.white;
            
            // Add helpful information
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "Tennis Ball Developer Settings:\n\n" +
                "• Bounce Energy Loss: Vertical bounce retention (higher = higher bounces)\n" +
                "• Horizontal Energy Loss: Forward speed retention after bounce\n" +
                "• Serve Power: Adjust serve speed and variations\n" +
                "• Hit Settings: Control player and student hit power\n" +
                "• Targeting: Fine-tune where hits are aimed\n" +
                "• Collision: Adjust hit detection distances and timing\n" +
                "• Student Hitting Zone Width: How far in front of paddle center ball can be hit\n" +
                "• Court Boundaries: Define playable area limits", 
                MessageType.Info);
        }
        
        private void ApplyDefaultSettings(SerializedProperty settingsProperty, BallPhysicsSettings defaults)
        {
            // Physics settings
            settingsProperty.FindPropertyRelative("gravity").floatValue = defaults.gravity;
            settingsProperty.FindPropertyRelative("bounceEnergyLoss").floatValue = defaults.bounceEnergyLoss;
            settingsProperty.FindPropertyRelative("horizontalEnergyLoss").floatValue = defaults.horizontalEnergyLoss;
            settingsProperty.FindPropertyRelative("minimumBounceVelocity").floatValue = defaults.minimumBounceVelocity;
            
            // Serve settings
            settingsProperty.FindPropertyRelative("serveSpeed").floatValue = defaults.serveSpeed;
            settingsProperty.FindPropertyRelative("serveSpeedVariationMin").floatValue = defaults.serveSpeedVariationMin;
            settingsProperty.FindPropertyRelative("serveSpeedVariationMax").floatValue = defaults.serveSpeedVariationMax;
            settingsProperty.FindPropertyRelative("servePowerMultiplier").floatValue = defaults.servePowerMultiplier;
            settingsProperty.FindPropertyRelative("serveHeightMin").floatValue = defaults.serveHeightMin;
            settingsProperty.FindPropertyRelative("serveHeightMax").floatValue = defaults.serveHeightMax;
            settingsProperty.FindPropertyRelative("serveHeightVariation").floatValue = defaults.serveHeightVariation;
            
            // Player hit settings
            settingsProperty.FindPropertyRelative("playerHitSpeed").floatValue = defaults.playerHitSpeed;
            settingsProperty.FindPropertyRelative("playerHitPowerMultiplier").floatValue = defaults.playerHitPowerMultiplier;
            settingsProperty.FindPropertyRelative("playerHitPowerBonus").floatValue = defaults.playerHitPowerBonus;
            settingsProperty.FindPropertyRelative("playerHitZVelocity").floatValue = defaults.playerHitZVelocity;
            
            // Student hit settings
            settingsProperty.FindPropertyRelative("studentHitSpeed").floatValue = defaults.studentHitSpeed;
            settingsProperty.FindPropertyRelative("studentHitPowerMultiplier").floatValue = defaults.studentHitPowerMultiplier;
            settingsProperty.FindPropertyRelative("studentHitZVelocity").floatValue = defaults.studentHitZVelocity;
            
            // Targeting settings
            settingsProperty.FindPropertyRelative("playerTargetOffsetX").floatValue = defaults.playerTargetOffsetX;
            settingsProperty.FindPropertyRelative("playerTargetOffsetY").floatValue = defaults.playerTargetOffsetY;
            settingsProperty.FindPropertyRelative("playerHitDirectionYMin").floatValue = defaults.playerHitDirectionYMin;
            settingsProperty.FindPropertyRelative("playerHitDirectionYMax").floatValue = defaults.playerHitDirectionYMax;
            settingsProperty.FindPropertyRelative("studentTargetX").floatValue = defaults.studentTargetX;
            settingsProperty.FindPropertyRelative("studentTargetYVariation").floatValue = defaults.studentTargetYVariation;
            
            // Collision settings
            settingsProperty.FindPropertyRelative("playerCollisionDistance").floatValue = defaults.playerCollisionDistance;
            settingsProperty.FindPropertyRelative("studentCollisionDistance").floatValue = defaults.studentCollisionDistance;
            settingsProperty.FindPropertyRelative("maxHitHeight").floatValue = defaults.maxHitHeight;
            settingsProperty.FindPropertyRelative("hitCooldownDuration").floatValue = defaults.hitCooldownDuration;
            settingsProperty.FindPropertyRelative("studentHittingZoneWidth").floatValue = defaults.studentHittingZoneWidth;
            
            // Court boundaries
            settingsProperty.FindPropertyRelative("courtMinX").floatValue = defaults.courtMinX;
            settingsProperty.FindPropertyRelative("courtMaxX").floatValue = defaults.courtMaxX;
            settingsProperty.FindPropertyRelative("courtMinY").floatValue = defaults.courtMinY;
            settingsProperty.FindPropertyRelative("courtMaxY").floatValue = defaults.courtMaxY;
            
            // Base settings
            settingsProperty.FindPropertyRelative("baseSpeed").floatValue = defaults.baseSpeed;
            settingsProperty.FindPropertyRelative("shadowOffsetMultiplier").floatValue = defaults.shadowOffsetMultiplier;
        }
    }
#endif
}