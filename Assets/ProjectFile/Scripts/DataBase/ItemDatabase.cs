using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


public enum StateType
{
    [EnumMember(Value = "strength")] Strength,
    [EnumMember(Value = "agility")] Agility,
    [EnumMember(Value = "intelligence")] Intelligence,
    [EnumMember(Value = "luck")] Luck,
    [EnumMember(Value = "hp")] Hp,
    [EnumMember(Value = "defense")] Defense,
    [EnumMember(Value = "speed")] Speed,
    [EnumMember(Value = "life")] Life,
    [EnumMember(Value = "spirit")] Spirit,
    [EnumMember(Value = "mortality")] Mortality
}

public enum ItemType
{
    [EnumMember(Value = "weapon")] Weapon,
    [EnumMember(Value = "armor")] Armor,
    [EnumMember(Value = "shield")] Shield,
    [EnumMember(Value = "accessory")] Accessory,
    [EnumMember(Value = "consumable")] Consumable,
    [EnumMember(Value = "special")] Special
}

public enum GripType
{
    [EnumMember(Value = "none")] None,
    [EnumMember(Value = "one-handed")] OneHanded,
    [EnumMember(Value = "two-handed")] TwoHanded
}

[System.Serializable]
public class Item
{
    public string id;
    public string name;
    public ItemType type; // weapon, armor, shield, accessory, consumable, special 등
    public int tier;
    public GripType grip; // 무기일 경우: one-handed, two-handed
    public Dictionary<StateType, int> stats;
    public int value;
    public string description;
    public string image;
    public Sprite itemSprite;
    
    public void LoadSprites()
    {
        itemSprite = Resources.Load<Sprite>(image);
    }
}

public class ShopItem
{
    public Dictionary<ItemType, List<Item>> items = new Dictionary<ItemType, List<Item>>
    {
        { ItemType.Weapon, new List<Item>() },
        { ItemType.Shield, new List<Item>() },
        { ItemType.Armor, new List<Item>() },
        { ItemType.Accessory, new List<Item>() },
        { ItemType.Special, new List<Item>() },
        { ItemType.Consumable, new List<Item>() }
    };
}