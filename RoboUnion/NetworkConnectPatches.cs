using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Linkoid.Repo.RoboUnion;

[HarmonyPatch(typeof(NetworkConnect))]
internal static class NetworkConnectPatches
{
    public static int? PhotonServerPlayerLimit { get; private set; } = null;

    [HarmonyTranspiler, HarmonyPatch(nameof(NetworkConnect.TryJoiningRoom))]
    static IEnumerable<CodeInstruction> TryJoiningRoom_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher head = new(instructions);

        // Match:
        //   roomOptions.MaxPlayers = 6
        head.MatchForward(false,
            new CodeMatch(OpCodes.Ldc_I4_6),
            new CodeMatch(OpCodes.Stfld, AccessTools.DeclaredField(typeof(RoomOptions), nameof(RoomOptions.MaxPlayers)))
        );
        head.ThrowIfInvalid("Could not match 'roomOptions.MaxPlayers = 6'");

        // Replace:
        //   6
        // With:
        //   TryJoiningRoom_GetMaxPlayers()
        head.Set(OpCodes.Call, ((Delegate)TryJoiningRoom_GetMaxPlayers).Method);

        return head.InstructionEnumeration();
    }

    static int TryJoiningRoom_GetMaxPlayers()
    {
        var maxPlayers = RoboUnion.ConfigModel.MaxPlayers.Value;
        if (PhotonServerPlayerLimit.HasValue && maxPlayers > PhotonServerPlayerLimit.Value)
        {
            return PhotonServerPlayerLimit.Value;
        }
        return maxPlayers;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkConnect.OnJoinRoomFailed))]
    [HarmonyPatch(nameof(NetworkConnect.OnCreateRoomFailed))]
    static void OnCreateRoomFailed_Prefix(short returnCode, ref string message)
    {
        if (!message.ToLowerInvariant().Contains("max players peer room value is too big")) return;

        int lobbyLimit;
        try
        {
            lobbyLimit = int.Parse(message.Split(':').LastOrDefault()?.Split('.')?.FirstOrDefault() ?? "");
        }
        catch
        {
            lobbyLimit = 20;
        }

        RoboUnion.Logger.LogWarning($"Max players exceeds current Photon servers limit. Temporarily limting players to {lobbyLimit}");
        PhotonServerPlayerLimit = lobbyLimit;

        message += $"\n<color=#{new Color(1f, 0.594f, 0f).ToHexString()}>RoboUnion max players limited to {lobbyLimit} for this session.</color>";
    }
}
