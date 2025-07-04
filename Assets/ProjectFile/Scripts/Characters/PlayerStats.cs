using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Str,
    Dex,
    Int,
    Lux
}

[System.Serializable]
public class PlayerStats : MonoBehaviour
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

    [Header("Base Stats")] public int strength = 10;
    public int agility = 10;
    public int intelligence = 10;
    public int luck = 10;

    [Header("Equipped Items")] public Item equippedWeapon;
    public Item armor;
    public Item shield;
    public Item accessory;

    [Header("Inventory")] public List<Item> inventory = new List<Item>();

    public bool IsAlive => hp > 0;

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
        maxHp = 20 + level * 10 + strength * 5 + GetStatFromEquip("hp");
        hp = Mathf.Clamp(hp, 0, maxHp);
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
        playerName = tempPlayerStats.playerName;
    }
}