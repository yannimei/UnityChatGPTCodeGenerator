using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChatGPTTester : MonoBehaviour
{
    [SerializeField]
    private Button askButton;

    [SerializeField]
    private ButtonManager buttonManager;

    [SerializeField]
    private TextMeshProUGUI chatGPTAnswer;

    //[SerializeField]
    //[TextArea(5, 12)]
    //private string prompt;

    [SerializeField]
    private SceneUnderstanding sceneUnderstanding;

    [SerializeField]
    private int conversationIDScene = 2;

    [SerializeField]
    private int conversationIDCodeInstructor = 3;

    [SerializeField]
    private ChatGPTQuestion[] chatGPTQuestion;

    [SerializeField]
    private TextMeshProUGUI questionText;

    private string gptPrompt;

    private ChatGPTResponse response;

    public bool immediateCompilation;

    public bool voiceInput = true;

    public Text transcription;

    

    public void Execute(int conversationID)
    {
        
        //if voiceInput is enabled then use script
        var selectedPrompt = voiceInput ? transcription.text : chatGPTQuestion[conversationID].prompt;

        //get sceneJson
        string sceneInfo = sceneUnderstanding.ExportScene();

        StartCoroutine(ChatGPTClient.Instance.Ask($"Request: {selectedPrompt},\n SceneJSON: {sceneInfo}", conversationIDScene, false, (r) =>
        {
            //CallCodeGenerator(conversationID, $"{selectedPrompt}, {r}");
            CallCodeInstructor(conversationIDCodeInstructor, conversationID, $"Request: {selectedPrompt} \n {r.Data}");
        }
       ));  
    }

    //public void ProcessResponse(ChatGPTResponse response)
    //{
    //    Logger.Instance.LogInfo(response.Data);
    //    //Roslyn run the code
    //    RoslynCodeRunner.Instance.RunCode(response.Data);
    //}.va

    private void CallCodeInstructor(int conversationID, int AnotherConversationID, string prompt)
    {
  
        StartCoroutine(ChatGPTClient.Instance.Ask(prompt,conversationID, false, (r) =>
        {
            CallCodeGenerator(AnotherConversationID, r.Data);
        }));
    }

    private void CallCodeGenerator(int conversationID, string selectedPrompt)
    {
        //gptPrompt = $"{chatGPTQuestion.promptPrefixConstant} {chatGPTQuestion.prompt}";
        gptPrompt = $"{chatGPTQuestion[conversationID].promptPrefixConstant} {selectedPrompt}";

        //questionText.text = chatGPTQuestion.prompt;
        questionText.text = selectedPrompt;

        // handle replacements
        Array.ForEach(chatGPTQuestion[conversationID].replacements, r =>
        {
            gptPrompt = gptPrompt.Replace("{" + $"{r.replacementType}" + "}", r.value);
        });

        // handle reminders
        if (chatGPTQuestion[conversationID].reminders.Length > 0)
        {
            gptPrompt += $", {string.Join(',', chatGPTQuestion[conversationID].reminders)}";
        }


        StartCoroutine(ChatGPTClient.Instance.Ask(gptPrompt, conversationID, true, (r) =>
        {
            //pass the generated code (r) to response
            response = r;

            // log the code to the text box
            Logger.Instance.LogInfo(response.Data);

            buttonManager.SetCompileButtonInteractable(true);

            //if true then run the genreated code
            if (immediateCompilation)
            {
                ProcessAndCompileResponse();
            }
        }
        ));
    }


    public void ProcessAndCompileResponse()
    {
        RoslynCodeRunner.Instance.RunCode(response.Data);
        buttonManager.SetCompileButtonInteractable(false);
        buttonManager.SetScriptsButtonsInteractable(true);
    }

    public void SetVoiceInputValue(bool newToggleValue)
    {
        voiceInput = newToggleValue;
    }
}