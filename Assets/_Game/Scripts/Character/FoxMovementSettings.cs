using System;
using UnityEngine;

namespace TilkiOyunu.Foundation
{
    [Serializable]
    public sealed class FoxMovementSettings
    {
        [Min(0.1f)] public float moveSpeed = 3.8f;
        [Min(0.1f)] public float sprintSpeed = 6.2f;
        [Min(1f)] public float rotationSpeed = 14f;
        [Min(0.1f)] public float jumpHeight = 1.05f;
        [Min(0.1f)] public float airControl = 0.45f;
        [Min(0.01f)] public float acceleration = 18f;
        [Min(0.01f)] public float deceleration = 22f;
        [Min(0.01f)] public float groundedProbeRadius = 0.18f;
        public float gravity = -24f;
        public float groundedStickVelocity = -2f;

        public void Sanitize()
        {
            moveSpeed = Mathf.Max(0.1f, moveSpeed);
            sprintSpeed = Mathf.Max(moveSpeed, sprintSpeed);
            rotationSpeed = Mathf.Max(1f, rotationSpeed);
            jumpHeight = Mathf.Max(0.1f, jumpHeight);
            airControl = Mathf.Clamp01(airControl);
            acceleration = Mathf.Max(0.01f, acceleration);
            deceleration = Mathf.Max(0.01f, deceleration);
            groundedProbeRadius = Mathf.Max(0.01f, groundedProbeRadius);
            gravity = gravity >= -0.01f ? -24f : gravity;
            groundedStickVelocity = Mathf.Min(-0.01f, groundedStickVelocity);
        }
    }
}
