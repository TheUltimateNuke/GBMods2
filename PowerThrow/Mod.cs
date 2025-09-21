using System;
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
    
    public override void OnInitializeMelon()
    {
        // Put starting code here
        HarmonyInstance.Patch(typeof(ControlHandeler_Human).GetMethod(nameof(ControlHandeler_Human.PunchGrab)), typeof(Mod).GetMethod(nameof(Mod.Prefix), AccessTools.all).ToNewHarmonyMethod());
        LoggerInstance.Msg(System.ConsoleColor.Green,
            $"Mod {MyModInfo.Name} initialized!"); // You must use MelonLoader's Logger instance, NOT Unity's Debug.Log or the system console.
    }
    
    
    private static void Prefix(ControlHandeler_Human __instance, ControlHandeler_Human.Arm arm)
    {
        if (arm != ControlHandeler_Human.Arm.Left)
        {
            if (arm == ControlHandeler_Human.Arm.Right)
            {
                __instance.punchTimer = __instance.rightPunchTimer;
                __instance.armActionTimer = __instance.rightArmActionTimer;
                __instance.punch = __instance.rightPunch;
                __instance.grab = __instance.rightGrab;
                __instance.grabJoint = __instance.actor.bodyHandeler.rightGrabJoint;
                __instance.grabRigidbody = __instance.actor.bodyHandeler.rightGrabRigidbody;
                __instance.grabTransform = __instance.actor.bodyHandeler.rightGrabTransform;
                __instance.side = MovementHandeler.Side.Right;
            }
        }
        else
        {
            __instance.punchTimer = __instance.leftPunchTimer;
            __instance.armActionTimer = __instance.leftArmActionTimer;
            __instance.punch = __instance.leftPunch;
            __instance.grab = __instance.leftGrab;
            __instance.grabJoint = __instance.actor.bodyHandeler.leftGrabJoint;
            __instance.grabRigidbody = __instance.actor.bodyHandeler.leftGrabRigidbody;
            __instance.grabTransform = __instance.actor.bodyHandeler.leftGrabTransform;
            __instance.side = MovementHandeler.Side.Left;
        }

        if ((!__instance.IsThisArmJustUp(ControlHandeler_Human.Arm.Right) &&
             !__instance.IsThisArmJustUp(ControlHandeler_Human.Arm.Left)) || !(__instance.armActionTimer > 0.2f) ||
            !__instance.grabRigidbody)
        {
            return;
        }

        Object.Destroy(__instance.grabJoint);
        __instance.grabJoint = null;
        __instance.grabRigidbody.AddForce(__instance.grabRigidbody.velocity * ThrowForce * 0.5f,
            ForceMode.Impulse);
    }
}