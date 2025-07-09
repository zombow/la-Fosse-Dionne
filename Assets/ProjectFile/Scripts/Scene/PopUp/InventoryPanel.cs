using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    private PlayerStats player; // inventoryPopup / shopPopup 에서 주입

    [Header("equip item images")] // 장착된 아이템 슬롯들
    public ItemSlot equipArmorSlot;
    public ItemSlot equipShieldSlot;
    public ItemSlot equipWeaponSlot;
    public ItemSlot equipAccessorySlot;

    [Header("Inventory item images")] // 인벤토리 아이템 슬롯들
    public List<ItemSlot> InventoryItemSlots;

    [Header("Player Stats")] // 플레이어의 상태 정보
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI criticalText;
    public TextMeshProUGUI evasionText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI coinText;

    [HideInInspector] public ItemSlot selectedSlot;
    public event Action<ItemSlot> OnInventorySlotSelected;


    public void SetupInventory(PlayerStats playerParam)
    {
        player = playerParam;
        player.OnStatsChanged += UpdateInventoryUI;

        equipArmorSlot.slotToggle.onValueChanged.AddListener(SelectedSlot);
        equipShieldSlot.slotToggle.onValueChanged.AddListener(SelectedSlot);
        equipWeaponSlot.slotToggle.onValueChanged.AddListener(SelectedSlot);
        equipAccessorySlot.slotToggle.onValueChanged.AddListener(SelectedSlot);

        foreach (var itemSlot in InventoryItemSlots)
        {
            itemSlot.slotToggle.onValueChanged.AddListener(SelectedSlot);
        }

        // 인벤토리 아이템 슬롯 초기화
        UpdateInventoryUI();
    }

    void SelectedSlot(bool isOn)
    {
        if (isOn)
        {
            selectedSlot = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>();
            OnInventorySlotSelected?.Invoke(selectedSlot);
        }
        else if (selectedSlot == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<ItemSlot>())
        {
            selectedSlot = null;
            OnInventorySlotSelected?.Invoke(selectedSlot);
        }
    }

    public void UpdateInventoryUI()
    {
        // 장착된 아이템 업데이트
        equipArmorSlot.UpdateUI(player.armor);
        equipShieldSlot.UpdateUI(player.shield);
        equipWeaponSlot.UpdateUI(player.equippedWeapon);
        equipAccessorySlot.UpdateUI(player.accessory);

        attackText.text = player.playerStateBlock.attack.ToString();
        defenseText.text = player.playerStateBlock.defense.ToString();
        criticalText.text = player.playerStateBlock.critChance.ToString();
        evasionText.text = player.playerStateBlock.evasion.ToString();
        healthText.text = player.playerStateBlock.playerStatus[StateType.Hp].ToString();
        coinText.text = player.playerStateBlock.gold.ToString();

        // 인벤토리 아이템 업데이트
        for (int i = 0; i < InventoryItemSlots.Count; i++)
        {
            if (i < player.inventory.Count)
            {
                InventoryItemSlots[i].UpdateUI(player.inventory[i]);
            }
            else
            {
                InventoryItemSlots[i].UpdateUI(null); // 빈 슬롯 처리
            }
        }
    }

    public void DiSelectSlot()
    {
        equipArmorSlot.slotToggle.isOn = false;
        equipShieldSlot.slotToggle.isOn = false;
        equipWeaponSlot.slotToggle.isOn = false;
        equipAccessorySlot.slotToggle.isOn = false;

        foreach (var itemSlot in InventoryItemSlots)
        {
            itemSlot.slotToggle.isOn = false;
        }

        OnInventorySlotSelected?.Invoke(null);
    }
}