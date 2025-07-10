using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

// Python 서버로 보낼 메시지 형식
[System.Serializable]
public class TRPGMessage
{
    public string message;
    public string user_id;
    
    public TRPGMessage(string msg, string userId = "unity_player")
    {
        message = msg;
        user_id = userId;
    }
}

// Python 서버에서 받을 응답 형식 (nullable 제거)
[System.Serializable]
public class TRPGResponse
{
    public string response = "";
    public float trait_score = 0f;
    public int gold_amount = 0;
    public int exp_amount = 0;
    public int mental_amount = 0;
    public int health_amount = 0;
    
    // 값이 있는지 확인하는 헬퍼 메소드들
    public bool HasTraitScore() { return trait_score != 0; }
    public bool HasGoldAmount() { return gold_amount != 0; }
    public bool HasExpAmount() { return exp_amount != 0; }
    public bool HasMentalAmount() { return mental_amount != 0; }
    public bool HasHealthAmount() { return health_amount != 0; }
}

// 플레이어 스탯 관리
[System.Serializable]
public class PlayerStats
{
    public float trait = 0f;       // 성향값 (선악)
    public int gold = 100;         // 골드
    public int experience = 0;     // 경험치
    public int mental = 100;       // 정신력
    public int health = 100;       // 생명력
    
    public void ApplyChanges(TRPGResponse response)
    {
        if (response.HasTraitScore())
            trait += response.trait_score;
        if (response.HasGoldAmount())
            gold += response.gold_amount;
        if (response.HasExpAmount())
            experience += response.exp_amount;
        if (response.HasMentalAmount())
            mental = Mathf.Clamp(mental + response.mental_amount, 0, 100);
        if (response.HasHealthAmount())
            health = Mathf.Clamp(health + response.health_amount, 0, 100);
    }
}

public class TRPGChatManager : MonoBehaviour
{
    [Header("=== Python 서버 설정 ===")]
    public string serverUrl = "http://127.0.0.1:8000/chat";
    public string playerId = "unity_player";
    
    [Header("=== 채팅 UI ===")]
    public TextMeshProUGUI chatText;
    public TMP_InputField messageInput;
    public Button sendButton;
    public Button startEncounterButton;
    
    [Header("=== 스탯 UI ===")]
    public TextMeshProUGUI traitText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI mentalText;
    public TextMeshProUGUI healthText;
    
    [Header("=== 플레이어 데이터 ===")]
    public PlayerStats playerStats = new PlayerStats();
    
    private bool isWaitingResponse = false;
    private bool encounterActive = false;
    
    void Start()
    {
        Debug.Log("🎮 TRPG 채팅 시스템 시작!");
        
        // 버튼 이벤트 연결 (null 체크 추가)
        if (sendButton != null)
            sendButton.onClick.AddListener(SendMessage);
        else
            Debug.LogWarning("⚠️ SendButton이 연결되지 않았습니다!");
            
        if (startEncounterButton != null)
            startEncounterButton.onClick.AddListener(StartEncounter);
        else
            Debug.LogWarning("⚠️ StartEncounterButton이 연결되지 않았습니다!");
        
        // Enter 키 입력 처리
        if (messageInput != null)
            messageInput.onEndEdit.AddListener(OnEnterPressed);
        else
            Debug.LogWarning("⚠️ MessageInput이 연결되지 않았습니다!");
        
        // 초기 UI 설정
        UpdateStatsUI();
        
        if (chatText != null)
            chatText.text = "⚔️ TRPG 인카운터 시스템에 오신 것을 환영합니다!\n'인카운터 시작' 버튼을 눌러 모험을 시작하세요!";
        else
            Debug.LogWarning("⚠️ ChatText가 연결되지 않았습니다!");
        
        // 서버 연결 테스트
        StartCoroutine(TestConnection());
    }
    
