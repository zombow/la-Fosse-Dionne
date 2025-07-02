using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameAlertPopup : MonoBehaviour
{
    public Button exitButton;
    public Button newGameButton;

    void Start()
    {
        exitButton.onClick.AddListener(OnExit);
    }

    private void OnExit()
    {
        gameObject.SetActive(false);
    }
}