using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Linkoid.Repo.RoboUnion;

[HarmonyPatch]
internal class PhotonWatchdog : MonoBehaviour
{
    static int messageRunningCount = 0;
    static int messagePreviousCount = 0;


    void Awake()
    {
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;
    }

    private float lastUpdate = 0;
    void Update()
    {
        if (Time.realtimeSinceStartup - lastUpdate > 1)
        {
            UpdateMessageSample();
            lastUpdate = Time.realtimeSinceStartup;
        }
    }

    void UpdateMessageSample()
    {
        messagePreviousCount = messageRunningCount;
        messageRunningCount = 0;
    }

    void OnGUI()
    {
        if (RoboUnion.ConfigModel.ShowMessagesPerSecond.Value)
        {
            GUILayout.Label($"Photon Room Messages/Second: {messagePreviousCount}");
        }
    }


    [HarmonyPatch(typeof(LoadBalancingPeer), nameof(LoadBalancingPeer.SendOperation))]
    public static void LoadBalancingPeer_SendOperation_Postfix(bool __result)
    {
        if (!__result) return;

        RoboUnion.Logger.LogDebug("LoadBalancingClient_OnMessage_Postfix");
        messageRunningCount++;
    }
}
