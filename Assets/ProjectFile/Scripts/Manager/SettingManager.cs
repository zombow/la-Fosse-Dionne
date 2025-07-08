using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }
    public SettingPopup settingPrefab;
    [HideInInspector]
    public SettingPopup _initPrefab;
    public GameObject safeArea;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        _initPrefab = Instantiate(settingPrefab, transform);
        _initPrefab.gameObject.SetActive(false);
        _initPrefab.transform.SetParent(safeArea.transform, false);
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