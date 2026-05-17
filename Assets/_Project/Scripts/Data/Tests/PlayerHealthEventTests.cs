using NUnit.Framework;
using UnityEngine;

namespace Platformer.Data.Tests
{
    public class PlayerHealthEventTests
    {
        [Test]
        public void Raise_CallsRegisteredListenerWithHealthValues()
        {
            var healthEvent = ScriptableObject.CreateInstance<PlayerHealthEvent>();
            var receivedCurrentHealth = 0;
            var receivedMaxHealth = 0;

            healthEvent.AddListener((currentHealth, maxHealth) =>
            {
                receivedCurrentHealth = currentHealth;
                receivedMaxHealth = maxHealth;
            });
            healthEvent.Raise(2, 3);

            Assert.AreEqual(2, receivedCurrentHealth);
            Assert.AreEqual(3, receivedMaxHealth);
            Object.DestroyImmediate(healthEvent);
        }

        [Test]
        public void RemoveListener_StopsReceivingRaise()
        {
            var healthEvent = ScriptableObject.CreateInstance<PlayerHealthEvent>();
            var callCount = 0;
            void Listener(int currentHealth, int maxHealth) => callCount++;

            healthEvent.AddListener(Listener);
            healthEvent.Raise(2, 3);
            healthEvent.RemoveListener(Listener);
            healthEvent.Raise(1, 3);

            Assert.AreEqual(1, callCount);
            Object.DestroyImmediate(healthEvent);
        }
    }
}
