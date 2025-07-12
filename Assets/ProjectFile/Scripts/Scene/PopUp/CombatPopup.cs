using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class CombatPopup : MonoBehaviour
{
    SettingPopup settingPopup;
    private PlayerStats player;
    private Monster monster;

    [Header("Panels")] public EnemyCombatPanel enemyPanel;
    public TextMeshProUGUI logPanel;
    public PlayerCombatPanel playerPanel;
    [SerializeField] float frameDelay = 0.1f; // 애니메이션 프레임 딜레이
    private int RandomDice;
    private Coroutine currentAnimationCoroutine;
    private event Action onExit;
    public event Action CheckCombatEnd;

    public void Initialize(PlayerStats playerRef, Monster monsterRef, Action callback)
    {
        player = playerRef;
        monster = monsterRef;

        settingPopup = SettingManager.Instance._initPrefab;
        settingPopup.NewGame += CloseCombat;
        onExit = callback;

        playerPanel.startEndButton.onClick.RemoveAllListeners();
        playerPanel.startEndButton.onClick.AddListener(StartBattle);
        playerPanel.giveUpButton.onClick.AddListener(CombatEnd);
        playerPanel.rollDiceButton.onClick.AddListener(RollDice);

        InitBattle();
        playerPanel.PlayerAttackReady += enemyPanel.PlayerAttacked;
        playerPanel.PlayerEndAction += enemyPanel.PlayerAttackEnd;
        enemyPanel.EnemyAttackReady += playerPanel.EnemyAttacked;
        enemyPanel.EnemyAttackReady += MonsterAttack;
        enemyPanel.EnemyEndAction += EndAction;
        UpdateCombatUI(player, monster);
    }

    public void InitBattle()
    {
        playerPanel.InitPanel(player);
        enemyPanel.InitPanel(monster);
    }

    private void StartBattle()
    {
        playerPanel.BattleStart();
        enemyPanel.BattleStart();
    }


    private void RollDice()
    {
        RandomDice = Random.Range(1, 20);
        playerPanel.rollDiceButton.interactable = false;
        currentAnimationCoroutine = null;
        currentAnimationCoroutine = StartCoroutine(PlayOnceThenFreezeLast(AssetManager.Instance.LoadDiceRolling(RandomDice)));
    }

    private IEnumerator PlayOnceThenFreezeLast(List<Sprite> frames)
    {
        foreach (var sprite in frames)
        {
            playerPanel.diceImage.sprite = sprite;
            yield return new WaitForSeconds(frameDelay);
        }

        // 마지막 프레임에서 멈춤
        playerPanel.diceImage.sprite = frames[^1];

        PlayerAttack();
    }

    public void CombatEnd() // combatManager에서 호출됨
    {
        // 승패에따라 UI만 컨트롤 (보상은 CombatManager에서 처리)
        playerPanel.BattleEnd("전투결과");
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

    public void PlayerAttack()
    {
        int damage = Mathf.Max(1,
            player.playerStateBlock.playerStatus[StateType.Strength] - monster.combatStats.defense + RandomDice); // dice 값을 단순히 더하는정도
        enemyPanel.PlayEffect("smoke", damage);
        enemyPanel.PlayAnimation(AnimationType.Hurt);
    }

    public void EndAction()
    {
        UpdateCombatUI(player, monster);
        CheckCombatEnd?.Invoke();
    }

    public void MonsterAttack()
    {
        int damage = Mathf.Max(1, monster.combatStats.attack - player.playerStateBlock.playerStatus[StateType.Strength] / 2);
        player.playerStateBlock.playerStatus[StateType.Hp] -= damage;
    }
}