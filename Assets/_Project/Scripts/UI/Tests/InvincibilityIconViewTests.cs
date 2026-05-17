using NUnit.Framework;
using System.Reflection;
using Platformer.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI.Tests
{
    public class InvincibilityIconViewTests
    {
        private GameObject _go;
        private Image _iconImg;
        private InvincibilityIconView _view;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("Img_InvincibleIcon");
            _iconImg = _go.AddComponent<Image>();
            _view = _go.AddComponent<InvincibilityIconView>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void ApplyInvincibleState_True_UsesActiveColor()
        {
            var activeColor = new Color(0.2f, 0.85f, 1f, 1f);
            _SetPrivateField(_view, "_activeColor", activeColor);

            _view.ApplyInvincibleState(true);

            Assert.IsTrue(_view.IsInvincible);
            Assert.AreEqual(activeColor, _iconImg.color);
            Assert.AreEqual(1f, _view.CurrentAlpha, 0.001f);
        }

        [Test]
        public void ApplyInvincibleState_False_UsesInactiveColor()
        {
            var inactiveColor = new Color(0.2f, 0.85f, 1f, 0.25f);
            _SetPrivateField(_view, "_inactiveColor", inactiveColor);

            _view.ApplyInvincibleState(false);

            Assert.IsFalse(_view.IsInvincible);
            Assert.AreEqual(inactiveColor, _iconImg.color);
            Assert.AreEqual(0.25f, _view.CurrentAlpha, 0.001f);
        }

        [Test]
        public void InvincibilityEvent_Raise_UpdatesIconState()
        {
            var invincibilityEvent = ScriptableObject.CreateInstance<PlayerInvincibilityEvent>();
            _SetPrivateField(_view, "_onInvincibilityChanged", invincibilityEvent);

            _view.enabled = false;
            _view.enabled = true;
            invincibilityEvent.Raise(true);

            Assert.IsTrue(_view.IsInvincible);
            Assert.AreEqual(1f, _iconImg.color.a, 0.001f);

            Object.DestroyImmediate(invincibilityEvent);
        }

        private void _SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }
    }
}
