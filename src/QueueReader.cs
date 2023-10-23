using IBM.WMQ;

namespace IBMMQClient
{
    internal class QueueReader
    {
        static QueueReader()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        public static void Get(MQQueueManager queueManager, string queueName, bool browse, Action<string, string> contentHandler)
        {
            int openOptions = MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_BROWSE;
            var queue = queueManager.AccessQueue(queueName, openOptions);

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
                    contentHandler(Convert.ToBase64String(message.MessageId), message.ReadString(message.DataLength));
                    if (!browse)
                    {
                        queue.Get(message, removeOptions);
                    }
                }
                catch (MQException ex)
                {
                    if (ex.Message != "MQRC_NO_MSG_AVAILABLE")
                        throw;
                    else
                        break;
                }
            }
            queue.Close();
        }
    }
}
