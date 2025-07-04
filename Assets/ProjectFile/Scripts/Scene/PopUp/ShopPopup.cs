using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : MonoBehaviour
{
    [Header("Panels")] 
    public GameObject shopPanel;
    public GameObject inventoryPanel;
    public GameObject itemDetailsPanel;
    
    public Button exitButton;
    private Action onExit;

    public void Initialize(int shopType, Action onExitCallback)
    {
        this.onExit = onExitCallback;
        SetupItems(shopType);
        exitButton.onClick.AddListener(CloseShop);
    }

    void SetupItems(int shopType)
    {
        // 아이템 구성 로직 구현
    }

    void CloseShop()
    {
        Destroy(gameObject);
        onExit?.Invoke();
    }
}
