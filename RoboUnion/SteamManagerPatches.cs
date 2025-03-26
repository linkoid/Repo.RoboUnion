using HarmonyLib;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Linkoid.Repo.RoboUnion;

[HarmonyPatch(typeof(SteamManager))]
internal static class SteamManagerPatches
{
    [HarmonyPrepare]
    private static void Patch()
    {
        var method_HostLobby = AccessTools.DeclaredMethod(typeof(SteamManager), nameof(SteamManager.HostLobby));
        var stateMachineAttribute = method_HostLobby.GetCustomAttribute<AsyncStateMachineAttribute>();

        var method_StateMachine_MoveNext = AccessTools.DeclaredMethod(stateMachineAttribute.StateMachineType, nameof(IAsyncStateMachine.MoveNext));
    }
}