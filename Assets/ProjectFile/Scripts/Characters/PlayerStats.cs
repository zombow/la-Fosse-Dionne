using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateBlock
{
    public string playerName = "Player";
    public string gender = "Unknown";
    public string appearance = "Normal";
    public Sprite looksSprite;

    public int level = 1;
    public int gold = 0;

    public int maxHp;
    public int maxLifepoint = 5;
    public int maxSpiritpoint = 5;
    public int experience = 0;

    public int attack;
    public int defense;
    public int evasion;
    public int critChance;
    public int speed;

    public Dictionary<StateType, int> playerStatus = new Dictionary<StateType, int>()
    {
        { StateType.Strength, 10 },
        { StateType.Agility, 10 },
        { StateType.Intelligence, 10 },
        { StateType.Luck, 10 },
        { StateType.Mortality, 0 },
        { StateType.Life, 5 },
        { StateType.Spirit, 5 },
        { StateType.Hp, 0 },
        { StateType.Defense, 0 },
        { StateType.Speed, 0 },
    };

    public PlayerStateBlock Cloning()
    {
        var clone = (PlayerStateBlock)this.MemberwiseClone();

        clone.playerStatus = new Dictionary<StateType, int>(this.playerStatus);

        return clone;
        ;
    }
}

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    // [HideInInspector]
    public PlayerStateBlock playerStateBlock = new PlayerStateBlock();
    private PlayerStateBlock basePlayerStateBlock = new PlayerStateBlock();

    [Header("Equipped Items")] public Item equippedWeapon;
    public Item armor;
    public Item shield;
    public Item accessory;

    [Header("Inventory")] public List<Item> inventory = new List<Item>();

    public bool IsAlive => playerStateBlock.playerStatus[StateType.Hp] > 0;
    public event Action OnStatsChanged;

    public bool Equip(Item item)
    {
        if (item == null) return false;

        Item tempItem = item;

        switch (item.type)
        {
            case ItemType.Weapon:
                if (item.grip == GripType.TwoHanded && shield != null)
                {
                    if (!Unequip(ItemType.Shield))
                    {
                        return false;
                    }
                }

                inventory.Remove(item);

                if (equippedWeapon != null)
                {
                    Unequip(ItemType.Weapon);
                }

                equippedWeapon = tempItem;

                break;
            case ItemType.Armor:
                inventory.Remove(item);
                if (armor != null)
                {
                    Unequip(ItemType.Armor);
                }

                armor = tempItem;
                break;
            case ItemType.Shield:
                if (equippedWeapon != null && equippedWeapon.grip == GripType.TwoHanded)
                {
                    if (!Unequip(ItemType.Weapon))
                    {
                        return false;
                    }
                }

                inventory.Remove(item);

                if (shield != null)
                {
                    Unequip(ItemType.Shield);
                }

                shield = tempItem;
                break;
            case ItemType.Accessory:
                inventory.Remove(item);
                if (accessory != null)
                {
                    Unequip(ItemType.Accessory);
                }

                accessory = tempItem;
                break;
            case ItemType.Consumable:
                if (!UseItem(tempItem))
                {
                    return false;
                }

                inventory.Remove(item);

                break;
        }

        RecalculateStats(true);
        return true;
    }

    private bool UseItem(Item tempItem)
    {
        foreach (var value in tempItem.stats)
        {
            if (value.Key == StateType.Life)
            {
                if (playerStateBlock.playerStatus[value.Key] >= playerStateBlock.maxLifepoint)
                {
                    return false;
                }

                playerStateBlock.playerStatus[value.Key] =
                    Mathf.Clamp(playerStateBlock.playerStatus[value.Key] + value.Value, 0, playerStateBlock.maxLifepoint);
            }
            else if (value.Key == StateType.Spirit)
            {
                if (playerStateBlock.playerStatus[value.Key] >= playerStateBlock.maxSpiritpoint)
                {
                    return false;
                }

                playerStateBlock.playerStatus[value.Key] =
                    Mathf.Clamp(playerStateBlock.playerStatus[value.Key] + value.Value, 0, playerStateBlock.maxSpiritpoint);
            }
            else
            {
                playerStateBlock.playerStatus[value.Key] += value.Value;
            }
        }
        RecalculateStats();
        return true;
    }

    public bool Unequip(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                if (equippedWeapon != null && inventory.Count < 8)
                {
                    inventory.Add(equippedWeapon);
                    equippedWeapon = null;
                }
                else return false;

                break;

            case ItemType.Armor:
                if (armor != null && inventory.Count < 8)
                {
                    inventory.Add(armor);
                    armor = null;
                }
                else return false;

                break;
            case ItemType.Shield:
                if (shield != null && inventory.Count < 8)
                {
                    inventory.Add(shield);
                    shield = null;
                }
                else return false;

                break;
            case ItemType.Accessory:
                if (accessory != null && inventory.Count < 8)
                {
                    inventory.Add(accessory);
                    accessory = null;
                }
                else return false;

                break;
        }

        RecalculateStats();
        return true;
    }

    public void RecalculateStats(bool heal = true)
    {
        playerStateBlock.attack = Mathf.CeilToInt(((playerStateBlock.playerStatus[StateType.Strength] + GetStatFromEquip(StateType.Strength)) * 1.5f
                                                   + (playerStateBlock.playerStatus[StateType.Agility] + GetStatFromEquip(StateType.Agility)) * 1.5f
                                                   + (playerStateBlock.playerStatus[StateType.Intelligence] + GetStatFromEquip(StateType.Intelligence)) * 1.5f)); // 공격력 생성 수정필요

        playerStateBlock.defense = Mathf.CeilToInt((playerStateBlock.playerStatus[StateType.Strength] + GetStatFromEquip(StateType.Strength)) * 0.8f
                                                   + (playerStateBlock.playerStatus[StateType.Agility] + GetStatFromEquip(StateType.Agility)) * 0.2f
                                                   + (playerStateBlock.playerStatus[StateType.Defense] + GetStatFromEquip(StateType.Defense))); // 방어력 생성 수정필요

        playerStateBlock.evasion = Mathf.CeilToInt(Mathf.Clamp(
            (playerStateBlock.playerStatus[StateType.Agility] + GetStatFromEquip(StateType.Agility)) * 1.2f
            + (playerStateBlock.playerStatus[StateType.Luck] + GetStatFromEquip(StateType.Luck)) * 0.5f, 0, 100));

        playerStateBlock.critChance = Mathf.CeilToInt(Mathf.Clamp(
            (playerStateBlock.playerStatus[StateType.Luck] + GetStatFromEquip(StateType.Luck)) * 1.0f
            + (playerStateBlock.playerStatus[StateType.Agility] + GetStatFromEquip(StateType.Agility)) * 0.3f, 0, 100));

        playerStateBlock.speed = Mathf.CeilToInt(Mathf.Clamp(
            (playerStateBlock.playerStatus[StateType.Agility] + GetStatFromEquip(StateType.Agility)) * 1.0f
            + (playerStateBlock.playerStatus[StateType.Luck] + GetStatFromEquip(StateType.Luck)) * 0.2f
            + (playerStateBlock.playerStatus[StateType.Speed] + GetStatFromEquip(StateType.Speed)), 0, 100));

        playerStateBlock.maxHp = 20 + playerStateBlock.level * 10
                                    + playerStateBlock.playerStatus[StateType.Strength] * 5
                                    + GetStatFromEquip(StateType.Hp);
        
        playerStateBlock.playerStatus[StateType.Life] = Mathf.Clamp(playerStateBlock.playerStatus[StateType.Life], 0, playerStateBlock.maxLifepoint);
        if (heal)
        {
            playerStateBlock.playerStatus[StateType.Hp] = playerStateBlock.maxHp;
        }

        OnStatsChanged?.Invoke();
    }

    public int GetStatFromEquip(StateType statName)
    {
        int total = 0;
        foreach (var item in new[] { equippedWeapon, armor, shield, accessory })
        {
            if (item != null && item.stats.ContainsKey(statName))
                total += item.stats[statName];
        }

        return total;
    }

    public void PlayerInit(PlayerStats tempPlayerStats)
    {
        ResetStats();
        OnStatsChanged = null;
        playerStateBlock.playerName = tempPlayerStats.playerStateBlock.playerName;
        playerStateBlock.gender = tempPlayerStats.playerStateBlock.gender;
        playerStateBlock.looksSprite = tempPlayerStats.playerStateBlock.looksSprite;
        foreach (var state in tempPlayerStats.playerStateBlock.playerStatus)
        {
            if (state.Value > 0)
                playerStateBlock.playerStatus[state.Key] += state.Value;
        }

        equippedWeapon = tempPlayerStats.equippedWeapon;
        accessory = tempPlayerStats.accessory;
        armor = tempPlayerStats.armor;

        RecalculateStats();
    }

    public bool ItemBuy(Item item)
    {
        if (playerStateBlock.gold <item.value)
        {
            Debug.Log("골드가 부족합니다!");
            // 추가팝업 필요?
            return false;
        }
        if (inventory.Count >= 8)
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
            return false;
        }
        inventory.Add(item);
        playerStateBlock.gold -= item.value;
        return true;
    }


    public void ResetStats()
    {
        playerStateBlock = basePlayerStateBlock.Cloning();

        equippedWeapon = null;
        armor = null;
        shield = null;
        accessory = null;

        inventory.Clear();
    }
    public bool AddItem(Item item)
    {
        if (item == null) return false;

        if (inventory.Count >= 8)
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
            return false;
        }

        inventory.Add(item);
        RecalculateStats();
        return true;
    }
    public bool DeleteItem(Item selectSlotSlotItem)
    {
        if (selectSlotSlotItem == null) return false;

        if (selectSlotSlotItem.type == ItemType.Special)
        {
            return false;
        }

        inventory.Remove(selectSlotSlotItem);
        RecalculateStats();
        return true;
    }
}