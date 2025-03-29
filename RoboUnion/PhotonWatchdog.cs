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

    static int messagesRoomEstimate = 0;
    static int messagesRoomEstimateMax = 0;

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
            EstimateMessages();
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

    void EstimateMessages()
    {
        messagesRoomEstimate = (messageSendPreviousCount + messageRecieverPreviousCount) * (PhotonNetwork.CurrentRoom?.PlayerCount ?? 1);
        if (messagesRoomEstimate > messagesRoomEstimateMax)
        {
            messagesRoomEstimateMax = messagesRoomEstimate;
            RoboUnion.Logger.LogDebug($"New Photon Room Est. Messages/Second Max: {messagesRoomEstimateMax}");
        }

        if (RoboUnion.ConfigModel.LogMessagesPerSecond.Value)
        {
            RoboUnion.Logger.LogDebug(
$"""
Photon Player Sent Messages/Second: {messageSendPreviousCount}
Photon Player Sent Reciever Messages/Second: {messageRecieverPreviousCount}
Photon Room Est. Messages/Second: {messagesRoomEstimate}
Photon Room Est. Messages/Second Max: {messagesRoomEstimateMax}
"""
            );
        }
    }

    void OnGUI()
    {
        if (!RoboUnion.ConfigModel.ShowMessagesPerSecond.Value) return;

        GUILayout.Label(
$"""
Photon Player Sent Messages/Second: {messageSendPreviousCount}
Photon Player Sent Reciever Messages/Second: {messageRecieverPreviousCount}
Photon Room Est. Messages/Second: {messagesRoomEstimate}
Photon Room Est. Messages/Second Max: {messagesRoomEstimateMax}
"""
        );
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PhotonPeer), nameof(PhotonPeer.SendOperation),
        new Type[] { typeof(byte), typeof(ParameterDictionary), typeof(SendOptions) })]
    public static void PhotonPeer_SendOperation_Postfix(bool __result, ParameterDictionary operationParameters)
    {
        try
        {
            //RoboUnion.Logger.LogDebug($"LoadBalancingPeer_SendOperation_Postfix (__result: {__result})");
            if (!__result) return;

            messageSendRunningCount++;

            ReceiverGroup receiverGroup;
            if (!operationParameters.TryGetValue(ParameterCode.ReceiverGroup, out receiverGroup))
            {
                receiverGroup = ReceiverGroup.Others;
            }
            switch (receiverGroup)
            {
                case ReceiverGroup.Others:
                case ReceiverGroup.All:
                    messageRecieverRunningCount += (PhotonNetwork.CurrentRoom?.PlayerCount ?? 1) - 1;
                    break;
                case ReceiverGroup.MasterClient:
                    messageRecieverRunningCount += 1;
                    break;
            }
        }
        catch (Exception ex)
        {
            RoboUnion.Logger.LogDebug($"LoadBalancingPeer_SendOperation_Postfix Exception:\n{ex.Message}\n{ex.StackTrace}");
        }
    }
}
