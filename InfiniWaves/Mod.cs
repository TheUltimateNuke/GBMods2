using Il2CppFemur;
using Il2CppGB.Game;
using Il2CppGB.Game.Data;
using Il2CppSystem.Collections.Generic;
using InfiniWaves;
using MelonLoader;
using MelonLoader.Utils;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(Mod), Mod.BuildInfo.Name /* Mod Name */, Mod.BuildInfo.Version /* Version (Semantic versioning convention recommended) */, Mod.BuildInfo.Author /* Your Name */, Mod.BuildInfo.DownloadLink /* Download Link for Cement auto-updating (optional, null means disabled) */)]
[assembly: MelonGame("Boneloaf", "Gang Beasts")] // Makes sure this mod is only loaded for Gang Beasts. Remove this if you want to use this template for other games.

namespace InfiniWaves;
public class Mod : MelonMod
{
    public static class BuildInfo
    {
        public const string Name = "InfiniWaves";
        public const string Author = "TheUltimateNuke";
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public static int WaveLimit => (int)_waveLimitPref.BoxedValue;
    public static int BeastPerWaveLimit => (int)_beastPerWaveLimitPref.BoxedValue;

    public static MelonLogger.Instance Logger => Melon<Mod>.Logger;

    private static MelonPreferences_Category _categ;
    private static MelonPreferences_Entry<int> _waveLimitPref;
    private static MelonPreferences_Entry<int> _beastPerWaveLimitPref;

    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg(System.ConsoleColor.Green, $"Mod InfiniWaves initialized!");

        _categ = MelonPreferences.CreateCategory(nameof(InfiniWaves));
        _waveLimitPref = _categ.CreateEntry(nameof(WaveLimit), 1000);
        _beastPerWaveLimitPref = _categ.CreateEntry(nameof(BeastPerWaveLimit), 1000);

        _categ.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "InfiniWavesConfig.cfg"));
        _categ.SaveToFile();
    }

    public override void OnDeinitializeMelon()
    {
        base.OnDeinitializeMelon();

        _categ.SaveToFile();
    }

    public override void OnUpdate()
    {
        if (Keyboard.current.vKey.IsPressed())
            foreach (var statusHandler in Object.FindObjectsOfType<StatusHandeler>())
            {
                if (statusHandler.actor.IsAI)
                    statusHandler.Kill();
            }
    }
}

internal static class GameMode_WavesPatches
{
    [HarmonyLib.HarmonyPatch(typeof(GameMode_Waves), nameof(GameMode_Waves.Init))]
    private static class InitPatch
    {
        private static void Postfix(GameMode_Waves __instance)
        {
            if (__instance._waveInfomation == null) return;
            var beastsIl2cpp = new List<BeastSetup>();

            for (int i = 0; i < 4; i++)
                beastsIl2cpp.Add(new BeastSetup()
                {
                    type = Random.Range(0, 4)
                });

            for (int waveNum = 4; waveNum < Mod.WaveLimit; waveNum++)
            {
                var output = new Wave();

                if (beastsIl2cpp.Count < Mod.BeastPerWaveLimit)
                    beastsIl2cpp.Add(new BeastSetup()
                    {
                        type = Random.Range(0, 4)
                    });

                output.beasts = new List<BeastSetup>(beastsIl2cpp.Cast<IEnumerable<BeastSetup>>());

                __instance._waveInfomation.levelWaves.Add(output);
            }
        }
    }
}
