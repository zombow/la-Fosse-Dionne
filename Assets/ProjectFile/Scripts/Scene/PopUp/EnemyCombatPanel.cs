using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombatPanel : MonoBehaviour
{
    public Image enemyImage;
    public TextMeshProUGUI enemyNameText;
    public Slider enemyHealthSlider;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyAttackText;
    public TextMeshProUGUI enemyDefenseText;

    public void UpdateEnemyUI(Monster monster)
    {
        enemyImage.sprite = monster.idleSprite;
        enemyNameText.text = monster.monsterName;
        enemyHealthSlider.maxValue = monster.maxHp;
        enemyHealthSlider.value = monster.hp;
        enemyHealthText.text = monster.hp.ToString();
        enemyAttackText.text = monster.attack.ToString();
        enemyDefenseText.text = monster.defense.ToString();
    }
}
