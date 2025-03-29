using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linkoid.Repo.RoboUnion;

internal record class ConfigModel(ConfigFile ConfigFile)
{
    internal const string GeneralSection = "General";
    public readonly ConfigEntry<int> MaxPlayers = ConfigFile.Bind(GeneralSection, "MaxPlayers", 10);

    internal const string DeveloperSection = "Developer";
    public readonly ConfigEntry<bool> LogMessagesPerSecond = ConfigFile.Bind(DeveloperSection, "LogMessagesPerSecond", false);
    public readonly ConfigEntry<bool> ShowMessagesPerSecond = ConfigFile.Bind(DeveloperSection, "ShowMessagesPerSecond", false);
}
