using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChatGPTTester : MonoBehaviour
{
    [SerializeField]
    private Button askButton;

    [SerializeField]
    private TextMeshProUGUI chatGPTAnswer;

    //[SerializeField]
    //[TextArea(5, 12)]
    //private string prompt;

    [SerializeField]
    private ChatGPTQuestion chatGPTQuestion;

    private string gptPrompt;

    public void Execute()
    {
        gptPrompt = $"{chatGPTQuestion.promptPrefixConstant} {chatGPTQuestion.prompt}";

        // handle replacements
        Array.ForEach(chatGPTQuestion.replacements, r =>
        {
            gptPrompt = gptPrompt.Replace("{" + $"{r.replacementType}" + "}", r.value);
        });

        // handle reminders
        if (chatGPTQuestion.reminders.Length > 0)
        {
            gptPrompt += $", {string.Join(',', chatGPTQuestion.reminders)}";
        }

        StartCoroutine(ChatGPTClient.Instance.Ask(gptPrompt, (r) => ProcessResponse(r)));
    }

    public void ProcessResponse(ChatGPTResponse response)
    {
        Logger.Instance.LogInfo(response.Data);
        //Roslyn run the code
        RoslynCodeRunner.Instance.RunCode(response.Data);
    }
}