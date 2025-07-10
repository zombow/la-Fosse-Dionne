using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum SlotType
{
    Equip,
    Inventory,
    Shop,
}
public class ItemSlot : MonoBehaviour
{
    public SlotType slotType;
    public Item slotItem;
    public Toggle slotToggle;
    public Image slotImage;

    public void UpdateUI(Item Item)
    {
        slotItem = null;
        slotImage.sprite = null;
        slotImage.color = Color.clear;

        if (Item != null)
        {
            slotItem = Item;
            slotImage.sprite = Item.itemSprite;
            slotImage.color = Color.white;
        }
    }
}