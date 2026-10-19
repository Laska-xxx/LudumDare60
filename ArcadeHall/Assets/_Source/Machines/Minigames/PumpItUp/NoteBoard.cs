using System;
using System.Collections.Generic;
using UnityEngine;

namespace Machines.Minigames.PumpItUp
{
    public class NoteBoard : MonoBehaviour
    {
        [SerializeField] private NotePool pool;
        [SerializeField] private Transform[] laneTargets;
        [SerializeField] private float spawnHeight = 5f;

        public event Action<int> NoteMissed;

        public int LaneCount => laneTargets.Length;

        private readonly List<NoteData> _chart = new();
        private List<NoteView>[] _activeNotes;
        private int _nextNote;
        private float _approachTime;
        private float _missWindow;

        private void Awake()
        {
            _activeNotes = new List<NoteView>[LaneCount];

            for (int lane = 0; lane < LaneCount; lane++)
                _activeNotes[lane] = new List<NoteView>();
        }

        public void Begin(List<NoteData> chart, float approachTime, float missWindow)
        {
            Clear();

            _chart.AddRange(chart);
            _approachTime = approachTime;
            _missWindow = missWindow;
        }

        public void Tick(float songTime)
        {
            SpawnNotes(songTime);
            MoveNotes(songTime);
        }

        public NoteView GetFirstNote(int lane)
        {
            List<NoteView> notes = _activeNotes[lane];
            return notes.Count > 0 ? notes[0] : null;
        }

        public void RemoveNote(NoteView note)
        {
            _activeNotes[note.Data.Lane].Remove(note);
            pool.Return(note);
        }

        public void Clear()
        {
            foreach (List<NoteView> notes in _activeNotes)
            {
                foreach (NoteView note in notes)
                    pool.Return(note);

                notes.Clear();
            }

            _chart.Clear();
            _nextNote = 0;
        }

        private void SpawnNotes(float songTime)
        {
            while (_nextNote < _chart.Count && _chart[_nextNote].Time - _approachTime <= songTime)
            {
                NoteData data = _chart[_nextNote];
                _nextNote++;

                NoteView note = pool.Take(data.Lane);
                note.Init(data);
                _activeNotes[data.Lane].Add(note);
            }
        }

        private void MoveNotes(float songTime)
        {
            for (int lane = 0; lane < LaneCount; lane++)
            {
                List<NoteView> notes = _activeNotes[lane];

                for (int i = notes.Count - 1; i >= 0; i--)
                {
                    NoteView note = notes[i];
                    float timeLeft = note.Data.Time - songTime;

                    if (timeLeft < -_missWindow)
                    {
                        notes.RemoveAt(i);
                        pool.Return(note);
                        NoteMissed?.Invoke(lane);
                        continue;
                    }

                    Vector3 target = laneTargets[lane].position;
                    Vector3 position = target + transform.up * (spawnHeight * timeLeft / _approachTime);
                    position.z = pool.Root.position.z;

                    note.transform.position = position;
                }
            }
        }
    }
}