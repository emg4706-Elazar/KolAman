import pika
import json



# Create the Rabbit connection
credentials = pika.PlainCredentials(username="app", password="secret")
parameters = pika.ConnectionParameters(host='localhost', credentials=credentials)
connection = pika.BlockingConnection(parameters=parameters)

# Create the Rabbit channel
channel = connection.channel()

# Create queue each one for each control
channel.queue_declare(queue='north', durable=True, arguments={'x-queue-type': 'quorum'})
channel.queue_declare(queue='center', durable=True, arguments={'x-queue-type': 'quorum'})
channel.queue_declare(queue='south', durable=True, arguments={'x-queue-type': 'quorum'})
channel.queue_declare(queue='overseas', durable=True, arguments={'x-queue-type': 'quorum'})


def publish(region, alert):
    try:
        json_body = json.dumps(alert)
        target = str.lower(region)
        channel.basic_publish(exchange='',
                              routing_key=target,
                              body=json_body)
        print(f" [x] Sent from '{alert["source"]} to '{target}' queue")
    except Exception as e:
        print(e)