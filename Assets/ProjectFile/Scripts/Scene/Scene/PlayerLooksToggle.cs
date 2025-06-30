using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ToggleEntry
{
    public int looksIndex;
    public Toggle toggle;
}
public class PlayerLooksToggle : MonoBehaviour
{
    public List<ToggleEntry> Toggles;

    public GenderSelcetToggle genderSelcetToggle;

    void Start()
    {
        foreach (var toggle in Toggles)
        {
            toggle.toggle.onValueChanged.AddListener((isOn) =>
            {
                OnLooksToggleChanged(isOn, toggle.looksIndex);
            });
        }
    }

    private void OnLooksToggleChanged(bool isOn, int looksIndex)
    {
        if (isOn)
        {
            genderSelcetToggle.SetLooksIndex(looksIndex);
        }
    }
}