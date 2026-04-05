using HarmonyLib;

namespace Linkoid.Repo.RoboUnion;

[HarmonyPatch(typeof(GameManager))]
internal static class GameManagerPatches
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    private static void GameManager_Awake(GameManager __instance)
    {
	    if(__instance != GameManager.instance || RoboUnion.ConfigModel.MaxPlayers.Value <= 0) return;
	    int _maxPlayers = RoboUnion.ConfigModel.MaxPlayers.Value > 0 ? RoboUnion.ConfigModel.MaxPlayers.Value : GameManager.maxPlayersDefault;
	    __instance.SetMaxPlayers(_maxPlayers);
    }
}