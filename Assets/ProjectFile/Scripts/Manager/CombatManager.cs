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

    private void PlayerAttack()
    {
        int damage = Mathf.Max(1, player.playerStateBlock.playerStatus[StateType.Strength] - monster.combatStats.defense); // 데미지 계산공식 + dice결과 사용필요
        //CombatPopupPrefab.PlayerAttack();
        Debug.Log($"플레이어가 몬스터에게 {damage} 데미지를 입혔습니다. (남은 HP: {monster.hp})");
    }

    private void MonsterAttack()
    {
        int damage = Mathf.Max(1, monster.combatStats.attack - player.playerStateBlock.playerStatus[StateType.Strength] / 2);
        player.playerStateBlock.playerStatus[StateType.Hp] = Mathf.Max(0, player.playerStateBlock.playerStatus[StateType.Hp] - damage);
        Debug.Log($"몬스터가 플레이어에게 {damage} 데미지를 입혔습니다. (남은 HP: {player.playerStateBlock.playerStatus[StateType.Hp]})");
    }
}