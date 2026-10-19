using UnityEngine;

namespace Machines.Minigames.PumpItUp
{
    public class NoteView : MonoBehaviour
    {
        public NoteData Data { get; private set; }

        public void Init(NoteData data)
        {
            Data = data;
        }
    }
}