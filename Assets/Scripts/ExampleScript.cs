using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour, ChatGPTScript
{
    public void DestroyScript()
    {
        Destroy(this);
    }

    public bool GetActive()
    {
        return this.enabled;
    }

    public string GetName()
    {
        return "scriptname";
    }

    public void SetActive(bool active)
    {
        this.enabled = active;
    }
}

