import requests
import time
import random

API_URL = "https://localhost:44331/api/sensores/movimento"
SALA_ID = 1

while True:
    movimento = random.choice([True, True, True, False])

    payload = {
        "salaId": SALA_ID,
        "movimento": movimento
    }

    try:
        r = requests.post(API_URL, json=payload, verify=False)
        print(f"[SALA {SALA_ID}] Movimento:", movimento)
    except Exception as e:
        print("Erro:", e)

    time.sleep(60)
