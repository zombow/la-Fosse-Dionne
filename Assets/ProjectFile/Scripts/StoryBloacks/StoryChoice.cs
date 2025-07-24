using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StoryChoice
{
    [TextArea]
    public string text;
    public StoryBlock nextBlock;

    [Header("Branch")] public bool requiresProbabilityCheck;
    public StoryBlock successBlock;
    public StoryBlock failBlock;
    public float baseChance;
    public float successChance;
    public StateType CalcType;

    [Header("MoralityBranch")] public bool requiresMoralityCheck;
    public int moralityLowThreshold;
    public int moralityHighThreshold;
    public StoryBlock UpperMoralityBlock;
    public StoryBlock MiddleMoralityBlock;
    public StoryBlock LowerMoralityBlock;

    [Header("StateBranch")] public bool requiresStateCheck;
    public StoryBlock StreangthBlock;
    public StoryBlock AgilityBlock;
    public StoryBlock IntelligenceBlock;

    public List<string> requiredItemId;

    public bool CalculateSuccessChance(PlayerStats player)
    {
        int mult = Random.Range(1, 6); // 최대 1부터 6까지랜덤값 (6면체 주사위를 생각)
        float chance = (baseChance + player.playerStateBlock.playerStatus[CalcType]) * mult;
        if (100 - successChance <= chance)
        {
            return false;
        }

        return true;
    }

    public StoryBlock GetMoralityBlock(PlayerStats player)
    {
        return requiresMoralityCheck
            ? player.playerStateBlock.playerStatus[StateType.Mortality] >= moralityHighThreshold ? UpperMoralityBlock
            : player.playerStateBlock.playerStatus[StateType.Mortality] >= moralityLowThreshold ? MiddleMoralityBlock
            : LowerMoralityBlock
            : null;
    }

    public StoryBlock GetStateBlock(PlayerStats player)
    {
        Dictionary<StateType, int> vale = new Dictionary<StateType, int>();
        vale.Add(StateType.Strength, player.playerStateBlock.playerStatus[StateType.Strength]);
        vale.Add(StateType.Intelligence, player.playerStateBlock.playerStatus[StateType.Intelligence]);
        vale.Add(StateType.Agility, player.playerStateBlock.playerStatus[StateType.Agility]);
        StateType maxType = vale
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key) // 키 순서대로 정렬
            .First().Key;
        if (maxType == StateType.Strength)
        {
            return StreangthBlock;
        }
        else if (maxType == StateType.Intelligence)
        {
            return IntelligenceBlock;
        }
        else if (maxType == StateType.Agility)
        {
            return AgilityBlock;
        }

        return null;
    }

    public bool RequiresItem(PlayerStats player)
    {
        if (requiredItemId.Count == 0) return true;
        foreach (var itemId in requiredItemId)
        {
            if (!player.inventory.Contains(AssetManager.Instance.itemList[itemId]))
            {
                return false;
            }
        }

        return true;
    }
}