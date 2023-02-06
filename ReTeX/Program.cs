// See https://aka.ms/new-console-template for more information
using ReTeX;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

var processor = new ReTeXProcessor(args[1], logger);
processor.Init(args[2], args[3]);
processor.Process(args[4]);