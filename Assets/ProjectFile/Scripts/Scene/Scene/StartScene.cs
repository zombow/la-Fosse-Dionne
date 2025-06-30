using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button startButton;
    public Button settingButton;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStart);
        settingButton.onClick.AddListener(OnSetting);
    }

    private void OnSetting()
    {
        var settingManager = FindObjectOfType<SettingManager>();
        if (!settingManager)
        {
            Debug.LogError("Manager Not Found!");
            return;
        }
        settingManager.PopupOnOff();
    }

    private void OnStart()
    {
        var uiManager = FindObjectOfType<SceneManager>();
        if (!uiManager)
        {
            Debug.LogError("Manager Not Found!");
            return;
        }

        uiManager.ChangeScene(SceneType.CharacterCreate);
    }
}