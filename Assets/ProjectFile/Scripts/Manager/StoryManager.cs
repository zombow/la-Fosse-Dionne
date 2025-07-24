using UnityEngine;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    [HideInInspector] public StoryScene Storyscene;

    public CombatManager combatManager;
    public ShopManager shopmanager;

    private StoryBlock currentblock = null;
    public PlayerStats player;

    private Dictionary<int, int> chapterBlockCountCache;

    public void InitAndStart(StoryBlock startBlock, StoryScene storyScene)
    {
        currentblock = startBlock;
        Storyscene = storyScene;

        ShowStoryBlock(currentblock);
    }

    public void ShowStoryBlock(StoryBlock block)
    {
        currentblock = block;
        Storyscene.UpdateGaugePanel(block); // 
        if (block.isBattleStart) // battle분기
        {
            Storyscene.BeginBattle(block, this);
            combatManager.InitCombat(player, block.spawnMonsterId, block, () =>
            {
                ShowStoryBlock(block.choices[0].nextBlock);
                Storyscene.storyBG.sprite
                    = Storyscene.defaultBgSprite;
            });
        }
        else if (block.isShop) // 상점 분기
        {
            Storyscene.BeginShop(block, this);
            shopmanager.ShowShop(player, block.shopType, () => { ShowStoryBlock(block.choices[0].nextBlock); });
        }
        else
        {
            Storyscene.Display(block, this);
        }
    }

    public void Choose(StoryChoice choice)
    {
        if (choice.RequiresItem(player))
        {
            if (currentblock.endingBlock)
            {
                SettingManager.Instance._initPrefab.OnNewGame();
            }
            else
            {
                if (choice.requiresProbabilityCheck)
                {
                    if (choice.CalculateSuccessChance(player))
                        ShowStoryBlock(choice.successBlock);
                    else
                        ShowStoryBlock(choice.failBlock);
                }
                else if (choice.requiresMoralityCheck)
                {
                    ShowStoryBlock(choice.GetMoralityBlock(player));
                }

                else if (choice.requiresStateCheck)
                {
                    ShowStoryBlock(choice.GetStateBlock(player));
                }
                else
                {
                    ShowStoryBlock(choice.nextBlock);
                }
            }
        }
    }
}