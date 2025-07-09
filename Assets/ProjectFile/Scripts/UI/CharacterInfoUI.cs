using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class StateTextUI
{
    public StateType stateType;
    public TextMeshProUGUI stateText;
}

public class CharacterInfoUI : MonoBehaviour
{
    private PlayerStats playerstate;
    [Header("Character")] public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public List<Image> hpGroup;
    public List<Image> spritGroup;
    public List<StateTextUI> stateText;

    [Header("Heart and Spirit Image")] public Sprite heartImage;
    public Sprite spiritImage;
    public Sprite emptyHeartImage;

    public void InfoInit(PlayerStats playerInfo)
    {
        playerstate = playerInfo; // StoryScene에서 주입
        playerstate.OnStatsChanged += UpdateStatUI;
        characterImage.sprite = playerInfo.playerStateBlock.looksSprite;
        characterNameText.text = playerInfo.playerStateBlock.playerName;

        UpdateStatUI();
    }

    public void UpdateStatUI()
    {
        // 스탯 표시
        Dictionary<StateType, TextMeshProUGUI> stateTextDictionary = new Dictionary<StateType, TextMeshProUGUI>();
        foreach (var state in stateText)
        {
            stateTextDictionary[state.stateType] = state.stateText;
        }

        int point = 0;
        foreach (var state in stateText)
        {
            if (playerstate.playerStateBlock.playerStatus.TryGetValue(state.stateType, out int value))
            {
                point += value + playerstate.GetStatFromEquip(state.stateType);
                stateTextDictionary[state.stateType].text = point.ToString();
            }
            else
            {
                stateTextDictionary[state.stateType].text = "0";
            }
            point = 0;
        }

        // Life 하트 표시
        for (int i = 0; i < hpGroup.Count; i++)
        {
            if (i < playerstate.playerStateBlock.playerStatus[StateType.Life])
                hpGroup[i].sprite = heartImage;
            else
                hpGroup[i].sprite = emptyHeartImage;
        }

        // Spirit 하트 표시
        for (int i = 0; i < spritGroup.Count; i++)
        {
            if (i < playerstate.playerStateBlock.playerStatus[StateType.Spirit])
                spritGroup[i].sprite = spiritImage;
            else
                spritGroup[i].sprite = emptyHeartImage;
        }
    }
}