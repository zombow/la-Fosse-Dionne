# logic.py
import re
from utils import request_gemini, load_pdfs_from_local

# 💾 사용자별 대화 히스토리 및 상태 저장
message_histories = {}  # user_id → message history
user_states = {}  # user_id → (trait, gold, exp)
user_story_active = {}  # user_id → 인카운터 진행 중 여부
user_current_scenario = {}  # user_id → 현재 시나리오 텍스트 저장

# 📚 전역 참조 문서 내용
GLOBAL_RAG_CONTENT = ""
last_result_cache = {}
last_user_choice = {}

def extract_relevant_block(text, number):
    pattern = rf"### 선택지:\s*(?:.*\n)*?\n*{number}\. .*\n"
    match = re.search(pattern, text)
    return match.group(0) if match else text

def init_rag_context():
    """
    서버 시작 시 PDF 폴더에서 문서 내용을 로딩하여 전역 상태에 저장
    """
    global GLOBAL_RAG_CONTENT
    GLOBAL_RAG_CONTENT = load_pdfs_from_local("pdfs")

def extract_scores(user_id: str, text: str):
    """
    응답 텍스트에서 성향값/골드/경험치 추출 후 저장
    """
    trait = gold = exp = None

    trait_match = re.search(r"성향값\s*[:：]\s*([+-]?\d+\.?\d*)", text)
    if trait_match:
        try:
            trait = float(trait_match.group(1))
        except ValueError:
            pass

    gold_match = re.search(r"골드\s*[:：]\s*(\d+)", text)
    if gold_match:
        try:
            gold = int(gold_match.group(1))
        except ValueError:
            pass

    exp_match = re.search(r"경험치\s*[:：]\s*(\d+)", text)
    if exp_match:
        try:
            exp = int(exp_match.group(1))
        except ValueError:
            pass

    user_states[user_id] = (trait, gold, exp)

