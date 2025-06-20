using UnityEngine;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{

    public StoryScene Storyscene;
    public CombatManager combatManager;

    private StoryBlock currentblock = null;
    private PlayerStats player;

    public void InitAndStart(StoryBlock startBlock, StoryScene storyScene)
    {
        player = FindObjectOfType<PlayerStats>();
        currentblock = startBlock;
        Storyscene = storyScene;
        
        ShowStoryBlock(currentblock);
    }
    
    public void ShowStoryBlock(StoryBlock block)
    {
        if (block.isBattleStart && block.monsterToSpawn != null)
        {
            combatManager.StartCombat(player, block.monsterToSpawn, () =>
            {
                ShowStoryBlock(block.returnBlockAfterBattle);
            });
        }
        else
        {
            Storyscene.Display(block, this);

        }
    }

    public void Choose(StoryChoice choice)
    {
        if (choice.requiresProbabilityCheck)
        {
            float chance = choice.CalculateSuccessChance(player);
            if (Random.Range(0f, 100f) <= chance)
                ShowStoryBlock(choice.successBlock);
            else
                ShowStoryBlock(choice.failBlock);
        }
        else
        {
            ShowStoryBlock(choice.nextBlock);
        }
    }
}