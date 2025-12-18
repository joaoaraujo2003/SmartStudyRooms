import time
import requests
import random
import json

API_URL = "https://localhost:5001/api/sensores/estado"   # ALTERA SE FOR NECESSÁRIO
SALA_ID = 1   # Sala a simular

def enviar_estado(ocupada):
    payload = {
        "salaId": SALA_ID,
        "ocupada": ocupada
    }

    try:
        response = requests.post(API_URL, json=payload, verify=False)
        print(f"🔵 Enviado -> {payload} | Código: {response.status_code}")
    except Exception as e:
        print(f"❌ Erro a enviar: {e}")


def sensor_loop():
    print("=== Sensor IoT Python Ativo ===")
    print("A enviar dados a cada 10 segundos...")

    while True:
        # Simula ocupação aleatória (podes mudar para True/False fixo)
        ocupacao = random.choice([True, False])
        
        enviar_estado(ocupacao)

        time.sleep(10)  # intervalo de envio em segundos


if __name__ == "__main__":
    sensor_loop()