    IEnumerator TestConnection()
    {
        string testUrl = serverUrl.Replace("/chat", "");
        Debug.Log($"🔍 서버 연결 테스트: {testUrl}");
        
        using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ 서버 연결 성공!");
                AddMessageToChat("✅ Python TRPG 서버에 연결되었습니다!");
            }
            else
            {
                Debug.LogWarning($"⚠️ 서버 연결 실패: {request.error}");
                AddMessageToChat("❌ TRPG 서버 연결 실패. 서버를 실행해주세요!");
            }
        }
    }
    
    public void StartEncounter()
    {
        Debug.Log("🎯 새로운 인카운터 시작!");
        SendMessageToServer("랜덤 인카운터");
        encounterActive = true;
        if (startEncounterButton != null)
            startEncounterButton.interactable = false;
    }
    
    void OnEnterPressed(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage();
        }
    }
    
    public void SendMessage()
    {
        if (messageInput == null)
        {
            Debug.LogWarning("⚠️ MessageInput이 null입니다!");
            return;
        }
        
        string message = messageInput.text.Trim();
        
        if (string.IsNullOrEmpty(message))
        {
            Debug.Log("📝 빈 메시지입니다.");
            return;
        }
        
        if (isWaitingResponse)
        {
            Debug.Log("⏳ 이미 응답을 기다리고 있습니다.");
            return;
        }
        
        Debug.Log($"📤 플레이어 행동: {message}");
        
        // 플레이어 메시지 표시
        AddMessageToChat($"🎭 플레이어: {message}");
        messageInput.text = "";
        
        // 서버로 전송
        SendMessageToServer(message);
    }
    
    void SendMessageToServer(string message)
    {
        StartCoroutine(SendMessageCoroutine(message));
    }
    
    IEnumerator SendMessageCoroutine(string message)
    {
        isWaitingResponse = true;
        if (sendButton != null)
            sendButton.interactable = false;
        
        // 로딩 표시
        AddMessageToChat("🎲 GM이 상황을 판단하는 중...");
        
        // 요청 데이터 준비 (Unity JsonUtility 사용)
        TRPGMessage requestData = new TRPGMessage(message, playerId);
        string jsonData = "";
        
        // JSON 직렬화
        try
        {
            jsonData = JsonUtility.ToJson(requestData);
            Debug.Log($"📋 전송할 JSON: {jsonData}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ JSON 직렬화 오류: {e.Message}");
            RemoveLastMessage();
            AddMessageToChat("❌ 메시지 전송 중 오류가 발생했습니다.");
            if (sendButton != null)
                sendButton.interactable = true;
            isWaitingResponse = false;
            yield break;
        }
        
        // HTTP 요청
        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 30;
            
            yield return request.SendWebRequest();
            
            // 로딩 메시지 제거
            RemoveLastMessage();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"📥 서버 응답 성공: {request.downloadHandler.text}");
                ProcessTRPGResponse(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"❌ 서버 오류: {request.error}");
                Debug.LogError($"❌ 응답 코드: {request.responseCode}");
                AddMessageToChat($"❌ 서버 연결 오류: {request.error}");
                AddMessageToChat("🔧 Python 서버가 실행되고 있는지 확인해주세요!");
            }
        }
        
        if (sendButton != null)
            sendButton.interactable = true;
        isWaitingResponse = false;
    }
    
    void ProcessTRPGResponse(string jsonResponse)
    {
        try
        {
            Debug.Log($"📥 받은 JSON: {jsonResponse}");
            
            // Unity JsonUtility로 파싱
            TRPGResponse response = JsonUtility.FromJson<TRPGResponse>(jsonResponse);
            
            if (response == null)
            {
                Debug.LogError("❌ 응답 객체가 null입니다!");
                AddMessageToChat("❌ 서버 응답을 처리할 수 없습니다.");
                return;
            }
            
            // GM 응답 표시
            if (!string.IsNullOrEmpty(response.response))
            {
                AddMessageToChat($"🎪 GM: {response.response}");
            }
            
            // 스탯 변화 적용 (안전한 복사)
            PlayerStats oldStats = new PlayerStats
            {
                trait = playerStats.trait,
                gold = playerStats.gold,
                experience = playerStats.experience,
                mental = playerStats.mental,
                health = playerStats.health
            };
            
            playerStats.ApplyChanges(response);
            
            // 스탯 변화 애니메이션
            StartCoroutine(ShowStatChanges(oldStats, response));
            
            // UI 업데이트
            UpdateStatsUI();
            
            // 인카운터 종료 확인
            if (response.response != null && response.response.Contains("인카운터 종료"))
            {
                encounterActive = false;
                if (startEncounterButton != null)
                    startEncounterButton.interactable = true;
                AddMessageToChat("📜 인카운터가 완료되었습니다! 새로운 모험을 시작할 수 있습니다.");
            }
            
            Debug.Log("✅ TRPG 응답 처리 완료!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 응답 처리 오류: {e.Message}");
            Debug.LogError($"❌ 받은 응답: {jsonResponse}");
            AddMessageToChat("❌ 서버 응답 처리 중 오류가 발생했습니다.");
        }
    }
    
    IEnumerator ShowStatChanges(PlayerStats oldStats, TRPGResponse response)
    {
        // 스탯 변화 표시
        string changes = "📊 변화: ";
        bool hasChanges = false;
        
        if (response.HasTraitScore())
        {
            changes += $"성향 {response.trait_score:+0.0} ";
            hasChanges = true;
        }
        if (response.HasGoldAmount())
        {
            changes += $"골드 {response.gold_amount:+0} ";
            hasChanges = true;
        }
        if (response.HasExpAmount())
        {
            changes += $"경험치 {response.exp_amount:+0} ";
            hasChanges = true;
        }
        if (response.HasMentalAmount())
        {
            changes += $"정신력 {response.mental_amount:+0} ";
            hasChanges = true;
        }
        if (response.HasHealthAmount())
        {
            changes += $"생명력 {response.health_amount:+0} ";
            hasChanges = true;
        }
        
        if (hasChanges)
        {
            yield return new WaitForSeconds(1f);
            AddMessageToChat(changes.Trim());
        }
    }
    
    void UpdateStatsUI()
    {
        if (traitText != null)
        {
            string traitDesc = GetTraitDescription(playerStats.trait);
            traitText.text = $"성향: {playerStats.trait:F1} ({traitDesc})";
            traitText.color = GetTraitColor(playerStats.trait);
        }
        
        if (goldText != null)
            goldText.text = $"골드: {playerStats.gold}";
        
        if (expText != null)
            expText.text = $"경험치: {playerStats.experience}";
        
        if (mentalText != null)
        {
            mentalText.text = $"정신력: {playerStats.mental}/100";
            mentalText.color = GetHealthColor(playerStats.mental);
        }
        
        if (healthText != null)
        {
            healthText.text = $"생명력: {playerStats.health}/100";
            healthText.color = GetHealthColor(playerStats.health);
        }
    }
    
    string GetTraitDescription(float trait)
    {
        if (trait >= 5f) return "성인";
        if (trait >= 2f) return "선량함";
        if (trait >= -2f) return "중립";
        if (trait >= -5f) return "이기적";
        return "악인";
    }
    
    Color GetTraitColor(float trait)
    {
        if (trait > 0) return new Color(0.8f, 0.9f, 1f);  // 연한 파랑 (선)
        if (trait < 0) return new Color(1f, 0.8f, 0.8f);  // 연한 빨강 (악)
        return Color.white;  // 중립
    }
    
    Color GetHealthColor(int health)
    {
        if (health >= 80) return Color.green;
        if (health >= 50) return Color.yellow;
        if (health >= 20) return new Color(1f, 0.5f, 0f);  // 오렌지색
        return Color.red;
    }
    
    void AddMessageToChat(string message)
    {
        if (chatText == null)
        {
            Debug.LogWarning("⚠️ ChatText가 null입니다!");
            return;
        }
        
        if (string.IsNullOrEmpty(chatText.text))
            chatText.text = message;
        else
            chatText.text += "\n\n" + message;
        
        StartCoroutine(ScrollToBottom());
        Debug.Log($"💬 채팅 추가: {message}");
    }
    
    void RemoveLastMessage()
    {
        if (chatText == null) return;
        
        string[] messages = chatText.text.Split(new string[] { "\n\n" }, System.StringSplitOptions.None);
        if (messages.Length > 1)
        {
            string[] newMessages = new string[messages.Length - 1];
            System.Array.Copy(messages, newMessages, messages.Length - 1);
            chatText.text = string.Join("\n\n", newMessages);
        }
    }
    
    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        
        if (chatText == null) 
            yield break;
        
        ScrollRect scrollRect = chatText.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}