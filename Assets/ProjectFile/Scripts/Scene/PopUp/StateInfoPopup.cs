using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateInfoPopup : MonoBehaviour
{
    public Image stateIcon;
    public TextMeshProUGUI stateNameText;
    public TextMeshProUGUI statePointText;
    public TextMeshProUGUI stateInfoText;
    public Button exitButton;
    // Start is called before the first frame update

    public void InitPopup(StateInfos infos)
    {
        stateNameText.text = $"모험가 {infos.playername}의 능력치";
        stateIcon.sprite = infos.stateIcon.sprite;
        statePointText.text = infos.statePoint.text;
        stateInfoText.text = infos.stateTextinfo;
    }
}