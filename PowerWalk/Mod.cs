using HarmonyLib;
using Il2CppFemur;
using Il2CppGB.Game.Gameplay;
using PowerWalk;
using MelonLoader;

// Mod info can be changed in PowerWalk.csproj.
[assembly:
    MelonInfo(typeof(Mod), MyModInfo.Name /* Mod Name */,
        MyModInfo.Version /* Version (Semantic versioning convention recommended) */, MyModInfo.Author /* Your Name */,
        null /* Download Link (optional, null means disabled) */)]
[assembly: MelonGame("Boneloaf", "Gang Beasts")] // Makes sure this mod is only loaded for Gang Beasts. Remove this if you want to use this template for other games.

namespace PowerWalk;

public class Mod : MelonMod
{
    public static MelonLogger.Instance Logger =>
        Melon<Mod>.Logger; // Forwards MelonLoader's Logger to static access, so it is easier to call for it from other classes (instead of Melon<Mod>.Logger.Msg("blah blah") it is simply Mod.Logger.Msg("blah blah"))

    private static readonly MelonPreferences_Category PowerWalkPrefCateg = MelonPreferences.CreateCategory("PowerWalk", "PowerWalk");
    private static readonly MelonPreferences_Entry<float> _powerWalkMoveSpeedPref = PowerWalkPrefCateg.CreateEntry("MoveSpeed", 1.5f, "Move Speed Multiplier", "How fast all beasts will move.");
    private static readonly MelonPreferences_Entry<float> _powerWalkJumpForceMultiplierPref = PowerWalkPrefCateg.CreateEntry("JumpForceMultiplier", 1f, "Jump Force Multiplier", "How high all beasts will jump.");
    private static readonly MelonPreferences_Entry<bool> _powerWalkDisableInputSpamDecayPref = PowerWalkPrefCateg.CreateEntry("DisableInputSpamDecay", true, "Disable Input Spam Decay", "Whether to disable the force decay when punching or jumping too frequently.");

    public static float PowerWalkJumpForceMultiplier
    {
        get
        {
            return _powerWalkJumpForceMultiplierPref.Value;
        }
        set
        {
            _powerWalkJumpForceMultiplierPref.Value = value;
            GameplayModifiers.jumpForceMul = value;
        }
    }

    public static bool DisableInputSpamDecay
    {
        get
        {
            return _powerWalkDisableInputSpamDecayPref.Value;
        }
        set
        {
            _powerWalkDisableInputSpamDecayPref.Value = value;
        }
    }
    
    public static float MoveSpeed
    {
        get
        {
            return _powerWalkMoveSpeedPref.Value;
        }
        set
        {
            _powerWalkMoveSpeedPref.Value = value;
        }
    }
    
    public override void OnInitializeMelon()
    {
        PowerWalkJumpForceMultiplier = _powerWalkJumpForceMultiplierPref.Value;
        
        LoggerInstance.Msg(System.ConsoleColor.Green,
            $"Mod {MyModInfo.Name} initialized!"); // You must use MelonLoader's Logger instance, NOT Unity's Debug.Log or the system console.
    }

    [HarmonyPatch(typeof(MovementHandeler_HumanoidMediumEctomorph), nameof(MovementHandeler_HumanoidMediumEctomorph.RunCycleRotateBall))]
    private static class MoveSpeedPatch
    {
        private static void Prefix(MovementHandeler_HumanoidMediumEctomorph __instance)
        {
            __instance._cycleModifer = MoveSpeed;
        }
    }
    
    [HarmonyPatch(typeof(Actor), nameof(Actor.UpdateState))]
    private static class InputSpeedPatch
    {
        private static void Postfix(Actor __instance)
        {
            if (DisableInputSpamDecay)
                __instance.inputSpamForceModifier = 1f;
        }
    }
}