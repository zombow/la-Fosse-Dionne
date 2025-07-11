using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatPanel : MonoBehaviour
{
    [Header("Player UI")]
    public Image playerImage;
    public TextMeshProUGUI playerNameText;
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerDefenseText;

    [Header("Interaction UI")]
    public GameObject dicePanel;
    public GameObject startEndPanel;
    public Button rollDiceButton;
    public Button giveUpButton;
    public Button startEndButton;
    public TextMeshProUGUI startEndButtonText;

    [Header("Combat UI")] 
    public Slider speedSlider;
    
    private PlayerStats currentplayer;

    public void InitPanel()
    {
        startEndButtonText.text = "전투시작";
        dicePanel.SetActive(false);
        startEndPanel.SetActive(true);
    }

    public void BattleStart()
    {
        dicePanel.SetActive(true);
        startEndPanel.SetActive(false);
        // 배틀시작후 주사위 굴리기는 이상태로 쭉보이기?
        
        
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
        
        speedSlider.maxValue = playerStats.playerStateBlock.speed;
    }


}