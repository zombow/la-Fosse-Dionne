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
    public event Action EnemyEndAction;


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

        EnemyAttack();
        speedSlider.value = target;
        speedCoroutine = null;
    }

    private void EnemyAttack()
    {
        EnemyAttackReady?.Invoke();
        PlayAnimation(AnimationType.Attack);
        TurnEnd();
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
                currentAnimationCoroutine = StartCoroutine(PlayOnceThenIdle(sprites));
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

    private IEnumerator PlayOnceThenIdle(Sprite[] frames)
    {
        foreach (var sprite in frames)
        {
            enemyImage.sprite = sprite;
            yield return new WaitForSeconds(frameDelay);
        }
        EnemyEndAction?.Invoke();
        // 다시 Idle로 전환
        PlayAnimation(AnimationType.Idle);
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
        enemyHealthSlider.maxValue = monster.hp;
        enemyHealthSlider.value = monster.hp;
        enemyHealthText.text = monster.hp.ToString();
        enemyAttackText.text = monster.combatStats.attack.ToString();
        enemyDefenseText.text = monster.combatStats.defense.ToString();

        speedSlider.maxValue = 100 - monster.combatStats.speed; // 100 - speed 값으로 생각중
    }

    public void BattleEnd()
    {
        if (currentMonster.hp > 0)
        {
            PlayAnimation(AnimationType.Idle);
        }
        else
        {
            PlayAnimation(AnimationType.Death);
        }
    }

    public void PlayEffect(string effectName, int damage)
    {
        int reserveDamage = damage;
        EffectCoroutine = StartCoroutine(EffectPlayOnce(AssetManager.Instance.EffectSprites[effectName], reserveDamage));
    }
    private IEnumerator EffectPlayOnce(Sprite[] frames, int damage)
    {
        EffectImage.color = Color.white;
        
        foreach (var sprite in frames)
        {
            EffectImage.sprite = sprite;
            yield return new WaitForSeconds(frameDelay);
        }

        EffectImage.color = Color.clear;
        EffectCoroutine = null;
        currentMonster.TakeDamage(damage);
    }

    public void PlayerAttacked()
    {
        // player의 공격이 시작될때 호출도미
        StopSpeedSlider();
    }

    public void PlayerAttackEnd()
    {
        RegenSpeedSlider();
    }
    
    public void TurnEnd()
    {
        speedSlider.value = 0;
        RegenSpeedSlider();
    }
}