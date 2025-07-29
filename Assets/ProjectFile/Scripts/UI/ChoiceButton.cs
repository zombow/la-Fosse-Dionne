using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [Header("Story")] public Image buttonIcon;
    public TextMeshProUGUI buttonText;
    public Button storyButton;

    [Header("RandomEncounter")] public TMP_InputField inputField;
    public Button sendButton;

    public void EndingUI()
    {
        buttonIcon.gameObject.SetActive(false);
        buttonText.gameObject.SetActive(true);
        inputField.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
        
        buttonText.rectTransform.anchorMin = new Vector2(0f, 0f);
        buttonText.rectTransform.anchorMax = new Vector2(1f, 1f);
        buttonText.alignment = TextAlignmentOptions.Center;
    }

    public void StoryUI()
    {
        buttonText.rectTransform.anchorMin = new Vector2(0.15f, 0.2f);
        buttonText.rectTransform.anchorMax = new Vector2(1f, 0.8f);
        buttonText.alignment = TextAlignmentOptions.Left;
        buttonIcon.gameObject.SetActive(true);
        buttonText.gameObject.SetActive(true);
        
        inputField.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
    }

    public void BeginRandomEncounterUI()
    {
        buttonIcon.gameObject.SetActive(false);
        buttonText.gameObject.SetActive(true);
        buttonText.rectTransform.anchorMin = new Vector2(0f, 0f);
        buttonText.rectTransform.anchorMax = new Vector2(1f, 1f);
        buttonText.alignment = TextAlignmentOptions.Center;
        
        inputField.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
    }

    public void RandomEncounterUI()
    {
        buttonIcon.gameObject.SetActive(false);
        buttonText.gameObject.SetActive(false);
        
        inputField.gameObject.SetActive(true);
        sendButton.gameObject.SetActive(true);
    }
}