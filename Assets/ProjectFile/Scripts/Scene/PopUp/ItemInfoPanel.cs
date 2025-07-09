using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemIcons
{
    public StateType stateType;
    public Sprite stateIcon;
}

public class ItemInfoPanel : MonoBehaviour
{
    [Header("Selected Item Info")] public Image selectedItemImage;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemValue;

    [Header("States")] public StateInfoBlock ItemState;
    public Transform ItemStateParent;

    private List<StateInfoBlock> ItemStateList = new List<StateInfoBlock>();
    public List<ItemIcons> ItemStateIcons = new List<ItemIcons>();
    Dictionary<StateType, Sprite> itemStateIconDictionary = new Dictionary<StateType, Sprite>();

    private void Awake()
    {
        foreach (var val in ItemStateIcons)
        {
            itemStateIconDictionary.Add(val.stateType, val.stateIcon);
        }
    }

    public void UpdateUI(Item item)
    {
        selectedItemImage.sprite = Resources.Load<Sprite>(item.image);
        selectedItemDescription.text = item.description;
        selectedItemName.text = item.name;
        selectedItemValue.text = item.value.ToString();

        if (ItemStateList != null)
        {
            foreach (var val in ItemStateList)
            {
                Destroy(val.gameObject);
            }
            ItemStateList.Clear();
        }


        foreach (var val in item.stats)
        {
            var instance = Instantiate(ItemState, ItemStateParent);
            instance.stateIcon.sprite = itemStateIconDictionary[val.Key];
            instance.statePoint.text = val.Value.ToString();
            ItemStateList.Add(instance);
        }
    }
}