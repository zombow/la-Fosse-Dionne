using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [Header("Inventory item images")] // 인벤토리 아이템 슬롯들
    public List<ItemSlot> shopItemSlots;

    [HideInInspector] public ItemSlot selectedSlot;
    public event Action<ItemSlot> OnShopSlotSelected;

    public void SetupShop(List<Item> shopItems)
    {
        foreach (var itemSlot in shopItemSlots)
        {
            itemSlot.slotToggle.onValueChanged.AddListener(SelectedSlot);
        }

        for (int i = 0; i < shopItemSlots.Count; i++)
        {
            if (i < shopItems.Count)
            {
                shopItemSlots[i].UpdateUI(shopItems[i]);
            }
            else
            {
                shopItemSlots[i].UpdateUI(null);
            }
        }
    }

    void SelectedSlot(bool isOn)
    {
        if (isOn)
        {
            selectedSlot = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>();
            OnShopSlotSelected?.Invoke(selectedSlot);
        }
        else if (selectedSlot == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>())
        {
            selectedSlot = null;
            OnShopSlotSelected?.Invoke(selectedSlot);
        }
    }

    public void DiSelectSlot()
    {
        foreach (var itemSlot in shopItemSlots)
        {
            itemSlot.slotToggle.isOn = false;
        }

        OnShopSlotSelected?.Invoke(null);
    }
}