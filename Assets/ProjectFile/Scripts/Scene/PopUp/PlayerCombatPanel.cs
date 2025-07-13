using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerCombatPanel : MonoBehaviour
{
    private PlayerStats currentplayer;
    [Header("Player UI")] public Image playerImage;
    public TextMeshProUGUI playerNameText;
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerDefenseText;

    [Header("Interaction UI")] public GameObject dicePanel;
    public Image diceImage;
    public GameObject startEndPanel;
    public Button rollDiceButton;
    public Button giveUpButton;
    public Button startEndButton;
    public TextMeshProUGUI startEndButtonText;
    public CanvasGroup DiceCanvasGroup;
    public CanvasGroup ButtonCanvasGroup;

    [Header("Combat UI")] public Slider speedSlider;
    private Coroutine speedCoroutine;

    private bool isMyTurnReady = false;
    private bool isPaused = false;
    public event Action PlayerAttackReady;
    public event Action<int, string> PlayerAttack; // dagame 넘기기, 무기에따른 effect type도 바뀌도록
    public event Action<int> PlayerEndReaction; // 플레이어가 공격받을때 등 액션이끝날때
    public event Action CombatStart;
    public event Action<bool> CombatEnd;
    public event Action CombatClose;
    
    private Coroutine RollDiceCoroutine;
    [SerializeField] float frameDelay = 0.1f; // 애니메이션 프레임 딜레이

    public void InitPanel(PlayerStats playerstats)
    {
        currentplayer = playerstats;
        startEndButtonText.text = "전투시작";
        dicePanel.SetActive(false);
        startEndPanel.SetActive(true);
        startEndButton.onClick.AddListener(() => CombatStart?.Invoke());
        giveUpButton.onClick.AddListener(() => CombatEnd?.Invoke(false));
        rollDiceButton.onClick.AddListener(RollDice);
    }

    public void BattleStart()
    {
        startEndButton.onClick.RemoveAllListeners();
        startEndButton.onClick.AddListener(() => CombatClose?.Invoke());
        dicePanel.SetActive(true);
        startEndPanel.SetActive(false);
        RegenSpeedSlider();
    }

    public void BattleEnd(string resultText)
    {
        StopSpeedSlider();
        dicePanel.SetActive(false);
        startEndPanel.SetActive(true);
        startEndButtonText.text = resultText;
    }

    private void RollDice()
    {
        int diceNumber = Random.Range(1, 20);
        rollDiceButton.interactable = false;
        RollDiceCoroutine = null;
        RollDiceCoroutine = StartCoroutine(DiceRoll(AssetManager.Instance.LoadDiceRolling(diceNumber), diceNumber));

    }

    private IEnumerator DiceRoll(List<Sprite> frames, int diceNumber)
    {
        foreach (var sprite in frames)
        {
            diceImage.sprite = sprite;
            yield return new WaitForSeconds(frameDelay);
        }

        // 마지막 프레임에서 멈춤
        diceImage.sprite = frames[^1];
        PlayerAttack?.Invoke(diceNumber, "smoke");
    }

    public void AttackReady()
    {
        // 플레이어의 speed 게이지가 가득찼을때 호출됨 
        StopSpeedSlider();
        isMyTurnReady = true;
        rollDiceButton.interactable = true;
        DiceCanvasGroup.alpha = 1f;
        ButtonCanvasGroup.alpha = 1f;
    }

    public void EnemyAttackReady()
    {
        // 적의 speed slider가 가득차면 호출됨
        StopSpeedSlider();
    }

    public void PlayerGetHit(int damage)
    {
        currentplayer.playerStateBlock.playerStatus[StateType.Hp] -= damage;
        PlayerEndReaction?.Invoke(damage);
    }
    public void TurnEnd()
    {
        isMyTurnReady = false;
        speedSlider.value = 0;
        RegenSpeedSlider();
    }

    public void EnemyTurnEnd()
    {
        RegenSpeedSlider();
    }

    public void RegenSpeedSlider()
    {
        isPaused = false;

        if (speedCoroutine == null)
        {
            speedCoroutine = StartCoroutine(FillSpeedSliderCoroutine());
        }

        rollDiceButton.interactable = false;
        DiceCanvasGroup.alpha = 0.5f;
        ButtonCanvasGroup.alpha = 0.5f;
    }

    private IEnumerator FillSpeedSliderCoroutine()
    {
        float target = speedSlider.maxValue;

        while (speedSlider.value < target)
        {
            if (!isPaused)
            {
                speedSlider.value += Time.deltaTime * 100;
            }

            yield return null;
        }

        speedSlider.value = target;
        speedCoroutine = null;
        PlayerAttackReady?.Invoke(); // 공격준비 델리게이트
    }

    public void StopSpeedSlider()
    {
        isPaused = true;

        if (!isMyTurnReady)
        {
            rollDiceButton.interactable = false;
            DiceCanvasGroup.alpha = 0.5f;
            ButtonCanvasGroup.alpha = 0.5f;
        }
    }

    public void UpdatePlayerUI(PlayerStats playerStats)
    {
        currentplayer = playerStats;
        playerImage.sprite = playerStats.playerStateBlock.looksSprite;
        playerNameText.text = playerStats.playerStateBlock.playerName;
        playerHealthSlider.maxValue = playerStats.playerStateBlock.maxHp;
        playerHealthSlider.value = playerStats.playerStateBlock.playerStatus[StateType.Hp];
        playerHealthText.text = playerStats.playerStateBlock.playerStatus[StateType.Hp].ToString();
        playerAttackText.text = playerStats.playerStateBlock.attack.ToString();
        playerDefenseText.text = playerStats.playerStateBlock.defense.ToString();

        speedSlider.maxValue = 100 - playerStats.playerStateBlock.speed;
    }


}