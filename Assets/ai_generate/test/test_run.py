# run_story_test.py
from logic_select import init_rag_context, process_user_message
from utils import save_story, save_story_as_json
import os
import sys

# 기본 메시지: '랜덤 인카운터'
user_input = sys.argv[1] if len(sys.argv) > 1 else "랜덤 인카운터"

if __name__ == "__main__":
    print("📂 참고 PDF 로딩 중...")
    init_rag_context()

    print("🧠 인카운터 생성 요청 중... (입력: '랜덤 인카운터')")
    response, trait, gold, exp = process_user_message("랜덤 인카운터")

    print("\n✅ 생성된 인카운터:\n")
    print(response)

    print("\n📊 성향값:", trait, "💰 골드:", gold, "📚 경험치:", exp)

    os.makedirs("output", exist_ok=True)
    save_story(response, "output/test_output.txt")
    save_story_as_json(response, "output/story_result.json")
    print("\n💾 저장 완료: test_output.txt, story_result.json")
