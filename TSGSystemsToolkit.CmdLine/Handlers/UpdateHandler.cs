﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using TSGSystemsToolkit.CmdLine.Options;

namespace TSGSystemsToolkit.CmdLine.Handlers
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IConfiguration _config;
        private readonly ILogger<UpdateHandler> _logger;

        public UpdateHandler(IConfiguration config, ILogger<UpdateHandler> logger)
        {
            _config = config;
            _logger = logger;
        }

        public int RunHandlerAndReturnExitCode(UpdateOptions options)
        {
            var fileVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version.Split('.');
            Version currentVersion = new(fileVersion[0], fileVersion[1], fileVersion[2]);
            _logger.LogInformation("Current version: {Current}", currentVersion);

            Version available = Extensions.GetAvailableVersion(_config.GetValue<string>("MasterLocation"));
            _logger.LogInformation("Available version: {Available}", available);

            if (available > currentVersion)
            {
                _logger.LogInformation("Update available! Updating...");

                Process.Start(available.InstallerPath, "/SILENT");
            }

            return 1;
        }
    }
}
