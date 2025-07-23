using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    public Image buttonIcon;
    public TextMeshProUGUI buttonText;

    public void EndingUI()
    {
        buttonText.rectTransform.anchorMin = new Vector2(0f, 0f);
        buttonText.rectTransform.anchorMax = new Vector2(1f, 1f);
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonIcon.gameObject.SetActive(false);
    }

    public void StoryUI()
    {
        buttonText.rectTransform.anchorMin = new Vector2(0.15f, 0.2f);
        buttonText.rectTransform.anchorMax = new Vector2(1f, 0.8f);
        buttonText.alignment = TextAlignmentOptions.Left;
        buttonIcon.gameObject.SetActive(true);
    }
}
