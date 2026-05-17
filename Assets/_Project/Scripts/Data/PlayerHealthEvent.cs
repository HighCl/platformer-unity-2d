using UnityEngine;
using UnityEngine.Events;

namespace Platformer.Data
{
    [CreateAssetMenu(fileName = "PlayerHealthEvent", menuName = "Platformer/Events/Player Health Event")]
    public class PlayerHealthEvent : ScriptableObject
    {
        private event UnityAction<int, int> _onRaised;

        public void Raise(int currentHealth, int maxHealth) => _onRaised?.Invoke(currentHealth, maxHealth);

        public void AddListener(UnityAction<int, int> listener) => _onRaised += listener;
        public void RemoveListener(UnityAction<int, int> listener) => _onRaised -= listener;
    }
}
