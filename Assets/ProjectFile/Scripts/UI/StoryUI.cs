// StoryUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StoryUI : MonoBehaviour
{
    public ScrollRect scrollRect; // 스크롤 전체
    public RectTransform contentTransform; // 스크롤 내부 Content (빈 VerticalLayoutGroup 제거됨)
    public GameObject textPrefab;
    public GameObject imagePrefab;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    public void Display(StoryBlock block, StoryManager manager)
    {
        // 이전 콘텐츠 제거
        foreach (Transform child in contentTransform)
            Destroy(child.gameObject);

        // 블록 내 콘텐츠 생성
        foreach (var element in block.contentElements)
        {
            switch (element.type)
            {
                case StoryElementType.Text:
                    var textGO = Instantiate(textPrefab, contentTransform);
                    textGO.GetComponent<TextMeshProUGUI>().text = element.text;
                    break;

                case StoryElementType.Image:
                    var imageGO = Instantiate(imagePrefab, contentTransform);
                    var image = imageGO.GetComponent<Image>();
                    image.sprite = element.image;
                    image.preserveAspect = true;
                    break;
            }
        }

        // 스크롤 최상단 초기화
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;

        // 기존 선택지 제거
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        // 선택지 생성
        foreach (var choice in block.choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => {
                manager.Choose(choice);
            });
        }
    }
}