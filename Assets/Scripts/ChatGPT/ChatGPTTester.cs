using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatGPTTester : MonoBehaviour
{
    [SerializeField]
    private Button askButton;

    [SerializeField]
    private TextMeshProUGUI chatGPTAnswer;

    [SerializeField]
    [TextArea(5, 12)]
    private string prompt;

    public void Execute()
    {
        StartCoroutine(ChatGPTClient.Instance.Ask(prompt, (r) => ProcessResponse(r)));
    }

    public void ProcessResponse(ChatGPTResponse response)
    {
        Logger.Instance.LogInfo(response.Data);
        //Roslyn run the code
        RoslynCodeRunner.Instance.RunCode(response.Data);
    }
}