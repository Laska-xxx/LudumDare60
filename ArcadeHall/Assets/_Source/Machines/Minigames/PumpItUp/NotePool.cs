using System.Collections.Generic;
using UnityEngine;

namespace Machines.Minigames.PumpItUp
{
    public class NotePool : MonoBehaviour
    {
        [SerializeField] private NoteView[] notePrefabs;
        [SerializeField] private int poolSizePerLane = 5;

        public Transform Root => transform;

        private Stack<NoteView>[] _pools;

        private void Awake()
        {
            _pools = new Stack<NoteView>[notePrefabs.Length];

            for (int lane = 0; lane < notePrefabs.Length; lane++)
            {
                _pools[lane] = new Stack<NoteView>();

                for (int i = 0; i < poolSizePerLane; i++)
                    _pools[lane].Push(CreateNote(lane));
            }
        }

        public NoteView Take(int lane)
        {
            NoteView note = _pools[lane].Count > 0 ? _pools[lane].Pop() : CreateNote(lane);
            note.gameObject.SetActive(true);
            return note;
        }

        public void Return(NoteView note)
        {
            note.gameObject.SetActive(false);
            _pools[note.Data.Lane].Push(note);
        }

        private NoteView CreateNote(int lane)
        {
            NoteView note = Instantiate(notePrefabs[lane], transform);
            note.gameObject.SetActive(false);
            return note;
        }
    }
}