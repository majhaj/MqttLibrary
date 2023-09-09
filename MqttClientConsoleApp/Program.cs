using MqttLibrary;

class Program
{
    static void Main(string[] args)
    {
        string brokerAddress = "mqtt-dashboard.com";
        int brokerPort = 8884;
        string clientId = "clientId-Xz6pIZBn5F";

        Console.WriteLine("Inicjalizacja klienta MQTT...");
        MqttClientManager mqttClient = new MqttClientManager(brokerAddress, brokerPort, clientId);

        string topicToSubscribe = "testtopic";
        mqttClient.Subscribe(topicToSubscribe);
        Console.WriteLine($"Zasubskrybowano temat: {topicToSubscribe}");

        string topicToPublish = "testtopic";
        string messageToPublish = "Hello world!";
        MqttMessage mqttMessage = new MqttMessage
        {
            Topic = topicToPublish,
            Message = messageToPublish
        };

        Console.WriteLine($"Publikowanie wiadomości na temat: {topicToPublish}");
        mqttClient.Publish(mqttMessage);

        string topicToUnsubscribe = "testtopic";
        mqttClient.Unsubscribe(topicToUnsubscribe);

        mqttClient.MessageReceived += (sender, e) =>
        {
            Console.WriteLine($"Odebrano wiadomość na temat: {e.Topic}");
            Console.WriteLine($"Treść wiadomości: {e.Message}");
        };

    }
}
