using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour, ChatGPTScript
{
    public float speed;
    public void SetSpeed(float speed) 
    { 
        this.speed = speed;
    }

    public float GetSpeed() 
    {
        return speed;
    }

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

    public ScriptParamData[] GetParams()
    {
        return new ScriptParamData[] {new ScriptParamData("speed", this.GetSpeed, this.SetSpeed)};
    }

    public void SetActive(bool active)
    {
        this.enabled = active;
    }
}

