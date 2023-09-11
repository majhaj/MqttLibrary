using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MqttLibrary
{
    public class MqttClientManager
    {
        private readonly MqttClient mqttClient;

        public event EventHandler<MqttMessageEventArgs> MessageReceived;

        public MqttClientManager(string brokerHostName,
                                 int brokerPort, 
                                 string clientId, 
                                 string username, 
                                 string password)
        {
            mqttClient = new MqttClient(brokerHostName, brokerPort, false, null, null, MqttSslProtocols.TLSv1_2);
            mqttClient.MqttMsgPublished += mqttClient_MqttMsgPublished;
            mqttClient.MqttMsgPublishReceived += mqttClient_MqttMsgPublishReceived;

           mqttClient.Connect(clientId, username, password);

        }

        public void Publish(MqttMessage mqttMessage)
        {
            if (mqttClient.IsConnected)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(mqttMessage.Message);
                mqttClient.Publish(mqttMessage.Topic, messageBytes, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            }
            else
            {
                Console.WriteLine("Brak połączenia z brokerem MQTT.");
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
                Console.WriteLine("Brak połączenia z brokerem MQTT.");
            }
        }

        public void Unsubscribe(string topic)
        {
            if (mqttClient.IsConnected)
            {
                mqttClient.Unsubscribe(new string[] { topic });
                Console.WriteLine($"Zakończono subskrypcję tematu: {topic}");
            }
            else
            {
                Console.WriteLine("Brak połączenia z brokerem MQTT.");
            }
        }

        private void mqttClient_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine($"Wiadomość opublikowana na temat: {e.MessageId}");
        }

        private void mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
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
}
