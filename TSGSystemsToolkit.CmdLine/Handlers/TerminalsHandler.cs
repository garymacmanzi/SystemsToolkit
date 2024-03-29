﻿using Pse.TerminalsToEmis;
using System.IO;
using TSGSystemsToolkit.CmdLine.Options;

namespace TSGSystemsToolkit.CmdLine.Handlers;

public class TerminalsHandler : ICommandHandler
{
    private ILogger<TerminalsHandler> _logger;
    private readonly TerminalsOptions _options;
    private readonly CancellationToken _ct;

    public TerminalsHandler(TerminalsOptions options, CancellationToken ct = default)
    {
        _options = options;
        _ct = ct;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        GetDependencies(context);

        if (_options.CreateEmisFile)
        {
            string outputPath;
            string terminalsPath;

            if (File.Exists(_options.FilePath))
                terminalsPath = _options.FilePath;
            else if (Directory.Exists(_options.FilePath))
                terminalsPath = $"{_options.FilePath}\\Terminals_044.csv";
            else
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] File or directory {_options.FilePath} does not exist.");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(_options.OutputPath))
            {
                outputPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                outputPath = $"{outputPath}\\FuelPos";
                if (!Directory.Exists(outputPath))
                {
                    throw new DirectoryNotFoundException($"Directory {outputPath} could not be found. Is Remote eMIS installed?");
                }

                _logger.LogDebug("Output path automatically set to: {Output}", outputPath);
            }
            else
            {
                outputPath = _options.OutputPath;
                _logger.LogDebug("Output path manually defined at: {Output}", outputPath);
            }

            TerminalsToEmis.Run(terminalsPath, outputPath);
        }

        return 0;
    }

    private void GetDependencies(InvocationContext context)
    {
        _logger = context.BindingContext.GetService(typeof(ILogger<TerminalsHandler>)) as ILogger<TerminalsHandler>;
    }
}
