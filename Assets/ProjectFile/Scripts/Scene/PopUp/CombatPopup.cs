using System;
using TMPro;
using UnityEngine;


public class CombatPopup : MonoBehaviour
{
    SettingPopup settingPopup;
    private PlayerStats player;
    private Monster monster;

    [Header("Panels")] public EnemyCombatPanel enemyPanel;
    public TextMeshProUGUI logPanel;
    public PlayerCombatPanel playerPanel;

    private bool isPlayerReady = false;
    private bool isEnemyReady = false;
    private bool isResolvingTurn = false;
    private event Action onExit;
    public event Action CheckCombatEnd;

    public void Initialize(PlayerStats playerRef, Monster monsterRef, Action callback)
    {
        player = playerRef;
        monster = monsterRef;

        settingPopup = SettingManager.Instance._initPrefab;
        settingPopup.NewGame += CloseCombat;
        onExit = callback;

        InitBattle();
        playerPanel.CombatStart += StartBattle;
        playerPanel.CombatEnd += EndBattle;
        playerPanel.CombatClose += CloseCombat;

        playerPanel.PlayerAttackReady += PlayerAttackReady;
        playerPanel.PlayerAttack += PlayerAttack;
        enemyPanel.EnemyEndReaction += PlayerEndTurn;

        enemyPanel.EnemyAttackReady += EnemyAttackReady;
        enemyPanel.EnemyAttack += EnemyAttack;
        playerPanel.PlayerEndReaction += EnemyEndTurn;

        UpdateCombatUI(player, monster);

        logPanel.text = monster.name + "과 전투를 시작합니다!";
    }


    public void InitBattle()
    {
        playerPanel.InitPanel(player);
        enemyPanel.InitPanel(monster);
    }

    private void StartBattle()
    {
        logPanel.text = " ";
        playerPanel.BattleStart();
        enemyPanel.BattleStart();
    }


    public void EndBattle(bool bwin) // combatManager에서 호출됨
    {
        if (bwin) // 승패에따라 UI만 컨트롤 (보상은 CombatManager에서 처리)
        {
            playerPanel.BattleEnd("전투 승리!");
            logPanel.text = "적이 쓰러졋습니다";
        }
        else
        {
            playerPanel.BattleEnd("패배...");
            logPanel.text = "당신은 쓰러졌습니다.";
        }

        enemyPanel.BattleEnd();
        playerPanel.startEndButton.onClick.RemoveAllListeners();
        playerPanel.startEndButton.onClick.AddListener(CloseCombat);
        CheckCombatEnd = null;
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

    private void PlayerAttackReady()
    {
        logPanel.text += "\n"+ player.playerStateBlock.playerName + "의 턴!";
        isPlayerReady = true;
        TryResolveTurn();
    }


    private void EnemyAttackReady()
    {
        // 적의 speed 게이지가 가득찼을때 호출됨
        logPanel.text += "\n"+ monster.name + "의 공격!";
        isEnemyReady = true;
        TryResolveTurn();
    }

    private void TryResolveTurn()
    {
        if (isResolvingTurn)
            return;

        if (isPlayerReady)
        {
            isResolvingTurn = true;

            // 플레이어 공격 처리
            playerPanel.AttackReady();
            enemyPanel.PlayerAttackReady();

            // isPlayerReady만 초기화 — 적이 준비됐어도 다음 프레임으로 미룸
            isPlayerReady = false;

            // 공격 후 턴 종료 처리에서 적의 공격으로 넘어감
        }
        else if (isEnemyReady)
        {
            isResolvingTurn = true;

            playerPanel.EnemyAttackReady();
            enemyPanel.AttackReady();

            isEnemyReady = false;
        }
    }

    public void PlayerEndTurn(int damage)
    {
        logPanel.text += "\n" + monster.name + "은 " + damage.ToString() + "의 피해를 입었다.";
        playerPanel.TurnEnd();
        enemyPanel.PlayerTurnEnd();
        UpdateCombatUI(player, monster);
        isResolvingTurn = false;
        TryResolveTurn(); // 적이 준비됐던 상태면 이어서 실행
        CheckCombatEnd?.Invoke();
    }

    public void EnemyEndTurn(int damage)
    {
        logPanel.text += "\n" + player.playerStateBlock.playerName + "은 " + damage.ToString() + "의 피해를 입었다.";
        enemyPanel.TurnEnd();
        playerPanel.EnemyTurnEnd();
        UpdateCombatUI(player, monster);
        isResolvingTurn = false;
        TryResolveTurn(); // 플레이어가 준비됐던 상태면 이어서 실행
        CheckCombatEnd?.Invoke();
    }

    public void PlayerAttack(int dicenumber, string effectName = "smoke")
    {
        int damage = Mathf.Max(1,
            player.playerStateBlock.playerStatus[StateType.Strength] - monster.combatStats.defense + dicenumber); // dice 값을 단순히 더하는정도
        enemyPanel.EnemyGetHit(damage, effectName);
    }

    public void EnemyAttack()
    {
        int damage = Mathf.Max(1, monster.combatStats.attack - player.playerStateBlock.playerStatus[StateType.Strength] / 2);
        playerPanel.PlayerGetHit(damage);
    }
}