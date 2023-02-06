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
            var command = CommandBuilder.BuildCompile("pdflatex  -output-directory {0} {0}/{1}", dirPath, rootFile);
            _remoteFileAccess.SyncDir(dirPath, dirPath);
            var result = _remoteCommand.ExecuteCommand(command);

            if (!result.IsSuccess)
                return;

            var possiblePdfName = Path.ChangeExtension(rootFile, "pdf");

            using var data = _remoteFileAccess.GetFile(dirPath, possiblePdfName);

            if (data is not MemoryStream safeData)
                return;

            using var file = new FileStream(outFile, FileMode.Create);
            safeData.Seek(0, SeekOrigin.Begin);
            data.CopyTo(file);
        }
    }
}