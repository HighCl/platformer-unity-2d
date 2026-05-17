using UnityEngine;
using UnityEngine.UI;
using Platformer.Data;

namespace Platformer.UI
{
    [RequireComponent(typeof(Image))]
    public class InvincibilityIconView : MonoBehaviour
    {
        [SerializeField] private PlayerInvincibilityEvent _onInvincibilityChanged;
        [SerializeField] private Image _iconImg;
        [SerializeField] private Color _inactiveColor = new Color(0.2f, 0.85f, 1f, 0.25f);
        [SerializeField] private Color _activeColor = new Color(0.2f, 0.85f, 1f, 1f);

        private bool _isInvincible;

        public float CurrentAlpha => _iconImg != null ? _iconImg.color.a : _inactiveColor.a;
        public bool IsInvincible => _isInvincible;

        void Awake()
        {
            if (_iconImg == null)
                _iconImg = GetComponent<Image>();
        }

        void OnEnable()
        {
            if (_onInvincibilityChanged != null)
                _onInvincibilityChanged.AddListener(_HandleInvincibilityChanged);
        }

        void Start()
        {
            ApplyInvincibleState(_isInvincible);
        }

        void OnDisable()
        {
            if (_onInvincibilityChanged != null)
                _onInvincibilityChanged.RemoveListener(_HandleInvincibilityChanged);
        }

        public void ApplyInvincibleState(bool isInvincible)
        {
            _isInvincible = isInvincible;

            if (_iconImg == null)
                return;

            _iconImg.color = _isInvincible ? _activeColor : _inactiveColor;
        }

        private void _HandleInvincibilityChanged(bool isInvincible)
        {
            ApplyInvincibleState(isInvincible);
        }
    }
}
