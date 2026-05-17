using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using Platformer.Data;
using UnityEngine;

namespace Platformer.Core.Tests
{
    public class PlayerControllerHealthTests
    {
        [Test]
        public void PlayerController_Awake_InitializesHealthToThree()
        {
            var go = _CreatePlayerObject(out var player);

            Assert.AreEqual(3, player.MaxHealth);
            Assert.AreEqual(3, player.CurrentHealth);
            Assert.IsFalse(player.IsDead);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PlayerController_TakeDamageOne_ReducesHealthAndKeepsAlive()
        {
            var go = _CreatePlayerObject(out var player);

            player.TakeDamage(1);

            Assert.AreEqual(2, player.CurrentHealth);
            Assert.IsTrue(player.IsInvincible);
            Assert.IsFalse(player.IsDead);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PlayerController_TakeDamageOne_RaisesHealthChangedEvent()
        {
            var go = _CreatePlayerObject(out var player);
            var healthEvent = ScriptableObject.CreateInstance<PlayerHealthEvent>();
            var receivedCurrentValues = new List<int>();
            var receivedMaxValues = new List<int>();
            healthEvent.AddListener((currentHealth, maxHealth) =>
            {
                receivedCurrentValues.Add(currentHealth);
                receivedMaxValues.Add(maxHealth);
            });
            _SetPrivateField(player, "_onHealthChanged", healthEvent);

            player.TakeDamage(1);

            CollectionAssert.AreEqual(new[] { 2 }, receivedCurrentValues);
            CollectionAssert.AreEqual(new[] { 3 }, receivedMaxValues);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(healthEvent);
        }

        [Test]
        public void PlayerController_TakeDamageThree_KillsPlayer()
        {
            var go = _CreatePlayerObject(out var player);

            player.TakeDamage(3);

            Assert.AreEqual(0, player.CurrentHealth);
            Assert.IsFalse(player.IsInvincible);
            Assert.IsTrue(player.IsDead);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PlayerController_TakeDamageAgainDuringInvincible_IgnoresDamage()
        {
            var go = _CreatePlayerObject(out var player);

            player.TakeDamage(1);
            player.TakeDamage(1);

            Assert.AreEqual(2, player.CurrentHealth);
            Assert.IsTrue(player.IsInvincible);
            Assert.IsFalse(player.IsDead);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PlayerController_TakeDamage_RaisesInvincibilityStartEvent()
        {
            var go = _CreatePlayerObject(out var player);
            var invincibilityEvent = ScriptableObject.CreateInstance<PlayerInvincibilityEvent>();
            bool? receivedValue = null;
            invincibilityEvent.AddListener(isInvincible => receivedValue = isInvincible);
            _SetPrivateField(player, "_onInvincibilityChanged", invincibilityEvent);

            player.TakeDamage(1);

            Assert.IsTrue(receivedValue.HasValue);
            Assert.IsTrue(receivedValue.Value);
            Assert.IsTrue(player.IsInvincible);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(invincibilityEvent);
        }

        [Test]
        public void PlayerController_FallDeathDuringInvincible_RaisesInvincibilityEndEvent()
        {
            var go = _CreatePlayerObject(out var player);
            var invincibilityEvent = ScriptableObject.CreateInstance<PlayerInvincibilityEvent>();
            var receivedValues = new List<bool>();
            invincibilityEvent.AddListener(receivedValues.Add);
            _SetPrivateField(player, "_onInvincibilityChanged", invincibilityEvent);

            player.TakeDamage(1);
            go.transform.position = new Vector3(0f, -11f, 0f);
            player.TryApplyFallDeath();

            CollectionAssert.AreEqual(new[] { true, false }, receivedValues);
            Assert.IsFalse(player.IsInvincible);
            Assert.IsTrue(player.IsDead);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(invincibilityEvent);
        }

        [Test]
        public void PlayerController_TryApplyFallDeath_KillsRegardlessOfHealth()
        {
            var go = _CreatePlayerObject(out var player);
            go.transform.position = new Vector3(0f, -11f, 0f);

            var isFallDeathApplied = player.TryApplyFallDeath();

            Assert.IsTrue(isFallDeathApplied);
            Assert.AreEqual(0, player.CurrentHealth);
            Assert.IsTrue(player.IsDead);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PlayerController_TryApplyFallDeath_RaisesZeroHealthChangedEvent()
        {
            var go = _CreatePlayerObject(out var player);
            var healthEvent = ScriptableObject.CreateInstance<PlayerHealthEvent>();
            var receivedCurrentValues = new List<int>();
            var receivedMaxValues = new List<int>();
            healthEvent.AddListener((currentHealth, maxHealth) =>
            {
                receivedCurrentValues.Add(currentHealth);
                receivedMaxValues.Add(maxHealth);
            });
            _SetPrivateField(player, "_onHealthChanged", healthEvent);
            go.transform.position = new Vector3(0f, -11f, 0f);

            var isFallDeathApplied = player.TryApplyFallDeath();

            Assert.IsTrue(isFallDeathApplied);
            CollectionAssert.AreEqual(new[] { 0 }, receivedCurrentValues);
            CollectionAssert.AreEqual(new[] { 3 }, receivedMaxValues);
            Assert.AreEqual(0, player.CurrentHealth);
            Assert.IsTrue(player.IsDead);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(healthEvent);
        }

        private GameObject _CreatePlayerObject(out PlayerController player)
        {
            var go = new GameObject("Player");
            go.AddComponent<Rigidbody2D>();
            player = go.AddComponent<PlayerController>();
            return go;
        }

        private void _SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }
    }
}