def process_user_message(user_id: str, message: str):
    result = ""  # result 변수를 초기화하여 UnboundLocalError 방지

    print(f"📨 [USER:{user_id}] 입력 메시지: {message}")

    if user_id not in message_histories:
        message_histories[user_id] = []
        user_story_active[user_id] = False
        user_current_scenario[user_id] = ""

    history = message_histories[user_id]

    if message.strip().lower() in ["랜덤 인카운터", "시작", "인카운터"]:
        user_story_active[user_id] = True
        history.clear()  # 기존 히스토리 완전 삭제
        user_current_scenario[user_id] = ""  # 현재 시나리오도 초기화
        
        # 새로운 게임 마스터 역할 설정
        system_prompt = {
            "role": "user",
            "content": (
                "너는 중세 판타지 RPG 인카운터를 이끄는 게임 마스터이다. "
                "사용자와의 상호작용을 통해 짧고 유한한 이야기의 흐름을 구성하고, 선택지 기반으로 갈등 또는 보상을 제공해야 한다."
            )
        }
        history.append(system_prompt)
        
        prompt = f"""판타지 세계를 배경으로 한 랜덤 인카운터를 하나 생성해.
        [조건]
        - 장소: 마을, 숲, 폐허, 동굴 등 다양한 환경 중 하나
        - 등장인물 또는 존재: 인간, 오크, 엘프, 드워프, 정령, 괴물 등
        - 사건 유형: 매복, 구조 요청, 비밀 거래, 저주, 전염병 등
        - 플레이어가 취할 수 있는 선택지를 최소 3개 이상 포함
        - 선택에 따라 결과가 달라질 수 있도록 만들어줘 (갈등 or 보상 중심)
        - 플레이어가 어딘가에 억압되거나 갇히게 만들면 안된다.
        - 무언가 물건을 얻게 되거나 잃게해서는 안된다.
        - 폭력적인 묘사나 자해 요소, 억압하는 묘사는 절대 넣지마
        - 선택지 아래에는 절대 사견, 평가, 부연 설명을 넣지 말 것.

        [형식]
        - 제목
        - 장소 설명
        - 등장 인물/존재
        - 상황 설명
        - 선택지

        참고 문서 내용을 참고해서 만들어. 단, 문서 내용의 세계관은 참고하되 주요 스토리와 연관 없이 만들어.
        ---
        참고 문서 내용:
        {GLOBAL_RAG_CONTENT}
        ---

        **형식을 따르되 절대 형식 제목은 나타내지 않고 이야기 하듯 풀어내. 하지만 선택지 아래로는 사견을 붙이지 마.**  
        **선택지에 따른 결과는 보이지 않게 해줘. 전투 관련된 선택지는 나오지 않게 해야 해.**  
        **인카운터의 길이가 짧게 만들어줘**
        **선택지를 제시하고 나서 어떤 결과도 출력하지 마.**
        **선택 결과가 입력되기 전에는 절대 인카운터 종료, 성향값, 골드를 출력하지 마.**
        **결과가 출력되지 않는 경우에는 절대 '인카운터 종료'나 성향값, 골드를 출력하지 않는다. 절대 중간에 출력하지 않는다.**
        **인카운터가 종료되는 마지막 턴에만 '인카운터 종료'를 맨 마지막 줄에 쓰고, 그 바로 아래 줄에 '성향값: +1' 형태로 -1, 0, 1 셋 중 하나를 판별하여 출력하라. 또한 골드도 '골드: 15' 형태로 10~20 사이의 수를 입력해라**
        **또한 '경험치: 3' 형태로 1에서 5 사이의 숫자를 랜덤으로 출력하라**

        **선택지를 1. [선택지1], 2. [선택지2], 3. [선택지3]과 같이 번호 매겨서 제공해줘. 각 선택지는 한 줄로 끝나야 해.**
        **만약 플레이어가 특정 행동을 한 후 결과만을 알려줄 때는, '### 결과:'로 시작하는 섹션 안에 내용을 서술하고, 이후에는 아무런 선택지도 제공하지 마세요.**
        **만약 플레이어가 다음 행동을 선택해야 할 때는, '### 선택지:'로 시작하는 섹션 안에 번호 매겨진 선택지 목록을 제공하세요.**
        **다음 행동에 대한 선택지를 제공할 때는 반드시 ### 선택지: 라는 헤더 아래에 번호 매겨진 목록을 제공해야 합니다. 다른 어떤 추가 텍스트도 헤더에 포함하지 마세요.**

        ** 절대 여러 번 선택지를 출력하지 마. 한 번만! **
        ** 절대 여러 턴을 한 번에 출력하지 마. 현재 선택에 대한 결과만 출력해. **
        ** 반드시 직전 선택에 대한 결과만 서술하라. 절대 새로운 이야기로 넘어가지 마라. **

        [NEVER DO THIS]
        - 선택지를 두 번 이상 출력하지 마
        - 한 턴 이상의 이야기를 이어서 전개하지 마
        - 직전 선택과 관련 없는 새 인카운터나 캐릭터, 장소, 설정을 소개하지 마
        - 이전 선택지와 무관한 갈등, 장소 전환, 무대 확장, 복선 암시 등을 하지 마
        - 감정적으로 몰아가는 흐름이나 시적 표현, 철학적 대사 등을 절대 쓰지 마
        - 대화형으로 인카운터를 종료하지 마
        - 플레이어에게 보상을 주지 마
        """
        
        history.append({"role": "user", "content": prompt})
        result = request_gemini(history)
        
        # 생성된 시나리오를 저장
        user_current_scenario[user_id] = result
        
        # AI 응답도 히스토리에 저장
        history.append({"role": "assistant", "content": result})
        
        extract_scores(user_id, result)

    elif message.strip().isdigit():
        if not user_story_active.get(user_id):
            return "⚠️ 선택지는 활성화된 인카운터가 없을 때 사용할 수 없습니다.", None, None, None

        selected_number = message.strip()
        last_user_choice[user_id] = selected_number
        
        # 현재 저장된 시나리오를 참조 (히스토리의 마지막 AI 응답)
        current_scenario = user_current_scenario.get(user_id, "")
        if not current_scenario and len(history) >= 2:
            # 히스토리에서 마지막 AI 응답 찾기
            for i in range(len(history) - 1, -1, -1):
                if history[i].get("role") == "assistant":
                    current_scenario = history[i]["content"]
                    break

        print(f"🎯 [DEBUG] 현재 시나리오: {current_scenario[:100]}...")
        print(f"🎯 [DEBUG] 선택된 번호: {selected_number}")

        followup_prompt = (
            f"현재 진행 중인 인카운터:\n{current_scenario}\n\n"
            f"사용자가 선택한 번호는 {selected_number}입니다.\n"
            f"위 인카운터에서 {selected_number}번 선택지에 대한 결과만을 작성하세요.\n"
            f"절대 새로운 이야기나 다른 설정으로 넘어가지 마세요.\n"
            f"선택 결과에 따라 인카운터가 종료된다면 '인카운터 종료', '성향값: +1', '골드: 15', '경험치: 3' 형식으로 출력하세요.\n"
            f"계속 이어진다면 반드시 '### 선택지:'라는 헤더 아래에 번호 매긴 3개의 선택지를 제공하세요.\n"
            f"직전 선택에 대한 이야기만 하고, 완전히 새로운 설정이나 인물을 도입하지 마세요."
        )

        history.append({"role": "user", "content": followup_prompt})
        result = request_gemini(history)
        
        # 새로운 결과를 현재 시나리오로 업데이트
        user_current_scenario[user_id] = result
        
        # AI 응답을 히스토리에 저장
        history.append({"role": "assistant", "content": result})
        
        extract_scores(user_id, result)

    else:
        history.append({"role": "user", "content": message})
        result = request_gemini(history)
        history.append({"role": "assistant", "content": result})
        extract_scores(user_id, result)

    if "인카운터 종료" in result:
        user_story_active[user_id] = False
        user_current_scenario[user_id] = ""  # 시나리오 초기화
        last_result_cache[user_id] = result

    trait, gold, exp = user_states.get(user_id, (None, None, None))
    return result, trait, gold, exp