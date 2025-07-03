using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StateInfos
{
    public Image stateIcone;
    public int statePoint;
    public Button button;
    [TextArea] public string stateTextinfo;
}

public class StatePopupController : MonoBehaviour
{
    public Transform StatePopupParent;
    public List<StateInfos> stateInfoButtons;
    public StateInfoPopup stateInfoPopupPrefab;
    private StateInfoPopup instanceStateInfoPopupPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // StateInfoPopup 초기화
        instanceStateInfoPopupPrefab = Instantiate(stateInfoPopupPrefab, StatePopupParent);
        instanceStateInfoPopupPrefab.gameObject.SetActive(false);
        instanceStateInfoPopupPrefab.exitButton.onClick.AddListener(() => instanceStateInfoPopupPrefab.gameObject.SetActive(false));

        foreach (var button in stateInfoButtons)
        {
            button.button.onClick.AddListener(() => OpenPopup(instanceStateInfoPopupPrefab, button));
        }
    }

    void OpenPopup(StateInfoPopup popup, StateInfos stateInfos)
    {
        popup.InitPopup(stateInfos);
        popup.gameObject.SetActive(!popup.gameObject.activeSelf);
    }
}