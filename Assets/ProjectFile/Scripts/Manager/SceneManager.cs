using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType
{
    Start,
    CharacterCreate,
    Story
}

[System.Serializable]
public struct ScenePrefab
{
    public SceneType sceneType;
    public GameObject prefab;
}

public class SceneManager : MonoBehaviour
{
    public Transform panelRoot; // UI 패널을 붙일 부모 (예: Canvas 하위)
    private GameObject currentPanel;

    public List<ScenePrefab> scenePrefabs;

    public void Start()
    {
        foreach (var scene in scenePrefabs)
        {
            if (scene.sceneType == SceneType.Start)
            {
                ShowPanel(scene.prefab);
            }
        }
    }

    public void ShowPanel(GameObject panelPrefab)
    {
        if (currentPanel != null)
            Destroy(currentPanel);

        currentPanel = Instantiate(panelPrefab, panelRoot);
    }

    public void ChangeScene(SceneType sceneType)
    {
        foreach (var scene in scenePrefabs)
        {
            if (scene.sceneType == sceneType)
            {
                ShowPanel(scene.prefab);
            }
        }
    }
}