
[System.Serializable]
public class ItemStats
{
    public int strength;
    public int agility;
    public int intelligence;
    public int defense;
    public int hp;
    public int speed;
    public int luck;
    public int life;
    public int spirit;
}

[System.Serializable]
public class ItemData
{
    public string id;
    public string name;
    public string type;      // weapon, armor, shield, accessory, consumable, special 등
    public int tier;
    public string grip;      // 무기일 경우: one-handed, two-handed
    public ItemStats stats;
    public int value;
    public string description;
    public string image;
}
