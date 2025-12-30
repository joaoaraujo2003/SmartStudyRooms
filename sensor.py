import requests
import time
import random

API_URL = "https://localhost:44331/api/sensores/movimento"
API_KEY = "ISI-2025-SECRET"   # 🔑 a tua chave
SALA_ID = 1

headers = {
    "X-API-KEY": API_KEY,
    "Content-Type": "application/json"
}

while True:
    movimento = random.choice([True, True, True, False])

    payload = {
        "salaId": SALA_ID,
        "movimento": movimento
    }

    try:
        r = requests.post(
            API_URL,
            json=payload,
            headers=headers,
            verify=False
        )

        print(
            f"[SALA {SALA_ID}] Movimento: {movimento} | "
            f"Status: {r.status_code}"
        )

        if r.status_code != 200:
            print("Resposta da API:", r.text)

    except Exception as e:
        print("Erro:", e)

    time.sleep(60)
