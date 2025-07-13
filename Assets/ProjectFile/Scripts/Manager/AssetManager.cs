using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


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

    public Dictionary<string, Monster> monsterList = new Dictionary<string, Monster>();
    private Dictionary<int, Sprite> DiceSprites = new Dictionary<int, Sprite>();
    public Sprite[] RollingSprites;
    public Dictionary<string, Sprite[]> EffectSprites = new Dictionary<string, Sprite[]>();

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
        LoadMonsterDatabase();
        LoadDiceSprites();
        LoadEffect();
    }


    private void LoadItemDatabase()
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
                item.LoadSprites();
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

    private void LoadMonsterDatabase()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("monsters");
        if (jsonText == null)
        {
            Debug.LogError("monsters.json을 찾을 수 없습니다.");
            return;
        }

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        // 먼저 List<Monster>로 역직렬화
        List<Monster> monsters = JsonConvert.DeserializeObject<List<Monster>>(jsonText.text, settings);

        // Dictionary로 변환
        monsterList = new Dictionary<string, Monster>();
        foreach (var monster in monsters)
        {
            if (!monsterList.ContainsKey(monster.id))
            {
                monster.LoadSprites();
                monsterList.Add(monster.id, monster);
            }
            else
            {
                Debug.LogWarning($"중복된 몬스터 ID: {monster.id} - 무시됨");
            }
        }
    }

    private void LoadDiceSprites()
    {
        for (int i = 1; i <= 20; i++)
        {
            Sprite Dicenumber = Resources.Load<Sprite>($"DiceImage/rolling dice {i}");
            DiceSprites.Add(i, Dicenumber);
        }

        RollingSprites = Resources.LoadAll<Sprite>($"DiceImage/rollingdice");
    }

    public List<Sprite> LoadDiceRolling(int diceNumber)
    {
        List<Sprite> numberSprite = new List<Sprite>();
        numberSprite.AddRange<Sprite>(RollingSprites);
        numberSprite.Add(DiceSprites[diceNumber]);
        return numberSprite;
    }

    private void LoadEffect()
    {
        EffectSprites.Add("smoke", Resources.LoadAll<Sprite>($"EffectImage/SmokeEffect"));
    }
}