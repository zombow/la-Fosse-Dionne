using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    [Header("Exit")]
    public Button exitButton;
    [Header("New Game")]
    public Button newGameButton;
    public NewGameAlertPopup newGameAlertPopup;
    private NewGameAlertPopup newGameAlertPopupInstance;
    [Header("ThanksTo")]
    public Button thanksToButton;
    public ThanksToPopup thanksToPopup;
    private ThanksToPopup thanksToPopupInstance;

    private void Start()
    {
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
        var settingManager = FindObjectOfType<SettingManager>();
        if (!settingManager)
        {
            Debug.LogError("Manager Not Found!");
            return;
        }

        settingManager.PopupOnOff();
    }

    private void OnNewGame()
    {
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