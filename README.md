# la-Fosse-Dionne
> Google Gemini AI를 이용한 모바일 TRPG게임

## 1. 게임 기획
AI가 실시간으로 생성하는 랜덤 인카운터를 통해 매번 새로운 이야기를 경험할 수 있는 모바일 TRPG 게임.
플레이어는 등장하는 상황과 캐릭터와의 대화를 통해 서사를 풀어나가며, 반복적인 플레이에도 변주되는 재미를 느낄 수 있도록하고
무작위성 기반의 AI 스토리텔링으로 예측 불가능한 전개와 높은 리플레이 가치를 제공하고자 하였습니다.

### 게임소개 영상
> YoutubeLink :
> 주인공은 돌이된 연인을 구하기위해 모험을 떠나면서 만나는 사람, 적, 상황에 대처하며 나아가고
> AI가 생성하는 랜덤인카운터와 채팅으로 대화하여 이야기를 풀어나가는 스토리입니다.

## 2. 게임제작 기간및 인원
**<ins>25.06.18 ~ 25.07.25 (약 1개월)</ins>**

### 3인개발
 - 기획 및 백앤드 `1명`
 - 프로그래밍 `1명`
 - 아트디자인 `1명`

## 3. 담당파트
3일중 `프로그래밍`으로
 - 프로젝트 기본구조
 - 전투 시스템
 - 이야기 흐름시스템
 - UI와 플레이어상태 연결

부분을 담당하였습니다.

### 담당 흐름도
Manager를 이용해 이야기가 흘러가는 플로우차트작성예정


## 랜덤 인카운터 AI구조 (박현열)
이 코드는 TRPG(테이블톱 롤플레잉 게임) 랜덤 인카운터 시스템입니다. Unity에서 사용할 수 있도록 FastAPI로 구현된 백엔드 서비스입니다.

### 코드구조
### 1. main.py – FastAPI 서버 메인
``` # FastAPI 웹 서버 설정
app = FastAPI(title="TRPG Encounter API", version="1.0", lifespan=lifespan)

# CORS 설정 (Unity에서 접근 가능하도록)
app.add_middleware(CORSMiddleware, allow_origins=["*"])

# API 엔드포인트: POST /chat
@app.post("/chat", response_model=MessageResponse)
```
#### 역할
Unity와 통신할 REST API 서버
/chat 엔드포인트로 메시지를 받아 응답 반환
CORS 설정을 통해 Unity 접근 허용

### 2. logic.py - 게임 로직 핵심
```
# 전역 참조 문서 (게임 세계관 정보)
GLOBAL_RAG_CONTENT = ""

# 활성 인카운터 세션 관리
active_encounters = {}  # user_id → {"history": [], "turn": int}
```
#### 주요기능

- 인카운터 시작: "랜덤 인카운터" 입력 시 새로운 에피소드 생성

- 턴 진행: 3-4턴으로 완결되는 짧은 스토리
  
- 수치 추출: 응답에서 성향값, 골드, 경험치, 정신력, 생명력 파싱
  
- [](url)세션 관리: 사용자별 인카운터 상태 추적

### 3. utils.py - 유틸리티 함수
```
# Gemini AI API 호출
def request_gemini(messages_history):
    # Google Gemini 1.5 Pro 모델 사용
    # 대화 히스토리를 유지하며 AI 응답 생성

```
#### 기능
 - Google Gemini AI와 통신
 - PDF 문서 로딩 (게임 세계관 자료)
 - API 오류 처리 및 재시도 로직
---
### 4. simple_chat.py - 테스트 클라이언트
- 콘솔에서 API를 테스트할 수 있는 간단한 클라이언트
