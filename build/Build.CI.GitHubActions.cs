using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "pr",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    OnPullRequestBranches = new[] { "master", "main" },
    OnPullRequestIncludePaths = new[] { "**/*.*" },
    OnPullRequestExcludePaths = new[] { "**/*.md" },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(Compile), nameof(Test), nameof(Pack) },
    CacheKeyFiles = new string[0]),
]
[GitHubActions(
    "build",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "master", "main" },
    OnPushTags = new[] { "v*.*.*" },
    OnPushIncludePaths = new[] { "**/*.*" },
    OnPushExcludePaths = new[] { "**/*.md" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(Compile), nameof(Test), nameof(Pack), nameof(Publish) },
    ImportSecrets = new [] { "NUGET_API_KEY" },
    CacheKeyFiles = new string[0])
]
public partial class Build
{
}
