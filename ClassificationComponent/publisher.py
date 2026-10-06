import pika




# Create the Rabbit connection
credentials = pika.PlainCredentials('guest', 'guest')
connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost', credentials=credentials))

# Create the Rabbit channel
channel = connection.channel()

# Create queue each one for each control
channel.queue_declare(queue='north', durable=True, arguments={'x-queue-type': 'quorum'})
channel.queue_declare(queue='center', durable=True, arguments={'x-queue-type': 'quorum'})
channel.queue_declare(queue='south', durable=True, arguments={'x-queue-type': 'quorum'})
channel.queue_declare(queue='overseas', durable=True, arguments={'x-queue-type': 'quorum'})


def publish(region, alert):
    try:

        target = str.lower(region)
        channel.basic_publish(exchange='',
                              routing_key=target,
                              body=alert)
        print(f" [x] Sent from '{alert["source"]} to '{target}' queue")

    finally:
        connection.close()