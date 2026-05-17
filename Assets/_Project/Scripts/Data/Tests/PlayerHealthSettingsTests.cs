using NUnit.Framework;
using UnityEngine;

namespace Platformer.Data.Tests
{
    public class PlayerHealthSettingsTests
    {
        [Test]
        public void CreateInstance_HasThreeHealthDefault()
        {
            var settings = ScriptableObject.CreateInstance<PlayerHealthSettings>();

            Assert.AreEqual(3, settings.maxHealth);
            Assert.AreEqual(0.75f, settings.invincibleDuration, 0.001f);
            Assert.AreEqual(0.1f, settings.blinkInterval, 0.001f);
            Assert.AreEqual(settings.invincibleDuration, settings.damageInvincibleDuration, 0.001f);

            Object.DestroyImmediate(settings);
        }
    }
}
