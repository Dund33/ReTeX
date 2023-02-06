// See https://aka.ms/new-console-template for more information
using ReTeX;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();


var host = args[0];
var username = args[1];
var password = args[2];
var dirname = args[3];
var filename = args[4];

logger.Information($"Running for user: {username} and directory {dirname} on {host}");
var processor = new ReTeXProcessor(host, logger);
processor.Init(username, password);
processor.Process(dirname, filename);