# utils.py
import os
import fitz  # PyMuPDF
import requests
import json
from dotenv import load_dotenv

load_dotenv()

# ✅ Gemini API 정보 (v1 엔드포인트 및 실제 모델명 사용)
GEMINI_API_KEY = os.getenv("GEMINI_API_KEY") or "YOUR_API_KEY_HERE"
GEMINI_ENDPOINT = f"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro:generateContent?key={GEMINI_API_KEY}"

def load_pdfs_from_local(pdf_dir: str) -> str:
    all_text = ""
    for filename in os.listdir(pdf_dir):
        if filename.endswith(".pdf"):
            with fitz.open(os.path.join(pdf_dir, filename)) as doc:
                for page in doc:
                    all_text += page.get_text()
    return all_text or "참고 문서 없음"

def save_story(text: str, path: str):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w", encoding="utf-8") as f:
        f.write(text)

def save_story_as_json(text: str, path: str):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    data = {"story": text.strip()}
    with open(path, "w", encoding="utf-8") as f:
        json.dump(data, f, ensure_ascii=False, indent=2)

def request_gemini(messages_history):
    headers = {"Content-Type": "application/json"}
    contents = [
        {
            "role": msg["role"],
            "parts": [{"text": msg["content"]}]
        } for msg in messages_history
    ]
    body = {
        "contents": contents,
        "generationConfig": {
            "temperature": 0.7,
            "maxOutputTokens": 1500
        }
    }
    try:
        response = requests.post(GEMINI_ENDPOINT, headers=headers, json=body)
        response.raise_for_status()
        return response.json()['candidates'][0]['content']['parts'][0]['text']
    except requests.exceptions.HTTPError as e:
        print(f"❌ Gemini 호출 오류: {e}")
        print(f"🔍 응답 본문: {response.text}")
        if response.status_code == 429:
            print("🔁 429 오류 - 잠시 대기 후 재시도합니다...")
            import time
            import json
            try:
                error_body = response.json()
                delay = 5  # 기본값
                for detail in error_body.get("error", {}).get("details", []):
                    if detail.get("@type") == "type.googleapis.com/google.rpc.RetryInfo":
                        retry_delay = detail.get("retryDelay", "5s")
                        delay = int(retry_delay.replace("s", ""))
                print(f"⏳ {delay}초 후 재시도...")
                time.sleep(delay)
            except Exception as parse_error:
                print(f"⚠️ 재시도 대기 시간 파싱 실패: {parse_error}, 기본 5초 대기")
                time.sleep(5)
            return request_gemini(messages_history)
        return "API 오류 발생"
    except Exception as e:
        print(f"❌ 기타 예외 발생: {e}")
        return "API 오류 발생"
