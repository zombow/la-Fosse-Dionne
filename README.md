# la-Fosse-Dionne
> Google Gemini AI를 이용한 모바일 TRPG게임

### APK배포 : https://drive.google.com/file/d/1wuhTFpJJciYYybvyZrKvonBCSWMywGQ-/view?usp=drive_link

## 1. 게임 기획
AI가 실시간으로 생성하는 랜덤 인카운터를 통해 매번 새로운 이야기를 경험할 수 있는 모바일 TRPG 게임.
플레이어는 등장하는 상황과 캐릭터와의 대화를 통해 서사를 풀어나가며, 반복적인 플레이에도 변주되는 재미를 느낄 수 있도록하고
무작위성 기반의 AI 스토리텔링으로 예측 불가능한 전개와 높은 리플레이 가치를 제공하고자 하였습니다.

### 게임소개 영상
[![](http://img.youtube.com/vi/WVs9kClF4Tw/0.jpg)](https://www.youtube.com/shorts/WVs9kClF4Tw)
> YoutubeLink :[https://youtu.be/Hsq3YUm6IiE](https://www.youtube.com/shorts/WVs9kClF4Tw)

> 주인공은 돌이된 연인을 구하기위해 모험을 떠나면서 만나는 사람, 적, 상황에 대처하며 나아가고
> AI가 생성하는 랜덤인카운터와 채팅으로 대화하여 이야기를 풀어나가는 스토리입니다.

## 2. 게임제작 기간및 인원
**<ins>25.06.18 ~ 25.07.25 (약 1개월)</ins>**

### 3인개발
 - 기획 및 백앤드 `1명`
 - 프로그래밍 `1명`
 - 아트디자인 `1명`

### 흐름도
<img width="2276" height="1078" alt="image" src="https://github.com/user-attachments/assets/195b03a5-5f1d-4b56-93b8-4e47731b793d" />

<img width="1088" height="1153" alt="image" src="https://github.com/user-attachments/assets/9912c046-93c1-4f91-a20f-7dcd9da5ea9d" />

## 랜덤 인카운터 AI구조 (박현열)
# 🎮 TRPG AI API

> **Gemini AI를 활용한 완전한 TRPG 랜덤 인카운터 시스템**  
> Unity 게임을 위한 FastAPI 백엔드 서비스 - Railway에서 실행 중

[![Deploy on Railway](https://railway.app/button.svg)](https://railway.app)
![Python](https://img.shields.io/badge/python-v3.11+-blue.svg)
![FastAPI](https://img.shields.io/badge/FastAPI-0.104.1-009688.svg)
![Unity](https://img.shields.io/badge/Unity-Compatible-black.svg)
![AI](https://img.shields.io/badge/AI-Google%20Gemini-orange.svg)

## 📖 소개

**TRPG AI API**는 Google Gemini AI를 활용하여 동적인 TRPG 랜덤 인카운터를 생성하는 RESTful API 서버입니다. Unity 클라이언트와 완벽하게 연동되며, 플레이어의 행동에 따라 실시간으로 스토리가 변화하고 게임 스탯을 자동으로 관리합니다.

### ✨ 주요 기능

- 🤖 **AI 게임 마스터**: Gemini 1.5 Pro가 실시간으로 TRPG 시나리오 생성
- 🎯 **완결형 인카운터**: 3-4턴 내에 완료되는 짧고 재미있는 에피소드
- 📊 **자동 스코어 관리**: 행동에 따른 성향값, 골드, 경험치, 정신력, 생명력 자동 계산
- 👥 **멀티 세션 지원**: 사용자별 독립적인 게임 진행
- 🌍 **커스텀 세계관**: PDF 문서를 통한 게임 규칙 및 세계관 확장 (선택사항)
- ⚡ **Unity 최적화**: CORS 설정 및 JSON 기반 통신으로 간편한 연동

## 🚀 빠른 시작

### 🌐 배포된 서버 사용 (바로 시작)

이미 Railway에 배포된 서버를 바로 사용할 수 있습니다:

```
API 엔드포인트: https://trpg-ai-api-production.up.railway.app/chat
```

### Unity에서 바로 사용하기

```csharp
string apiUrl = "https://trpg-ai-api-production.up.railway.app/chat";

[System.Serializable]
public class TRPGRequest
{
    public string message;
    public string user_id;
}

[System.Serializable]
public class TRPGResponse
{
    public string response;
    public float? trait_score;
    public int? gold_amount;
    public int? exp_amount;
    public int? mental_amount;
    public int? health_amount;
}
```

### 🏠 로컬 개발 환경

```bash
# 저장소 클론
git clone https://github.com/yourusername/trpg-ai-api.git
cd trpg-ai-api

# 가상환경 생성
python -m venv venv
source venv/bin/activate  # Linux/Mac
# 또는
venv\Scripts\activate     # Windows

# 의존성 설치
pip install -r requirements.txt

# 환경변수 설정
echo "GEMINI_API_KEY=your_gemini_api_key_here" > .env

# 서버 실행
python main.py
```

> 💡 **API 키 발급**: [Google AI Studio](https://makersuite.google.com/app/apikey)에서 무료로 발급받을 수 있습니다.

## 📡 API 사용법

### 기본 엔드포인트

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/` | 서버 상태 확인 |
| `GET` | `/health` | 헬스체크 |
| `POST` | `/chat` | AI 인카운터 진행 |

### 인카운터 시작

```bash
curl -X POST https://trpg-ai-api-production.up.railway.app/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "인카운터", 
    "user_id": "player_1"
  }'
```

### 플레이어 행동

```bash
curl -X POST https://trpg-ai-api-production.up.railway.app/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "상인을 친절하게 도와준다", 
    "user_id": "player_1"
  }'
```

### 응답 형식

```json
{
  "response": "상인이 고맙다며 미소를 짓습니다. '정말 고마워요!' 하며 작은 치유 물약을 건넵니다.",
  "trait_score": 1.5,
  "gold_amount": 5,
  "exp_amount": 2,
  "mental_amount": 1,
  "health_amount": 0
}
```

## 🎲 게임 시스템

### 스코어 계산 규칙

| 요소 | 선한 행동 | 중립 행동 | 악한 행동 |
|------|-----------|-----------|-----------|
| **성향값** | +1 ~ +2 | 0 | -1 ~ -2 |
| **골드** | +3 ~ 10 | +5 ~ 15 | +15 ~ 30 |
| **경험치** | +1 ~ 2 | +2 ~ 3 | +3 ~ 5 |
| **정신력** | +1 (평화로운 해결) | 0 | -1 (스트레스) |
| **생명력** | +1 (휴식/치료) | 0 | -1 (위험한 행동) |

### 인카운터 예시

```
🎭 시나리오: 여관에서 만난 울고 있는 상인
📝 선택지:
  - "무슨 일인지 물어본다" → 정보 수집, 경험치 +2
  - "위로의 말을 건넨다" → 성향값 +1, 정신력 +1
  - "무시하고 지나간다" → 변화 없음
  - "물건을 훔친다" → 골드 +20, 성향값 -2
```

### 세션 관리

- **자동 시작**: "인카운터", "랜덤 인카운터", "start" 입력 시 새 에피소드 시작
- **턴 제한**: 3-4턴 내 자동 완료로 빠른 진행
- **사용자별 독립**: `user_id`를 통한 개별 세션 관리
- **메모리 기반**: 서버 재시작 시 세션 초기화

## 🏗️ 프로젝트 구조

```
trpg-ai-api/
├── 🚀 main.py              # FastAPI 서버 엔트리포인트
├── 🧠 logic.py             # 게임 로직 및 AI 처리
├── 🔧 utils.py             # Gemini API 연동 유틸리티
├── 📋 requirements.txt     # Python 의존성 패키지
├── 🐳 Dockerfile           # 컨테이너 배포 설정
├── 📚 pdfs/               # 게임 규칙 문서 (선택사항)
│   └── *.pdf              # 세계관 확장 문서들
└── 📖 README.md           # 프로젝트 문서
```

### 핵심 모듈

#### 🚀 main.py - FastAPI 서버
- Unity와 통신할 REST API 서버 구축
- CORS 설정으로 Unity 클라이언트 접근 허용
- 요청/응답 데이터 검증 및 형태 정의

#### 🧠 logic.py - 게임 로직
- AI 프롬프트 기반 시나리오 생성
- 정규표현식을 통한 게임 스탯 추출
- 사용자별 세션 상태 관리
- 턴 제한 및 자동 종료 처리

#### 🔧 utils.py - AI 연동
- Google Gemini 1.5 Pro API 호출
- PDF 문서 로딩 및 세계관 확장
- API 오류 처리 및 재시도 로직

## 🌐 Unity 연동 가이드

### C# 스크립트 예시

```csharp
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class TRPGManager : MonoBehaviour
{
    private string apiUrl = "https://trpg-ai-api-production.up.railway.app/chat";
    
    [System.Serializable]
    public class TRPGRequest
    {
        public string message;
        public string user_id;
    }
    
    [System.Serializable]
    public class TRPGResponse
    {
        public string response;
        public float? trait_score;
        public int? gold_amount;
        public int? exp_amount;
        public int? mental_amount;
        public int? health_amount;
    }
    
    public async Task<TRPGResponse> SendMessage(string message, string userId)
    {
        var request = new TRPGRequest { message = message, user_id = userId };
        string json = JsonUtility.ToJson(request);
        
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, json, "application/json"))
        {
            await www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                return JsonUtility.FromJson<TRPGResponse>(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"API 오류: {www.error}");
                return null;
            }
        }
    }
    
    // 사용 예시
    public async void StartEncounter()
    {
        var response = await SendMessage("인카운터", "player_001");
        if (response != null)
        {
            Debug.Log($"스토리: {response.response}");
            if (response.gold_amount.HasValue)
                Debug.Log($"골드 변화: {response.gold_amount}");
        }
    }
}
```

## 🚀 Railway 배포

### One-Click 배포

[![Deploy on Railway](https://railway.app/button.svg)](https://railway.app)

### 수동 배포 단계

1. **GitHub 저장소 생성**
   ```bash
   git clone https://github.com/yourusername/trpg-ai-api.git
   cd trpg-ai-api
   ```

2. **Railway 프로젝트 생성**
   - [Railway Dashboard](https://railway.app) 접속
   - "New Project" → "Deploy from GitHub repo"
   - 저장소 선택

3. **환경변수 설정** ⚠️ **중요!**
   ```
   Railway Variables 탭에서 추가:
   GEMINI_API_KEY = your_actual_gemini_api_key_here
   ```

4. **배포 확인**
   ```bash
   curl https://your-app.railway.app/health
   ```

## 🧪 테스트 가이드

### API 테스트

```bash
# 1. 서버 상태 확인
curl https://trpg-ai-api-production.up.railway.app/

# 2. 인카운터 시작
curl -X POST https://trpg-ai-api-production.up.railway.app/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "인카운터", "user_id": "test"}'

# 3. 플레이어 행동
curl -X POST https://trpg-ai-api-production.up.railway.app/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "친절하게 도와준다", "user_id": "test"}'
```

### 성공 확인

✅ **서버 실행**: `{"status": "online"}` 응답  
✅ **AI 응답**: Gemini가 실제 TRPG 시나리오 생성  
✅ **세션 관리**: 사용자별 인카운터 진행상황 저장  
✅ **스코어 시스템**: 행동에 따른 점수 변화  

## 🔧 문제해결

<details>
<summary><strong>🚫 Gemini API 키 오류</strong></summary>

```bash
# 환경변수 확인
echo $GEMINI_API_KEY

# Railway Variables 탭에서 GEMINI_API_KEY 설정 확인
# Google AI Studio에서 새 API 키 발급: https://makersuite.google.com/app/apikey
```
</details>

<details>
<summary><strong>🐍 PyMuPDF 빌드 오류</strong></summary>

Dockerfile에 시스템 의존성이 포함되어 있어 자동으로 해결됩니다:
```dockerfile
RUN apt-get update && apt-get install -y gcc
```
최악의 경우 requirements.txt에서 PyMuPDF 제거 가능
</details>

<details>
<summary><strong>💾 메모리 및 리소스</strong></summary>

- Railway Pro 플랜 사용 중: 8GB RAM, 8 vCPU 지원
- 현재 사용량: 약 200-300MB (충분한 여유 공간)
- 동시 접속 제한 없음 (Pro 플랜 혜택)
- 안정적인 24/7 서비스 보장
</details>

<details>
<summary><strong>🌐 CORS 오류 (Unity)</strong></summary>

서버에서 모든 도메인 허용하도록 설정되어 있습니다:
```python
app.add_middleware(CORSMiddleware, allow_origins=["*"])
```
</details>

## 📊 성능 및 제한사항

- **응답 시간**: 평균 2-5초 (AI 생성 시간 포함)
- **동시 접속**: Railway Pro 플랜으로 대용량 트래픽 지원
- **메모리 사용량**: 약 200-300MB (8GB 중)
- **세션 저장**: 메모리 기반 (서버 재시작 시 초기화)
- **API 호출 제한**: Gemini API 한도에 따라 제한
- **가용성**: 99.9% 업타임 보장 (Pro 플랜)

## 🛠️ 개발 및 커스터마이징

### 새로운 인카운터 타입 추가

`logic.py`에서 프롬프트 수정:

```python
# 새로운 시나리오 추가
if message.strip().lower() == "던전 인카운터":
    prompt = """던전 탐험 중 발생하는 인카운터를 만들어줘.
    - 몬스터, 함정, 보물 등 다양한 요소 포함
    - 위험도 높은 상황으로 구성
    ..."""
```

### PDF 세계관 추가

```bash
# pdfs 폴더에 PDF 파일 추가
mkdir pdfs
cp your_world_setting.pdf pdfs/
cp your_game_rules.pdf pdfs/
```

## 🤝 기여하기

1. Fork 프로젝트
2. Feature 브랜치 생성 (`git checkout -b feature/amazing-feature`)
3. 변경사항 커밋 (`git commit -m 'Add amazing feature'`)
4. 브랜치에 Push (`git push origin feature/amazing-feature`)
5. Pull Request 열기

## 📄 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다.

## 🙏 감사의 말

- 이소연(010-2741-1382) - 아트 
- 양호열(010-6861-0830) - 초기 전투 구현 
- 조연모(010-6695-5272) - 초기 프론트 구현
- 박준영(010-3763-6671) - 음악 

---

<div align="center">



</div>
