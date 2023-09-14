using IBM.WMQ;
using System.Collections;

namespace IBMMQClient
{
    internal class QueueWriter: IDisposable
    {
        private readonly MQQueueManager qManager;
        private readonly MQQueue queue;
        public QueueWriter(string userId, string password, string host, string port, string channel, 
            string queueManager, string queueName) 
        {
            Hashtable connectionProperties = new()
            {
                { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
                { MQC.HOST_NAME_PROPERTY, host },
                { MQC.PORT_PROPERTY, port },
                { MQC.CHANNEL_PROPERTY, channel },
                { MQC.USER_ID_PROPERTY, userId },
                { MQC.PASSWORD_PROPERTY, password }
            };
            this.qManager = new (queueManager, connectionProperties);
            int openOptions = MQC.MQOO_OUTPUT;
            this.queue = qManager.AccessQueue(queueName, openOptions);
        }
        public string Put(string content)
        {
            MQMessage message = new();
            message.WriteUTF(content);
            queue.Put(message);
            return Convert.ToBase64String(message.MessageId);
        }
        public void Dispose()
        {
            queue.Close();
            qManager.Disconnect();
        }
    }
}
