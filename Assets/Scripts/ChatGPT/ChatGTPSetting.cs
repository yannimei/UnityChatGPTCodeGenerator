using UnityEngine;

[CreateAssetMenu(fileName ="ChatGPTSetting", menuName ="ChatGPT/ChatGPTSettings")]
public class ChatGTPSetting : ScriptableObject
{
    public string apiURL;

    public string apiKey;

    //public bool debug;

    public string[] reminders;
}
