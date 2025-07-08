using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LookSprites
{
    public Sprite LooksSprites;
    public Sprite SelectSprites;
}
public class GenderSelcetToggle : MonoBehaviour
{
    public Toggle femaleToggle;
    public Toggle maleToggle;
    public CharacterCreateScene characterCreateScene;

    public List<Image> looksImages;
    public List<Image> selectLooksImages;
    public List<LookSprites> femaleLooksSprites;
    public List<LookSprites> maleLooksSprites;

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
        List<LookSprites> targetList = (genderType == "Female") ? femaleLooksSprites : maleLooksSprites;

        for (int i = 0; i < looksImages.Count; i++)
        {
            looksImages[i].sprite = targetList[i].LooksSprites;
            selectLooksImages[i].sprite = targetList[i].SelectSprites;
        }

        // 현재 선택된 위치의 looksSprite를 다시 Set
        SetLookssprite(targetList[currentSelectedIndex].LooksSprites);
    }

    public void SetCurrentSelectedIndex(int index)
    {
        currentSelectedIndex = index;
    }
}