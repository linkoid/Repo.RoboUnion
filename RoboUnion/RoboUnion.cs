using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace Linkoid.Repo.RoboUnion;

[BepInPlugin("Linkoid.Repo.RoboUnion", "RoboUnion", "0.6")]
public class RoboUnion : BaseUnityPlugin
{
    internal static RoboUnion Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    internal static ConfigModel ConfigModel { get; private set; } = null!;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    
    /// <summary> Feature flag for REPO v0.4 patches </summary>
    internal static bool UseV04MaxPlayersLogic => GameManagerPatches.method_SetMaxPlayers != null;

    private void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        ConfigModel = new ConfigModel(this.Config);
        ConfigModel.MaxPlayers.SettingChanged += OnMaxPlayersChanged;

        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    private void Start()
    {
        Logger.LogInfo($"Creating {nameof(PhotonWatchdog)}");
        var photonWatchdog = new GameObject(nameof(PhotonWatchdog), typeof(PhotonWatchdog));
    }

    private void OnMaxPlayersChanged(object sender, EventArgs args)
    {
        int maxPlayers = ConfigModel.MaxPlayers.Value > 0 ? ConfigModel.MaxPlayers.Value : GameManager.maxPlayersDefault;
        if (GameManager.instance)
        {
            GameManager.instance.SetMaxPlayersSafe(maxPlayers);
        }
    }
}