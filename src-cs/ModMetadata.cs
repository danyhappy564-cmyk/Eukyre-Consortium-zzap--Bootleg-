using SPTarkov.Server.Core.Models.Spt.Mod;

namespace EukyreConsortium;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.probablyeukyre.consortiumofthings";
    public string Name { get; init; } = "Eukyre's Consortium of Things";
    public string Author { get; init; } = "ProbablyEukyre";
    public List<string>? Contributors { get; init; } = ["GrooveypenguinX (framework)"];
    public SemanticVersioning.Version Version { get; init; } = new("2.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
    public bool HasPrepatcher { get; init; } = false;

    public List<string>? Incompatibilities { get; init; } = ["Blahaj"];

    /// <summary>
    /// On 3.11 the WTT item framework was vendored into this mod as TypeScript. On 4.1 it is a
    /// real package, so the framework is now a hard dependency instead of a copy.
    /// </summary>
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = new()
    {
        ["com.wtt.commonlib"] = new("~3.0"),
    };

    public string? Url { get; init; } = "https://github.com/danyhappy564-cmyk/Eukyre-Consortium4.1";
    public string License { get; init; } = "MIT";
}
