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
    private Monster monster;

    [Header("Panels")] public EnemyCombatPanel enemyPanel;
    public TextMeshProUGUI logPanel;
    public PlayerCombatPanel playerPanel;

    private Action onExit;

    public void Initialize(PlayerStats playerRef, Monster monsterRef, Action callback)
    {
        player = playerRef;
        monster = monsterRef;

        settingPopup = SettingManager.Instance._initPrefab;
        settingPopup.NewGame += CloseCombat;
        onExit = callback;

        playerPanel.startEndButton.onClick.AddListener(StartBattle);
        playerPanel.giveUpButton.onClick.AddListener(CombatEnd);
        playerPanel.rollDiceButton.onClick.AddListener(RollDice);

        InitBattle();
        UpdateCombatUI(player, monster);
    }

    public void InitBattle()
    {
        playerPanel.InitPanel();
        enemyPanel.InitPanel();

        playerPanel.startEndButton.onClick.RemoveAllListeners();
        playerPanel.startEndButton.onClick.AddListener(StartBattle);
    }

    private void StartBattle()
    {
        playerPanel.BattleStart();
        enemyPanel.BattleStart();
    }

    private void RollDice()
    {
    }

    public void CombatEnd() // combatManager에서 호출됨
    {
        // 승패에따라 UI만 컨트롤 (보상은 CombatManager에서 처리)
        playerPanel.BattleEnd("전투결과");
        enemyPanel.BattleEnd();
        playerPanel.startEndButton.onClick.RemoveAllListeners();
        playerPanel.startEndButton.onClick.AddListener(CloseCombat);
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