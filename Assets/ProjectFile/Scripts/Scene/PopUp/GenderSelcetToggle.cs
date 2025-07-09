using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

public class GenderSelcetToggle : MonoBehaviour
{
    public Toggle femaleToggle;
    public Toggle maleToggle;
    public CharacterCreateScene characterCreateScene;

    public List<Image> looksSlotImages;

    public List<Sprite> femaleLooksSprites;
    public List<Sprite> maleLooksSprites;

    private int currentSelectedIndex = 0;
    void Start()
    {
        femaleToggle.onValueChanged.AddListener(OnFemaleToggleChanged);
        maleToggle.onValueChanged.AddListener(OnMaleToggleChanged);
        ImageSetSprite("Male");
    }

    private void OnMaleToggleChanged(bool isOn)
    {
        if (isOn)
        {
            characterCreateScene.SetPlayerGenderType("Male");
            ImageSetSprite("Male");
        }
    }

    private void OnFemaleToggleChanged(bool isOn)
    {
        if (isOn)
        {
            characterCreateScene.SetPlayerGenderType("Female");
            ImageSetSprite("Female");
        }
    }

    public void SetLookssprite(Sprite looksSprite)
    {
        characterCreateScene.SetPlayerGenderlooks(looksSprite);
    }

    public void ImageSetSprite(string genderType)
    {
        List<Sprite> targetList = (genderType == "Female") ? femaleLooksSprites : maleLooksSprites;

        for (int i = 0; i < looksSlotImages.Count; i++)
        {
            looksSlotImages[i].sprite = targetList[i];
        }

        // 현재 선택된 위치의 looksSprite를 다시 Set
        SetLookssprite(targetList[currentSelectedIndex]);
    }

    public void SetCurrentSelectedIndex(int index)
    {
        currentSelectedIndex = index;
    }
}