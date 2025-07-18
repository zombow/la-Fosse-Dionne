using UnityEngine;
using System;

public class CombatManager : MonoBehaviour
{
    public Transform SafeAreaTransform;
    public CombatPopup CombatPopupPrefab;
    private CombatPopup instanceCombatPopupPrefab;
    public PlayerStats player;
    public Monster monster;
    private StoryBlock currentBlock;

    public void InitCombat(PlayerStats playerRef, string monsterId, StoryBlock battleInfo, Action callback)
    {
        player = playerRef;
        player.playerStateBlock.playerStatus[StateType.Hp] = player.playerStateBlock.maxHp;
        player.RecalculateStats();

        monster = AssetManager.Instance.monsterList[monsterId];
        monster.ResetHp();

        currentBlock = battleInfo;

        instanceCombatPopupPrefab = Instantiate(CombatPopupPrefab, SafeAreaTransform);
        instanceCombatPopupPrefab.Initialize(playerRef, monster, callback);
        instanceCombatPopupPrefab.CheckCombatEnd += CheckTurn;
    }

    private void CheckTurn()
    {
        instanceCombatPopupPrefab.UpdateCombatUI(player, monster);
        if (!player.IsAlive)
        {
            Debug.Log("플레이어가 사망했습니다.");
            instanceCombatPopupPrefab.EndBattle(false);
            player.playerStateBlock.playerStatus[StateType.Life]--;
        }
        else if (!monster.IsAlive)
        {
            Debug.Log("몬스터를 처치했습니다!");
            instanceCombatPopupPrefab.EndBattle(true);
            if (currentBlock.rewardGold > 0)
            {
                player.playerStateBlock.gold += currentBlock.rewardGold;
                instanceCombatPopupPrefab.LogPanelUpdate("골드 획득 :" + currentBlock.rewardGold);
            }

            if (currentBlock.rewardLifePoint > 0)
            {
                player.playerStateBlock.playerStatus[StateType.Life] += currentBlock.rewardLifePoint;
                instanceCombatPopupPrefab.LogPanelUpdate("생명력 회복 :" + currentBlock.rewardLifePoint);
            }

            if (currentBlock.rewardSpiritPoint > 0)
            {
                player.playerStateBlock.playerStatus[StateType.Spirit] += currentBlock.rewardSpiritPoint;
                instanceCombatPopupPrefab.LogPanelUpdate("정신력 회복 :" + currentBlock.rewardSpiritPoint);
            }

            // if (currentBlock.rewardTrait)
            // {
            //     성향 획득은 미구현
            // }
            if (currentBlock.rewardMorality > 0)
            {
                player.playerStateBlock.playerStatus[StateType.Mortality] += currentBlock.rewardMorality;
                instanceCombatPopupPrefab.LogPanelUpdate("도덕성 변동 :" + currentBlock.rewardMorality);
            }

            foreach (var itemId in currentBlock.rewardItems)
            {
                if (player.AddItem(AssetManager.Instance.itemList[itemId]))
                {
                    instanceCombatPopupPrefab.LogPanelUpdate("아이템 획득 :" + AssetManager.Instance.itemList[itemId].name);
                }
            }

        }
    }
}