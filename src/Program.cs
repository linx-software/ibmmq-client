// See https://aka.ms/new-console-template for more information
using IBM.WMQ;
using IBMMQClient;
using System.Collections;
using System.Configuration;

if (args.Length == 0)
{
    throw new Exception(String.Join("\r\n", Args.GetHelp()));
}

var arguments = new Args(args);
if(arguments.IsNotComplete(out var errors))
{
    throw new Exception(String.Join("\r\n", errors));
}

var trace = ConfigurationManager.AppSettings.Get("MQDOTNET_TRACE_ON");
if (trace != null) System.Environment.SetEnvironmentVariable("MQDOTNET_TRACE_ON", trace);
var tracePath = ConfigurationManager.AppSettings.Get("MQTRACEPATH");
if (tracePath != null) System.Environment.SetEnvironmentVariable("MQTRACEPATH", tracePath);
var errorPath = ConfigurationManager.AppSettings.Get("MQERRORPATH");
if (errorPath != null) System.Environment.SetEnvironmentVariable("MQERRORPATH", errorPath);

string logFilepath = Path.Combine(arguments.LogDir!, DateTime.UtcNow.ToString("yyyyMMdd") + ".log");
void Log(string message)
{
    Console.WriteLine(message);
    var contents = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + " " + message + "\r\n";
    File.AppendAllText(logFilepath, contents);
}

try
{
    Log($"command={arguments.Command}");
    Log($"host={arguments.Host}");
    Log($"port={arguments.Port}");
    Log($"channel={arguments.Channel}");
    Log($"queueManager={arguments.QueueManager}");
    Log($"queueName={arguments.QueueName}");
    Log($"logDir={arguments.LogDir}");

   Hashtable connectionProperties = new()
    {
        { MQC.TRANSPORT_PROPERTY, MQC.TRANSPORT_MQSERIES_MANAGED },
        { MQC.HOST_NAME_PROPERTY, arguments.Host },
        { MQC.PORT_PROPERTY, arguments.Port },
        { MQC.CHANNEL_PROPERTY, arguments.Channel }
    };
    if (arguments.UserId != null) { connectionProperties.Add(MQC.USER_ID_PROPERTY, arguments.UserId); };
    if (arguments.Password != null) { connectionProperties.Add(MQC.PASSWORD_PROPERTY, arguments.Password); };
    if (arguments.CertStore != null) { connectionProperties.Add(MQC.SSL_CERT_STORE_PROPERTY, arguments.CertStore); };
    if (arguments.CipherSpec != null) { connectionProperties.Add(MQC.SSL_CIPHER_SPEC_PROPERTY, arguments.CipherSpec); };
    
    MQQueueManager queueManager = new(arguments.QueueManager, connectionProperties);

    if (arguments.Command == "upload")
    {
        Log($"inputDir={arguments.InputDir}");
        Log($"archiveDir={arguments.ArchiveDir}");
        using var q = new QueueWriter(queueManager, arguments.QueueName!);
        foreach (var filepath in Directory.GetFiles(arguments.InputDir!))
        {
            Log($"put {filepath} on queue");
            var messageId = q.Put(File.ReadAllText(filepath));
            var archiveFilepath = Path.Combine(arguments.ArchiveDir!, $"{messageId}_" + Path.GetFileName(filepath));
            Log($"move {filepath} to {archiveFilepath}");
            File.Move(filepath, archiveFilepath);
        }
    }
    else
    {
        Log($"outputDir={arguments.OutputDir!}");
        var browse = arguments.Command == "browse";
        QueueReader.Get(queueManager, arguments.QueueName!, browse, (messageId, contents) =>
        {
            var messagePath = Path.Combine(arguments.OutputDir!, $"{messageId}.txt");
            Log($"write messageId {messageId} to {messagePath}");
            File.WriteAllText(messagePath, contents);
        });
    }

    queueManager.Disconnect();
}
catch (MQException ex)
{
    Log($"An IBM MQ error occurred: {ex}");
}
catch (System.Exception ex)
{
    Log($"An error occurred: {ex}");
}