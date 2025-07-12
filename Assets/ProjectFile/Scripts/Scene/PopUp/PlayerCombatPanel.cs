using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatPanel : MonoBehaviour
{
    private PlayerStats currentplayer;
    [Header("Player UI")] public Image playerImage;
    public TextMeshProUGUI playerNameText;
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerDefenseText;

    [Header("Interaction UI")] 
    public GameObject dicePanel;
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
    public event Action PlayerEndAction;

    public void InitPanel(PlayerStats playerstats)
    {
        currentplayer = playerstats;
        startEndButtonText.text = "전투시작";
        dicePanel.SetActive(false);
        startEndPanel.SetActive(true);
    }

    public void BattleStart()
    {
        dicePanel.SetActive(true);
        startEndPanel.SetActive(false);
        // 배틀시작후 주사위 굴리기는 이상태로 쭉보이기?
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

    public void TurnEnd()
    {
        isMyTurnReady = false;
        speedSlider.value = 0;
        RegenSpeedSlider();
        PlayerEndAction?.Invoke();
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
        isMyTurnReady = true;
        rollDiceButton.interactable = true;
        DiceCanvasGroup.alpha = 1f;
        ButtonCanvasGroup.alpha = 1f;
        speedCoroutine = null;
        PlayerAttackReady?.Invoke(); // 공격준비 델리게이트
    }

    public void BattleEnd(string resultText)
    {
        dicePanel.SetActive(false);
        startEndPanel.SetActive(true);
        startEndButtonText.text = resultText;
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

    public void EnemyAttacked()
    {
        StopSpeedSlider();
        // 적의공격이 들어올때 호출
    }
}