using HarmonyLib;
using Il2CppFemur;
using UnityEngine;

namespace PowerLift;

[HarmonyPatch(typeof(MovementHandeler_HumanoidMediumEctomorph), nameof(MovementHandeler_HumanoidMediumEctomorph.ArmActionGrabbing))]
public static class GrabPatch
{
    private static void Postfix(MovementHandeler_HumanoidMediumEctomorph __instance, MovementHandeler.Side side)
    {
        ConfigurableJoint configurableJoint = null;
        Rigidbody rigidbody = null;
        Transform partTransform = __instance.actor.bodyHandeler.Chest.PartTransform;
        Transform transform = null;
        Transform transform2 = null;
        Transform transform3 = null;
        Rigidbody rigidbody2 = null;
        Rigidbody rigidbody3 = null;
        Rigidbody rigidbody4 = null;
        if (side != MovementHandeler.Side.Left)
        {
            if (side != MovementHandeler.Side.Right)
            {
                return;
            }

            transform = __instance.actor.bodyHandeler.RightArm.PartTransform;
            transform2 = __instance.actor.bodyHandeler.RightForarm.PartTransform;
            transform3 = __instance.actor.bodyHandeler.RightHand.PartTransform;
            rigidbody2 = __instance.actor.bodyHandeler.RightArm.PartRigidbody;
            rigidbody3 = __instance.actor.bodyHandeler.RightForarm.PartRigidbody;
            rigidbody4 = __instance.actor.bodyHandeler.RightHand.PartRigidbody;
            configurableJoint = __instance.actor.bodyHandeler.rightGrabJoint;
            rigidbody = __instance.actor.bodyHandeler.rightGrabRigidbody;
            Transform rightGrabTransform = __instance.actor.bodyHandeler.rightGrabTransform;
        }
        else
        {
            transform = __instance.actor.bodyHandeler.LeftArm.PartTransform;
            transform2 = __instance.actor.bodyHandeler.LeftForarm.PartTransform;
            transform3 = __instance.actor.bodyHandeler.LeftHand.PartTransform;
            rigidbody2 = __instance.actor.bodyHandeler.LeftArm.PartRigidbody;
            rigidbody3 = __instance.actor.bodyHandeler.LeftForarm.PartRigidbody;
            rigidbody4 = __instance.actor.bodyHandeler.LeftHand.PartRigidbody;
            configurableJoint = __instance.actor.bodyHandeler.leftGrabJoint;
            rigidbody = __instance.actor.bodyHandeler.leftGrabRigidbody;
            Transform leftGrabTransform = __instance.actor.bodyHandeler.leftGrabTransform;
        }

        if (!__instance.actor.controlHandeler.lift || !configurableJoint || !rigidbody)
        {
            return;
        }

        float num = Mathf.Clamp(Mathf.Sin(rigidbody.mass / 800f) * 200f, 0f, Mod.LiftStrength);
        __instance.AlignToVector(rigidbody2, -transform.up, Vector3.up + partTransform.forward * 0.8f, 0.1f, Mathf.Approximately(Mod.LiftStrength, 25) ? Mod.LiftStrength : Mod.LiftStrength / 4);
        __instance.AlignToVector(rigidbody3, -transform2.up, Vector3.up + partTransform.forward * 0.8f, 0.1f, Mathf.Approximately(Mod.LiftStrength, 25) ? Mod.LiftStrength : Mod.LiftStrength / 4);
        if (!__instance.actor.controlHandeler.onGround)
        {
            return;
        }
        
        rigidbody.AddForce(Vector3.up * num, ForceMode.Impulse);
        rigidbody2.AddForce(Vector3.down * num, ForceMode.Impulse);
    }
}