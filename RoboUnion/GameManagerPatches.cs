using HarmonyLib;
using Photon.Pun;
using System;
using System.Reflection;

namespace Linkoid.Repo.RoboUnion;

[HarmonyPatch(typeof(GameManager))]
internal static class GameManagerPatches
{
    internal static readonly MethodInfo method_SetMaxPlayers = typeof(GameManager).GetMethod(nameof(GameManager.SetMaxPlayers));

    [HarmonyPrepare]
    static void Patch()
    {
        if (!RoboUnion.UseV04MaxPlayersLogic) return;

        HarmonyMethod setMaxPlayersPostfix = new HarmonyMethod(((Delegate)SetMaxPlayers_Postfix).Method);
        RoboUnion.Instance.Harmony!.Patch(method_SetMaxPlayers, postfix: setMaxPlayersPostfix);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameManager.Awake))]
    static void Awake_Postfix(GameManager __instance)
    {
        if (RoboUnion.ConfigModel.MaxPlayers.Value <= 0) return;
        __instance.SetMaxPlayersSafe(RoboUnion.ConfigModel.MaxPlayers.Value);
    }

    //[HarmonyPostfix]
    //[HarmonyPatch(nameof(GameManager.SetMaxPlayers))]
    static void SetMaxPlayers_Postfix(GameManager __instance)
    {
        if (!SemiFunc.IsMasterClient() || PhotonNetwork.CurrentRoom == null) return;

        RoboUnion.Logger.LogInfo($"Photon room MaxPlayers changed to {PhotonNetwork.CurrentRoom.MaxPlayers}");
    }

    public static void SetMaxPlayersSafe(this GameManager __instance, int target)
    {
        if (RoboUnion.UseV04MaxPlayersLogic)
        {
            SetMaxPlayersUnsafe(__instance, target);
        }

        // Place direct call in an anonymous function to avoid type load exceptions
        // in versions of the game without this method.
        static void SetMaxPlayersUnsafe(GameManager __instance, int target)
        {
            __instance.maxPlayersPhoton = target;
            __instance.SetMaxPlayers(target);
        }
    }
}