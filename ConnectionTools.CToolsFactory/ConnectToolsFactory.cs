using System;
using ConnectionTools.ConnectTools;
using ConnectionTools.FTPTools;
using Microsoft.Extensions.Logging;

//using SSHTools;

namespace ConnectionTools.CToolsFactory;

public static class ConnectToolsFactory
{
    public static CTools? CreateConnectToolsByAddress(ConnectToolParameters parameters, ILogger logger,
        bool useConsole = false)
    {
        if (!Uri.TryCreate(parameters.SiteRootAddress, UriKind.Absolute, out Uri? uri))
        {
            return null;
        }

        return uri.Scheme.ToUpperInvariant() switch
        {
            "FTP" => new FtpTools(parameters, logger, useConsole),
            //"SSH" => new SshTools(parameters, logger),
            _ => null
        };
    }
}
