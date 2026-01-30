using System;
using System.Collections;
using HarmonyLib;
using Il2CppFemur;
using Il2CppGB.Game.Gameplay;
using PowerThrow;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

// Mod info can be changed in PowerThrow.csproj.
[assembly:
    MelonInfo(typeof(Mod), MyModInfo.Name /* Mod Name */,
        MyModInfo.Version /* Version (Semantic versioning convention recommended) */, MyModInfo.Author /* Your Name */,
        null /* Download Link (optional, null means disabled) */)]
[assembly: MelonGame("Boneloaf", "Gang Beasts")] // Makes sure this mod is only loaded for Gang Beasts. Remove this if you want to use this template for other games.

namespace PowerThrow;

[HarmonyPatch]
internal static class Patches
{
    [HarmonyPatch(typeof(ControlHandeler_Human), nameof(ControlHandeler_Human.PunchGrab))]
    [HarmonyPrefix]
    private static void Prefix(ControlHandeler_Human __instance, ControlHandeler_Human.Arm arm)
    {
        if ((!__instance.IsThisArmJustUp(ControlHandeler_Human.Arm.Right) &&
             !__instance.IsThisArmJustUp(ControlHandeler_Human.Arm.Left)) || __instance.armActionTimer <= 0.2f || !__instance.onGround || __instance.grabRigidbody == null)
        {
            return;
        }
        
        MelonCoroutines.Start(Mod.WaitForArmRelease(__instance, arm));
    }
}

public class Mod : MelonMod
{
    public static MelonLogger.Instance Logger =>
        Melon<Mod>.Logger; // Forwards MelonLoader's Logger to static access, so it is easier to call for it from other classes (instead of Melon<Mod>.Logger.Msg("blah blah") it is simply Mod.Logger.Msg("blah blah"))

    private static readonly MelonPreferences_Category _preferencesCategory = MelonPreferences.CreateCategory("PowerThrow", "PowerThrow");
    private static readonly MelonPreferences_Entry<float> _throwForcePref = _preferencesCategory.CreateEntry("ThrowForce", 800f, "Throw Force");

    public static float ThrowForce
    {
        get
        {
            return _throwForcePref.Value;
        }
        set
        {
            _throwForcePref.Value = value;
        }
    }

    internal static IEnumerator WaitForArmRelease(ControlHandeler_Human __instance, ControlHandeler_Human.Arm arm)
    {
        var rb = __instance.grabRigidbody;
        while (__instance.grabJoint != null)
        {
            rb = __instance.grabRigidbody;
            yield return null;
        }

        bool isRagdoll = false;
        switch (arm)
        {
            case ControlHandeler_Human.Arm.Left:
                isRagdoll = __instance.actor.bodyHandeler.leftGrabInteractable.partOfRagdoll;
                break;
            case ControlHandeler_Human.Arm.Right:
                isRagdoll = __instance.actor.bodyHandeler.rightGrabInteractable.partOfRagdoll;
                break;
        }
        
        ApplyThrowImpulse(rb, isRagdoll);
    }
    
    private static void ApplyThrowImpulse(Rigidbody rb, bool isRagdoll)
    {
        rb.AddForce(rb.velocity * ThrowForce * (isRagdoll ? 80f : 1),
            ForceMode.VelocityChange);
    }
    
    public override void OnInitializeMelon()
    {
        // Put starting code here
        LoggerInstance.Msg(System.ConsoleColor.Green,
            $"Mod {MyModInfo.Name} initialized!"); // You must use MelonLoader's Logger instance, NOT Unity's Debug.Log or the system console.
    }
}