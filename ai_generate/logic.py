# logic.py - 수정된 최종 버전
import re
from utils import request_gemini, load_pdfs_from_local

# 📚 전역 참조 문서 내용
GLOBAL_RAG_CONTENT = ""

# 임시 세션 저장소
active_encounters = {}  # user_id → {"history": [], "turn": int}

def init_rag_context():
    """
    서버 시작 시 PDF 폴더에서 문서 내용을 로딩하여 전역 상태에 저장
    """
    global GLOBAL_RAG_CONTENT
    GLOBAL_RAG_CONTENT = load_pdfs_from_local("pdfs")

def extract_scores(text: str):
    """
    응답 텍스트에서 성향값/골드/경험치/정신력/생명력 추출
    """
    trait = gold = exp = mental = health = None
    
    trait_match = re.search(r"성향값\s*[:：]\s*([+-]?\d+\.?\d*)", text)
    if trait_match:
        try:
            trait = float(trait_match.group(1))
        except ValueError:
            pass

    gold_match = re.search(r"골드\s*[:：]\s*([+-]?\d+)", text)
    if gold_match:
        try:
            gold = int(gold_match.group(1))
        except ValueError:
            pass

    exp_match = re.search(r"경험치\s*[:：]\s*([+-]?\d+)", text)
    if exp_match:
        try:
            exp = int(exp_match.group(1))
        except ValueError:
            pass
            
    mental_match = re.search(r"정신력\s*[:：]\s*([+-]?\d+)", text)
    if mental_match:
        try:
            mental = int(mental_match.group(1))
        except ValueError:
            pass
            
    health_match = re.search(r"생명력\s*[:：]\s*([+-]?\d+)", text)
    if health_match:
        try:
            health = int(health_match.group(1))
        except ValueError:
            pass

    return trait, gold, exp, mental, health

def process_user_message(user_id: str, message: str):
    """
    메인 API 함수 - 사용자 메시지 처리
    """
    print(f"📨 [USER:{user_id}] 메시지: {message}")
    
    # 새 인카운터 시작
    if message.strip().lower() in ["랜덤 인카운터", "인카운터", "start"]:
        # 새로운 히스토리 생성
        history = [{
            "role": "user",
            "content": (
                "너는 TRPG 게임 마스터다. "
                "메인 스토리의 분위기 환기용 짧은 랜덤 인카운터를 만들어라. "
                "3-4번의 상호작용 후 자연스럽게 끝나는 완결된 에피소드여야 한다."
            )
        }]
        
        prompt = f"""짧고 완결성 있는 랜덤 인카운터를 시작해줘.

[조건]
- 3-4턴 내에 완료되는 간단한 에피소드
- 중세 판타지 배경 (마을, 숲, 여관 등)
- 가볍고 재미있는 내용 (분위기 환기용)
- 즉시 상황에 몰입할 수 있도록 시작
- 골드나 보상은 구체적인 물건이나 상황으로 묘사

[게임 세계관]
- 플레이어는 모험가로서 길드에 소속되어 있음
- 얻은 물건들은 길드나 상점에서 골드로 환전 가능
- 정보나 경험은 모험가로서의 실력 향상에 도움

[참고 자료]
{GLOBAL_RAG_CONTENT}

"당신은..." 형식으로 바로 상황을 시작해줘.
절대 성향값, 골드, 경험치, 정신력, 생명력은 아직 출력하지 마."""

        history.append({"role": "user", "content": prompt})
        result = request_gemini(history)
        history.append({"role": "assistant", "content": result})
        
        # 세션에 저장
        active_encounters[user_id] = {"history": history, "turn": 1}
        
        trait, gold, exp, mental, health = extract_scores(result)
        return result, trait, gold, exp, mental, health
    
    # 인카운터 진행 중인 사용자의 응답
    elif user_id in active_encounters:
        encounter = active_encounters[user_id]
        history = encounter["history"]
        turn = encounter["turn"]
        
        # 3턴 이후는 자동 종료
        should_end = turn >= 3
        
        action_prompt = f"""플레이어의 행동: "{message}"

현재 {turn}턴째입니다.

{'[마지막 턴] 이 인카운터를 자연스럽게 마무리해주세요.' if should_end else '[계속 진행] 상황을 발전시켜주세요.'}

[응답 규칙]
- 플레이어의 행동에 대한 자연스러운 결과를 이야기로 서술해주세요
- "### 결과:" 같은 형식 제목을 사용하지 마세요
- 바로 이야기로 시작해주세요
- 플레이어를 대신해서 행동하지 말고, 상황의 반응만 묘사해주세요

{'[종료 시 포함사항 - 플레이어의 행동과 결과에 따라 다양하게 책정]' if should_end else ''}
{'- 이야기의 자연스러운 마무리' if should_end else ''}
{'- 맨 마지막에만: 인카운터 종료' if should_end else ''}
{'- 성향값: 선한 행동 +1~+2, 중립 0, 악한/이기적 행동 -1~-2' if should_end else ''}
{'- 골드: 선한 행동 +3~10, 중립 +5~15, 악한/이기적 행동 +15~30 (더 많은 이익)' if should_end else ''}  
{'- 경험치: 선한 행동 +1~2, 중립 +2~3, 악한/교활한 행동 +3~5 (더 많은 경험)' if should_end else ''}
{'- 정신력: 스트레스 상황 -1, 평화로운 해결 +1, 무서운/충격적 경험 0' if should_end else ''}
{'- 생명력: 위험한 행동 -1, 휴식/치료 +1, 안전한 행동 0' if should_end else ''}

{'[중요] 도덕적 선택에 따른 보상 차별화:' if should_end else ''}
{'- 선한 행동: 성향값 높음, 골드/경험치 적음 (정의로운 길)' if should_end else ''}
{'- 악한 행동: 성향값 낮음, 골드/경험치 많음 (쉽고 이익되는 길)' if should_end else ''}
{'- 생명력과 정신력은 매우 소중한 자원이므로 -1, 0, +1 중에서만 선택하세요.' if should_end else ''}
{'[보상 설명 방식] 골드를 직접 주지 말고 물건이나 상황으로 묘사한 후 설명:' if should_end else ''}
{'- "상인이 고마움의 표시로 작은 보석을 건네줍니다. 나중에 길드에서 골드로 환전할 수 있을 것입니다."' if should_end else ''}
{'- "주머니에서 떨어진 동전들을 주워 담습니다. 대략 15골드 정도의 가치로 보입니다."' if should_end else ''}
{'- "귀중한 약초를 발견했습니다. 연금술사에게 팔면 좋은 값을 받을 수 있을 것 같습니다."' if should_end else ''}
{'- "NPC가 정보나 지도 조각을 제공합니다. 이는 나중에 유용할 정보로 경험치가 됩니다."' if should_end else ''}

**형식 제목 없이 바로 이야기로 응답해주세요.**"""

        history.append({"role": "user", "content": action_prompt})
        result = request_gemini(history)
        history.append({"role": "assistant", "content": result})
        
        # 턴 증가
        encounter["turn"] += 1
        
        # 인카운터 종료 시 세션 삭제
        if should_end or "인카운터 종료" in result:
            del active_encounters[user_id]
        
        trait, gold, exp, mental, health = extract_scores(result)
        return result, trait, gold, exp, mental, health
    
    # 활성 인카운터가 없는 경우 새로 시작
    else:
        return process_user_message(user_id, "랜덤 인카운터")