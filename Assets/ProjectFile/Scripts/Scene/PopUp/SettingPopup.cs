using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    // Start is called before the first frame update
    public Button ExitButton;


    private void Awake()
    {
        ExitButton.onClick.AddListener(OnExit);

    }

    // Update is called once per frame
    void OnExit()
    {
        var settingManager = FindObjectOfType<SettingManager>();
        if (!settingManager)
        {
            Debug.LogError("Manager Not Found!");
            return;
        }
        settingManager.PopupOnOff();
    }
}