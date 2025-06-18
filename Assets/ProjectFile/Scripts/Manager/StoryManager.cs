using UnityEngine;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    public List<StoryBlock> storyBlocks;
    public StoryUI storyUI;
    public CombatManager combatManager;

    private int currentIndex = 0;
    private PlayerStats player;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        ShowStoryBlock(currentIndex);
    }

    public void ShowStoryBlock(int index)
    {
        if (index < 0 || index >= storyBlocks.Count) return;
        currentIndex = index;
        var block = storyBlocks[index];

        if (block.isBattleStart && block.monsterToSpawn != null)
        {
            combatManager.StartCombat(player, block.monsterToSpawn, () =>
            {
                ShowStoryBlock(block.returnIndexAfterBattle);
            });
        }
        else
        {
            storyUI.Display(block, this);
        }
    }

    public void Choose(StoryChoice choice)
    {
        if (choice.requiresProbabilityCheck)
        {
            float chance = choice.CalculateSuccessChance(player);
            if (Random.Range(0f, 100f) <= chance)
                ShowStoryBlock(choice.successIndex);
            else
                ShowStoryBlock(choice.failIndex);
        }
        else
        {
            ShowStoryBlock(choice.nextIndex);
        }
    }
}