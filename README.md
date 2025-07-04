# la-Fosse-Dionne

이 코드는 TRPG(테이블톱 롤플레잉 게임) 랜덤 인카운터 시스템입니다. Unity에서 사용할 수 있도록 FastAPI로 구현된 백엔드 서비스입니다. 각 파일의 역할과 Unity 연동 방법을 자세히 설명드리겠습니다.
📋 코드 구조 분석
1. main.py - FastAPI 서버 메인
python# FastAPI 웹 서버 설정
app = FastAPI(title="TRPG Encounter API", version="1.0", lifespan=lifespan)

# CORS 설정 (Unity에서 접근 가능하도록)
app.add_middleware(CORSMiddleware, allow_origins=["*"])

# API 엔드포인트: POST /chat
@app.post("/chat", response_model=MessageResponse)
역할:

Unity와 통신할 REST API 서버
/chat 엔드포인트로 메시지 받고 응답 반환
CORS 설정으로 Unity에서 접근 가능

2. logic.py - 게임 로직 핵심
python# 전역 참조 문서 (게임 세계관 정보)
GLOBAL_RAG_CONTENT = ""

# 활성 인카운터 세션 관리
active_encounters = {}  # user_id → {"history": [], "turn": int}
주요 기능:

인카운터 시작: "랜덤 인카운터" 입력 시 새로운 에피소드 생성
턴 진행: 3-4턴으로 완결되는 짧은 스토리
수치 추출: 응답에서 성향값, 골드, 경험치, 정신력, 생명력 파싱
세션 관리: 사용자별 인카운터 상태 추적

3. utils.py - 유틸리티 함수
python# Gemini AI API 호출
def request_gemini(messages_history):
    # Google Gemini 1.5 Pro 모델 사용
    # 대화 히스토리를 유지하며 AI 응답 생성

기능:
Google Gemini AI와 통신
PDF 문서 로딩 (게임 세계관 자료)
API 오류 처리 및 재시도 로직

4. simple_chat.py - 테스트 클라이언트
콘솔에서 API를 테스트할 수 있는 간단한 클라이언트