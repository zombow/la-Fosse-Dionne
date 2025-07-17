using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStoryBlock", menuName = "RPG/Story Block")]
public class StoryBlock : ScriptableObject
{
    public List<StoryContentElement> contentElements = new();
    public List<StoryChoice> choices = new();
    public int myChapter;
    public int myIndex;
    public int maxIndex;
    public int goldGained;
    public List<string> itemsGained;
    public string traitGained;

    public bool isBattleStart;
    public string spawnMonsterId;
    public StoryBlock returnBlockAfterBattle;
    
    public int moralityChange;
    public int lifePointReward;
    public int spiritPointReward;

    public bool isShop;
    public int shopType;
    public float shopChance;
    public StoryBlock nextBlockAfterShop;

    public bool isRandomEncounter;
    public bool answer;
    
    
}