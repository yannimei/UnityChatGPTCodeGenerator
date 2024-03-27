using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sketchfab;

public class SketchFabCall : MonoBehaviour
{
    public string email = "yannimei951225@gmail.com";
    public string password = "Myn19951225.";
    public string accessToken;

    public string _uid;
    // Start is called before the first frame update
    void Start()
    {
        this.Call();
        //this.GetModel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Call()
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

    public void GetModel()
    {
        // This first call will get the model information
        SketchfabAPI.GetModel(_uid, (resp) =>
        {
            // This second call will get the model information, download it and instantiate it
            SketchfabModelImporter.Import(resp.Object, (obj) =>
            {
                if (obj != null)
                {
                    // Here you can do anything you like to obj (A unity game object containing the sketchfab model)
                }
            });
        });
    }
}

