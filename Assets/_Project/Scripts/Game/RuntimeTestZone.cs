using UnityEngine;

namespace Platformer.Game
{
    [RequireComponent(typeof(Collider2D))]
    public class RuntimeTestZone : MonoBehaviour
    {
        [SerializeField] private RuntimeTestSequenceController _controller;
        [SerializeField] private RuntimeTestZoneKind _kind;
        [SerializeField] private int _testIndex;

        public RuntimeTestZoneKind Kind => _kind;
        public int TestIndex => _testIndex;

        private void Reset()
        {
            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || _controller == null)
                return;

            _controller.HandleZoneEntered(this, other);
        }
    }
}
