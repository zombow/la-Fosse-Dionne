using UnityEngine;

[System.Serializable]
public class StoryChoice
{
    public string text;
    public StoryBlock nextBlock;
    public int nextChapter;

    public string requiredItemId;
    public string itemReward;
    public string traitReward;
    public int goldReward;
    public int moralityChange;
    public int lifePointReward;
    public int spiritPointReward;

    public bool requiresDiceRoll;
    public int successThreshold;
    public StoryBlock successBlock;
    public StoryBlock failBlock;

    public int requiredStatMin;
    public string requiredStatName;
    public int passStatIndex;
    public int failStatIndex;

    public bool requiresProbabilityCheck;
    public float baseChance;
    public float strengthMultiplier;
    public float agilityMultiplier;
    public float intelligenceMultiplier;
    public float luckMultiplier;

    public int moralityLowThreshold;
    public int moralityMidThreshold;
    public int moralityHighThreshold;

    public int indexIfLowMorality;
    public int indexIfMidMorality;
    public int indexIfHighMorality;

    public float CalculateSuccessChance(PlayerStats player)
    {
        return baseChance + player.strength * strengthMultiplier; //TODO: 힘배수만 적용중 수정필요
    }
}