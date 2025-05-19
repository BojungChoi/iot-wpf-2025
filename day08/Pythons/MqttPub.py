## 파이썬 MQTT Publish
# paho-mqtt 라이브러리 설치
# pip install paho-mqtt
import paho.mqtt.client as mqtt
import json
import datetime as dt
import uuid
from collections import OrderedDict
import random
import time

PUB_ID = 'IOT57' # 본인 아이피 마지막 주소
# BROKER  = '210.119.12.57' # 내 IP
BROKER  = '210.119.12.58' # 내 IP

PORT = 1883
TOPIC = 'smarthome/57/topic' # publish/subscribe 에서 사용할 토픽
COLORS = ['RED', 'ORANGE', 'YELLOW', 'GREEN', 'BLUE', 'NAVY', 'PURPLE']
COUNT = 0


# 연결 콜백
def on_connect(client, userdata, flags, reason_code, properties=None):
    print(f'Connected with reason code : {reason_code}')
    

# 퍼블리시 완료후 발생 콜백
def on_publish(client, useradata, mid):
    print(f'Message published mid : {mid}')


try:
    client = mqtt.Client(client_id=PUB_ID, protocol=mqtt.MQTTv5)
    client.on_connect = on_connect
    client.on_publish = on_publish

    client.connect(BROKER, PORT)
    client.loop_start()

    while True:
        # 퍼블리시
        currtime = dt.datetime.now()
        selected = random.choice(COLORS)
        COUNT += 1
        client.publish(TOPIC, payload = f' Emulate from{PUB_ID}[{COUNT}] : {selected} / {currtime}', qos = 1)
        time.sleep(2)


except Exception as ex:
    print(f'Error raised : {ex}')
except KeyboardInterrupt:
    print('MQTT 전송중단')
    client.loop_stop()
    client.disconnect()