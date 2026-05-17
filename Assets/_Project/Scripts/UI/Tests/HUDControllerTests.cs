using NUnit.Framework;
using System.Reflection;
using Platformer.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI.Tests
{
    public class HUDControllerTests
    {
        private GameObject _go;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("HUDController");
            _go.AddComponent<HUDController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void HUDController_CanBeCreated()
        {
            Assert.IsNotNull(_go.GetComponent<HUDController>());
        }

        [Test]
        public void HUDController_EnableDisable_DoesNotThrow()
        {
            var hud = _go.GetComponent<HUDController>();
            Assert.DoesNotThrow(() =>
            {
                hud.enabled = false;
                hud.enabled = true;
            });
        }

        [Test]
        public void RefreshHealthGauge_UpdatesCachedFillAmount()
        {
            var hud = _go.GetComponent<HUDController>();

            hud.RefreshHealthGauge(2, 3);

            Assert.AreEqual(2f / 3f, hud.CurrentHealthFillAmount, 0.001f);
        }

        [Test]
        public void RefreshHealthGauge_UpdatesSliderValueAndDisablesClick()
        {
            var slider = _CreateHealthSlider(out var sliderGo, out var targetGraphic, out var fillGraphic);
            var hud = _go.GetComponent<HUDController>();
            _SetPrivateField(hud, "_healthGaugeSlider", slider);

            hud.RefreshHealthGauge(1, 3);

            Assert.AreEqual(1f / 3f, slider.value, 0.001f);
            Assert.AreEqual(0f, slider.minValue, 0.001f);
            Assert.AreEqual(1f, slider.maxValue, 0.001f);
            Assert.IsFalse(slider.interactable);
            Assert.AreEqual(Selectable.Transition.None, slider.transition);
            Assert.AreEqual(Navigation.Mode.None, slider.navigation.mode);
            Assert.IsFalse(targetGraphic.raycastTarget);
            Assert.IsFalse(fillGraphic.raycastTarget);

            Object.DestroyImmediate(sliderGo);
        }

        [Test]
        public void RefreshHealthGauge_ZeroMaxHealth_UsesSafeMaxValue()
        {
            var hud = _go.GetComponent<HUDController>();

            hud.RefreshHealthGauge(1, 0);

            Assert.AreEqual(1f, hud.CurrentHealthFillAmount);
        }

        [Test]
        public void RefreshHealthGauge_HealthThree_FillsThreeSlots()
        {
            var hud = _go.GetComponent<HUDController>();
            var slider = _CreateHealthSlider(out var sliderGo, out _, out _);
            var slotImgs = _CreateHealthSlots();
            var filledColor = new Color(1f, 0.45f, 0.45f, 1f);
            var emptyColor = new Color(0.45f, 0.45f, 0.45f, 1f);
            _SetPrivateField(hud, "_healthGaugeSlider", slider);
            _SetPrivateField(hud, "_healthSlotImgs", slotImgs);
            _SetPrivateField(hud, "_healthFilledColor", filledColor);
            _SetPrivateField(hud, "_healthEmptyColor", emptyColor);

            hud.RefreshHealthGauge(3, 3);

            Assert.AreEqual(1f, slider.value, 0.001f);
            Assert.AreEqual(filledColor, slotImgs[0].color);
            Assert.AreEqual(filledColor, slotImgs[1].color);
            Assert.AreEqual(filledColor, slotImgs[2].color);

            _DestroyHealthSlots(slotImgs);
            Object.DestroyImmediate(sliderGo);
        }

        [Test]
        public void RefreshHealthGauge_HealthTwo_FillsTwoSlotsAndEmptiesOne()
        {
            var hud = _go.GetComponent<HUDController>();
            var slider = _CreateHealthSlider(out var sliderGo, out _, out _);
            var slotImgs = _CreateHealthSlots();
            var filledColor = new Color(1f, 0.45f, 0.45f, 1f);
            var emptyColor = new Color(0.45f, 0.45f, 0.45f, 1f);
            _SetPrivateField(hud, "_healthGaugeSlider", slider);
            _SetPrivateField(hud, "_healthSlotImgs", slotImgs);
            _SetPrivateField(hud, "_healthFilledColor", filledColor);
            _SetPrivateField(hud, "_healthEmptyColor", emptyColor);

            hud.RefreshHealthGauge(2, 3);

            Assert.AreEqual(2f / 3f, slider.value, 0.001f);
            Assert.AreEqual(filledColor, slotImgs[0].color);
            Assert.AreEqual(filledColor, slotImgs[1].color);
            Assert.AreEqual(emptyColor, slotImgs[2].color);

            _DestroyHealthSlots(slotImgs);
            Object.DestroyImmediate(sliderGo);
        }

        [Test]
        public void RefreshHealthGauge_HealthZero_EmptiesAllSlots()
        {
            var hud = _go.GetComponent<HUDController>();
            var slider = _CreateHealthSlider(out var sliderGo, out _, out _);
            var slotImgs = _CreateHealthSlots();
            var filledColor = new Color(1f, 0.45f, 0.45f, 1f);
            var emptyColor = new Color(0.45f, 0.45f, 0.45f, 1f);
            _SetPrivateField(hud, "_healthGaugeSlider", slider);
            _SetPrivateField(hud, "_healthSlotImgs", slotImgs);
            _SetPrivateField(hud, "_healthFilledColor", filledColor);
            _SetPrivateField(hud, "_healthEmptyColor", emptyColor);

            hud.RefreshHealthGauge(0, 3);

            Assert.AreEqual(0f, slider.value, 0.001f);
            Assert.AreEqual(emptyColor, slotImgs[0].color);
            Assert.AreEqual(emptyColor, slotImgs[1].color);
            Assert.AreEqual(emptyColor, slotImgs[2].color);

            _DestroyHealthSlots(slotImgs);
            Object.DestroyImmediate(sliderGo);
        }

        [Test]
        public void HealthChangedEvent_HealthThreeTwoZero_UpdatesSliderAndSlots()
        {
            var hud = _go.GetComponent<HUDController>();
            var healthEvent = ScriptableObject.CreateInstance<PlayerHealthEvent>();
            var slider = _CreateHealthSlider(out var sliderGo, out _, out _);
            var slotImgs = _CreateHealthSlots();
            var filledColor = new Color(1f, 0.45f, 0.45f, 1f);
            var emptyColor = new Color(0.45f, 0.45f, 0.45f, 1f);
            _SetPrivateField(hud, "_onHealthChanged", healthEvent);
            _SetPrivateField(hud, "_healthGaugeSlider", slider);
            _SetPrivateField(hud, "_healthSlotImgs", slotImgs);
            _SetPrivateField(hud, "_healthFilledColor", filledColor);
            _SetPrivateField(hud, "_healthEmptyColor", emptyColor);

            hud.enabled = false;
            hud.enabled = true;

            healthEvent.Raise(3, 3);
            _AssertHealthView(slider, slotImgs, filledColor, emptyColor, 1f, 3);

            healthEvent.Raise(2, 3);
            _AssertHealthView(slider, slotImgs, filledColor, emptyColor, 2f / 3f, 2);

            healthEvent.Raise(0, 3);
            _AssertHealthView(slider, slotImgs, filledColor, emptyColor, 0f, 0);

            hud.enabled = false;
            _DestroyHealthSlots(slotImgs);
            Object.DestroyImmediate(sliderGo);
            Object.DestroyImmediate(healthEvent);
        }

        private void _SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(target, value);
        }

        private Slider _CreateHealthSlider(out GameObject sliderGo, out Image targetGraphic, out Image fillGraphic)
        {
            sliderGo = new GameObject("Pan_HealthGauge");
            targetGraphic = sliderGo.AddComponent<Image>();
            targetGraphic.raycastTarget = true;

            var fillGo = new GameObject("Img_HealthGaugeFill");
            fillGo.transform.SetParent(sliderGo.transform, false);
            fillGraphic = fillGo.AddComponent<Image>();
            fillGraphic.raycastTarget = true;

            var slider = sliderGo.AddComponent<Slider>();
            slider.targetGraphic = targetGraphic;
            slider.fillRect = fillGraphic.rectTransform;
            slider.interactable = true;
            slider.transition = Selectable.Transition.ColorTint;
            slider.navigation = new Navigation { mode = Navigation.Mode.Automatic };

            return slider;
        }

        private Image[] _CreateHealthSlots()
        {
            return new[]
            {
                new GameObject("Img_HealthSlot_01").AddComponent<Image>(),
                new GameObject("Img_HealthSlot_02").AddComponent<Image>(),
                new GameObject("Img_HealthSlot_03").AddComponent<Image>()
            };
        }

        private void _DestroyHealthSlots(Image[] slotImgs)
        {
            foreach (var slotImg in slotImgs)
                Object.DestroyImmediate(slotImg.gameObject);
        }

        private void _AssertHealthView(
            Slider slider,
            Image[] slotImgs,
            Color filledColor,
            Color emptyColor,
            float expectedFillAmount,
            int expectedFilledSlotCount)
        {
            Assert.AreEqual(expectedFillAmount, slider.value, 0.001f);

            for (var i = 0; i < slotImgs.Length; i++)
            {
                var expectedColor = i < expectedFilledSlotCount
                    ? filledColor
                    : emptyColor;
                Assert.AreEqual(expectedColor, slotImgs[i].color);
            }
        }
    }
}
