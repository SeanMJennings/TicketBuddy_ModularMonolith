namespace TicketBuddy.AppHost;

public static class UserInterface
{
    public const string ImageName = "localhosting-ui:local";
    public static async Task CreateImage()
    {
        var repoRoot = FindRepoRootContaining(AppContext.BaseDirectory, "UI");
        var uiBuildContext = Path.GetFullPath(Path.Combine(repoRoot, "UI"));
        var uiDockerfile = Path.GetFullPath(Path.Combine(uiBuildContext, "Dockerfile"));

        if (!Directory.Exists(uiBuildContext)) throw new DirectoryNotFoundException($"Could not locate folder `UI` at `{uiBuildContext}`");
        if (!File.Exists(uiDockerfile)) throw new FileNotFoundException($"Dockerfile not found at `{uiDockerfile}`");

        var psi = new System.Diagnostics.ProcessStartInfo("docker",
            $"build -f \"{uiDockerfile}\" -t {ImageName} \"{uiBuildContext}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var proc = System.Diagnostics.Process.Start(psi)!;
        var stdout = proc.StandardOutput.ReadToEndAsync();
        var stderr = proc.StandardError.ReadToEndAsync();
        await proc.WaitForExitAsync();
        if (proc.ExitCode != 0)
        {
            var outText = await stdout;
            var errText = await stderr;
            throw new InvalidOperationException($"Docker build failed for `UI`.\nStdout:\n{outText}\nStderr:\n{errText}");
        }
    }
    
    private static string FindRepoRootContaining(string startDir, string markerFolder)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, markerFolder)))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException($"Could not locate folder `{markerFolder}` from `{startDir}` upward.");
    }
}