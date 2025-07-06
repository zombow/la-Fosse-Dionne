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
    [Header("Character")] public Image characterImage;
    public TextMeshProUGUI characterNameText;
    public List<Image> hpGroup; // 나중에 자료형을 바꿔야할것같음
    public List<Image> spritGroup; // 나중에 자료형을 바꿔야할것같음
    public List<StateTextUI> stateText;

    public void InfoInit(PlayerStateBlock playerInfo)
    {
        characterImage.sprite = playerInfo.looksSprite;
        characterNameText.text = playerInfo.playerName;
        Dictionary<StateType, TextMeshProUGUI> stateTextDictionary = new Dictionary<StateType, TextMeshProUGUI>();
        foreach (var state in stateText)
        {
            stateTextDictionary.Add(state.stateType, state.stateText);
        }

        foreach (var state in stateText)
        {
            stateTextDictionary[state.stateType].text = playerInfo.playerStatus[state.stateType].ToString();
        }
    }
}