using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Platformer.Data;

namespace Platformer.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        #region 상수
        private const int DEFAULT_MAX_HEALTH = 3;
        private const float DEFAULT_INVINCIBLE_DURATION = 0.75f;
        private const float DEFAULT_BLINK_INTERVAL = 0.1f;
        private const float MIN_BLINK_INTERVAL = 0.02f;
        #endregion

        #region 변수
        [SerializeField] private RespawnData _respawnData;
        [SerializeField] private PlayerHealthSettings _healthSettings;
        [SerializeField] private PlayerHealthEvent _onHealthChanged;
        [SerializeField] private PlayerInvincibilityEvent _onInvincibilityChanged;
        [SerializeField] private GameEvent _onPlayerDied;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private int _maxAirJumps = 1;
        [SerializeField] private float _fallGravityMultiplier = 2.5f;
        [SerializeField] private float _fallDeathThreshold = -10f;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private int _groundOverlapCapacity = 8;
        [SerializeField] private float _deathReloadDelay = 1f;
        [SerializeField] private string _deathTriggerName = AnimatorParams.DIE_TRIGGER;

        private Rigidbody2D _rb;
        private Animator _animator;
        private InputSystem_Actions _input;
        private PlayerState _state = PlayerState.Idle;
        private float _moveInput;
        private bool _isJumpRequested;
        private bool _isAirJumpPending;
        private int _airJumpsRemaining;
        private int _currentHealth;
        private float _defaultGravityScale;
        private bool _defaultSpriteRendererEnabled = true;
        private bool _isInvincible;
        private bool _isDead;
        private Coroutine _invincibilityCoroutine;
        private Collider2D[] _groundOverlapResults;
        #endregion

        #region 프로퍼티
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _healthSettings != null
            ? Mathf.Max(1, _healthSettings.maxHealth)
            : DEFAULT_MAX_HEALTH;
        public bool IsInvincible => _isInvincible;
        public bool IsDead => _isDead;

        private float InvincibleDuration => _healthSettings != null
            ? Mathf.Max(0f, _healthSettings.invincibleDuration)
            : DEFAULT_INVINCIBLE_DURATION;

        private float BlinkInterval => _healthSettings != null
            ? Mathf.Max(MIN_BLINK_INTERVAL, _healthSettings.blinkInterval)
            : DEFAULT_BLINK_INTERVAL;
        #endregion

        #region 유니티 라이프사이클
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _input = new InputSystem_Actions();
            _currentHealth = MaxHealth;
            _defaultGravityScale = _rb.gravityScale;
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            if (_spriteRenderer != null)
                _defaultSpriteRendererEnabled = _spriteRenderer.enabled;
            _groundOverlapResults = new Collider2D[Mathf.Max(1, _groundOverlapCapacity)];
        }

        void OnEnable()
        {
            if (_input == null)
                _input = new InputSystem_Actions();

            _input.Player.Enable();
        }

        void Start()
        {
            if (_respawnData != null && _respawnData.hasCheckpoint)
                transform.position = _respawnData.position;

            _RaiseHealthChanged();
            _RaiseInvincibilityChanged();
        }

        void Update()
        {
            if (_isDead)
                return;

            if (TryApplyFallDeath())
                return;

            var raw = _input.Player.Move.ReadValue<Vector2>();
            _moveInput = raw.x != 0f ? Mathf.Sign(raw.x) : 0f;

            if (_IsGrounded())
                _airJumpsRemaining = _maxAirJumps;

            if (_input.Player.Jump.WasPressedThisFrame())
            {
                if (_IsGrounded())
                {
                    _isJumpRequested = true;
                    _isAirJumpPending = false;
                }
                else if (_airJumpsRemaining > 0)
                {
                    _isJumpRequested = true;
                    _isAirJumpPending = true;
                }
            }

            _UpdateState();
            _UpdateFacing();
        }

        void FixedUpdate()
        {
            if (_isDead)
                return;

            var isGrounded = _IsGrounded();
            var platformVel = Vector2.zero;
            if (isGrounded && _TryGetMovingGroundVelocity(out var pv))
                platformVel = pv;

            if (_isJumpRequested)
            {
                _rb.linearVelocity = new Vector2(_moveInput * _moveSpeed + platformVel.x, 0f);
                _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                _isJumpRequested = false;
                if (_isAirJumpPending)
                {
                    _airJumpsRemaining--;
                    _isAirJumpPending = false;
                }
            }
            else if (isGrounded && platformVel.sqrMagnitude > 0f)
            {
                _rb.linearVelocity = new Vector2(_moveInput * _moveSpeed + platformVel.x, platformVel.y);
            }
            else
            {
                _rb.linearVelocity = new Vector2(_moveInput * _moveSpeed, _rb.linearVelocity.y);
            }

            _rb.gravityScale = _rb.linearVelocity.y < 0f
                ? _defaultGravityScale * _fallGravityMultiplier
                : _defaultGravityScale;
        }

        void OnDisable()
        {
            if (_input != null)
                _input.Player.Disable();

            _StopInvincibility();
        }
        #endregion

        #region Public 메서드
        public void TakeDamage(int amount)
        {
            if (_isDead || amount <= 0 || !_CanTakeDamage())
                return;

            var nextHealth = _currentHealth - amount;
            if (nextHealth <= 0)
            {
                _Die();
                return;
            }

            _SetCurrentHealth(nextHealth);
            _StartInvincibility();
        }

        public bool TryApplyFallDeath()
        {
            if (_isDead || transform.position.y >= _fallDeathThreshold)
                return false;

            // 낙사는 남은 체력과 무관하게 즉시 사망 처리한다.
            _Die();
            return true;
        }
        #endregion

        #region Private 메서드
        private bool _CanTakeDamage()
        {
            return !_isInvincible;
        }

        private void _SetCurrentHealth(int value, bool shouldForceRaise = false)
        {
            var clampedValue = Mathf.Clamp(value, 0, MaxHealth);
            if (_currentHealth == clampedValue && !shouldForceRaise)
                return;

            _currentHealth = clampedValue;
            _RaiseHealthChanged();
        }

        private void _RaiseHealthChanged()
        {
            if (_onHealthChanged != null)
                _onHealthChanged.Raise(_currentHealth, MaxHealth);
        }

        private void _RaiseInvincibilityChanged()
        {
            if (_onInvincibilityChanged != null)
                _onInvincibilityChanged.Raise(_isInvincible);
        }

        private void _StartInvincibility()
        {
            if (InvincibleDuration <= 0f)
                return;

            _SetInvincible(true);

            if (!Application.isPlaying || !isActiveAndEnabled)
                return;

            if (_invincibilityCoroutine != null)
                StopCoroutine(_invincibilityCoroutine);

            _invincibilityCoroutine = StartCoroutine(_CoInvincibility(InvincibleDuration));
        }

        private void _StopInvincibility()
        {
            if (_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
                _invincibilityCoroutine = null;
            }

            _RestoreSpriteRendererVisibility();
            _SetInvincible(false);
        }

        private IEnumerator _CoInvincibility(float duration)
        {
            var endTime = Time.time + duration;
            var waitTime = BlinkInterval;

            while (Time.time < endTime && !_isDead)
            {
                _SetSpriteRendererBlinkVisible(false);
                yield return new WaitForSeconds(waitTime);

                _SetSpriteRendererBlinkVisible(true);
                yield return new WaitForSeconds(waitTime);
            }

            _RestoreSpriteRendererVisibility();
            _SetInvincible(false);
            _invincibilityCoroutine = null;
        }

        private void _SetInvincible(bool isInvincible)
        {
            if (_isInvincible == isInvincible)
                return;

            _isInvincible = isInvincible;
            _RaiseInvincibilityChanged();
        }

        private void _SetSpriteRendererBlinkVisible(bool isVisible)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = _defaultSpriteRendererEnabled && isVisible;
        }

        private void _RestoreSpriteRendererVisibility()
        {
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = _defaultSpriteRendererEnabled;
        }

        private bool _IsGrounded()
        {
            if (_groundCheck == null)
                return false;
            return Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);
        }

        private bool _TryGetMovingGroundVelocity(out Vector2 velocity)
        {
            velocity = Vector2.zero;
            if (_groundCheck == null || _groundOverlapResults == null)
                return false;

            var n = Physics2D.OverlapCircleNonAlloc(
                _groundCheck.position,
                _groundCheckRadius,
                _groundOverlapResults,
                _groundLayer);

            for (var i = 0; i < n; i++)
            {
                var col = _groundOverlapResults[i];
                if (col == null)
                    continue;

                var rb = col.attachedRigidbody;
                if (rb == null)
                    rb = col.GetComponentInParent<Rigidbody2D>();
                if (rb == null)
                    continue;

                if (rb.TryGetComponent<IMovingGround>(out var moving))
                {
                    velocity = moving.GetPointVelocity(_groundCheck.position);
                    return true;
                }
            }

            return false;
        }

        private void _UpdateState()
        {
            if (!_IsGrounded())
            {
                _SetState(_rb.linearVelocity.y > 0f ? PlayerState.Jumping : PlayerState.Falling);
                return;
            }
            _SetState(Mathf.Abs(_moveInput) > 0.01f ? PlayerState.Running : PlayerState.Idle);
        }

        private void _SetState(PlayerState newState)
        {
            if (_state == newState) return;
            _state = newState;
            _UpdateAnimator();
        }

        private void _UpdateAnimator()
        {
            if (_animator == null) return;
            _animator.SetFloat(AnimatorParams.SPEED, Mathf.Abs(_moveInput));
            _animator.SetBool(AnimatorParams.IS_GROUNDED, _IsGrounded());
        }

        private void _UpdateFacing()
        {
            if (Mathf.Abs(_moveInput) < 0.01f) return;
            var scale = transform.localScale;
            scale.x = _moveInput > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        private void _Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            _StopInvincibility();
            _SetCurrentHealth(0, true);

            if (_onPlayerDied != null)
                _onPlayerDied.Raise();

            _moveInput = 0f;
            _isJumpRequested = false;
            _isAirJumpPending = false;
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0f;
            _rb.constraints = RigidbodyConstraints2D.FreezePositionX
                | RigidbodyConstraints2D.FreezePositionY
                | RigidbodyConstraints2D.FreezeRotation;
            _SetState(PlayerState.Dead);

            if (_animator != null)
            {
                _animator.SetBool(AnimatorParams.IS_GROUNDED, true);
                _animator.SetFloat(AnimatorParams.SPEED, 0f);

                if (_HasAnimatorParameter(_deathTriggerName, AnimatorControllerParameterType.Trigger))
                    _animator.SetTrigger(_deathTriggerName);

                var deathStateHash = Animator.StringToHash(AnimatorParams.DEATH_STATE);
                if (_animator.HasState(0, deathStateHash))
                    _animator.Play(deathStateHash, 0, 0f);
            }

            if (Application.isPlaying)
                Invoke(nameof(_ReloadCurrentScene), _deathReloadDelay);
        }

        private void _ReloadCurrentScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }

        private bool _HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
        {
            if (_animator == null || _animator.runtimeAnimatorController == null)
                return false;

            foreach (var parameter in _animator.parameters)
            {
                if (parameter.type == parameterType && parameter.name == parameterName)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
