﻿using Microsoft.Extensions.Hosting;
using System.CommandLine.Binding;
using System.IO;
using TSGSystemsToolkit.CmdLine.GraphQL;
using TSGSystemsToolkit.CmdLine.Handlers;
using TSGSystemsToolkit.CmdLine.Options;

namespace TSGSystemsToolkit.CmdLine.Binders;

public class SendFileOptionsOptionsBinder : BinderBase<SendFileOptions>
{
    private readonly Argument<string> _filePath;
    private readonly Option<string> _cluster;
    private readonly Option<string> _target;
    private readonly Option<string> _list;
    private readonly Option<string> _site;
    private readonly IHost _host;

    public SendFileOptionsOptionsBinder(Argument<string> filePath, Option<string> cluster, Option<string> list, Option<string> target, Option<string> site, IHost host)
    {
        _filePath = filePath;
        _cluster = cluster;
        _target = target;
        _list = list;
        _site = site;
        _host = host;
    }

    protected override SendFileOptions GetBoundValue(BindingContext bindingContext)
    {
        AddDependencies(bindingContext);

        return new()
        {
            FilePath = bindingContext.ParseResult.GetValueForArgument(_filePath),
            Cluster = bindingContext.ParseResult.GetValueForOption(_cluster),
            Target = ValidateTarget(bindingContext),
            List = bindingContext.ParseResult.GetValueForOption(_list),
            Site = bindingContext.ParseResult.GetValueForOption(_site)
        };
    }

    private void AddDependencies(BindingContext bindingContext)
    {
        bindingContext.AddService<SysTkApiClient>(x =>
            _host.Services.GetService(typeof(SysTkApiClient)) as SysTkApiClient);

        bindingContext.AddService<ILogger<SendFileHandler>>(x =>
            _host.Services.GetService(typeof(ILogger<SendFileHandler>)) as ILogger<SendFileHandler>);
    }

    private string ValidateTarget(BindingContext bindingContext)
    {
        string targetPath = bindingContext.ParseResult.GetValueForOption(_target);

        if (!Path.HasExtension(targetPath))
        {
            string filePath = bindingContext.ParseResult.GetValueForArgument(_filePath);
            targetPath = $"{targetPath}/{Path.GetFileName(filePath)}";
        }

        return targetPath;
    }
}
