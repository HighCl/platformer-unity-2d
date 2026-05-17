using System.Collections;
using Platformer.Core;
using Platformer.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer.Game
{
    public class RuntimeTestSequenceController : MonoBehaviour
    {
        #region 상수
        private const string PLAYER_PREFS_KEY_PREFIX = "Platformer.RuntimeTest.CurrentIndex.";
        private const int MOVE_TEST_INDEX = 0;
        private const int DEATH_TEST_INDEX = 1;
        private const int CHECKPOINT_TEST_INDEX = 2;
        private const int TEST_COUNT = 3;
        private const int LETHAL_DAMAGE = 999;
        private const float CHECKPOINT_WAIT_SECONDS = 0.05f;
        private const float CHECKPOINT_TOLERANCE = 0.25f;
        #endregion

        #region 변수
        [SerializeField] private Transform _player;
        [SerializeField] private RespawnData _respawnData;
        [SerializeField] private Vector2[] _spawnPositions =
        {
            new Vector2(0f, 1f),
            new Vector2(0f, -9f),
            new Vector2(0f, -29f)
        };
        [SerializeField] private string[] _testNames =
        {
            "1번 이동 테스트",
            "2번 사망 테스트",
            "3번 세이브 테스트"
        };

        private int _currentTestIndex;
        private string _statusText = "테스트 준비";
        private bool _isCheckpointCheckRunning;
        #endregion

        #region 프로퍼티
        public int CurrentTestIndex => _currentTestIndex;
        public string StatusText => _statusText;
        private string SaveKey => PLAYER_PREFS_KEY_PREFIX + SceneManager.GetActiveScene().name;
        #endregion

        #region 유니티 라이프사이클
        private void Start()
        {
            _currentTestIndex = Mathf.Clamp(PlayerPrefs.GetInt(SaveKey, 0), 0, TEST_COUNT - 1);
            _MovePlayerToCurrentSpawn();
            _SetStatus($"{_GetCurrentTestName()} 시작");
        }

        private void OnGUI()
        {
            var rect = new Rect(16f, 16f, 520f, 112f);
            GUI.Box(rect, string.Empty);
            GUI.Label(new Rect(32f, 28f, 480f, 24f), $"현재 테스트: {_GetCurrentTestName()}");
            GUI.Label(new Rect(32f, 56f, 480f, 24f), $"상태: {_statusText}");
            GUI.Label(new Rect(32f, 84f, 480f, 24f), "R 키: 테스트 진행도 초기화 후 첫 위치로 이동");

            if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.R)
                ResetProgress();
        }
        #endregion

        #region Public 메서드
        public void HandleZoneEntered(RuntimeTestZone zone, Collider2D playerCollider)
        {
            if (zone == null || zone.TestIndex != _currentTestIndex)
                return;

            switch (zone.Kind)
            {
                case RuntimeTestZoneKind.Start:
                    _SetStatus($"{_GetCurrentTestName()} 구역 진입");
                    break;
                case RuntimeTestZoneKind.Pass:
                    _HandlePassZone();
                    break;
                case RuntimeTestZoneKind.Damage:
                    _HandleDamageZone(playerCollider);
                    break;
                case RuntimeTestZoneKind.Checkpoint:
                    _HandleCheckpointZone(zone);
                    break;
            }
        }

        public void ResetProgress()
        {
            PlayerPrefs.SetInt(SaveKey, 0);
            PlayerPrefs.Save();
            _currentTestIndex = 0;

            if (_respawnData != null)
                _respawnData.Reset();

            _MovePlayerToCurrentSpawn();
            _SetStatus("테스트 진행도 초기화");
        }
        #endregion

        #region Private 메서드
        private void _HandlePassZone()
        {
            if (_currentTestIndex != MOVE_TEST_INDEX)
                return;

            _CompleteCurrentTest("이동 목표 지점 도달");
        }

        private void _HandleDamageZone(Collider2D playerCollider)
        {
            if (_currentTestIndex != DEATH_TEST_INDEX)
                return;

            if (!playerCollider.TryGetComponent<PlayerController>(out var player))
            {
                _SetStatus("PlayerController를 찾지 못해 사망 테스트 실패");
                return;
            }

            PlayerPrefs.SetInt(SaveKey, CHECKPOINT_TEST_INDEX);
            PlayerPrefs.Save();
            _SetStatus("사망 트리거 진입, 씬 재시작 후 3번 테스트로 이동");
            player.TakeDamage(LETHAL_DAMAGE);
        }

        private void _HandleCheckpointZone(RuntimeTestZone zone)
        {
            if (_currentTestIndex != CHECKPOINT_TEST_INDEX || _isCheckpointCheckRunning)
                return;

            StartCoroutine(_CoVerifyCheckpoint(zone.transform.position));
        }

        private IEnumerator _CoVerifyCheckpoint(Vector2 expectedPosition)
        {
            _isCheckpointCheckRunning = true;
            yield return new WaitForSeconds(CHECKPOINT_WAIT_SECONDS);

            if (_respawnData == null)
            {
                _SetStatus("RespawnData가 연결되지 않아 세이브 테스트 실패");
                _isCheckpointCheckRunning = false;
                yield break;
            }

            var distance = Vector2.Distance(_respawnData.position, expectedPosition);
            if (_respawnData.hasCheckpoint && distance <= CHECKPOINT_TOLERANCE)
                _CompleteCurrentTest("체크포인트 저장 확인");
            else
                _SetStatus("체크포인트 저장값 불일치");

            _isCheckpointCheckRunning = false;
        }

        private void _CompleteCurrentTest(string reason)
        {
            _SetStatus($"{_GetCurrentTestName()} 통과: {reason}");

            var nextIndex = Mathf.Min(_currentTestIndex + 1, TEST_COUNT - 1);
            PlayerPrefs.SetInt(SaveKey, nextIndex);
            PlayerPrefs.Save();

            if (_currentTestIndex == CHECKPOINT_TEST_INDEX)
                return;

            _currentTestIndex = nextIndex;
            _MovePlayerToCurrentSpawn();
            _SetStatus($"{_GetCurrentTestName()} 시작");
        }

        private void _MovePlayerToCurrentSpawn()
        {
            if (_player == null || _spawnPositions == null || _spawnPositions.Length == 0)
                return;

            var index = Mathf.Clamp(_currentTestIndex, 0, _spawnPositions.Length - 1);
            _player.position = _spawnPositions[index];

            if (_player.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        private string _GetCurrentTestName()
        {
            if (_testNames == null || _currentTestIndex < 0 || _currentTestIndex >= _testNames.Length)
                return $"{_currentTestIndex + 1}번 테스트";

            return _testNames[_currentTestIndex];
        }

        private void _SetStatus(string statusText)
        {
            _statusText = statusText;
#if UNITY_EDITOR
            Debug.Log($"[RuntimeTestSequenceController] {_statusText}");
#endif
        }
        #endregion
    }
}
