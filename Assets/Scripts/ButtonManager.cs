using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Button compileButton;
    [SerializeField] private Button[] scriptsButtons;
    [SerializeField] private Button[] paramButtons;
    public void SetCompileButtonInteractable(bool active)
    {
        compileButton.interactable = active;
    }

    public void SetScriptsButtonsInteractable(bool active)
    {
        foreach (Button button in scriptsButtons)
        {
            button.interactable = active;
        }
    }

    public void SetParamButtonsInteractable(bool active)
    {
        foreach (Button button in paramButtons)
        {
            button.interactable = active;
        }
    }
}
