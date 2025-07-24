using UnityEngine;

namespace TennisCoachCho.Utilities
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string buttonText;
        
        public ButtonAttribute(string text)
        {
            buttonText = text;
        }
    }
}