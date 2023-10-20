namespace IBMMQClient
{
    internal class Args
    {
        private readonly string[] _args;
        public Args(string[] args) 
        {
            _args = args;
        }
        private string? GetArg(string name)
        {
            int index = Array.FindIndex(_args, x => x.ToLower() == name.ToLower());
            if (index >= 0)
            {
                return _args[index + 1];
            }
            else
            {
                return null;
            }
        }
        public string? Command { get { return _args.Length == 0 ? null : _args[0]; } }
        public string? UserId { get { return GetArg("-u"); } }
        public string? Password { get { return GetArg("-w"); } }
        public string Host { get { return GetArg("-h")??"localhost"; } }
        public string Port { get { return GetArg("-p")??"1414"; } }
        public string? Channel { get { return GetArg("-c"); } }
        public string? QueueManager { get { return GetArg("-m"); } }
        public string? QueueName { get { return GetArg("-n"); } }
        public string? InputDir { get { return GetArg("-i"); } }
        public string? OutputDir { get { return GetArg("-o"); } }
        public string? ArchiveDir { get { return GetArg("-a"); } }
        public string? LogDir { get { return GetArg("-l"); } }
        public string? CipherSpec { get { return GetArg("-r"); } }
        public string? CertStore { get { return GetArg("-c"); } }
        public bool IsNotComplete(out string[] errors)
        {
            var e = new List<string>();
            if (Command == null) { e.Add("Command upload|download|browse is required"); };
            if (Channel == null) { e.Add("-c channel is required"); };
            if (QueueManager == null) { e.Add("- queueManager is required"); };
            if (QueueName == null) { e.Add("-n queueName is required"); };
            if (LogDir == null) { e.Add("-l logDir is required"); };
            if(Command == "upload")
            {
                if (InputDir == null) { e.Add("-i inputDir is required"); };
                if (ArchiveDir == null) { e.Add("-i archiveDir is required"); };
            }
            if (Command == "download" || Command == "browse")
            {
                if (OutputDir == null) { e.Add("-o outputDir is required"); };
            }
            errors = e.ToArray();
            return e.Any();
        }
        public static string[] GetHelp()
        {
            return new string[]
            {
                @"Usage: IBMMQClient upload|download|browse -h host -p port -c channel -m queueManager -n queueName 
-i inputDir -o outputDir -a archiveDir -l logDir 
-u userId -w password -r cipherSpec -s certStore",
                "upload|download|browse: Required first argument. Any one of the three.",
                "-h host: Like -h 127.0.0.1. Default is localhost.",
                "-p port: Like -p 1234. Default is 1414.",
                "-c channel: Like -c DEV.APP.SVRCONN",
                "-m queueManager: Like -m QM1",
                "-n queueName: Like -n DEV.QUEUE.1",
                "-i inputDir: Like -i c:\\temp\\mq\\input. Directory where messages are read from.",
                "-o outputDir: Like -o c:\\temp\\mq\\output. Directory where messages are written to.",
                "-a archiveDir: Like -a c:\\temp\\mq\\archive. Directory where messages are archived.",
                "-l logDir: Like -l c:\\temp\\mq\\log. Directory where log files are written to.",
                "-u userID: Like -u appuser",
                "-w password: Like -w verylongpassword",
                "-r cipherSpec: Like -r TLS_RSA_WITH_AES_256_CBC_SHA256",
                "-s certStore: Like -s *USER. Where to find the cert in the Windows certificate store. Can be *SYSTEM or *USER"
            };
        }
    }
}
