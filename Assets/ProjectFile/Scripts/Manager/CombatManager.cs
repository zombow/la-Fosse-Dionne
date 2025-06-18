using UnityEngine;
using System;

public class CombatManager : MonoBehaviour
{
    public PlayerStats player;
    public Monster monster;

    private Action onCombatEnd;

    public void StartCombat(PlayerStats playerRef, Monster monsterRef, Action callback)
    {
        player = playerRef;
        monster = Instantiate(monsterRef);
        monster.ResetHp();
        onCombatEnd = callback;

        Debug.Log($"전투 시작: {player.playerName} vs {monster.monsterName}");
        NextTurn();
    }

    private void NextTurn()
    {
        if (!player.IsAlive)
        {
            Debug.Log("플레이어가 사망했습니다.");
            onCombatEnd?.Invoke();
            return;
        }

        if (!monster.IsAlive)
        {
            Debug.Log("몬스터를 처치했습니다!");
            onCombatEnd?.Invoke();
            return;
        }

        PlayerAttack();
        if (monster.IsAlive)
            MonsterAttack();

        NextTurn();
    }

    private void PlayerAttack()
    {
        int damage = Mathf.Max(1, player.strength - monster.defense);
        monster.TakeDamage(damage);
        Debug.Log($"플레이어가 몬스터에게 {damage} 데미지를 입혔습니다. (남은 HP: {monster.hp})");
    }

    private void MonsterAttack()
    {
        int damage = Mathf.Max(1, monster.attack - player.strength / 2);
        player.hp = Mathf.Max(0, player.hp - damage);
        Debug.Log($"몬스터가 플레이어에게 {damage} 데미지를 입혔습니다. (남은 HP: {player.hp})");
    }
}