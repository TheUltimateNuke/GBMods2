using HarmonyLib;
using Il2CppGB.Game;
using MelonLoader;
using UnityEngine;

namespace NoTimeLimit;

public class Mod : MelonMod
{
    public static class BuildInfo
    {
        public const string Name = nameof(NoTimeLimit);
        public const string Version = "1.0.0";
        public const string Description = "A mod for Gang Beasts that removes the time limit in local games.";
        public const string Author = "TheUltimateNuke";
        public const string Company = "CementGB";
    }

    [HarmonyPatch(typeof(GameMode), nameof(GameMode.SetupTimer))]
    private static class GameModeSetupTimerPatch
    {
        private static void Postfix(GameMode __instance)
        {
            __instance.timer = float.MaxValue;
        }
    }
}
