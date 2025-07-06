# main.py - 호스트 문제 해결 버전
from fastapi import FastAPI, Request
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
from logic import process_user_message, init_rag_context
from contextlib import asynccontextmanager
import os

@asynccontextmanager
async def lifespan(app: FastAPI):
    print("🔄 앱 시작 중...")
    print("📂 PDF 문서를 로딩 중...")
    init_rag_context()
    yield
    print("🔄 앱 종료 중...")

app = FastAPI(title="TRPG Encounter API", version="1.0", lifespan=lifespan)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

class MessageRequest(BaseModel):
    message: str
    user_id: str = "default_user"

class MessageResponse(BaseModel):
    response: str
    trait_score: float | None = None
    gold_amount: int | None = None
    exp_amount: int | None = None
    mental_amount: int | None = None
    health_amount: int | None = None

@app.get("/")
async def root():
    """서버 상태 확인용 엔드포인트"""
    return {"message": "TRPG 서버가 정상 작동 중입니다! 🎮", "status": "running"}

@app.post("/chat", response_model=MessageResponse)
async def chat(request: MessageRequest):
    message = request.message.strip()
    user_id = request.user_id.strip() or "default_user"

    if not message:
        return MessageResponse(response="입력된 메시지가 없습니다.")

    try:
        response, trait_score, gold_amount, exp_amount, mental_amount, health_amount = process_user_message(user_id=user_id, message=message)
    except Exception as e:
        print(f"❌ 오류: {e}")
        return MessageResponse(response="서버 오류가 발생했습니다.")

    print("📤 Gemini 응답:")
    print(response)

    # 디버깅용 로그 저장
    debug_path = os.path.join("logs", f"last_output_{user_id}.txt")
    os.makedirs("logs", exist_ok=True)
    with open(debug_path, "w", encoding="utf-8") as f:
        f.write(response)

    return MessageResponse(
        response=response,
        trait_score=trait_score,
        gold_amount=gold_amount,
        exp_amount=exp_amount,
        mental_amount=mental_amount,
        health_amount=health_amount
    )

# 서버 실행 코드 - 호스트 설정 변경
if __name__ == "__main__":
    import uvicorn
    print("🐍 TRPG 서버를 시작합니다...")
    print("🌐 http://127.0.0.1:8000 에서 실행됩니다")
    print("🔗 브라우저에서 http://127.0.0.1:8000 접속하여 확인하세요")
    
    # 호스트를 127.0.0.1로 변경
    uvicorn.run(app, host="127.0.0.1", port=8000, reload=False)