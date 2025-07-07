using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class StoryScene : MonoBehaviour
{
    private StoryManager manager;
    public PlayerStats player;
    public StoryBlock StartstoryBlock;
    public ScrollRect scrollRect;
    public RectTransform contentTransform;
    public GameObject textPrefab;
    public GameObject imagePrefab;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;

    [Header("Utility")] public Button inventoryButton;
    public InventoryPopup inventoryPopupPrefab;
    private InventoryPopup instanceInventoryPopup;
    public Button settingButton;
    private SettingManager settingManager;
    public GameObject storyPanel; // popup발동시 비활성화시키기위해서

    [Header("Battle")] public GameObject gaugePanel; // 전투시 비활성화를위해
    public Image storyBG;
    public Sprite defaultBgSprite;
    public Sprite battleBgSprite;

    [Header("Character")] public CharacterInfoUI characterInfoUI;

    public float typingSpeed = 0.04f;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipRequested = false;
    private int lastDisplayedElement = -1;

    private List<GameObject> contentObjects = new();
    private List<TextMeshProUGUI> textComponents = new();

    // 직렬화용 데이터 저장
    private List<string> textOriginals = new();
    private StoryBlock currentBlock;
    private StoryManager currentManager;

    private void Awake()
    {
        manager = FindObjectOfType<StoryManager>();
        settingManager = FindObjectOfType<SettingManager>();
        if (!manager)
        {
            Debug.LogError("Manager Not Found!");
            return;
        }

        manager.InitAndStart(StartstoryBlock, this);
    }

    public void Start()
    {
        player = manager.player;
        // InventroyPopup 초기화
        instanceInventoryPopup = Instantiate(inventoryPopupPrefab, transform);
        instanceInventoryPopup.SetupInventory(player);
        instanceInventoryPopup.gameObject.SetActive(false);

        inventoryButton.onClick.AddListener(() => OpenPopup(instanceInventoryPopup.gameObject));
        settingButton.onClick.AddListener(settingManager.PopupOnOff);

        characterInfoUI.InfoInit(player);
    }

    public void OpenPopup(GameObject instancedPopup)
    {
        if ((currentBlock.isShop || currentBlock.isBattleStart) && instancedPopup.gameObject.GetComponentInChildren<InventoryPopup>())
        {
            return;
        }

        storyPanel.SetActive(instancedPopup.activeSelf);
        choiceContainer.gameObject.SetActive(instancedPopup.activeSelf);

        instancedPopup.SetActive(!instancedPopup.activeSelf);
    }

    public void ClearDisplay()
    {
        // 이전 UI 초기화
        foreach (Transform child in contentTransform)
            Destroy(child.gameObject);
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);
        contentObjects.Clear();
        textComponents.Clear();
        textOriginals.Clear();
        lastDisplayedElement = -1;
    }

    public void Display(StoryBlock block, StoryManager manager)
    {
        ClearDisplay();

        currentBlock = block;
        currentManager = manager;

        SetChoiceButtonsActive(false);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        skipRequested = false;
        typingCoroutine = StartCoroutine(TypeContentElements(block, manager));
    }

    public void BeginBattle(StoryBlock block, StoryManager storyManager)
    {
        currentBlock = block;
        currentManager = storyManager;
        ClearDisplay();
        gaugePanel.SetActive(false);
        storyBG.sprite = battleBgSprite;
    }

    public void BeginShop(StoryBlock block, StoryManager storyManager)
    {
        currentBlock = block;
        currentManager = storyManager;
        ClearDisplay();
    }

