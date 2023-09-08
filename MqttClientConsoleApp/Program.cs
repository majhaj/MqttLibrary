using MqttLibrary;

MqttMessage message = new MqttMessage
{
    Topic = "example/topic",
    Message = "Hello, MQTT!"
};


MqttClientManager mqttClientManager = new MqttClientManager();
mqttClientManager.Publish(message);

mqttClientManager.Subscribe("example/topic");

mqttClientManager.MessageReceived += (sender, e) =>
{
    Console.WriteLine($"Odebrano wiadomość na temat {e.Topic}: {e.Message}");
};


Console.WriteLine("Wciśnij Enter, aby zakończyć.");
