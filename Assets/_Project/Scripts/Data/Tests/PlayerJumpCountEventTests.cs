using NUnit.Framework;
using UnityEngine;

namespace Platformer.Data.Tests
{
    public class PlayerJumpCountEventTests
    {
        [Test]
        public void Raise_CallsRegisteredListenerWithJumpCountValues()
        {
            var jumpCountEvent = ScriptableObject.CreateInstance<PlayerJumpCountEvent>();
            var receivedRemainingJumpCount = 0;
            var receivedMaxJumpCount = 0;

            jumpCountEvent.AddListener((remainingJumpCount, maxJumpCount) =>
            {
                receivedRemainingJumpCount = remainingJumpCount;
                receivedMaxJumpCount = maxJumpCount;
            });
            jumpCountEvent.Raise(2, 3);

            Assert.AreEqual(2, receivedRemainingJumpCount);
            Assert.AreEqual(3, receivedMaxJumpCount);
            Object.DestroyImmediate(jumpCountEvent);
        }

        [Test]
        public void RemoveListener_StopsReceivingRaise()
        {
            var jumpCountEvent = ScriptableObject.CreateInstance<PlayerJumpCountEvent>();
            var callCount = 0;
            void Listener(int remainingJumpCount, int maxJumpCount) => callCount++;

            jumpCountEvent.AddListener(Listener);
            jumpCountEvent.Raise(2, 3);
            jumpCountEvent.RemoveListener(Listener);
            jumpCountEvent.Raise(1, 3);

            Assert.AreEqual(1, callCount);
            Object.DestroyImmediate(jumpCountEvent);
        }
    }
}
