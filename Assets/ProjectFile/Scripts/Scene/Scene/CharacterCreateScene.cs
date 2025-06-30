using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum GenderType
{
    Female,
    Male
}

public class GenderInfo
{
    public GenderType genderType; // 성별타입
    public int looksIndex; // 외모 인덱스
}

public class CharacterCreateScene : MonoBehaviour
{
    public Button StartButton;
    private SceneManager sceneManager;
    private Transform pannelroot;
    public AlertPopup alertPopupPrefab;
    private AlertPopup alertPopupInstance;

    [Header("---Player Name---")] [SerializeField]
    public TMP_InputField playerNameTextbox;

    [Header("Player Gender")] [SerializeField]
    private GenderInfo PlayerGender = new GenderInfo();
    public GenderType basePlayerGender = GenderType.Male;
    public int baseLooksIndex = 0; // 기본 외모 인덱스


    [Header("Player State")] [SerializeField]
    public int baseStatPoints, remainingPoints = 16; // 기본 능력치 포인트


    void Start()
    {
        StartButton.onClick.AddListener(OnStart);

        sceneManager = FindObjectOfType<SceneManager>();
        if (!sceneManager)
        {
            Debug.LogError("Manager Not Found!");
        }

        PlayerGender.genderType = basePlayerGender;// 기본 플레이어 성별 설정
        PlayerGender.looksIndex = baseLooksIndex; // 기본 플레이어 외모 인덱스 설정
        
        pannelroot = sceneManager.baseCanvas.transform;
        alertPopupInstance = Instantiate(alertPopupPrefab, pannelroot);
        alertPopupInstance.gameObject.SetActive(false);
    }

    public void SetPlayerGenderType(GenderType genderType)
    {
        PlayerGender.genderType = genderType;
    }
    
    public void SetPlayerGenderlooks(int looksIndex)
    {
        PlayerGender.looksIndex = looksIndex;
    }

    private void OnStart()
    {
        EventSystem.current.SetSelectedGameObject(null); // 포커스 초기화로 최종값 설정
        if (string.IsNullOrWhiteSpace(playerNameTextbox.text))
        {
            alertPopupInstance.SetText("모험가의 이름을 적어주세요");
        }

        else if (PlayerGender == null)
        {
            alertPopupInstance.SetText("모험가의 성별을 선택해주세요");
        }

        else if (remainingPoints > 0)
        {
            alertPopupInstance.SetText("남은 능력치 포인트가 있습니다.\n" +
                                       "능력치를 모두 분배해주세요.");
        }
        else
        {
            // TODO:Player정보 초기화 하는동작
            sceneManager.ChangeScene(SceneType.Story);
            return;
        }

        alertPopupInstance.gameObject.SetActive(true);
    }
}