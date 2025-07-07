using UnityEngine;
using System.Collections.Generic;

public class StoryManager : MonoBehaviour
{
    [HideInInspector] public StoryScene Storyscene;

    public CombatManager combatManager;
    public ShopManager shopmanager;

    private StoryBlock currentblock = null;
    public PlayerStats player;

    public void InitAndStart(StoryBlock startBlock, StoryScene storyScene)
    {
        currentblock = startBlock;
        Storyscene = storyScene;

        ShowStoryBlock(currentblock);
    }

    public void ShowStoryBlock(StoryBlock block)
    {
        Storyscene.UpdateGaugePanel(1); // 게이지 패널 업데이트 (게이지 패널값은 Block의 갯수를 백분율로 나누는것으로 수정필요)
        if (block.isBattleStart && block.monsterToSpawn) // battle분기
        {
            Storyscene.BeginBattle(block, this);
            combatManager.StartCombat(player, block.monsterToSpawn, () =>
            {
                ShowStoryBlock(block.returnBlockAfterBattle);
                Storyscene.storyBG.sprite
                    = Storyscene.defaultBgSprite;
            });
        }
        else if (block.isShop) // 상점 분기
        {
            Storyscene.BeginShop(block, this);
            shopmanager.ShowShop(block.shopType, () => { ShowStoryBlock(block.nextBlockAfterShop); });
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