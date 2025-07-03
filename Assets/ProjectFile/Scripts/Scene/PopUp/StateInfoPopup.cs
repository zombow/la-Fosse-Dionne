using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StateInfoPopup : MonoBehaviour
{
    public Image stateIcon;
    public TextMeshProUGUI statePointText;
    public TextMeshProUGUI stateInfoText;
    public Button exitButton;
    // Start is called before the first frame update

    public void InitPopup(StateInfos infos)
    {
        stateIcon.sprite = infos.stateIcone.sprite;
        statePointText.text = infos.statePoint.ToString();
        stateInfoText.text = infos.stateTextinfo;
    }
}