﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSGSystemsToolkit.CmdLine.Handlers;
using TSGSystemsToolkit.CmdLine.Options;

namespace TSGSystemsToolkit.CmdLine.Commands
{
    internal class RootCommands : IRootCommands
    {
        private readonly ILogger<RootCommands> _logger;
        private readonly IVeederRootHandler _veederRootHandler;
        private readonly IProgaugeHandler _progaugeHandler;
        private readonly ITerminalsHandler _terminalsHandler;
        private readonly IMutationHandler _mutationHandler;
        private readonly ISurveyHandler _surveyHandler;

        public RootCommands(ILogger<RootCommands> logger,
                            IVeederRootHandler veederRootHandler,
                            IProgaugeHandler progaugeHandler,
                            ITerminalsHandler terminalsHandler,
                            IMutationHandler mutationHandler,
                            ISurveyHandler surveyHandler)
        {
            _logger = logger;
            _veederRootHandler = veederRootHandler;
            _progaugeHandler = progaugeHandler;
            _terminalsHandler = terminalsHandler;
            _mutationHandler = mutationHandler;
            _surveyHandler = surveyHandler;
        }

        public RootCommand Create() => new()
        {
            CreateTanktableCommand(),
            CreatePseCommand(),
            CreateFuelPosCommand(),
            CreateSurveyCommand()
        };

        private Command CreateTanktableCommand() => new("tanktables", "Generate useful files from tank gauge output")
        {
            CreateVeederRootCommand(),
            CreateProgaugeCommand()
        };

        private Command CreateVeederRootCommand()
        {
            Command cmd = new("vdr-root", "Functions for working with output scripts from Veeder Root tank gauges")
            {
                new Argument<string>("filepath", "Path to either an individual tank gauge output, or a directory containing multiple files."),
                new Option<string?>(new[] { "--output", "-o" }, "Path to output directory. If not specified, files will be generated in the same directory as the initial path."),
                new Option<bool>(new[] { "--fuelposfile", "-p" }, "Creates a FuelPOS tank table file (TMS_AOF.INP)"),
                new Option<bool>(new[] { "--csv", "-c" }, "Creates a CSV file containing tank setup information"),
            };

            cmd.Handler = CommandHandler.Create((VeederRootOptions options) =>
            {
                _veederRootHandler.RunHandlerAndReturnExitCode(options);
            });

            return cmd;
        }

        private Command CreateProgaugeCommand()
        {
            Command cmd = new("progauge", "Functions for working with output scripts from Pro Gauge tank gauges")
            {
                new Argument<string>("filepath", "Path to a directory containing ."),
                new Option<string?>(new[] { "--output", "-o" }, "Path to output directory. If not specified, files will be generated in the same directory as the initial path."),
                new Option<bool>(new[] { "--fuelposfile", "-p" }, "Creates a FuelPOS tank table file (TMS_AOF.INP)")
            };

            cmd.Handler = CommandHandler.Create((ProgaugeOptions options) =>
            {
                _progaugeHandler.RunHandlerAndReturnExitCode(options);
            });

            return cmd;
        }

        private Command CreatePseCommand() => new("pse", "Tools for working with the reports generated by Petrol Server")
        {
            CreateTerminalsCommand()
        };

        private Command CreateTerminalsCommand() 
        {
            Command cmd = new("terminals", "Commands for use with Terminals_044.csv")
            {
                new Argument<string>("filepath", "Path to Terminals_044.csv"),
                new Option<bool>(new[] { "--emisfile", "-e" }, "Create a Remote eMIS site-list file. If no output path is specified, will deploy the file " +
                "directlty to it's location in your AppData folder.")
            };

            cmd.Handler = CommandHandler.Create((TerminalsOptions options) =>
            {
                _terminalsHandler.RunHandlerAndReturnExitCode(options);
            });

            return cmd;
        }

        private Command CreateFuelPosCommand() => new("fuelpos", "FuelPOS related commands, such as mutation handling and file transfers")
        {
            CreateMutationCommand()
        };

        private Command CreateMutationCommand() 
        {
            Command cmd = new("create-mutation", "Create mutations for FuelPOS")
            {
                new Option<string?>(new[] { "--card-id", "-c" }, "Path to CardIdentifications.db3 - creates a CRDID_MUT based on the database provided."),
                new Option<string?>(new[] { "--output", "-o" }, "Output directory - leave blank to create files in the same directory as the db3.")
            };

            cmd.Handler = CommandHandler.Create((CreateMutationOptions options) =>
            {
                _mutationHandler.RunHandlerAndReturnExitCode(options);
            });

            return cmd;
        }


        private Command CreateSurveyCommand() 
        {
            Command cmd = new("surveyor", "Tools for generating survey outputs based on one or more StatDev.xml files")
            {
                new Argument<string>("filepath", "Path to either an individual file, or directory containing multile StatDev XMLs."),
                new Option<string>(new[] { "--output", "-o" }, "Path to store any created files.") { IsRequired = true },
                new Option<bool>(new[] { "--sheet", "-s" }, "Create a survey spreadsheet.")
            };

            cmd.Handler = CommandHandler.Create((SurveyOptions options) =>
            {
                _surveyHandler.RunHandlerAndReturnExitCode(options);
            });

            return cmd;
        } 
    }
}