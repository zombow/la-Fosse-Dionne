using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public enum AnimationType
{
    [EnumMember(Value = "idle")] Idle,
    [EnumMember(Value = "attack")] Attack,
    [EnumMember(Value = "hurt")] Hurt,
    [EnumMember(Value = "death")] Death
}

[Serializable]
public class Monster
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

    [JsonProperty("animations")]
    public Dictionary<string, string> animationPathsRaw;

    [JsonIgnore]
    public Dictionary<AnimationType, string> animationPaths;
    
    public Sprite[] idleSprite;
    public Sprite[] attackSprite;
    public Sprite[] hurtSprite;
    public Sprite[] deathSprite;

    public bool IsAlive => hp > 0;

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(0, hp - damage);
    }

    public void ResetHp()
    {
        hp = maxHp;
    }

    public void ProcessAnimationPaths()
    {
        animationPaths = new Dictionary<AnimationType, string>();
        foreach (var kvp in animationPathsRaw)
        {
            if (Enum.TryParse<AnimationType>(kvp.Key, true, out var enumKey))
            {
                animationPaths[enumKey] = kvp.Value;
            }
            else
            {
                Debug.LogWarning($"[Monster:{id}] 알 수 없는 애니메이션 키: {kvp.Key}");
            }
        }
    }

    public void LoadSprites()
    {
        ProcessAnimationPaths();
        idleSprite = Resources.LoadAll<Sprite>(animationPaths[AnimationType.Idle]);
        attackSprite = Resources.LoadAll<Sprite>(animationPaths[AnimationType.Attack]);
        hurtSprite = Resources.LoadAll<Sprite>(animationPaths[AnimationType.Hurt]);
        deathSprite = Resources.LoadAll<Sprite>(animationPaths[AnimationType.Death]);
    }
}
