using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
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

        RoboUnion.Instance.Harmony!.Patch(method_StateMachine_MoveNext,
            transpiler: new HarmonyMethod(((Delegate)HostLobby_Transpiler).Method));
    }

    private static IEnumerable<CodeInstruction> HostLobby_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher head = new(instructions);
        head.MatchForward(false,
            new CodeMatch(OpCodes.Ldc_I4_6),
            new CodeMatch(OpCodes.Call, AccessTools.DeclaredMethod(typeof(SteamMatchmaking), nameof(SteamMatchmaking.CreateLobbyAsync)))
        );
        head.ThrowIfInvalid("Could not match 'SteamMatchmaking.CreateLobbyAsync(6)'");

        // Replace:
        //   6
        // With:
        //   TryJoiningRoom_GetMaxPlayers()
        head.Set(OpCodes.Call, ((Delegate)NetworkConnectPatches.TryJoiningRoom_GetMaxPlayers).Method);

        return head.InstructionEnumeration();
    }
}