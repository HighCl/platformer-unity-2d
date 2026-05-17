using UnityEngine;
using UnityEngine.UI;
using Platformer.Data;

namespace Platformer.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private GameEvent _onPlayerDied;
        [SerializeField] private PlayerHealthEvent _onHealthChanged;
        [SerializeField] private Slider _healthGaugeSlider;
        [SerializeField] private Image _healthGaugeFillImg;
        [SerializeField] private Image[] _healthSlotImgs;
        [SerializeField] private Color _healthFilledColor = new Color(1f, 0.45f, 0.45f, 1f);
        [SerializeField] private Color _healthEmptyColor = new Color(0.45f, 0.45f, 0.45f, 1f);

        private float _currentHealthFillAmount = 1f;
        private int _currentHealth = 3;
        private int _maxHealth = 3;

        public float CurrentHealthFillAmount => _currentHealthFillAmount;

        void Awake()
        {
            _ConfigureHealthSlider();
        }

        void OnEnable()
        {
            if (_onPlayerDied != null)
                _onPlayerDied.AddListener(_HandlePlayerDied);

            if (_onHealthChanged != null)
                _onHealthChanged.AddListener(_HandleHealthChanged);
        }

        void Start()
        {
            _ApplyHealthView(_currentHealth, _maxHealth);
        }

        void OnDisable()
        {
            if (_onHealthChanged != null)
                _onHealthChanged.RemoveListener(_HandleHealthChanged);

            if (_onPlayerDied != null)
                _onPlayerDied.RemoveListener(_HandlePlayerDied);
        }

        public void RefreshHealthGauge(int currentHealth, int maxHealth)
        {
            var safeMaxHealth = Mathf.Max(1, maxHealth);
            _currentHealth = Mathf.Clamp(currentHealth, 0, safeMaxHealth);
            _maxHealth = safeMaxHealth;

            _ApplyHealthFillAmount((float)_currentHealth / _maxHealth);
            _ApplyHealthSlots(_currentHealth);
        }

        private void _HandlePlayerDied()
        {
            _ApplyHealthView(0, _maxHealth);
        }

        private void _HandleHealthChanged(int currentHealth, int maxHealth)
        {
            RefreshHealthGauge(currentHealth, maxHealth);
        }

        private void _ApplyHealthFillAmount(float fillAmount)
        {
            _currentHealthFillAmount = Mathf.Clamp01(fillAmount);

            if (_healthGaugeSlider != null)
            {
                _ConfigureHealthSlider();
                _healthGaugeSlider.SetValueWithoutNotify(_currentHealthFillAmount);
            }

            if (_healthGaugeFillImg != null)
                _healthGaugeFillImg.fillAmount = _currentHealthFillAmount;
        }

        private void _ApplyHealthView(int currentHealth, int maxHealth)
        {
            RefreshHealthGauge(currentHealth, maxHealth);
        }

        private void _ApplyHealthSlots(int currentHealth)
        {
            if (_healthSlotImgs == null)
                return;

            for (var i = 0; i < _healthSlotImgs.Length; i++)
            {
                var healthSlotImg = _healthSlotImgs[i];
                if (healthSlotImg == null)
                    continue;

                healthSlotImg.color = i < currentHealth
                    ? _healthFilledColor
                    : _healthEmptyColor;
            }
        }

        private void _ConfigureHealthSlider()
        {
            if (_healthGaugeSlider == null)
                return;

            _healthGaugeSlider.minValue = 0f;
            _healthGaugeSlider.maxValue = 1f;
            _healthGaugeSlider.wholeNumbers = false;
            _healthGaugeSlider.interactable = false;
            _healthGaugeSlider.transition = Selectable.Transition.None;
            _healthGaugeSlider.navigation = new Navigation { mode = Navigation.Mode.None };

            _DisableSliderRaycast(_healthGaugeSlider.targetGraphic);

            if (_healthGaugeSlider.fillRect != null)
                _DisableSliderRaycast(_healthGaugeSlider.fillRect.GetComponent<Graphic>());

            if (_healthGaugeSlider.handleRect != null)
                _DisableSliderRaycast(_healthGaugeSlider.handleRect.GetComponent<Graphic>());
        }

        private void _DisableSliderRaycast(Graphic graphic)
        {
            if (graphic != null)
                graphic.raycastTarget = false;
        }
    }
}
