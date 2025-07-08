using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem
{
    public Dictionary<ItemType, List<Item>> items = new Dictionary<ItemType, List<Item>>
    {
        { ItemType.Weapon, new List<Item>() },
        { ItemType.Shield, new List<Item>() },
        { ItemType.Armor, new List<Item>() },
        { ItemType.Accessory, new List<Item>() },
        { ItemType.Special, new List<Item>() },
        { ItemType.Consumable, new List<Item>() }
    };
}

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance { get; private set; }
    public Dictionary<string, Item> itemList = new Dictionary<string, Item>();

    public Dictionary<int, ShopItem> shopItemList = new Dictionary<int, ShopItem>
    {
        { 1, new ShopItem() },
        { 2, new ShopItem() },
        { 3, new ShopItem() },
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadItemDatabase();
    }

    void LoadItemDatabase()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("equipment_with_images");
        if (jsonText == null)
        {
            Debug.LogError("item_database.json을 찾을 수 없습니다.");
            return;
        }

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        // 먼저 List<Item>으로 역직렬화
        List<Item> items = JsonConvert.DeserializeObject<List<Item>>(jsonText.text, settings);

        // Dictionary로 변환
        itemList = new Dictionary<string, Item>();
        foreach (var item in items)
        {
            if (!itemList.ContainsKey(item.id))
            {
                itemList.Add(item.id, item);
                if (itemList[item.id].type != ItemType.Special)
                {
                    shopItemList[itemList[item.id].tier].items[itemList[item.id].type].Add(item);
                }
            }
            else
            {
                Debug.LogWarning($"중복된 아이템 ID: {item.id} - 무시됨");
            }
        }
    }
}