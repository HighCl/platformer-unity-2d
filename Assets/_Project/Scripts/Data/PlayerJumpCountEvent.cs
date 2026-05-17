using UnityEngine;
using UnityEngine.Events;

namespace Platformer.Data
{
    [CreateAssetMenu(fileName = "PlayerJumpCountEvent", menuName = "Platformer/Events/Player Jump Count Event")]
    public class PlayerJumpCountEvent : ScriptableObject
    {
        private event UnityAction<int, int> _onRaised;

        public void Raise(int remainingJumpCount, int maxJumpCount) => _onRaised?.Invoke(remainingJumpCount, maxJumpCount);

        public void AddListener(UnityAction<int, int> listener) => _onRaised += listener;
        public void RemoveListener(UnityAction<int, int> listener) => _onRaised -= listener;
    }
}
