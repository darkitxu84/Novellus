using Novellus.Lib.Backend.Logging;
using Novellus.Lib.Core.Packages;

namespace Novellus.Lib.Backend;

public enum ArgKind
{
    /// <summary>
    /// Represents a normalized path (like relative to game files)
    /// </summary>
    Path,
    /// <summary>
    /// Represents a normalized path relative to the package
    /// </summary>
    PackagePath,
    /// <summary>
    /// Represents a normalized file relative to the package
    /// </summary>
    PackageFile
}

public static class ApiCalls
{
    public static string[][]? ResolveArgs(IPackage package, string apiCall, ArgKind[] kinds)
    {
        var wantedApiCalls = package.ApiCalls
            .Where(x => string.Equals(x.Run, apiCall, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        // if there are not any calls to resolve, just early return
        if (wantedApiCalls.Length == 0) return null;

        var resolvedArgs = new List<string[]>();

        foreach (var call in wantedApiCalls)
        {
            if (call.With.Count != kinds.Length)
            {
                Logger.Error($"Api call '{call.Run}' in package {package.Metadata.Id} has {call.With.Count} arguments, " +
                             $"but {kinds.Length} were expected");
                continue;
            }

            var argToResolve = new string[kinds.Length];
            bool failed = false;
            for (int j = 0; j < kinds.Length; j++)
            {
                var argKind = kinds[j];
                var arg = call.With[j];
                if (string.IsNullOrEmpty(arg))
                {
                    Logger.Error($"Argument {j+1} of api call '{call.Run}' in package {package.Metadata.Id} " +
                                 $"is empty");
                    failed = true;
                    break;
                }

                switch (argKind)
                {
                    case ArgKind.Path:
                        argToResolve[j] = PathUtils.Normalize(arg);
                        break;
                    case ArgKind.PackagePath:
                        argToResolve[j] = Path.Combine(package.Path, PathUtils.Normalize(arg));
                        if (!Directory.Exists(argToResolve[j]))
                        {
                            Logger.Error($"Error trying to resolve argument {j+1} of api call '{call.Run}' " +
                                         $"in package '{package.Metadata.Id}': path '{argToResolve[j]}' doesn't exist");
                            failed = true;
                        }
                        break;
                    case ArgKind.PackageFile:
                        argToResolve[j] = Path.Combine(package.Path, PathUtils.Normalize(arg));
                        if (!File.Exists(argToResolve[j]))
                        {
                            Logger.Error($"Error trying to resolve argument {j + 1} of api call '{call.Run}' " +
                                         $"in package '{package.Metadata.Id}': file '{argToResolve[j]}' doesn't exist");
                            failed = true;
                        }
                        break;
                    default:
                        Logger.Error($"Unknown argument kind: {argKind}");
                        failed = true;
                        break;
                }
            }
            if (!failed) resolvedArgs.Add(argToResolve);
        }
        return resolvedArgs.Count > 0 ? resolvedArgs.ToArray() : null;
    }

    // helpers 
    public static (string, string)[]? ResolveArgs(IPackage package, string apiCall, ArgKind kind1, ArgKind kind2)
    {
        var rows = ResolveArgs(package, apiCall, [kind1, kind2]);
        return rows?.Select(r => (r[0], r[1])).ToArray();
    }
    public static string[]? ResolveArgs(IPackage package, string apiCall, ArgKind kind)
    {
        var rows = ResolveArgs(package, apiCall, [kind]);
        return rows?.Select(r => r[0]).ToArray();
    }
}
