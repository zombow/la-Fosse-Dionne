using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPanel : MonoBehaviour
{
    [Header("Selected Item Info")] 
    public Image selectedItemImage;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemValue;

    [Header("States")] 
    public TextMeshProUGUI ItemStateStrength;
    public TextMeshProUGUI ItemStateAttack;
    public TextMeshProUGUI ItemStateLuck;
    public TextMeshProUGUI ItemStateDefense;
    
    public void UpdateUI(Item item)
    {
        selectedItemImage.sprite = Resources.Load<Sprite>(item.image);
        selectedItemDescription.text = item.description;
        selectedItemName.text = item.name;
        selectedItemValue.text = item.value.ToString();
        
        // 아이템의 힘, 공격력, 행운, 방어력 은 어떻게?
    }
    
}