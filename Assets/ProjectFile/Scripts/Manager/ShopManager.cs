using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Transform safeAreaTransform;
    public ShopPopup shopPopup;
    private ShopPopup instanceshopPopup;

    public void ShowShop(PlayerStats player, int shopType, Action onExit)
    {
        instanceshopPopup = Instantiate(shopPopup, safeAreaTransform);
        instanceshopPopup.Initialize(player, shopType, onExit);
    }
}

