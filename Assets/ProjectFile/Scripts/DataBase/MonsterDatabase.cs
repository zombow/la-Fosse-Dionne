using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class CombatStats
{
    public int attack;
    public int defense;
    public int evasion;
    public int crit_chance;
    public int speed;
}
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
    public string name;
    public string type;

    public int maxHp;
    public int hp;
    public int tier;
    
    [JsonProperty("combat_stats")]
    public CombatStats combatStats;

    [JsonProperty("animations")]
    public Dictionary<string, string> animationPathsRaw;
    
    [JsonIgnore]
    public Dictionary<AnimationType, Sprite[]> animationSprites = new();

    public bool IsAlive => hp > 0;

    public void TakeDamage(int damage)
    {
        hp = Mathf.Max(0, hp - damage);
    }
    
    public void ResetHp()
    {
        hp = maxHp;
    }

    public void LoadSprites()
    {
        animationSprites = new Dictionary<AnimationType, Sprite[]>();
        foreach (var kvp in animationPathsRaw)
        {
            if (Enum.TryParse<AnimationType>(kvp.Key, true, out var animType))
            {
                Sprite[] sprites = Resources.LoadAll<Sprite>(kvp.Value);
                if (sprites != null && sprites.Length > 0)
                {
                    animationSprites[animType] = sprites;
                }
                else
                {
                    Debug.LogWarning($"[Monster:{id}] 애니메이션 {animType}에 대한 스프라이트를 찾을 수 없습니다: {kvp.Value}");
                }
            }
            else
            {
                Debug.LogWarning($"[Monster:{id}] 알 수 없는 애니메이션 키: {kvp.Key}");
            }
        }
    }
    
    
}
