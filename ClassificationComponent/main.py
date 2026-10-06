from confluent_kafka import Consumer, KafkaException
from classification import get_region_with_geopandas
import json
from publisher import publish, connection

filepath = r"C:\Users\EHRE14\source\repos\KolAman\alert-simulator\alert-simulator\regions.geojson"
topic_name = "alerts"
conf = {'bootstrap.servers': 'localhost:9092',
        'group.id': 'classification-consumer',
        'auto.offset.reset': 'earliest',
        'enable.auto.commit': 'false'}


consumer = Consumer(conf)


try:
    consumer.subscribe([topic_name])

    while True:
        msg = consumer.poll(timeout=1.0)
        if msg is None: continue

        if msg.error():
            raise KafkaException(msg.error())
        else:
            consumer.commit(asynchronous=False)

            # Convert from JSON to dict
            alert = json.loads(msg.value())

            # Classify by longitude and latitude
            region = get_region_with_geopandas(filepath, alert["lon"], alert["lat"])

            # Send to Rabbit by 'region'
            publish(region, alert)


finally:
    consumer.close()
    connection.close()






