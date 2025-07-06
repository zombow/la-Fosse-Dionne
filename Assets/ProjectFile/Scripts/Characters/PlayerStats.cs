using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum StateType
{
    Str,
    Dex,
    Int,
    Lux,
    Mortality
}

public class PlayerStateBlock
{
    public string playerName = "Player";
    public string gender = "Unknown";
    public string appearance = "Normal";
    public Sprite looksSprite;

    public int level = 1;
    public int gold = 0;

    public int hp;
    public int maxHp;
    public int lifepoint = 5;
    public int maxLifepoint = 5;
    public int spiritpoint = 5;
    public int maxSpiritpoint = 5;
    public int experience = 0;

    public Dictionary<StateType, int> playerStatus = new Dictionary<StateType, int>()
    {
        { StateType.Str, 10 },
        { StateType.Dex, 10 },
        { StateType.Int, 10 },
        { StateType.Lux, 10 },
        { StateType.Mortality, 10 } // 도덕성은 초기값은 몇?
    };

    public PlayerStateBlock Cloning()
    {
        return (PlayerStateBlock)this.MemberwiseClone();
    }
}

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [HideInInspector] public PlayerStateBlock playerStateBlock = new PlayerStateBlock();
    private PlayerStateBlock basePlayerStateBlock = new PlayerStateBlock();

    [Header("Equipped Items")] public Item equippedWeapon;
    public Item armor;
    public Item shield;
    public Item accessory;

    [Header("Inventory")] public List<Item> inventory = new List<Item>();

    public bool IsAlive => playerStateBlock.hp > 0;


    public void Equip(Item item)
    {
        if (item == null) return;

        switch (item.type)
        {
            case ItemType.Weapon:
                if (item.grip == GripType.TwoHanded && shield != null)
                    Unequip(ItemType.Shield);
                equippedWeapon = item;
                break;
            case ItemType.Armor:
                armor = item;
                break;
            case ItemType.Shield:
                if (equippedWeapon != null && equippedWeapon.grip == GripType.TwoHanded)
                    Unequip(ItemType.Weapon);
                shield = item;
                break;
            case ItemType.Accessory:
                accessory = item;
                break;
        }

        inventory.Remove(item);
        RecalculateStats();
    }

    public void Unequip(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                if (equippedWeapon != null) inventory.Add(equippedWeapon);
                equippedWeapon = null;
                break;
            case ItemType.Armor:
                if (armor != null) inventory.Add(armor);
                armor = null;
                break;
            case ItemType.Shield:
                if (shield != null) inventory.Add(shield);
                shield = null;
                break;
            case ItemType.Accessory:
                if (accessory != null) inventory.Add(accessory);
                accessory = null;
                break;
        }

        RecalculateStats();
    }

    public void RecalculateStats()
    {
        playerStateBlock.maxHp = 20 + playerStateBlock.level * 10 + playerStateBlock.playerStatus[StateType.Str] * 5 + GetStatFromEquip("hp");
        playerStateBlock.hp = Mathf.Clamp(playerStateBlock.hp, 0, playerStateBlock.maxHp);
    }

    private int GetStatFromEquip(string statName)
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
        playerStateBlock.playerName = tempPlayerStats.playerStateBlock.playerName;
        playerStateBlock.gender = tempPlayerStats.playerStateBlock.gender;
        playerStateBlock.looksSprite = tempPlayerStats.playerStateBlock.looksSprite;
        foreach (var state in tempPlayerStats.playerStateBlock.playerStatus)
        {
            playerStateBlock.playerStatus[state.Key] += state.Value;
        }
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
}