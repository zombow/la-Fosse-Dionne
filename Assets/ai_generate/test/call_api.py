# call_api.py
import requests
import sys

url = "http://127.0.0.1:8000/chat"
user_id = "tester01"
message = sys.argv[1] if len(sys.argv) > 1 else "랜덤 인카운터"

payload = {"message": message, "user_id": user_id}
response = requests.post(url, json=payload)

print("📨 응답 결과:")
print(response.json())
