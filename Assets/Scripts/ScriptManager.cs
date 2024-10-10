using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ChatGPTScript
{
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
    [SerializeField] private TMP_Text scriptLabel;
    [SerializeField] private Text prompt;
    [SerializeField] private TMP_Text indexIndicator;

    // called by button on the UI
    public void DestroyCurrentScript()
    {
        ChatGPTScript script = this.scripts[this.currentScriptIndex];
        this.scripts.Remove(script);
        script.DestroyScript();
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

    private void ShowScript(int index)
    {
        currentScriptIndex = index;
        scriptLabel.text = prompts[index];
        // TODO: update UI to show name of the new script param (check if the script has at least one param)
        indexIndicator.text = $"{index+1}/{scripts.Count}";
    }

    public void AddScript(ChatGPTScript newScript)
    {
        scripts.Add(newScript);
        prompts.Add(prompt.text);

        ShowScript(currentScriptIndex);
    }
}