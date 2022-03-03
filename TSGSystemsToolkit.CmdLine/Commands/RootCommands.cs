﻿using Microsoft.Extensions.Hosting;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using TSGSystemsToolkit.CmdLine.Binders;
using TSGSystemsToolkit.CmdLine.Handlers;
using TSGSystemsToolkit.CmdLine.Options;

namespace TSGSystemsToolkit.CmdLine.Commands
{
    internal class RootCommands : IRootCommands
    {
        private readonly IHost _host;

        public RootCommands(IHost host)
        {
            _host = host;
        }

        public RootCommand Create()
        {
            var updateOption = new Option<bool>(new[] { "--update" }, "Update your version of Systems Toolkit");
            RootCommand cmd = new()
            {
                CreateTanktableCommand(),
                CreatePseCommand(),
                CreateFuelPosCommand(),
                CreateSurveyCommand(),
                CreateDatabaseCommand(),
                updateOption
            };

            var binder = new UpdateOptionsBinder(updateOption, _host);

            cmd.SetHandler((UpdateOptions options, InvocationContext ctx, CancellationToken ct) =>
            {
                var handler = new UpdateHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);
            
            return cmd;
        }

        private Command CreateTanktableCommand() => new("tanktables", "Generate useful files from tank gauge output")
        {
            CreateVeederRootCommand(),
            CreateProgaugeCommand()
        };

        private Command CreateVeederRootCommand()
        {
            Argument<string> filePathArg = new("filepath", "Path to either an individual tank gauge output, or a directory containing multiple files.");
            Option<string?> outputOpt = new(new[] { "--output", "-o" }, "Path to output directory. If not specified, files will be generated in the same directory as the initial path.");
            Option<bool> fuelPosFileOpt = new(new[] { "--fuelposfile", "-p" }, "Creates a FuelPOS tank table file (TMS_AOF.INP)");
            Option<bool> csvOpt = new(new[] { "--csv", "-c" }, "Creates a CSV file containing tank setup information");

            Command cmd = new("vdr-root", "Functions for working with output scripts from Veeder Root tank gauges")
            {
                filePathArg,
                outputOpt,
                fuelPosFileOpt,
                csvOpt
            };

            var binder = new VeederRootOptionsBinder(filePathArg, outputOpt, fuelPosFileOpt, csvOpt, _host);

            cmd.SetHandler((VeederRootOptions options, InvocationContext ctx, CancellationToken ct) =>
            {
                var handler = new VeederRootHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);

            return cmd;
        }

        private Command CreateProgaugeCommand()
        {
            Argument<string> filePathArg = new("filepath", "Path to a directory containing multiple CSV files from a Pro Gauge.");
            Option<string?> outputOpt = new(new[] { "--output", "-o" }, "Path to output directory. If not specified, files will be generated in the same directory as the initial path.");
            Option<bool> fuelPosFileOption = new(new[] { "--fuelposfile", "-p" }, "Creates a FuelPOS tank table file (TMS_AOF.INP)");

            Command cmd = new("progauge", "Functions for working with output scripts from Pro Gauge tank gauges")
            {
                filePathArg,
                outputOpt,
                fuelPosFileOption
            };

            var binder = new ProGaugeOptionsBinder(filePathArg, outputOpt, fuelPosFileOption, _host);

            cmd.SetHandler((ProgaugeOptions options, InvocationContext ctx, CancellationToken ct) =>
            {
                var handler = new ProgaugeHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);

            return cmd;
        }

        private Command CreatePseCommand() => new("pse", "Tools for working with the reports generated by Petrol Server")
        {
            CreateTerminalsCommand()
        };

        private Command CreateTerminalsCommand()
        {
            Argument<string> filePathArg = new("filepath", "Path to Terminals_044.csv");
            Option<bool> emisFileOpt = new(new[] { "--emisfile", "-e" }, "Create a Remote eMIS site-list file. If no output path is specified, will deploy the file " +
                "directly to its location in your AppData folder.");
            Option<string> outputOpt = new(new[] { "--output", "-o" }, "Output path for any created files.");

            Command cmd = new("terminals", "Commands for use with Terminals_044.csv")
            {
                filePathArg,
                emisFileOpt,
                outputOpt
            };

            var binder = new TerminalsOptionsBinder(filePathArg, emisFileOpt, outputOpt, _host);

            cmd.SetHandler((TerminalsOptions options, InvocationContext ctx, CancellationToken ct) =>
            {
                var handler = new TerminalsHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);

            return cmd;
        }

        private Command CreateFuelPosCommand() => new("fuelpos", "FuelPOS related commands, such as mutation handling and file transfers")
        {
            CreateMutationCommand(),
            CreateSendFileCommand(),
            CreateUpdateEmisCommand()
        };

        private Command CreateMutationCommand()
        {
            Option<string?> cardIdOption = new Option<string?>(new[] { "--cardid", "-c" }, "Path to CardIdentifications.db3 - creates a CRDID_MUT based on the database provided.");
            Option<string?> outputOption = new Option<string?>(new[] { "--output", "-o" }, "Output directory - leave blank to create files in the same directory as the db3.");

            Command cmd = new("create-mutation", "Create mutations for FuelPOS")
            {
                cardIdOption,
                outputOption
            };

            var binder = new MutationOptionsBinder(cardIdOption, outputOption, _host);

            cmd.SetHandler((MutationOptions options,
                            InvocationContext context,
                            CancellationToken ct) =>
            {
                var handler = new MutationHandler(options, ct);
                return handler.InvokeAsync(context);
            }, binder);

            return cmd;
        }

        private Command CreateUpdateEmisCommand()
        {
            Option<string?> filePathOption = new(new[] { "--file", "-f" }, "Path to the emis FmsSettings.xml. If this option is not provided, the file will be updated in its default location.");

            Command cmd = new("update-emis", "Update your eMIS site list from the stations available on the API.")
            {
                filePathOption
            };




            return cmd;
        }

        private Command CreateSurveyCommand()
        {
            Argument<string> filePathArg = new("filepath", "Path to either an individual file, or directory containing multile StatDev XMLs.");
            Option<string> outputOpt = new(new[] { "--output", "-o" }, "Path to store any created files.") { IsRequired = true };
            Option<bool> fuelPosOpt = new(new[] { "--fuelpos", "-f" }, "Create a FuelPOS survey spreadsheet.");
            Option<bool> serialNumOpt = new(new[] { "--serialnumbers", "-s" }, "Create a PIN pad serial number survey.");

            Command cmd = new("surveyor", "Generates a FuelPOS survey from one or multiple StatDev.xml files.")
            {
                filePathArg,
                outputOpt,
                fuelPosOpt,
                serialNumOpt
            };

            var binder = new SurveyOptionsBinder(filePathArg, outputOpt, fuelPosOpt, serialNumOpt, _host);

            cmd.SetHandler((SurveyOptions options, InvocationContext ctx, CancellationToken ct) =>
            {
                var handler = new SurveyHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);

            return cmd;
        }

        private Command CreateSendFileCommand()
        {
            var filePathArg = new Argument<string>("filepath", "Path to the file to be sent.");
            var clusterOpt = new Option<string>(new[] { "--cluster", "-c" }, "Send file to every station in the specified Petrol Server cluster.");
            var listOpt = new Option<string>(new[] { "--list", "-l" }, "Path to a CSV file containing a list of PSE station IDs of which to send file to.");
            var targetOpt = new Option<string>(new[] { "--target", "-t" }, "Target path on the FuelPOS system (excluding file name)") { IsRequired = true };
            var siteOpt = new Option<string>(new[] { "--site", "-s" }, "Site ID of a station to send the file to.");

            Command cmd = new("send-file", "Send a file to one or many FuelPOS systems")
            {
                filePathArg,
                clusterOpt,
                listOpt,
                targetOpt,
                siteOpt
            };

            var binder = new SendFileBinder(filePathArg, clusterOpt, listOpt, targetOpt, siteOpt, _host);

            cmd.SetHandler((SendFileOptions options,
                            InvocationContext ctx,
                            CancellationToken ct) =>
            {
                var handler = new SendFileHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);

            return cmd;

            // fuelpos send-file "C:\Users\GaryM\Documents\Work\HTEC_GEMPAY_IPT.xml" -t "/" -c "ShellRba"
        }

        private Command CreateDatabaseCommand()
        {
            Command cmd = new("db", "Commands to add/modify data stored in the database")
            {
                CreateAddStationsCommand(),
                CreateFileZillaCommand()
            };

            return cmd;
        }

        private Command CreateAddStationsCommand()
        {
            var fileOption = new Option<string?>(new[] { "--file", "-f" }, "Path to a CSV file in format \"Site ID;Cluster;Station Name;FTP://Username:Password@IPAddress\" with stations to be added.");
            var individualOption = new Option<string?>(new[] { "--one", "-o" }, "Station details to add, must be in format \"Site ID;Cluster;Station Name;FTP://Username:Password@IPAddress\" (the FTP section is the link copied from Petrol Server)");
            Command cmd = new("add-stations", "Add one or more stations to the database")
            {
                fileOption,
                individualOption
            };

            var binder = new AddStationsBinder(fileOption, individualOption, _host);

            cmd.SetHandler((AddStationsOptions options, InvocationContext ctx, CancellationToken ct) =>
            {
                var handler = new AddStationsHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);

            return cmd;
        }

        private Command CreateFileZillaCommand()
        {
            var siteManagerOption = new Option<string?>(new[] { "--filePath", "-f" }, "Path to your sitemanager.xml. Do not include this option, and SysTk will find it automatically.");

            Command cmd = new("update-filezilla", "Update your FileZilla site manager list with all FuelPOS stations from the API.")
            {
                siteManagerOption
            };

            var binder = new FileZillaOptionsBinder(siteManagerOption, _host);

            cmd.SetHandler((FileZillaOptions options, InvocationContext ctx, CancellationToken ct) =>
            {
                var handler = new FileZillaHandler(options, ct);
                return handler.InvokeAsync(ctx);
            }, binder);

            return cmd;
        }
    }
}

// R2798;RontecUK;Billinghurst;FTP://SUPERVISOR:DF572BBC9@10.68.25.21