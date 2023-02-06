using DataAccess;
using Renci.SshNet;
using Serilog;
using System.Text;

namespace ReTeX
{
    public class RemoteCommand
    {
        private SshClient? _sshClient;
        private readonly ILogger _logger;
        private string? _username;
        public RemoteCommand(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<CommandResult> ExecuteCommand(CommandType command)
        {
            if (_sshClient is not SshClient client)
            {
                _logger.Error("SSH Client not initialized");
                return await Task.FromResult(
                    new CommandResult(false, null)
                );
            }

            var commandResult = await Task.Run(() =>
                client.RunCommand(((int)command).ToString())
            );

            if (commandResult.ExitStatus != 0)
            {
                _logger.Error("Command execution error");
                _logger.Error(commandResult.Error);

                var byteResult = Encoding.UTF8.GetBytes(commandResult.Error);

                return await Task.FromResult(
                    new CommandResult(false, byteResult)
                );
            }

            var resultBytes = Convert.FromBase64String(commandResult.Result);

            return new CommandResult(true, resultBytes);
        }

        public bool Authorize(string host, string username, string password)
        {
            _username = username;

            var connectionInfo = new ConnectionInfo(
                host,
                username,
                new PasswordAuthenticationMethod(username, password)
            );

            _sshClient = new SshClient(connectionInfo);
            _sshClient.Connect();

            _logger.Debug("New client constructed");
            _logger.Information("Authorized");

            return true;
        }
    }
}
