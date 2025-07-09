using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPopup : MonoBehaviour
{
    SettingPopup settingPopup;
    private PlayerStats player;

    [Header("Panels")] public EnemyCombatPanel enemyPanel;
    public TextMeshProUGUI logPanel;
    public PlayerCombatPanel playerPanel;

    [Header("Interaction")] public GameObject DicePanel;
    public GameObject ResultPanel;
    public Button exitButton;

    private Action onExit;

    public void Initialize(PlayerStats playerRef, Monster monsterRef, Action callback)
    {
        player = playerRef;

        settingPopup = SettingManager.Instance._initPrefab;
        settingPopup.NewGame += CloseCombat;
        onExit = callback;
        exitButton.onClick.AddListener(CloseCombat);
        
        playerPanel.rollDiceButton.onClick.AddListener(RollDice);
    }

    private void RollDice()
    {
        
    }

    public void CombatStart()
    {
        playerPanel.BattleStart();
    }
    
    public void CombatEnd()
    {
        // 승패에따라 UI만 컨트롤 (보상은 CombatManager에서 처리)
        playerPanel.BattleEnd("전투결과");
    }

    public void CloseCombat()
    {
        settingPopup.NewGame -= CloseCombat;
        player.playerStateBlock.playerStatus[StateType.Hp] = player.playerStateBlock.maxHp; // 전투 종료시 플레이어 체력 회복
        Destroy(gameObject);
        onExit?.Invoke();
    }

    public void UpdateCombatUI(PlayerStats playerStats, Monster monster)
    {
        playerPanel.UpdatePlayerUI(playerStats);
        enemyPanel.UpdateEnemyUI(monster);
    }


}