using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreateScene : MonoBehaviour
{
    public Button StartButton;

    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(OnStart);
    }

    private void OnStart()
    {
        // TODO: 캐릭터생성 조건추가 (State적용, 이름작성여부 판단)
        var uiManager = FindObjectOfType<SceneManager>();
        if (!uiManager)
        {
            Debug.LogError("Manager Not Found!");
            return;
        }
        
        uiManager.ChangeScene(SceneType.Story);
    }
}