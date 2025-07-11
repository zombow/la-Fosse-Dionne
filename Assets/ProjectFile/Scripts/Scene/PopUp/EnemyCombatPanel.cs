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

    [Header("Combat UI")] public Slider speedSlider;

    private Coroutine currentAnimationCoroutine;
    [SerializeField] private float frameDelay = 0.1f;
    private Monster currentMonster;


    public void InitPanel()
    {
    }

    public void BattleStart()
    {
        // speed slider 채우기 시작
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

    public void StopAnimation()
    {
        if (currentAnimationCoroutine != null)
        {
            StopCoroutine(currentAnimationCoroutine);
            currentAnimationCoroutine = null;
        }
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
        
        speedSlider.maxValue = monster.combatStats.speed;
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
}