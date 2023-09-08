
using MqttLibrary;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System.Net;

public class MqttClientManager
{
    private MqttClient mqttClient;
    private string MQTT_BROKER_ADDRESS = "mqtt-dashboard.com";
    private int MQTT_BROKER_PORT = 8884;

    public event EventHandler<MqttMessageEventArgs> MessageReceived;

    public MqttClientManager()
    {
        mqttClient = new MqttClient(IPAddress.Parse(MQTT_BROKER_ADDRESS));
        mqttClient.MqttMsgPublished += mqttClient_MqttMsgPublished;
        mqttClient.MqttMsgPublishReceived += mqttClient_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        mqttClient.Connect(clientId);
    }

    public void Publish(MqttMessage mqttMessage)
    {
        if (mqttClient.IsConnected)
        {
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(mqttMessage.Message);
            mqttClient.Publish(mqttMessage.Topic, messageBytes, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        }
        else
        {
            Console.WriteLine("Błąd: Nie można połączyć się z brokerem MQTT.");
        }
    }

    public void Subscribe(string topic)
    {
        if (mqttClient.IsConnected)
        {
            mqttClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }
        else
        {
            Console.WriteLine("Błąd: Nie można połączyć się z brokerem MQTT.");
        }
    }

    private void mqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
    {
        Console.WriteLine($"Wiadomość opublikowana na temat: {e.MessageId}");
    }

    private void mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string message = System.Text.Encoding.UTF8.GetString(e.Message);
        OnMessageReceived(e.Topic, message);
    }

    protected virtual void OnMessageReceived(string topic, string message)
    {
        MessageReceived?.Invoke(this, new MqttMessageEventArgs(topic, message));
    }
}

public class MqttMessageEventArgs : EventArgs
{
    public string Topic { get; }
    public string Message { get; }

    public MqttMessageEventArgs(string topic, string message)
    {
        Topic = topic;
        Message = message;
    }
}
