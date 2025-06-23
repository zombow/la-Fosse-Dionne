using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    // Start is called before the first frame update
    public SettingPopup settingPrefab;
    private SettingPopup _initPrefab;

    public void Start()
    {
        _initPrefab = Instantiate(settingPrefab, transform);
        _initPrefab.gameObject.SetActive(false);
    }

    public void PopupOnOff()
    {
        if (_initPrefab == null)
        {
            Debug.LogError("Setting Popup Prefab is not initialized!");
            return;
        }


        _initPrefab.gameObject.SetActive(!_initPrefab.gameObject.activeSelf);
    }
}