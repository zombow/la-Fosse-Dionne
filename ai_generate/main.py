# main.py - Railway 배포용
import os
from fastapi import FastAPI, Request
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
from contextlib import asynccontextmanager

# Railway 환경에서는 간단한 버전으로 시작
app = FastAPI(title="TRPG Encounter API", version="1.0")

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
    trait_score: float = 0.0
    gold_amount: int = 0
    exp_amount: int = 0
    mental_amount: int = 0
    health_amount: int = 0

@app.get("/")
async def root():
    """서버 상태 확인"""
    return {
        "message": "🎮 TRPG 서버가 Railway에서 실행 중입니다!",
        "status": "online",
        "platform": "Railway",
        "endpoints": {
            "chat": "/chat",
            "health": "/health"
        }
    }

@app.get("/health")
async def health():
    """헬스 체크"""
    return {"status": "healthy", "service": "TRPG API"}

@app.post("/chat", response_model=MessageResponse)
async def chat(request: MessageRequest):
    """TRPG 채팅 처리"""
    message = request.message.lower()
    user_id = request.user_id
    
    print(f"📨 [USER:{user_id}] 메시지: {request.message}")
    
    # 간단한 TRPG 로직 (AI 없이 동작)
    if "인카운터" in message or "시작" in message:
        encounters = [
            "🌲 당신은 신비로운 숲에서 반짝이는 보석을 발견했습니다. 보석 주변에는 이상한 기운이 느껴집니다. 어떻게 하시겠습니까?",
            "🏰 고대 성의 입구에서 수상한 상인을 만났습니다. '특별한 물건을 팔고 있어요!'라고 속삭입니다. 접근하시겠습니까?",
            "🌙 달빛 아래 우물에서 신비한 목소리가 들려옵니다. '소원을 말해보세요...' 우물에 다가가시겠습니까?",
            "⚔️ 길에서 다친 기사를 발견했습니다. 그는 당신에게 도움을 요청하고 있습니다. 도와주시겠습니까?",
            "📜 오래된 도서관에서 금지된 마법서를 발견했습니다. 책에서 이상한 빛이 나오고 있습니다. 책을 여시겠습니까?"
        ]
        import random
        response_text = random.choice(encounters)
        return MessageResponse(response=response_text)
    
    # 선한 행동
    elif any(word in message for word in ["도와", "친절", "구해", "치료", "도움"]):
        response_text = "당신의 선한 행동이 좋은 결과를 가져왔습니다! 주변 사람들이 감사해하며 작은 보상을 줍니다. 따뜻한 마음씨에 모든 이들이 미소를 짓습니다.\n\n✨ 인카운터 종료"
        return MessageResponse(
            response=response_text,
            trait_score=2.0,
            gold_amount=8,
            exp_amount=2,
            mental_amount=1,
            health_amount=0
        )
    
    # 공격적 행동
    elif any(word in message for word in ["공격", "싸우", "빼앗", "위협", "죽이"]):
        response_text = "폭력적인 해결책이었지만 효과적이었습니다. 더 많은 전리품을 얻었으나 양심이 무겁습니다. 주변 사람들이 당신을 두려워하는 눈빛으로 바라봅니다.\n\n⚔️ 인카운터 종료"
        return MessageResponse(
            response=response_text,
            trait_score=-2.0,
            gold_amount=20,
            exp_amount=4,
            mental_amount=-1,
            health_amount=-1
        )
    
    # 대화/평화적 해결
    elif any(word in message for word in ["대화", "말", "설득", "협상", "평화"]):
        response_text = "현명한 말로 상황을 해결했습니다. 모든 당사자가 만족하는 결과를 이끌어냈으며, 귀중한 경험을 쌓았습니다. 지혜로운 중재로 갈등이 평화롭게 마무리되었습니다.\n\n🕊️ 인카운터 종료"
        return MessageResponse(
            response=response_text,
            trait_score=1.0,
            gold_amount=12,
            exp_amount=3,
            mental_amount=1,
            health_amount=0
        )
    
    # 기타 행동
    else:
        import random
        responses = [
            f"'{request.message}'는 흥미로운 선택이었습니다! 예상치 못한 결과가 나타났습니다.",
            f"당신의 창의적인 접근 방식이 독특한 상황을 만들어냅니다.",
            f"모험가다운 결정이군요! 이런 선택이 진정한 모험의 묘미입니다."
        ]
        response_text = random.choice(responses) + "\n\n🎲 인카운터 종료"
        
        return MessageResponse(
            response=response_text,
            trait_score=0.0,
            gold_amount=random.randint(5, 12),
            exp_amount=random.randint(2, 3),
            mental_amount=0,
            health_amount=0
        )

# Railway가 자동으로 PORT 환경변수 설정
if __name__ == "__main__":
    import uvicorn
    port = int(os.environ.get("PORT", 8000))
    print(f"🚀 Railway에서 TRPG 서버 시작! 포트: {port}")
    uvicorn.run(app, host="0.0.0.0", port=port)