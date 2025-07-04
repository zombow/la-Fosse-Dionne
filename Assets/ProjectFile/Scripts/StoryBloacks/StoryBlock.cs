using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStoryBlock", menuName = "RPG/Story Block")]
public class StoryBlock : ScriptableObject
{
    public List<StoryContentElement> contentElements = new();
    public List<StoryChoice> choices = new();

    public int goldGained;
    public List<string> itemsGained;
    public string traitGained;

    public bool isBattleStart;
    public Monster monsterToSpawn;
    public StoryBlock returnBlockAfterBattle;
    public int moralityChange;
    public int nextIndex;
    public int nextChapter;
    public int lifePointReward;
    public int spiritPointReward;

    public bool isShop;
    public int shopType;
    public float shopChance;
    public StoryBlock nextBlockAfterShop;
    
    public bool isRandomEncounter;
    public bool answer;
}