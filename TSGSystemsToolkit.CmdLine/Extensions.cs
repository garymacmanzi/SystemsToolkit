﻿using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace TSGSystemsToolkit.CmdLine;

internal static class Extensions
{
    internal static Version GetAvailableVersion(string folderPath)
    {
        Version ver = new();
        foreach (var file in Directory.EnumerateFiles(folderPath).Where(x => Path.GetFileNameWithoutExtension(x).StartsWith("systk_installer_")))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            var versionInfo = fileName.Split('_')[2];
            var splitVers = versionInfo.Split('.');
            Version availVersion = new(splitVers[0], splitVers[1], splitVers[2]);

            if (availVersion > ver)
            {
                ver = availVersion;
                ver.InstallerPath = file;
            }
        }

        return ver;
    }

    internal static bool IsUpdateAvailable(string installerLocation)
    {
        // TODO: This is duplicated in UpdateHandler, spin off to separate helper
        var assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
        Version currentVersion = new(assemblyVersion.Major.ToString(), assemblyVersion.Minor.ToString(), assemblyVersion.Build.ToString());

        try
        {
            Version available = GetAvailableVersion(installerLocation);
            return available > currentVersion;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"Error checking for updates: [red]{ex.Message}[/e]");
        }

        return false;
    }

    internal static void UpdateAccessToken(string accessToken)
    {
        var asPath = GetAppsettingsPath();
        var text = File.ReadAllText(asPath);

        var json = JsonSerializer.Deserialize<ExpandoObject>(text) as IDictionary<string, object>;

        json["AccessToken"] = accessToken;

        var newJson = JsonSerializer.Serialize<dynamic>(json,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
        File.WriteAllText(asPath, newJson);
    }

    internal static void UpdateEmailAddress(string emailAddress)
    {
        var asPath = GetAppsettingsPath();
        var text = File.ReadAllText(asPath);

        var json = JsonSerializer.Deserialize<ExpandoObject>(text) as IDictionary<string, object>;

        json["EmailAddress"] = emailAddress;

        var newJson = JsonSerializer.Serialize<dynamic>(json,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
        File.WriteAllText(asPath, newJson);
    }

    internal static string GetAppsettingsPath()
    {
        var absolutePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

        return @$"{absolutePath}\appsettings.json";
    }

    internal static bool IsDirectory(this string path)
    {
        FileAttributes attr = File.GetAttributes(path);

        if (attr == FileAttributes.Directory)
        {
            return true;
        }

        return false;
    }
}
