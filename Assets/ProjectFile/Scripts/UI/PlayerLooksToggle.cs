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
        for (int i = 0; i < Toggles.Count; i++)
        {
            int index = i;
            Toggles[i].toggle.onValueChanged.AddListener((isOn) =>
            {
                OnLooksToggleChanged(isOn, Toggles[index].looksImage.sprite, index);
            });
        }
    }

    private void OnLooksToggleChanged(bool isOn, Sprite looksSprite, int index)
    {
        if (isOn)
        {
            genderSelectToggle.SetLookssprite(looksSprite);
            genderSelectToggle.SetCurrentSelectedIndex(index);
        }
    }

}