// [핵심] 텍스트/이미지를 순서대로 출력. skip 시 즉시 모든 내용 표시
    private IEnumerator TypeContentElements(StoryBlock block, StoryManager manager)
    {
        isTyping = true;
        for (int idx = 0; idx < block.contentElements.Count; idx++)
        {
            var element = block.contentElements[idx];

            if (element.type == StoryElementType.Text)
            {
                var textGO = Instantiate(textPrefab, contentTransform);
                var tmp = textGO.GetComponent<TextMeshProUGUI>();
                textComponents.Add(tmp);
                textOriginals.Add(element.text);
                contentObjects.Add(textGO);

                if (skipRequested)
                {
                    tmp.text = element.text;
                }
                else
                {
                    yield return StartCoroutine(TypeRichTextSmart(element.text, tmp, typingSpeed));
                }
            }
            else if (element.type == StoryElementType.Image)
            {
                var imageGO = Instantiate(imagePrefab, contentTransform);
                var image = imageGO.GetComponentInChildren<Image>();
                image.sprite = element.image;
                image.preserveAspect = true;
                var layout = imageGO.GetComponent<LayoutElement>();
                if (layout != null && element.image != null)
                {
                    float aspect = (float)element.image.rect.width / element.image.rect.height;
                    layout.preferredHeight = contentTransform.rect.width / aspect;
                }

                contentObjects.Add(imageGO);
            }

            lastDisplayedElement = idx;

            if (skipRequested)
                break;
        }

        // skip요청시 남은 모든 텍스트/이미지 즉시 노출
        if (skipRequested)
        {
            for (int idx = lastDisplayedElement + 1; idx < block.contentElements.Count; idx++)
            {
                var element = block.contentElements[idx];
                if (element.type == StoryElementType.Text)
                {
                    var textGO = Instantiate(textPrefab, contentTransform);
                    var tmp = textGO.GetComponent<TextMeshProUGUI>();
                    tmp.text = element.text;
                    textComponents.Add(tmp);
                    textOriginals.Add(element.text);
                    contentObjects.Add(textGO);
                }
                else if (element.type == StoryElementType.Image)
                {
                    var imageGO = Instantiate(imagePrefab, contentTransform);
                    var image = imageGO.GetComponentInChildren<Image>();
                    image.sprite = element.image;
                    image.preserveAspect = true;
                    var layout = imageGO.GetComponent<LayoutElement>();
                    if (layout != null && element.image != null)
                    {
                        float aspect = (float)element.image.rect.width / element.image.rect.height;
                        layout.preferredHeight = contentTransform.rect.width / aspect;
                    }

                    contentObjects.Add(imageGO);
                }
            }
        }

        isTyping = false;
        typingCoroutine = null;
        SetChoiceButtons(block, manager);
        SetChoiceButtonsActive(true);
    }

// RichText 안전 타이핑 (태그/특수문자 지원, '<' 문자 구분)
    private IEnumerator TypeRichTextSmart(string src, TextMeshProUGUI tmp, float speed)
    {
        List<RichChar> chars = ParseRichText(src);
        int charCount = chars.Count;
        for (int step = 1; step <= charCount; step++)
        {
            if (skipRequested) break;
            int upTo = chars[step - 1].srcIndex + 1;
            string partial = src.Substring(0, upTo);
            partial = CloseUnclosedTags(partial);
            tmp.text = partial;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            yield return new WaitForSeconds(speed);
        }

        tmp.text = src;
    }

    public struct RichChar
    {
        public string displayed;
        public int srcIndex;
    }

    public static List<RichChar> ParseRichText(string src)
    {
        var result = new List<RichChar>();
        int i = 0;
        while (i < src.Length)
        {
            if (src[i] == '<')
            {
                int close = src.IndexOf('>', i);
                bool isTag = false;
                if (close > i + 1)
                {
                    string inner = src.Substring(i + 1, close - i - 1);
                    isTag = Regex.IsMatch(inner, @"^/?[a-zA-Z0-9\=\#""'% ]+$");
                }

                if (isTag)
                {
                    i = close + 1;
                    continue;
                }
                else
                {
                    result.Add(new RichChar { displayed = "<", srcIndex = i });
                    i++;
                    continue;
                }
            }

            result.Add(new RichChar { displayed = src[i].ToString(), srcIndex = i });
            i++;
        }

        return result;
    }

    string CloseUnclosedTags(string text)
    {
        Stack<string> tagStack = new();
        Regex tagRe = new(@"<(/?[^<>]+?)>");
        var matches = tagRe.Matches(text);
        foreach (Match match in matches)
        {
            string tag = match.Groups[1].Value;
            if (!tag.StartsWith("/"))
                tagStack.Push(tag);
            else if (tagStack.Count > 0)
                tagStack.Pop();
        }

        string closing = "";
        while (tagStack.Count > 0)
        {
            string tag = tagStack.Pop().Split(' ')[0];
            closing += $"</{tag}>";
        }

        return text + closing;
    }

    public void OnPanelClick()
    {
        if (isTyping)
        {
            skipRequested = true;
        }
    }

    private void SetChoiceButtons(StoryBlock block, StoryManager manager)
    {
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (var choice in block.choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => { manager.Choose(choice); });
        }
    }

    private void SetChoiceButtonsActive(bool active)
    {
        foreach (Transform child in choiceContainer)
        {
            var btn = child.GetComponent<Button>();
            if (btn != null)
                btn.interactable = active;
        }
    }

    public void UpdateGaugePanel(object gaugeValue)
    {
        gaugePanel.SetActive(true);
    }
}