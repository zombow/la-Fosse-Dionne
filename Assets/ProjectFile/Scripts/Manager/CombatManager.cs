using UnityEngine;
using System;

public class CombatManager : MonoBehaviour
{
    public Transform SafeAreaTransform;
    public CombatPopup CombatPopupPrefab;
    private CombatPopup instanceCombatPopupPrefab;
    public PlayerStats player;
    public Monster monster;

    public void InitCombat(PlayerStats playerRef, string monsterId, Action callback)
    {
        player = playerRef;
        player.playerStateBlock.playerStatus[StateType.Hp] = player.playerStateBlock.maxHp;
        player.RecalculateStats();

        monster = AssetManager.Instance.monsterList[monsterId];
        monster.ResetHp();
        
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
            player.playerStateBlock.playerStatus[StateType.Life]--;
            instanceCombatPopupPrefab.EndBattle(false);
        }
        else if (!monster.IsAlive)
        {
            Debug.Log("몬스터를 처치했습니다!");
            instanceCombatPopupPrefab.EndBattle(true);
        }
    }
}