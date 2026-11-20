using StarterAssets;
using UnityEngine;

/* Why it broke: it expects the StarterAsset script to be in: StarterAssets/Runtime/InputSystem. It has been moved to Assets/Script for referencing ease.
namespace StarterAssets
{
    public class VirtualInput : MonoBehaviour
    {
        [Header("Output")]
        public StarterAssetsInputs StarterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            StarterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            StarterAssetsInputs.lookFromTouch = true; 
            StarterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            StarterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            StarterAssetsInputs.SprintInput(virtualSprintState);
        }
    }
} */
