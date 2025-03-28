using ExitGames.Client.Photon;
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
    static int messageSendRunningCount = 0;
    static int messageSendPreviousCount = 0;

    static int messageRecieverRunningCount = 0;
    static int messageRecieverPreviousCount = 0;

    static int maxEstimatedMessages = 0;

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
        messageSendPreviousCount = messageSendRunningCount;
        messageSendRunningCount = 0;

        messageRecieverPreviousCount = messageRecieverRunningCount;
        messageRecieverRunningCount = 0;
    }

    void OnGUI()
    {
        if (RoboUnion.ConfigModel.ShowMessagesPerSecond.Value)
        {
            GUILayout.Label($"Photon Player Sent Messages/Second: {messageSendPreviousCount}");
            GUILayout.Label($"Photon Player Sent Reciever Messages/Second: {messageRecieverPreviousCount}");

            int estimatedMessages = (messageSendPreviousCount + messageRecieverPreviousCount) * (PhotonNetwork.CurrentRoom?.PlayerCount ?? 1);
            GUILayout.Label($"Photon Room Est. Messages/Second: {estimatedMessages}");

            if (estimatedMessages > maxEstimatedMessages)
            {
                maxEstimatedMessages = estimatedMessages;
            }
            GUILayout.Label($"Photon Room Est. Messages/Second Max: {maxEstimatedMessages}");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LoadBalancingPeer), nameof(LoadBalancingPeer.OpRaiseEvent))]
    public static void LoadBalancingPeer_OpRaiseEvent_Postfix(bool __result, RaiseEventOptions raiseEventOptions)
    {
        //RoboUnion.Logger.LogDebug($"LoadBalancingPeer_SendOperation_Postfix (__result: {__result})");
        if (!__result) return;

        switch (raiseEventOptions.Receivers)
        {
            case ReceiverGroup.Others:
            case ReceiverGroup.All:
                messageRecieverRunningCount += PhotonNetwork.CurrentRoom?.PlayerCount ?? 0;
                break;
            case ReceiverGroup.MasterClient:
                messageRecieverRunningCount += 1;
                break;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PhotonPeer), nameof(PhotonPeer.SendOperation),
        new Type[] { typeof(byte), typeof(ParameterDictionary), typeof(SendOptions) })]
    public static void PhotonPeer_SendOperation_Postfix(bool __result, SendOptions sendOptions)
    {
        //RoboUnion.Logger.LogDebug($"LoadBalancingPeer_SendOperation_Postfix (__result: {__result})");
        if (!__result) return;

        messageSendRunningCount++;
    }
}
