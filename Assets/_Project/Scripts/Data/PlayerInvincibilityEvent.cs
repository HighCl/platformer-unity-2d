using UnityEngine;
using UnityEngine.Events;

namespace Platformer.Data
{
    [CreateAssetMenu(fileName = "PlayerInvincibilityEvent", menuName = "Platformer/Events/Player Invincibility Event")]
    public class PlayerInvincibilityEvent : ScriptableObject
    {
        private event UnityAction<bool> _onRaised;

        public void Raise(bool isInvincible) => _onRaised?.Invoke(isInvincible);

        public void AddListener(UnityAction<bool> listener) => _onRaised += listener;
        public void RemoveListener(UnityAction<bool> listener) => _onRaised -= listener;
    }
}
