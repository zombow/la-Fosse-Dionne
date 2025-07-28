# test_client.py - 간단한 테스트용
import requests
import json

API_URL = "http://127.0.0.1:8000/chat"
USER_ID = "test_player"

def send_message(message):
    try:
        payload = {"message": message, "user_id": USER_ID}
        response = requests.post(API_URL, json=payload, timeout=15)
        response.raise_for_status()
        
        data = response.json()
        return data
        
    except requests.exceptions.Timeout:
        return {"response": "⏰ 응답 시간 초과", "error": True}
    except requests.exceptions.ConnectionError:
        return {"response": "❌ 서버 연결 실패", "error": True}
    except Exception as e:
        return {"response": f"❌ 오류: {str(e)}", "error": True}

def clean_response(text):
    """불필요한 형식 제거"""
    lines = []
    for line in text.split('\n'):
        line = line.strip()
        if line and not any(skip in line.lower() for skip in [
            "### 결과:", "### 선택지:", "결과:",
            "인카운터가", "모험이 완료", "새로운 모험", "아무 메시지"
        ]):
            lines.append(line)
    return '\n'.join(lines)

def show_status(data):
    """상태 변화 표시"""
    if data.get("error"):
        return
        
    status_parts = []
    if data.get("trait_score") is not None:
        status_parts.append(f"성향값: {data['trait_score']:+.1f}")
    if data.get("gold_amount") is not None:
        status_parts.append(f"골드: {data['gold_amount']:+d}")
    if data.get("exp_amount") is not None:
        status_parts.append(f"경험치: {data['exp_amount']:+d}")
    if data.get("mental_amount") is not None:
        status_parts.append(f"정신력: {data['mental_amount']:+d}")
    if data.get("health_amount") is not None:
        status_parts.append(f"생명력: {data['health_amount']:+d}")
    
    if status_parts:
        print(f"\n📊 변화: {' | '.join(status_parts)}")

def main():
    # 자동 시작
    data = send_message("랜덤 인카운터")
    
    if data.get("error"):
        print(data["response"])
        return
    
    print("\n" + clean_response(data["response"]))
    show_status(data)
    
    # 대화 루프
    while True:
        try:
            user_input = input("\n> ").strip()
            
            if not user_input or user_input.lower() in ['quit', 'exit', '종료']:
                print("게임을 종료합니다.")
                break
            
            data = send_message(user_input)
            
            if data.get("error"):
                print(data["response"])
                continue
            
            response = clean_response(data["response"])
            if response:
                print(f"\n{response}")
            
            show_status(data)
            
            # 인카운터 종료 감지
            if "인카운터 종료" in data.get("response", ""):
                print(f"\n{'-'*30}")
                print("인카운터가 완료되었습니다!")
                break
                
        except KeyboardInterrupt:
            print("\n게임을 종료합니다.")
            break

if __name__ == "__main__":
    main()