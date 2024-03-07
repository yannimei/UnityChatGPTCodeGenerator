using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChatGPTQuestion", menuName = "ChatGPT/ChatGPTQuestion", order = 2)]
public class ChatGPTQuestion : ScriptableObject
{
    public string scenarioTitle;

    public string promptPrefixConstant;

    //[TextArea(8, 20)]
    //public string prompt;

    [TextArea(3, 12)]
    public List<string> promptList;

    public int index = 0;

    public ChatGPTReplacement[] replacements;

    public string[] reminders;

    public string prompt
    {
        get
        {
            if (promptList != null && promptList.Count > index && index >= 0)
            {
                // Return the prompt at the current index
                return promptList[index];
            }
            else
            {
                // Return an empty string or a default value if the index is out of bounds
                return string.Empty;
            }
        }
    }
}

[Serializable]
public struct ChatGPTReplacement
{
    public Replacements replacementType;

    public string value;
}

[Serializable]
public enum Replacements
{
    CLASS_NAME,
    ACTION,
    API_KEY
}
