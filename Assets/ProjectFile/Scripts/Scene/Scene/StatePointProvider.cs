using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StateType
{
    Str,
    Dex,
    Int,
    Lux
}

[System.Serializable]
public class StateGroup
{
    public StateType stateType;
    public Button minusButton;
    public TextMeshProUGUI stateText;
    public Button plusButton;
    public int stateValue;
}

public class StatePointProvider : MonoBehaviour
{
    public CharacterCreateScene characterCreateScene;
    public TextMeshProUGUI statePointText;
    public int statePoint = 0;

    public List<StateGroup> States;
    public Dictionary<StateType, StateGroup> StatesDictionary = new Dictionary<StateType, StateGroup>();


    void Start()
    {
        foreach (var state in States)
        {
            StatesDictionary.Add(state.stateType, state);
            state.minusButton.onClick.AddListener(() => StatePointReturn(state.stateType));
            state.plusButton.onClick.AddListener(() => StatePointUse(state.stateType));
        }

        statePoint = characterCreateScene.remainingPoints;
    }

    public void StatePointUse(StateType stateType)
    {
        if (statePoint > 0)
        {
            statePoint--;
            StatesDictionary[stateType].stateValue++;
            StatesDictionary[stateType].stateText.text = StatesDictionary[stateType].stateValue.ToString();
        }
        else
        {
            Debug.LogWarning("No remaining state points to use.");
        }

        UpdateStatePointText();
    }

    public void StatePointReturn(StateType stateType)
    {
        if (StatesDictionary[stateType].stateValue > 0)
        {
            statePoint++;
            StatesDictionary[stateType].stateValue--;
            StatesDictionary[stateType].stateText.text = StatesDictionary[stateType].stateValue.ToString();
        }
        else
        {
            Debug.LogWarning("No points to return for this state.");
        }

        UpdateStatePointText();
    }

    public void ResetStatePoint()
    {
        statePoint = characterCreateScene.baseStatPoints;
        foreach (var state in StatesDictionary)
        {
            StatesDictionary[state.Key].stateValue = 0;
            StatesDictionary[state.Key].stateText.text = StatesDictionary[state.Key].stateValue.ToString();
        }

        UpdateStatePointText();
    }

    public void RandomStatePoint()
    {
        ResetStatePoint();
        while (statePoint > 0)
        {
            int randomPoints = Random.Range(1, statePoint);
            StateType randomState = (StateType)Random.Range(0, System.Enum.GetValues(typeof(StateType)).Length);
            StatesDictionary[randomState].stateValue += randomPoints;
            StatesDictionary[randomState].stateText.text = StatesDictionary[randomState].stateValue.ToString();
            statePoint -= randomPoints;
        }

        
    }

    public void UpdateStatePointText()
    {
        statePointText.text = $"능력치 분배 ({statePoint})";
    }
}