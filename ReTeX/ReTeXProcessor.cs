using Serilog;

namespace ReTeX
{
    public class ReTeXProcessor
    {
        private readonly RemoteCommand _remoteCommand;
        private readonly RemoteFileAccess _remoteFileAccess;
        private readonly string _host;

        public ReTeXProcessor(string host, ILogger logger)
        {
            _remoteCommand = new(logger);
            _remoteFileAccess = new(logger);
            _host = host;
        }

        public void Init(string username, string password)
        {
            _remoteFileAccess.Authorize(_host, username, password);
            _remoteCommand.Authorize(_host, username, password);
        }

        public void Process(string dirPath, string rootFile = "main.tex", string outFile = "main.pdf")
        {
            var command = CommandBuilder.BuildCompile("latexpdf {}/{}", dirPath, rootFile);
            _remoteFileAccess.SyncDir(_host, dirPath);
            var result = _remoteCommand.ExecuteCommand(command);

            if (result.IsSuccess && result.Result is not null)
                File.WriteAllBytes(outFile, result.Result);
        }
    }
}