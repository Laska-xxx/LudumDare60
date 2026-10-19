using UnityEngine;

namespace Machines.Minigames.HotBall
{
    public class ScoreHole : MonoBehaviour
    {
        [field: SerializeField] public int Points { get; private set; } = 1000;
    }
}
