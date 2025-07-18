using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class ShopPopup : MonoBehaviour
{
    public SettingPopup settingPrefab; // 설정 팝업 프리팹
    private PlayerStats Player; // shopManager 에서주입
    [Header("Panels")] public ShopPanel shopPanel;
    public InventoryPanel inventoryPanel;
    public ItemInfoPanel itemDetailsPanel;

    [Header("Interaction")] public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    public Button exitButton;
    private Action onExit;

    private List<Item> shopItems = new List<Item>();

    private ItemSlot selectSlot;

    public void Initialize(PlayerStats player, int shopType, Action onExitCallback)
    {
        settingPrefab = SettingManager.Instance._initPrefab;
        settingPrefab.NewGame += CloseShop;
       
        Player = player;
        this.onExit = onExitCallback;
        SetupItems(shopType);

        inventoryPanel.SetupInventory(player);
        inventoryPanel.OnInventorySlotSelected += OnSlotSelected;
        shopPanel.OnShopSlotSelected += OnSlotSelected;

        exitButton.onClick.AddListener(CloseShop);


        OnSlotSelected(null); // 최초 초기화
    }

    private void OnSlotSelected(ItemSlot obj)
    {
        buyButton.gameObject.SetActive(false);
        if (obj == null || obj.slotItem == null)
        {
            selectSlot = null;
            itemDetailsPanel.gameObject.SetActive(false);
            UpdateInteractionPanel();
            return;
        }

        selectSlot = obj;
        itemDetailsPanel.gameObject.SetActive(true);

        itemDetailsPanel.UpdateUI(obj.slotItem);
        UpdateInteractionPanel();
    }

    private void UpdateInteractionPanel()
    {
        if (selectSlot == null || selectSlot.slotItem == null)
        {
            buyButton.gameObject.SetActive(false);
            buyButton.onClick.RemoveAllListeners();
        }
        else if (selectSlot.slotType == SlotType.Equip)
        {
            ButtonSetup(false);
        }
        else if (selectSlot.slotType == SlotType.Shop)
        {
            ButtonSetup(true);
        }
        else if (selectSlot.slotType == SlotType.Inventory && selectSlot.slotItem.type != ItemType.Special)
        {
            ButtonSetup(false);
        }
    }

    void ButtonSetup(bool isBuy)
    {
        buyButton.onClick.RemoveAllListeners();

        if (isBuy)
        {
            buyButtonText.text = $"구매하기 ({selectSlot.slotItem.value})";
            buyButton.onClick.AddListener(BuyItem);
            itemDetailsPanel.transform.SetParent(inventoryPanel.transform, false);
        }
        else
        {
            buyButtonText.text = $"판매하기 ({selectSlot.slotItem.value})";
            buyButton.onClick.AddListener(SellItem);
            itemDetailsPanel.transform.SetParent(shopPanel.transform, false);
        }

        buyButton.gameObject.SetActive(true);
    }

    void SetupItems(int shopType)
    {
        List<Item> temp = new List<Item>();

        temp.AddRange(AssetManager.Instance.shopItemList[shopType].items[ItemType.Weapon]);
        temp.AddRange(AssetManager.Instance.shopItemList[shopType].items[ItemType.Armor]);
        temp.AddRange(AssetManager.Instance.shopItemList[shopType].items[ItemType.Shield]);

        ItemSelect(temp, 6); // 무작위 장비 6개
        temp.Clear();

        if (shopType == 1)
        {
            shopItems.Add(AssetManager.Instance.itemList["acc_silver_ring"]); // silver ring
        }
        else if (shopType == 2)
        {
            temp.AddRange(AssetManager.Instance.shopItemList[shopType].items[ItemType.Accessory]);
            ItemSelect(temp, 2);
            temp.Clear();
        }

        temp.AddRange(AssetManager.Instance.shopItemList[shopType].items[ItemType.Consumable]);
        ItemSelect(temp, 1); // 무작위 소모품 1개
        shopItems.AddRange(temp);

        shopPanel.SetupShop(shopItems);
    }

    void ItemSelect(List<Item> itemList, int itemCount)
    {
        for (int i = 0; i < itemCount; i++)
        {
            int index = Random.Range(0, itemList.Count);
            shopItems.Add(itemList[index]);
            itemList.RemoveAt(index);
        }
    }

    private void BuyItem()
    {
        // 이전에 경고 popup을만들까?
        Player.ItemBuy(selectSlot.slotItem);
        inventoryPanel.UpdateInventoryUI();
        shopPanel.DiSelectSlot();
    }

    private void SellItem()
    {
        // 이전에 경고 popup을만들까?
        Player.playerStateBlock.gold += selectSlot.slotItem.value;
        if (selectSlot.slotType == SlotType.Equip)
        {
            if (selectSlot.slotItem.type == ItemType.Armor)
            {
                Player.armor = null;
            }
            else if (selectSlot.slotItem.type == ItemType.Weapon)
            {
                Player.equippedWeapon = null;
            }
            else if (selectSlot.slotItem.type == ItemType.Shield)
            {
                Player.shield = null;
            }
            else if (selectSlot.slotItem.type == ItemType.Accessory)
            {
                Player.accessory = null;
            }

            Player.RecalculateStats();
        }
        else if (selectSlot.slotType == SlotType.Inventory)
        {
            Player.inventory.Remove(selectSlot.slotItem);
        }

        inventoryPanel.UpdateInventoryUI();
        inventoryPanel.DiSelectSlot();
    }


    void CloseShop()
    {
        Player.OnStatsChanged -= inventoryPanel.UpdateInventoryUI;
        settingPrefab.NewGame -= CloseShop;
        
        Destroy(gameObject);
        onExit?.Invoke();
    }
}