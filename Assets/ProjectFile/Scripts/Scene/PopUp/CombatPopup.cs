using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatPopup : MonoBehaviour
{
    [Header("Panels")] public GameObject enemyPanel;
    public GameObject logPanel;
    public GameObject playerPanel;

    [Header("Interaction")] public GameObject DicePanel;
    public GameObject ResultPanel;
    public Button exitButton;

    private Action onExit;

    public void Initialize(PlayerStats playerRef, Monster monsterRef, Action callback)
    {
        onExit = callback;
        exitButton.onClick.AddListener(CloseCombat);
    }

    public void CombatEnd()
    {
        DicePanel.SetActive(false);
        ResultPanel.SetActive(true);
    }

    public void CloseCombat()
    {
        Destroy(gameObject);
        onExit?.Invoke();
    }
}