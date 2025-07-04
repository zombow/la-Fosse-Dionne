using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetManager : MonoBehaviour
{
    public List<ItemData> itemList;
    void Awake()
    {
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

        itemList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText.text);
    }

}
