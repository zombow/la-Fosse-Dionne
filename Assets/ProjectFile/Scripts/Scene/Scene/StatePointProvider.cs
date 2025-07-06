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
    public List<StateGroup> Status;
    public Dictionary<StateType, StateGroup> StatesDictionary = new Dictionary<StateType, StateGroup>();


    void Start()
    {
        foreach (var state in Status)
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
                statePoint -= pointvalue;
                StatesDictionary[stateType].stateValue += pointvalue;
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
        StateType[] allStates = (StateType[])System.Enum.GetValues(typeof(StateType));
        List<StateType> filteredStates = new List<StateType>();
        foreach (StateType state in allStates) // Mortality제외
        {
            if (state != StateType.Mortality)
            {
                filteredStates.Add(state);
            }
        }

        while (statePoint > 0)
        {
            int randomPoints = Random.Range(1, statePoint);


            StateType randomState = (StateType)Random.Range(0, filteredStates.Count);

            StatePointEdit(randomState, randomPoints);
        }

        UpdateStatePointText();
    }

    public void UpdateStatePointText()
    {
        statePointText.text = $"({statePoint})";
        characterCreateScene.SetPlayerStatePoint(GetStatePointDictionary());
    }

    private Dictionary<StateType, int> GetStatePointDictionary()
    {
        Dictionary<StateType, int> temp = new Dictionary<StateType, int>();
        foreach (var Value in StatesDictionary)
        {
            temp.Add(Value.Key, Value.Value.stateValue);
        }
        
        temp.Add(StateType.Mortality, 0); // Mortality는 0으로 설정
        return temp;
    }
}