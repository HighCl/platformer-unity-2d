using NUnit.Framework;
using UnityEngine;

namespace Platformer.Data.Tests
{
    public class PlayerInvincibilityEventTests
    {
        [Test]
        public void Raise_CallsRegisteredListenerWithInvincibilityValue()
        {
            var invincibilityEvent = ScriptableObject.CreateInstance<PlayerInvincibilityEvent>();
            bool? receivedValue = null;

            invincibilityEvent.AddListener(isInvincible => receivedValue = isInvincible);
            invincibilityEvent.Raise(true);

            Assert.IsTrue(receivedValue.HasValue);
            Assert.IsTrue(receivedValue.Value);
            Object.DestroyImmediate(invincibilityEvent);
        }

        [Test]
        public void RemoveListener_StopsReceivingRaise()
        {
            var invincibilityEvent = ScriptableObject.CreateInstance<PlayerInvincibilityEvent>();
            var callCount = 0;
            void Listener(bool isInvincible) => callCount++;

            invincibilityEvent.AddListener(Listener);
            invincibilityEvent.Raise(true);
            invincibilityEvent.RemoveListener(Listener);
            invincibilityEvent.Raise(false);

            Assert.AreEqual(1, callCount);
            Object.DestroyImmediate(invincibilityEvent);
        }
    }
}
