using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenderSelcetToggle : MonoBehaviour
{
    public Toggle femaleToggle;
    public Toggle maleToggle;
    public CharacterCreateScene characterCreateScene;

    void Start()
    {
        femaleToggle.onValueChanged.AddListener(OnFemaleToggleChanged);
        maleToggle.onValueChanged.AddListener(OnMaleToggleChanged);
    }

    private void OnMaleToggleChanged(bool isOn)
    {
        if (isOn)
        {
            characterCreateScene.SetPlayerGenderType("Male");
        }
    }

    private void OnFemaleToggleChanged(bool isOn)
    {
        if (isOn)
        {
            characterCreateScene.SetPlayerGenderType("Female");
        }
    }
    
    public void SetLooksIndex(int index)
    {
        characterCreateScene.SetPlayerGenderlooks(index);
    }
}