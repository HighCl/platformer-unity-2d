using UnityEngine;
using UnityEngine.Serialization;

namespace Platformer.Data
{
    [CreateAssetMenu(fileName = "PlayerHealthSettings", menuName = "Platformer/Settings/Player Health")]
    public class PlayerHealthSettings : ScriptableObject
    {
        [Tooltip("플레이어 최대 체력")]
        public int maxHealth = 3;

        [Tooltip("피격 후 추가 피해를 무시하는 시간(초)")]
        [FormerlySerializedAs("damageInvincibleDuration")]
        public float invincibleDuration = 0.75f;

        [Tooltip("무적 시간 중 플레이어 스프라이트가 깜빡이는 간격(초)")]
        public float blinkInterval = 0.1f;

        public float damageInvincibleDuration
        {
            get => invincibleDuration;
            set => invincibleDuration = value;
        }
    }
}
