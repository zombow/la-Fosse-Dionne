using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "RPG/Monster")]
public class Monster : ScriptableObject
{
    public string id;
    public string monsterName;
    public string type;

    public int hp;
    public int maxHp;
    public int attack;
    public int defense;
    public int evasion;
    public int critChance;
    public int speed;
    public int tier;

    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite hurtSprite;
    public Sprite deathSprite;

    public bool IsAlive => hp > 0;

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(0, hp - damage);
    }

    public void ResetHp()
    {
        hp = maxHp;
    }
}

