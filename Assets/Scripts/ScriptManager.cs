using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ScriptParamData
{
    public string name;
    public Func<float> Get;
    public Action<float> Set;
    // TODO: add reset method?

    public ScriptParamData(string name, Func<float> Get, Action<float> Set)
    {
        this.name = name;
        this.Get = Get;
        this.Set = Set;
    }
}

public interface ChatGPTScript
{
    public ScriptParamData[] GetParams();
    public string GetName();
    public void DestroyScript();
    // TODO: add method to duplicate the script?

    public void SetActive(bool active);

    public bool GetActive();
}


public class ScriptManager : MonoBehaviour
{
    private List<ChatGPTScript> scripts = new List<ChatGPTScript>();
    private List<string> prompts = new List<string>();
    private int currentScriptIndex;

    private ScriptParamData[] paramData;
    private int currentParamIndex;

    [SerializeField] private TMP_Text scriptLabel;
    [SerializeField] private Text prompt;
    [SerializeField] private TMP_Text indexIndicator;
    [SerializeField] private TMP_Text paraName;
    [SerializeField] private TMP_Text paraValue;

    // called by button on the UI
    public void DestroyCurrentScript()
    {
        ChatGPTScript script = this.scripts[this.currentScriptIndex];
        this.scripts.Remove(script);
        script.DestroyScript();
        // TODO: update UI
       if (currentScriptIndex >= scripts.Count)
        {
            currentScriptIndex = scripts.Count - 1;
        }
        ShowScript(currentScriptIndex);
        ShowParam(0);
    }

    public void DisableCurrentScript()
    {
        this.scripts[this.currentScriptIndex].SetActive(false);
    }

    public void EnableCurrentScript()
    {
        this.scripts[this.currentScriptIndex].SetActive(true);
    }

    // called by button on the UI
    public void ShowNextScript()
    {
        if (this.currentScriptIndex + 1 < this.scripts.Count) this.ShowScript(this.currentScriptIndex + 1);
    }

    // called by button on the UI
    public void ShowPreviousScript()
    {
        if (this.currentScriptIndex > 0) this.ShowScript(this.currentScriptIndex - 1);
    }

    private void ShowScript(int index, bool forceParamUpdate=false)
    {
        if (index < 0) 
        {
            currentScriptIndex = 0;
            scriptLabel.text = "";
            indexIndicator.text = $"0/0";
            return;
        }

        bool changed = index != currentScriptIndex;
        currentScriptIndex = index;
        scriptLabel.text = prompts[index];
        indexIndicator.text = $"{index + 1}/{scripts.Count}";
        // update UI to show name of the new script param (check if the script has at least one param)
        if (changed || forceParamUpdate)
        {
            paramData = scripts[currentScriptIndex].GetParams(); // update the parameter list for a new script
            // reset the parameter index to 0 when switching to a new script
            ShowParam(0);
        }
    }

    private void ShowParam(int index)
    {
        currentParamIndex = index;
        if (paramData.Length >= index)
        {
            paraName.text = paramData[currentParamIndex].name;
            paraValue.text = paramData[currentParamIndex].Get().ToString();
        }
        else
        {
            paraName.text = "";
            paraValue.text = "";
        }
    }

    public void AddScript(ChatGPTScript newScript)
    {
        scripts.Add(newScript);
        prompts.Add(prompt.text);

        ShowScript(currentScriptIndex, true);
        ShowParam(currentParamIndex);
    }

    // called by different buttons on the UI
    public void ChangeCurrentParam(float delta)
    {
        float current = this.paramData[this.currentParamIndex].Get();
        this.paramData[this.currentParamIndex].Set(current + delta);
        ShowParam(currentParamIndex);
    }

    public void ShowNextParam()
    {
        if (this.currentParamIndex + 1 < this.paramData.Length) this.ShowParam(this.currentParamIndex + 1);
    }

    // called by button on the UI
    public void ShowPreviousParam()
    {
        if (this.currentParamIndex > 0) this.ShowParam(this.currentParamIndex - 1);
    }
}