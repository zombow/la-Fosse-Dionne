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

    public bool isBattleStart;
    public string spawnMonsterId;
    public StoryBlock returnBlockAfterBattle;
    
    public int rewardGold;
    public List<string> rewardItems = new();
    public string rewardTrait;
    public int rewardMorality;
    public int rewardLifePoint;
    public int rewardSpiritPoint;

    public bool isShop;
    public int shopType;
    public float shopChance;
    public StoryBlock nextBlockAfterShop;

    public bool isRandomEncounter;
    public bool answer;
    
    
}