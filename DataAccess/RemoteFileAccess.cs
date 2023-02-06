using Renci.SshNet;
using Renci.SshNet.Common;

namespace DataAccess
{
    public class RemoteFileAccess
    {
        private SftpClient? _sftpClient;
        private readonly ILogger _logger;
        private string? _username;

        public RemoteFileAccess(ILogger logger)
        {
            _logger = logger;
        }

        public bool Authorize(string host, string username, string password)
        {

            Cleanup(_sftpClient);

            _username = username;

            var connectionInfo = new ConnectionInfo(
                host,
                username,
                new PasswordAuthenticationMethod(username, password)
            );

            _sftpClient = new SftpClient(connectionInfo);
            _sftpClient.Connect();

            _logger.Debug("New client constructed");
            _logger.Information("Authorized");

            return true;
        }

        public void Put(Stream dataStream, string filename)
        {
            if (_sftpClient is not SftpClient client)
            {
                _logger.Error("Client connection not initialized");
                return;
            }

            try
            {
                client.UploadFile(dataStream, filename);
            }
            catch (SftpPermissionDeniedException e)
            {
                _logger.Error(e, "Upload permission denied");
            }
            catch (SshConnectionException e)
            {
                _logger.Error(e, "Connection terminated");
            }
            catch (Exception e)
            {
                _logger.Error(e, "General error");
            }

        }

        public async Task PutAsync(Stream dataStream, string filename)
        {

            if (_sftpClient is not SftpClient client)
            {
                _logger.Error("Client connection not initialized");
                return;
            }

            try
            {
                await Task.Run(() =>
                    client.BeginUploadFile(dataStream, filename)
                    .AsyncWaitHandle
                    .WaitOne()
                );
            }
            catch (SftpPermissionDeniedException e)
            {
                _logger.Error(e, "Upload permission denied");
            }
            catch (SshConnectionException e)
            {
                _logger.Error(e, "Connection terminated");
            }
            catch (Exception e)
            {
                _logger.Error(e, "General error");
            }
        }

        public void SyncDir(string path, string targetDir)
        {
            if (_sftpClient is not SftpClient client)
            {
                _logger.Error("Client connection not initialized");
                return;
            }

            if (_username is not string username)
            {
                _logger.Error("Username invalid");
                return;
            }

            var targetPath = Path.Combine(username, targetDir);

            try
            {
                client.SynchronizeDirectories(path, targetPath, "*");
            }
            catch (SftpPermissionDeniedException e)
            {
                _logger.Error(e, "Upload permission denied");
            }
            catch (SshConnectionException e)
            {
                _logger.Error(e, "Connection terminated");
            }
            catch (Exception e)
            {
                _logger.Error(e, "General error");
            }
        }

        private void Cleanup(SftpClient? client)
        {
            if (client is SftpClient realClient)
            {
                realClient.Disconnect();
                realClient.Dispose();
                _logger.Debug("Finished client cleanup");
            }
        }
    }
}
