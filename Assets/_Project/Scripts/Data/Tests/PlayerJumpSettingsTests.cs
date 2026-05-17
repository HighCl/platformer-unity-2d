using NUnit.Framework;
using UnityEngine;

namespace Platformer.Data.Tests
{
    public class PlayerJumpSettingsTests
    {
        [Test]
        public void PlayerJumpSettings_DefaultMaxJumpCount_IsThree()
        {
            var settings = ScriptableObject.CreateInstance<PlayerJumpSettings>();

            Assert.AreEqual(3, settings.maxJumpCount);
            Object.DestroyImmediate(settings);
        }
    }
}
