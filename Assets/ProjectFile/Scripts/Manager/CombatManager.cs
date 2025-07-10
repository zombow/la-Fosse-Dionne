using UnityEngine;
using System;

public class CombatManager : MonoBehaviour
{
    public Transform SafeAreaTransform;
    public CombatPopup CombatPopupPrefab;
    private CombatPopup instanceCombatPopupPrefab;
    public PlayerStats player;
    public Monster monster;

    public void StartCombat(PlayerStats playerRef, string monsterId, Action callback)
    {

        player = playerRef;
        player.playerStateBlock.playerStatus[StateType.Hp] = player.playerStateBlock.maxHp;
        player.RecalculateStats();

        monster = AssetManager.Instance.monsterList["monster_fire_worm"]; // 예시로 Fire Worm 몬스터를 사용, 실제로는 monsterId를 사용하여 AssetManager에서 불러와야 함
        monster.ResetHp();
        
        instanceCombatPopupPrefab = Instantiate(CombatPopupPrefab, SafeAreaTransform);
        instanceCombatPopupPrefab.Initialize(playerRef, monster, callback);
        instanceCombatPopupPrefab.CombatStart();
        NextTurn(); // 주사위 굴리기 대기
    }

    private void NextTurn()
    {
        instanceCombatPopupPrefab.UpdateCombatUI(player, monster);

        if (!player.IsAlive)
        {
            Debug.Log("플레이어가 사망했습니다.");
            player.playerStateBlock.playerStatus[StateType.Life]--;
            instanceCombatPopupPrefab.CombatEnd();
            return;
        }

        if (!monster.IsAlive)
        {
            Debug.Log("몬스터를 처치했습니다!");
            instanceCombatPopupPrefab.CombatEnd();
            return;
        }

        // 공격 순서 계산로직 적용
        PlayerAttack();
        if (monster.IsAlive) 
            MonsterAttack();

        NextTurn();
    }

    private void PlayerAttack()
    {
        int damage = Mathf.Max(1, player.playerStateBlock.playerStatus[StateType.Strength] - monster.defense);
        monster.TakeDamage(damage);
        Debug.Log($"플레이어가 몬스터에게 {damage} 데미지를 입혔습니다. (남은 HP: {monster.hp})");
    }

    private void MonsterAttack()
    {
        int damage = Mathf.Max(1, monster.attack - player.playerStateBlock.playerStatus[StateType.Strength] / 2);
        player.playerStateBlock.playerStatus[StateType.Hp] = Mathf.Max(0, player.playerStateBlock.playerStatus[StateType.Hp] - damage);
        Debug.Log($"몬스터가 플레이어에게 {damage} 데미지를 입혔습니다. (남은 HP: {player.playerStateBlock.playerStatus[StateType.Hp]})");
    }
}