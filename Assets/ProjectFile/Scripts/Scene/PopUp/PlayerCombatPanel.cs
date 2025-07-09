using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatPanel : MonoBehaviour
{
    public Image playerImage;
    public TextMeshProUGUI playerNameText;
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerDefenseText;

    public GameObject dicePanel;
    public GameObject resultPanel;

    public Button rollDiceButton;
    public Button giveUpButton;

    public Button exitButton;
    public TextMeshProUGUI exitButtonText;

    public void BattleStart()
    {
        dicePanel.SetActive(true);
        resultPanel.SetActive(false);
    }

    public void BattleEnd(string resultText)
    {
        dicePanel.SetActive(false);
        resultPanel.SetActive(true);
        exitButtonText.text = resultText;
    }

    public void UpdatePlayerUI(PlayerStats playerStats)
    {
        playerImage.sprite = playerStats.playerStateBlock.looksSprite;
        playerNameText.text = playerStats.playerStateBlock.playerName;
        playerHealthSlider.maxValue = playerStats.playerStateBlock.maxHp;
        playerHealthSlider.value = playerStats.playerStateBlock.playerStatus[StateType.Hp];
        playerHealthText.text = playerStats.playerStateBlock.playerStatus[StateType.Hp].ToString();
        playerAttackText.text = playerStats.playerStateBlock.attack.ToString();
        playerDefenseText.text = playerStats.playerStateBlock.defense.ToString();
    }
}