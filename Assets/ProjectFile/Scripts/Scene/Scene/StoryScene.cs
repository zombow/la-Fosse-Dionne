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
    private PlayerStats player;
    private StoryManager manager;
    public StoryBlock StartstoryBlock;
    public ScrollRect scrollRect;
    public RectTransform contentTransform;
    public GameObject textPrefab;
    public GameObject imagePrefab;
    public GameObject choiceContainer;
    public ChoiceButton choiceButtonPrefab;

    [Header("Utility")] public Toggle inventoryButton;
    public InventoryPopup inventoryPopupPrefab;
    private InventoryPopup instanceInventoryPopup;
    public Button settingButton;
    private SettingManager settingManager;
    public RectTransform storyPanel; // popup발동시 비활성화시키기위해서
    public RectTransform bottomPanel; // engin시 크기변경

    [Header("Battle")] public GameObject gaugePanel; // 전투시 비활성화를위해
    public Image storyBG;
    public Sprite defaultBgSprite;
    public Sprite battleBgSprite;

    [Header("Progress")] public Slider progressSlider;

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

        inventoryButton.onValueChanged.AddListener((isOn) =>
        {
            OpenPopup(instanceInventoryPopup.gameObject);
            instanceInventoryPopup.inventoryPanelPrefab.UpdateInventoryUI();
            instanceInventoryPopup.inventoryPanelPrefab.DiSelectSlot();
        });

        settingButton.onClick.AddListener(settingManager.PopupOnOff);

        characterInfoUI.InfoInit(player);
        settingManager.FontSizeChanged += ChangeFontSize;
        ChangeFontSize(settingManager.settingPrefab.fontSizeText.fontSize);
    }

    public void OpenPopup(GameObject instancedPopup)
    {
        if ((currentBlock.isShop || currentBlock.isBattleStart) && instancedPopup.gameObject.GetComponentInChildren<InventoryPopup>())
        {
            inventoryButton.isOn = false;
            return;
        }

        storyPanel.gameObject.SetActive(instancedPopup.activeSelf);
        choiceContainer.gameObject.SetActive(instancedPopup.activeSelf);

        instancedPopup.SetActive(!instancedPopup.activeSelf);
    }

    public void ClearDisplay()
    {
        // 이전 UI 초기화
        foreach (Transform child in contentTransform)
            Destroy(child.gameObject);
        foreach (Transform child in choiceContainer.transform)
            Destroy(child.gameObject);
        contentObjects.Clear();
        textComponents.Clear();
        textOriginals.Clear();
        lastDisplayedElement = -1;
    }

    public void Display(StoryBlock block)
    {
        ClearDisplay();

        currentBlock = block;

        SetChoiceButtonsActive(false);

        if (currentBlock.endingBlock)
        {
            gaugePanel.SetActive(false);
            storyPanel.anchorMin = new Vector2(0f, 0.18f);
            bottomPanel.anchorMax = new Vector2(1f, 0.18f);

            choiceButtonPrefab.EndingUI();
        }
        else
        {
            gaugePanel.SetActive(true);
            storyPanel.anchorMin = new Vector2(0f, 0.3f);
            bottomPanel.anchorMax = new Vector2(1f, 0.3f);

            choiceButtonPrefab.StoryUI();
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        skipRequested = false;
        typingCoroutine = StartCoroutine(TypeContentElements(block, manager));
    }

    private void ChangeFontSize(float size)
    {
        currentBlock.contentElements.ForEach(element =>
        {
            if (element.type == StoryElementType.Text)
            {
                foreach (var textComponent in textComponents)
                {
                    textComponent.fontSize = size;
                }
            }
        });
    }

    public void BeginBattle(StoryBlock block)
    {
        currentBlock = block;

        ClearDisplay();
        gaugePanel.SetActive(false);
        storyBG.sprite = battleBgSprite;
    }

    public void BeginShop(StoryBlock block)
    {
        currentBlock = block;
        ClearDisplay();
    }

    public void BeginRandomEncounter(StoryBlock block, TRPGChatManager chatManager)
    {
        currentBlock = block;
        ClearDisplay();

        GameObject buttonObj = Instantiate(choiceButtonPrefab.gameObject, choiceContainer.transform);
        ChoiceButton button = buttonObj.GetComponent<ChoiceButton>();
        chatManager.sendButton = button.sendButton;
        chatManager.messageInput = button.inputField;
        chatManager.startEncounterButton = button.storyButton;
        chatManager.startEncounterText = button.buttonText;
        button.buttonText.text = " ";

        button.BeginRandomEncounterUI();

        var textGO = Instantiate(textPrefab, contentTransform);
        var tmp = textGO.GetComponent<TextMeshProUGUI>();
        chatManager.chatText = tmp;
        chatManager.OnEncounterStart += button.RandomEncounterUI;
        chatManager.RandomEncounterEnd += () => { EndRandomEncounter(block); };
        chatManager.InitChat(player);
        gaugePanel.SetActive(false);
    }

    private void EndRandomEncounter(StoryBlock block)
    {
        SetChoiceButtons(block, manager);
    }

    private string GenerateRewardText(StoryBlock block)
    {
        List<string> lines = new();

        if (block.rewardGold > 0)
        {
            lines.Add($"골드: {block.rewardGold}");
            player.playerStateBlock.gold += currentBlock.rewardGold;
        }

        if (block.rewardItems != null && block.rewardItems.Count > 0)
        {
            foreach (var itemId in currentBlock.rewardItems)
            {
                player.AddItem(AssetManager.Instance.itemList[itemId]);
                lines.Add($"획득 아이템: {string.Join(", ", AssetManager.Instance.itemList[itemId].name)}");
            }
        }

        if (!string.IsNullOrEmpty(block.rewardTrait))
        {
            lines.Add($"획득 특성: {block.rewardTrait}"); // 성향은 미구현상태
        }

        if (block.rewardMorality != 0)
        {
            lines.Add($"도덕성 변화: {block.rewardMorality}");
            player.playerStateBlock.playerStatus[StateType.Mortality] += currentBlock.rewardMorality;
        }

        if (block.rewardLifePoint != 0)
        {
            player.playerStateBlock.playerStatus[StateType.Life] += currentBlock.rewardLifePoint;
            lines.Add($"생명력 변화: {block.rewardLifePoint}");
        }

        if (block.rewardSpiritPoint != 0)
        {
            player.playerStateBlock.playerStatus[StateType.Spirit] += currentBlock.rewardSpiritPoint;
            lines.Add($"정신력 변화: {block.rewardSpiritPoint}");
        }

        if (block.deleteItems != null && block.deleteItems.Count > 0)
        {
            foreach (var itemId in currentBlock.deleteItems)
            {
                player.DeleteItem(AssetManager.Instance.itemList[itemId], true);
                lines.Add($"제거된 아이템: {string.Join(", ", AssetManager.Instance.itemList[itemId].name)}");
            }
        }

        return lines.Count > 0 ? string.Join("\n", lines) : null;
    }


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

        string rewardText = GenerateRewardText(block);
        if (!string.IsNullOrEmpty(rewardText))
        {
            var rewardGO = Instantiate(textPrefab, contentTransform);
            var tmp = rewardGO.GetComponent<TextMeshProUGUI>();
            tmp.text = rewardText;
            textComponents.Add(tmp);
            textOriginals.Add(rewardText);
            contentObjects.Add(rewardGO);
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        isTyping = false;
        typingCoroutine = null;

        player.RecalculateStats();
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
        foreach (Transform child in choiceContainer.transform)
            Destroy(child.gameObject);

        foreach (var choice in block.choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab.gameObject, choiceContainer.transform);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => { manager.Choose(choice); });
        }
    }

    private void SetChoiceButtonsActive(bool active)
    {
        foreach (Transform child in choiceContainer.transform)
        {
            var btn = child.GetComponent<Button>();
            if (btn != null)
                btn.interactable = active;
        }
    }

    public void UpdateGaugePanel(StoryBlock block)
    {
        gaugePanel.SetActive(true);
        progressSlider.maxValue = block.maxIndex;
        progressSlider.value = block.myIndex;
    }
}