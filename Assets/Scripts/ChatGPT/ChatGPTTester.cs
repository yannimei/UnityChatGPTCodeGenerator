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

    private ChatGPTResponse response;

    public bool immediateCompilation;

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

        //(ChatGPTClient.Instance.Ask(gptPrompt, (r) => ProcessResponse(r)));

        StartCoroutine(ChatGPTClient.Instance.Ask(gptPrompt, (r) =>
           {
               //pass the generated code (r) to response
               response = r;

               // log the code to the text box
               Logger.Instance.LogInfo(response.Data);

               //if true then run the genreated code
               if (immediateCompilation)
               {
                   ProcessAndCompileResponse();
               }
           }
        ));
    }

    //public void ProcessResponse(ChatGPTResponse response)
    //{
    //    Logger.Instance.LogInfo(response.Data);
    //    //Roslyn run the code
    //    RoslynCodeRunner.Instance.RunCode(response.Data);
    //}


    public void ProcessAndCompileResponse()
    {
        RoslynCodeRunner.Instance.RunCode(response.Data);
    }
}