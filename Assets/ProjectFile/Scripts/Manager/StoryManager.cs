using UnityEngine;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    public StoryBlock StartstoryBlock;
    public StoryUI storyUI;
    public CombatManager combatManager;

    private StoryBlock currentblock = null;
    private PlayerStats player;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        currentblock = StartstoryBlock;
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
            storyUI.Display(block, this);
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