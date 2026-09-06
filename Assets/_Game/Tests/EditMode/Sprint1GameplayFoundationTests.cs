using NUnit.Framework;
using UnityEditor;
using UnityEngine.InputSystem;

namespace TilkiOyunu.Foundation.Tests
{
    public sealed class Sprint1GameplayFoundationTests
    {
        [Test]
        public void MovementSettingsSanitizeKeepsPlayableValues()
        {
            FoxMovementSettings settings = new()
            {
                moveSpeed = -1f,
                sprintSpeed = 0f,
                rotationSpeed = 0f,
                jumpHeight = 0f,
                gravity = 4f,
                airControl = 3f
            };

            settings.Sanitize();

            Assert.That(settings.moveSpeed, Is.GreaterThan(0f));
            Assert.That(settings.sprintSpeed, Is.GreaterThanOrEqualTo(settings.moveSpeed));
            Assert.That(settings.rotationSpeed, Is.GreaterThan(0f));
            Assert.That(settings.jumpHeight, Is.GreaterThan(0f));
            Assert.That(settings.gravity, Is.LessThan(0f));
            Assert.That(settings.airControl, Is.InRange(0f, 1f));
        }

        [Test]
        public void InputActionIdsMatchSprintOneInputAsset()
        {
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/_Game/Settings/TilkiInputActions.inputactions");
            Assert.That(inputActions, Is.Not.Null);

            InputActionMap player = inputActions.FindActionMap(InputActionIds.MapPlayer);

            Assert.That(player.FindAction(InputActionIds.Move), Is.Not.Null);
            Assert.That(player.FindAction(InputActionIds.Look), Is.Not.Null);
            Assert.That(player.FindAction(InputActionIds.Jump), Is.Not.Null);
            Assert.That(player.FindAction(InputActionIds.Sprint), Is.Not.Null);
            Assert.That(player.FindAction(InputActionIds.Interact), Is.Not.Null);
            Assert.That(player.FindAction(InputActionIds.Pause), Is.Not.Null);
        }
    }
}
