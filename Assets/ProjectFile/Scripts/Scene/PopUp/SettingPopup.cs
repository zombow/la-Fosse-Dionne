using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    private SettingManager settingManager;
    [Header("Exit")]
    public Button exitButton;
    [Header("New Game")]
    public Button newGameButton;
    public NewGameAlertPopup newGameAlertPopup;
    [HideInInspector]
    public NewGameAlertPopup newGameAlertPopupInstance;
    [Header("ThanksTo")]
    public Button thanksToButton;
    public ThanksToPopup thanksToPopup;
    private ThanksToPopup thanksToPopupInstance;

    public event Action NewGame;

    private void Start()
    {
        settingManager = SettingManager.Instance;
        exitButton.onClick.AddListener(OnExit);
        newGameButton.onClick.AddListener(()=>SetPopup(newGameAlertPopupInstance.gameObject));
        
        newGameAlertPopupInstance = Instantiate(newGameAlertPopup, transform);
        newGameAlertPopupInstance.gameObject.SetActive(false);
        newGameAlertPopupInstance.newGameButton.onClick.AddListener(OnNewGame);
        
        thanksToButton.onClick.AddListener(()=>SetPopup(thanksToPopupInstance.gameObject));
        thanksToPopupInstance = Instantiate(thanksToPopup, transform);
        thanksToPopupInstance.gameObject.SetActive(false);
        
    }

    private void SetPopup(GameObject instance)
    {
        instance.SetActive(!instance.activeSelf);
    }

    void OnExit()
    {
        settingManager.PopupOnOff();
    }

    private void OnNewGame()
    {
        NewGame?.Invoke();
        var sceneManager = FindObjectOfType<SceneManager>();
        if (!sceneManager)
        {
            Debug.LogError("SceneManager Not Found!");
            return;
        }
        SetPopup(newGameAlertPopupInstance.gameObject);
        sceneManager.ChangeScene(SceneType.Start);
        OnExit();
    }
}