using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public StoryManager StoryManager;

    public Transform panelRoot; // UI 패널을 붙일 부모 (예: Canvas 하위)
    private GameObject currentPanel;
    public StartScene startScenePrefab;
    public CharacterCreateScene characterCreateScenePrefab;
    public StoryScene storyScenePrefab;

    public void Start()
    {
        if (startScenePrefab != null)
        {
            ShowPanel(startScenePrefab.gameObject);
        }
    }

    public void ShowPanel(GameObject panelPrefab)
    {
        if (currentPanel != null)
            Destroy(currentPanel);

        currentPanel = Instantiate(panelPrefab, panelRoot);
    }

    public void ChangeScene()
    {
        ShowPanel(storyScenePrefab.gameObject);
    }
}