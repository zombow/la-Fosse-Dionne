using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Transform safeAreaTransform;
    public ShopPopup shopPopup;
    private ShopPopup instanceshopPopup;

    public void ShowShop(int shopType, Action onExit)
    {
        instanceshopPopup = Instantiate(shopPopup, safeAreaTransform);
        instanceshopPopup.Initialize(shopType, onExit);
    }
}

