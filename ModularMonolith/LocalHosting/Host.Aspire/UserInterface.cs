using System.Diagnostics;

namespace TicketBuddy.AppHost;

public static class UserInterface
{
    public const string ImageName = "ticketbuddy-ui:local";
    public static async Task CreateImage()
    {
        var repoRoot = FindRepoRootContaining(AppContext.BaseDirectory, "UI");
        var uiBuildContext = Path.GetFullPath(Path.Combine(repoRoot, "UI"));
        var uiDockerfile = Path.GetFullPath(Path.Combine(uiBuildContext, "Dockerfile"));

        if (!Directory.Exists(uiBuildContext)) throw new DirectoryNotFoundException($"Could not locate folder `UI` at `{uiBuildContext}`");
        if (!File.Exists(uiDockerfile)) throw new FileNotFoundException($"Dockerfile not found at `{uiDockerfile}`");

        await RunDockerCommandAsync($"image rm -f {ImageName}", throwOnError: false);

        var buildArgs = $"build -f \"{uiDockerfile}\" -t {ImageName} \"{uiBuildContext}\"";
        await RunDockerCommandAsync(buildArgs, throwOnError: true);
    }
    
    private static async Task RunDockerCommandAsync(string args, bool throwOnError)
    {
        var psi = new ProcessStartInfo("docker", args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var proc = Process.Start(psi)!;
        var stdoutTask = proc.StandardOutput.ReadToEndAsync();
        var stderrTask = proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();
        var outText = await stdoutTask;
        var errText = await stderrTask;

        if (proc.ExitCode != 0 && throwOnError)
        {
            throw new InvalidOperationException($"Docker command `docker {args}` failed.\nStdout:\n{outText}\nStderr:\n{errText}");
        }
    }
    
    private static string FindRepoRootContaining(string startDir, string markerFolder)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, markerFolder))) return dir.FullName;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException($"Could not locate folder `{markerFolder}` from `{startDir}` upward.");
    }
}