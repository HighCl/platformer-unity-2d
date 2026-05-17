using UnityEngine;

namespace Platformer.Data
{
    [CreateAssetMenu(fileName = "PlayerJumpSettings", menuName = "Platformer/Settings/Player Jump")]
    public class PlayerJumpSettings : ScriptableObject
    {
        public int maxJumpCount = 3;
    }
}
