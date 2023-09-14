using IBM.WMQ;
using System.Collections;

namespace IBMMQClient
{
    internal class QueueReader: IDisposable
    {
        private readonly MQQueueManager qManager;
        private readonly MQQueue queue;
        public QueueReader(string userId, string password, string host, string port, string channel,
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
            this.qManager = new(queueManager, connectionProperties);
            int openOptions = MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_BROWSE;
            this.queue = qManager.AccessQueue(queueName, openOptions);
        }
        public void Get(Action<string, string> contentHandler)
        {
            MQGetMessageOptions browseOptions = new()
            {
                Options = MQC.MQGMO_BROWSE_NEXT | MQC.MQGMO_NO_WAIT | MQC.MQGMO_FAIL_IF_QUIESCING
            };
            MQGetMessageOptions removeOptions = new()
            {
                Options = MQC.MQGMO_MSG_UNDER_CURSOR | MQC.MQGMO_NO_WAIT | MQC.MQGMO_FAIL_IF_QUIESCING
            };
            while (true)
            {
                MQMessage message = new();
                try
                {
                    queue.Get(message, browseOptions);
                    contentHandler(Convert.ToBase64String(message.MessageId), message.ReadUTF());
                    queue.Get(message, removeOptions);
                }
                catch (MQException ex)
                {
                    if (ex.Message != "MQRC_NO_MSG_AVAILABLE")
                        throw;
                    else
                        break;
                }
            }
        }
        public void Dispose()
        {
            queue.Close();
            qManager.Disconnect();
        }
    }
}
