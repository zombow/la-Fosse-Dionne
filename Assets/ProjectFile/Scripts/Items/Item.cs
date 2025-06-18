using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "RPG/Item")]
public class Item : ScriptableObject
{
    public string id;
    public string itemName;
    public ItemType type;
    public GripType grip;
    public int tier;
    public int value;
    public string description;
    public Sprite icon;
    public Dictionary<string, int> stats = new();
}

public enum ItemType { Weapon, Armor, Shield, Accessory }
public enum GripType { None, OneHanded, TwoHanded }