// See https://aka.ms/new-console-template for more information
using IBM.WMQ;
using IBMMQClient;
using System.Diagnostics;

string? GetArg(string name)
{
    int index = Array.FindIndex(args, x => x.ToLower() == name.ToLower()) ;
    if (index >= 0) 
    {
        return args[index + 1];
    } 
    else
    {
        return null;
    }
}
string GetRequiredArg(string name)
{
    var value = GetArg(name);
    return value ?? throw new Exception($"Argument {name} is required.");
}
string logFilepath;
void Log(string message)
{
    Console.WriteLine(message);
    var contents = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + " " + message + "\r\n";
    File.AppendAllText(logFilepath, contents);
}

Trace.Assert(args.Length > 0, "No arguments found");
string command = args[0];
if (command != "upload" && command != "download")
    throw new Exception("Expected 'upload' or 'download' as the first arg but found " + command);
var userId = GetRequiredArg("-userId");
var password = GetRequiredArg("-password");
var host = GetRequiredArg("-host");
var port = GetRequiredArg("-port");
var channel = GetRequiredArg("-channel");
var queueManager = GetRequiredArg("-queueManager");
var queueName = GetRequiredArg("-queueName");
var logDir = GetRequiredArg("-logdir");
logFilepath = Path.Combine(logDir, DateTime.UtcNow.ToString("yyyyMMdd") + ".log");

try
{
    Log($"command={command}");
    Log($"host={host}");
    Log($"port={port}");
    Log($"channel={channel}");
    Log($"queueManager={queueManager}");
    Log($"queueName={queueName}");
    Log($"logDir={logDir}");

    if (command == "upload")
    {
        var inputDir = GetRequiredArg("-inputdir");
        var archiveDir = GetRequiredArg("-archivedir");
        Log($"inputDir={inputDir}");
        Log($"archiveDir={archiveDir}");
        using var q = new QueueWriter(userId, password, host, port, channel, queueManager, queueName);
        foreach (var filepath in Directory.GetFiles(inputDir))
        {
            Log($"put {filepath} on queue");
            var messageId = q.Put(File.ReadAllText(filepath));
            var archiveFilepath = Path.Combine(archiveDir, $"{messageId}_" + Path.GetFileName(filepath));
            Log($"move {filepath} to {archiveFilepath}");
            File.Move(filepath, archiveFilepath);
        }
    }
    else
    {
        var outputDir = GetRequiredArg("-outputdir");
        Log($"outputDir={outputDir}");
        using var q = new QueueReader(userId, password, host, port, channel, queueManager, queueName);
        q.Get((messageId, contents) =>
        {
            var messagePath = Path.Combine(outputDir, $"{messageId}.txt");
            Log($"write messageId {messageId} to {messagePath}");
            File.WriteAllText(messagePath, contents);
        });
    }
}
catch (MQException ex)
{
    Log($"An IBM MQ error occurred: {ex}");
}
catch (System.Exception ex)
{
    Log($"An error occurred: {ex}");
}