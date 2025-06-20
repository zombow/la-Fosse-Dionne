using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button startButton;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStart);
    }

    private void OnStart()
    {
        var uiManager = FindObjectOfType<SceneManager>();
        uiManager.ChangeScene();
    }
}
