using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    private SettingManager settingManager;
    [Header("Exit")] public Button exitButton;
    [Header("New Game")] public Button newGameButton;
    public NewGameAlertPopup newGameAlertPopup;
    [HideInInspector] public NewGameAlertPopup newGameAlertPopupInstance;

    [Header("FontSize")] public TextMeshProUGUI fontSizeText;
    public TextMeshProUGUI fontText;
    public Button fontSizeUpButton;
    public Button fontSizeDownButton;

    [Header("ThanksTo")] public Button thanksToButton;
    public ThanksToPopup thanksToPopup;
    private ThanksToPopup thanksToPopupInstance;


    public event Action NewGame;

    public void InitPopup()
    {
        settingManager = SettingManager.Instance;
        exitButton.onClick.AddListener(OnExit);
        newGameButton.onClick.AddListener(() => SetPopup(newGameAlertPopupInstance.gameObject));

        newGameAlertPopupInstance = Instantiate(newGameAlertPopup, transform);
        newGameAlertPopupInstance.gameObject.SetActive(false);
        newGameAlertPopupInstance.newGameButton.onClick.AddListener(OnNewGame);

        fontSizeUpButton.onClick.AddListener(() => FontSizeChange(1));
        fontSizeDownButton.onClick.AddListener(() => FontSizeChange(-1));
        thanksToButton.onClick.AddListener(() => SetPopup(thanksToPopupInstance.gameObject));
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

    private void FontSizeChange(int changeValue)
    {
        float currentSize = fontSizeText.fontSize;
        float newSize = currentSize + changeValue;

        if (newSize >= 24 && newSize <= 42)
        {
            fontSizeText.fontSize = newSize;
            fontSizeText.text = newSize.ToString();
            fontText.fontSize = newSize;
            settingManager.FontSizeChanged?.Invoke(newSize);
        }
    }

    public void OnNewGame()
    {
        NewGame?.Invoke();
        settingManager.FontSizeChanged = null;
        var sceneManager = FindObjectOfType<SceneManager>();
        if (!sceneManager)
        {
            Debug.LogError("SceneManager Not Found!");
            return;
        }
        newGameAlertPopupInstance.gameObject.SetActive(false);
        gameObject.SetActive(false);
        sceneManager.ChangeScene(SceneType.Start);
    }


}