using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ToggleEntry
{
    public Image looksImage;
    public Toggle toggle;
}
public class PlayerLooksToggle : MonoBehaviour
{
    public List<ToggleEntry> Toggles;

    public GenderSelcetToggle genderSelectToggle;

    void Start()
    {
        foreach (var toggle in Toggles)
        {
            toggle.toggle.onValueChanged.AddListener((isOn) =>
            {
                OnLooksToggleChanged(isOn, toggle.looksImage.sprite);
            });
        }
    }

    private void OnLooksToggleChanged(bool isOn, Sprite looksSprite)
    {
        if (isOn)
        {
            genderSelectToggle.SetLookssprite(looksSprite);
        }
    }
}