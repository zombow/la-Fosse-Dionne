using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class StateGroup
{
    public StateType stateType;
    public Button minusButton;
    public TextMeshProUGUI stateText;
    public Button plusButton;
    public int stateValue;
    public Slider stateValueSlider;
}

public class StatePointProvider : MonoBehaviour
{
    public CharacterCreateScene characterCreateScene;
    public TextMeshProUGUI statePointText;
    public int statePoint = 0;

    public Button resetButton;
    public Button randomButton;
    public List<StateGroup> States;
    public Dictionary<StateType, StateGroup> StatesDictionary = new Dictionary<StateType, StateGroup>();


    void Start()
    {
        foreach (var state in States)
        {
            StatesDictionary.Add(state.stateType, state);
            state.minusButton.onClick.AddListener(() => StatePointEdit(state.stateType, -1));
            state.plusButton.onClick.AddListener(() => StatePointEdit(state.stateType, 1));
            state.stateValueSlider.value = 0;
        }

        resetButton.onClick.AddListener(ResetStatePoint);
        randomButton.onClick.AddListener(RandomStatePoint);

        statePoint = characterCreateScene.remainingPoints;
    }

    public void StatePointEdit(StateType stateType, int pointvalue)
    {
        if (pointvalue > 0)
        {
            if (statePoint > 0)
            {
                statePoint -= pointvalue;
                StatesDictionary[stateType].stateValue += pointvalue;
                StatesDictionary[stateType].stateText.text = StatesDictionary[stateType].stateValue.ToString();
                StatesDictionary[stateType].stateValueSlider.value = StatesDictionary[stateType].stateValue;
            }
            else
            {
                Debug.LogWarning("No remaining state points to use.");
            }
        }
        else
        {
            if (StatesDictionary[stateType].stateValue > 0)
            {
                statePoint += pointvalue;
                StatesDictionary[stateType].stateValue -= pointvalue;
                StatesDictionary[stateType].stateText.text = StatesDictionary[stateType].stateValue.ToString();
                StatesDictionary[stateType].stateValueSlider.value = StatesDictionary[stateType].stateValue;
            }
            else
            {
                Debug.LogWarning("No points to return for this state.");
            }
        }

        characterCreateScene.remainingPoints = statePoint;
        UpdateStatePointText();
    }


    public void ResetStatePoint()
    {
        statePoint = characterCreateScene.baseStatPoints;
        foreach (var state in StatesDictionary)
        {
            StatesDictionary[state.Key].stateValue = 0;
            StatesDictionary[state.Key].stateText.text = StatesDictionary[state.Key].stateValue.ToString();
            StatesDictionary[state.Key].stateValueSlider.value = StatesDictionary[state.Key].stateValue;
        }

        characterCreateScene.remainingPoints = statePoint;
        UpdateStatePointText();
    }

    public void RandomStatePoint()
    {
        ResetStatePoint();
        while (statePoint > 0)
        {
            int randomPoints = Random.Range(1, statePoint);
            StateType randomState = (StateType)Random.Range(0, System.Enum.GetValues(typeof(StateType)).Length);

            StatePointEdit(randomState, randomPoints);
        }

        UpdateStatePointText();
    }

    public void UpdateStatePointText()
    {
        statePointText.text = $"({statePoint})";
    }
}