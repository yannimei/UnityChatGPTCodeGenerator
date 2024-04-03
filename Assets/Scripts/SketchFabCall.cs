using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class SketchFabCall : MonoBehaviour
{
    public string email = "mei_yanni@163.com";
    public string password = "Myn19951225.";
    public string accessToken;
    public List<SketchfabModel> m_ModelList;

    public Button searchButton;
    public Text inputKeyowrd;

    // Start is called before the first frame update
    void Start()
    {
        // Add a listener to the button to handle clicks
        searchButton.onClick.AddListener(OnSearchButtonClicked);

        //Log in the sketchfab account
        this.LogIn();
    }

    public void LogIn()
    {
        SketchfabAPI.GetAccessToken(email, password, (SketchfabResponse<SketchfabAccessToken> answer) =>
        {
            if (answer.Success)
            {
                accessToken = answer.Object.AccessToken;
                SketchfabAPI.AuthorizeWithAccessToken(answer.Object);
                Debug.Log("success");
            }
            else
            {
                Debug.LogError(answer.ErrorMessage);
            }

        });
    }

    public void GetModel(string _uid)
    {
        // This first call will get the model information
        bool enableCache = true;
        SketchfabAPI.GetModel(_uid, (resp) =>
        {
            // This second call will get the model information, download it and instantiate it
            SketchfabModelImporter.Import(resp.Object, (obj) =>
            {
                if (obj != null)
                {
                    // Here you can do anything you like to obj (A unity game object containing the sketchfab model)
                    obj.name = inputKeyowrd.text;
                }
            }, enableCache);
        }, enableCache);
    }

    public void SearchModels(string[] keywords, Action<string> onModelFoundCallback, bool isDownloadable = true)
    {
        UnityWebRequestSketchfabModelList.Parameters p = new UnityWebRequestSketchfabModelList.Parameters
        {
            downloadable = isDownloadable
        };

        SketchfabAPI.ModelSearch((_answer) =>
        {
            if (_answer.Success)
            {
                // Assuming 'm_ModelList' is where you want to store your models
                m_ModelList = _answer.Object.Models;
                Debug.Log("Successfully found " + m_ModelList[0].Uid);
                string _uid = m_ModelList[0].Uid;
                onModelFoundCallback?.Invoke(_uid); // Call the callback function with the _uid
            }
            else
            {
                // Handle errors (e.g., log them, notify the user)
                Debug.LogError($"Error retrieving models: {_answer.ErrorMessage}");
            }
        }, p, keywords);
    }

    public void OnSearchButtonClicked()
    {
        // Retrieve the keyword(s) from the input field
        string[] keywords = new string[] { inputKeyowrd.text };

        // Call SearchModels with the keyword(s) and a callback
        SearchModels(keywords, (_uid) =>
        {
            GetModel(_uid); // Implement GetModel as shown previously
        });
    }

}





