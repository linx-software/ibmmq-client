using IBM.WMQ;

namespace IBMMQClient
{
    internal class QueueWriter: IDisposable
    {
        private readonly MQQueue queue;
        public QueueWriter(MQQueueManager queueManager, string queueName) 
        {
            int openOptions = MQC.MQOO_OUTPUT;
            this.queue = queueManager.AccessQueue(queueName, openOptions);
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
        }
    }
}
