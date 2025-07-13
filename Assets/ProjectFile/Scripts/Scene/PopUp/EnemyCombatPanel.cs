using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombatPanel : MonoBehaviour
{
    [Header("Enemy UI")] public Image enemyImage;
    public TextMeshProUGUI enemyNameText;
    public Slider enemyHealthSlider;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyAttackText;
    public TextMeshProUGUI enemyDefenseText;

    [Header("Combat UI")] 
    public Slider speedSlider;

    public Image EffectImage;
    [SerializeField] private float frameDelay = 0.1f;
    private Coroutine currentAnimationCoroutine;
    private Monster currentMonster;
    private Coroutine EffectCoroutine;
    private Coroutine speedCoroutine;
    private bool isPaused = false;
    public event Action EnemyAttackReady;
    public event Action EnemyAttack;
    public event Action<int> EnemyEndReaction;

    private int getDamage;
    public void InitPanel(Monster monster)
    {
        currentMonster = monster;
        EffectImage.color = Color.clear;
        PlayAnimation(AnimationType.Idle);
    }

    public void BattleStart()
    {
        // speed slider 채우기 시작
        RegenSpeedSlider();
    }
    
    public void BattleEnd()
    {
        StopSpeedSlider();
        if (currentMonster.hp > 0)
        {
            PlayAnimation(AnimationType.Idle);
        }
        else
        {
            PlayAnimation(AnimationType.Death);
        }
    }
    public void AttackReady()
    {
        // enemy의 speed slider가 가득차면 호출됨
        PlayAnimation(AnimationType.Attack);
    }
    public void PlayerAttackReady()
    {
        // 플레이어의 speed slider가 가득차면 호출됨
        StopSpeedSlider();
    }
    public void EnemyGetHit(int damage, string effectName = "smoke")
    {
        getDamage = damage;
        currentMonster.TakeDamage(damage);
        PlayEffect(effectName);
        PlayAnimation(AnimationType.Hurt);
    }
    
    public void TurnEnd()
    {
        speedSlider.value = 0;
        RegenSpeedSlider();
    }
    
    public void PlayerTurnEnd()
    {
        RegenSpeedSlider();
    }
    
    private void RegenSpeedSlider()
    {
        isPaused = false;

        if (speedCoroutine == null)
        {
            speedCoroutine = StartCoroutine(FillSpeedSliderCoroutine());
        }
    }

    private IEnumerator FillSpeedSliderCoroutine()
    {
        float target = speedSlider.maxValue;

        while (speedSlider.value < target)
        {
            if (!isPaused)
            {
                speedSlider.value += Time.deltaTime * 100;
            }

            yield return null;
        }

        EnemyAttackReady?.Invoke();
        speedSlider.value = target;
        speedCoroutine = null;
    }

    public void StopSpeedSlider()
    {
        isPaused = true;
    }
    
    public void PlayAnimation(AnimationType type)
    {
        if (!currentMonster.animationSprites.TryGetValue(type, out var sprites) || sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning($"애니메이션 {type} 재생 실패 - 스프라이트가 존재하지 않음");
            return;
        }

        if (currentAnimationCoroutine != null)
            StopCoroutine(currentAnimationCoroutine);

        switch (type)
        {
            case AnimationType.Idle:
                currentAnimationCoroutine = StartCoroutine(PlayLoopingAnimation(sprites));
                break;
            case AnimationType.Attack:
            case AnimationType.Hurt:
                currentAnimationCoroutine = StartCoroutine(PlayOnceThenIdle(sprites, type));
                break;
            case AnimationType.Death:
                currentAnimationCoroutine = StartCoroutine(PlayOnceThenFreezeLast(sprites));
                break;
        }
    }

    private IEnumerator PlayLoopingAnimation(Sprite[] frames)
    {
        int index = 0;
        while (true)
        {
            enemyImage.sprite = frames[index];
            index = (index + 1) % frames.Length;
            yield return new WaitForSeconds(frameDelay);
        }
    }

    private IEnumerator PlayOnceThenIdle(Sprite[] frames, AnimationType type)
    {
        foreach (var sprite in frames)
        {
            enemyImage.sprite = sprite;
            yield return new WaitForSeconds(frameDelay);
        }
        
        PlayAnimation(AnimationType.Idle);

        if (type == AnimationType.Attack)
        {
            EnemyAttack?.Invoke();
        }
        else if (type == AnimationType.Hurt)
        {
            EnemyEndReaction?.Invoke(getDamage);
        }
    }

    private IEnumerator PlayOnceThenFreezeLast(Sprite[] frames)
    {
        foreach (var sprite in frames)
        {
            enemyImage.sprite = sprite;
            yield return new WaitForSeconds(frameDelay);
        }

        // 마지막 프레임에서 멈춤
        enemyImage.sprite = frames[^1];
    }

    public void UpdateEnemyUI(Monster monster)
    {
        currentMonster = monster;

        // 기본 스탯 UI 갱신
        enemyNameText.text = monster.name;
        enemyHealthSlider.maxValue = monster.maxHp;
        enemyHealthSlider.value = monster.hp;
        enemyHealthText.text = monster.hp.ToString();
        enemyAttackText.text = monster.combatStats.attack.ToString();
        enemyDefenseText.text = monster.combatStats.defense.ToString();

        speedSlider.maxValue = 100 - monster.combatStats.speed; // 100 - speed 값으로 생각중
    }

    public void PlayEffect(string effectName)
    {
        EffectCoroutine = StartCoroutine(EffectPlayOnce(AssetManager.Instance.EffectSprites[effectName]));
    }
    private IEnumerator EffectPlayOnce(Sprite[] frames)
    {
        EffectImage.color = Color.white;
        
        foreach (var sprite in frames)
        {
            EffectImage.sprite = sprite;
            yield return new WaitForSeconds(frameDelay);
        }

        EffectImage.color = Color.clear;
        EffectCoroutine = null;
    }
    
}