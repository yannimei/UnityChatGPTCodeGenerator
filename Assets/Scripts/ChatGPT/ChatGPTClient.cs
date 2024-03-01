using System.Collections;
using UnityEngine;
using DilmerGames.Core.Singletons;
using Newtonsoft.Json;
using UnityEngine.Networking;
public class ChatGPTClient : Singleton<ChatGPTClient>
{
    [SerializeField]
    private ChatGTPSetting chatGPTSetting;

    //what is callback
    public IEnumerator Ask (string prompt, System.Action<ChatGPTResponse> callback)
    {
        var url = chatGPTSetting.apiURL;

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ChatGPTRequest
            {
                Question = prompt,
                //TODO add reminders to the question
            }));

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;
            request.disposeUploadHandlerOnDispose = true;
            request.disposeCertificateHandlerOnDispose = true;

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log(request.error);

                string responseInfo = request.downloadHandler.text;
                var response = new ChatGPTResponse { Data = responseInfo };
                callback(response);
            }

        }
    }
   
}
