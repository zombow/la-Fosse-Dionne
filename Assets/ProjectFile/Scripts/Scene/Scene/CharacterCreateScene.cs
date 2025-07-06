using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class CharacterCreateScene : MonoBehaviour
{
    public Button StartButton;
    private SceneManager sceneManager;
    private PlayerStats player;
    private PlayerStats tempPlayerStats;

    public AlertPopup alertPopupPrefab;
    private AlertPopup alertPopupInstance;

    [Header("---Player Name---")] [SerializeField]
    public TMP_InputField playerNameTextbox;

    public string basePlayerGender = "Female";
    public Image baseLooksImage;

    [Header("Player State")] [SerializeField]
    public int baseStatPoints, remainingPoints = 16; // 기본 능력치 포인트

    [Header("Player Weapon")] [SerializeField]
    public int baseWeaponIndex = 0; // 기본 무기 인덱스

    private int weaponIndex = 0; // 현재 선택된 무기 인덱스
    public TMP_Dropdown weaponDropdownBox;


    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        tempPlayerStats = gameObject.AddComponent<PlayerStats>();
        
        StartButton.onClick.AddListener(OnStart);
        sceneManager = FindObjectOfType<SceneManager>();
        if (!sceneManager)
        {
            Debug.LogError("Manager Not Found!");
        }


        tempPlayerStats.playerStateBlock.gender = basePlayerGender; // 기본 플레이어 성별 설정
        tempPlayerStats.playerStateBlock.looksSprite = baseLooksImage.sprite; // 기본 플레이어 외모 인덱스 설정

        weaponDropdownBox.onValueChanged.AddListener(SetWeaponIndex);

        alertPopupInstance = Instantiate(alertPopupPrefab, transform);
        alertPopupInstance.gameObject.SetActive(false);
    }

    public void SetPlayerGenderType(string genderType)
    {
        tempPlayerStats.playerStateBlock.gender = genderType;
    }

    public void SetPlayerGenderlooks(Sprite looksSprite)
    {
        tempPlayerStats.playerStateBlock.looksSprite = looksSprite;
    }

    public void SetWeaponIndex(int index)
    {
        weaponIndex = index;
    }

    public void SetPlayerStatePoint(Dictionary<StateType, int> playerStatsParam)
    {
        tempPlayerStats.playerStateBlock.playerStatus = playerStatsParam;

    }
    private void OnStart()
    {
        EventSystem.current.SetSelectedGameObject(null); // 포커스 초기화로 최종값 설정
        if (string.IsNullOrWhiteSpace(playerNameTextbox.text))
        {
            alertPopupInstance.SetText("모험가의 이름을 적어주세요");
        }

        else if (remainingPoints > 0)
        {
            alertPopupInstance.SetText("남은 능력치 포인트가 있습니다.\n" +
                                       "능력치를 모두 분배해주세요.");
        }
        else
        {
            tempPlayerStats.playerStateBlock.playerName = playerNameTextbox.text;

            player.PlayerInit(tempPlayerStats);
            sceneManager.ChangeScene(SceneType.Story);
            return;
        }

        alertPopupInstance.gameObject.SetActive(true);
    }
}