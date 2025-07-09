using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopup : MonoBehaviour
{
    [HideInInspector] public PlayerStats player; // storyScene에서 주입

    public InventoryPanel inventoryPanelPrefab;
    public ItemInfoPanel itemInfoPanelPrefab;
    [Header("Interaction Buttons")] public Button equipButton;
    public TextMeshProUGUI equipButtonText;
    public Button itemDeleteButton;

    [HideInInspector] public ItemSlot selectSlot; // 선택된 아이템 슬롯


    public void SetupInventory(PlayerStats playerStats)
    {
        player = playerStats;
        inventoryPanelPrefab.SetupInventory(player);
        inventoryPanelPrefab.OnInventorySlotSelected += OnSlotSelected;
        
        itemDeleteButton.onClick.AddListener(OnItemDeleteButtonClicked);

        OnSlotSelected(null);
    }

    private void OnEquip()
    {
        if (player.Equip(selectSlot.slotItem))
        {
            inventoryPanelPrefab.UpdateInventoryUI();
            inventoryPanelPrefab.DiSelectSlot();
        }
    }
    private void UnEquip()
    {
        if (player.Unequip(selectSlot.slotItem.type))
        {
            inventoryPanelPrefab.UpdateInventoryUI();
            inventoryPanelPrefab.DiSelectSlot();
        }
    }

    private void OnItemDeleteButtonClicked()
    {
        if (player.DeleteItem(selectSlot.slotItem))
        {
            inventoryPanelPrefab.UpdateInventoryUI();
            inventoryPanelPrefab.DiSelectSlot();
        }
    }


    private void OnSlotSelected(ItemSlot obj)
    {
        equipButton.gameObject.SetActive(false);
        itemDeleteButton.gameObject.SetActive(false);
        if (obj == null || obj.slotItem == null)
        {
            selectSlot = null;
            itemInfoPanelPrefab.gameObject.SetActive(false);
            UpdateInteractionPanel();
            return;
        }

        selectSlot = obj;
        itemInfoPanelPrefab.gameObject.SetActive(true);
        itemInfoPanelPrefab.UpdateUI(obj.slotItem);

        UpdateInteractionPanel();
    }

    private void UpdateInteractionPanel()
    {
        equipButton.onClick.RemoveAllListeners();
        if (selectSlot == null || selectSlot.slotItem == null)
        {
            equipButton.gameObject.SetActive(false);
            itemDeleteButton.gameObject.SetActive(false);
        }
        else if (selectSlot.slotType == SlotType.Equip)
        {
            equipButtonText.text = "장착해제";
            equipButton.onClick.AddListener(UnEquip);
            equipButton.gameObject.SetActive(true);
            itemDeleteButton.gameObject.SetActive(false);
        }
        else if (selectSlot.slotType == SlotType.Inventory)
        {
            if (selectSlot.slotItem.type == ItemType.Weapon ||
                selectSlot.slotItem.type == ItemType.Armor ||
                selectSlot.slotItem.type == ItemType.Shield ||
                selectSlot.slotItem.type == ItemType.Accessory)
            {
                equipButtonText.text = "착용하기";
                equipButton.onClick.AddListener(OnEquip);
                equipButton.gameObject.SetActive(true);
                itemDeleteButton.gameObject.SetActive(true);
            }
            else if (selectSlot.slotItem.type == ItemType.Consumable)
            {
                equipButtonText.text = "사용하기";
                equipButton.gameObject.SetActive(true);
                itemDeleteButton.gameObject.SetActive(true);
            }
        }
    }
}