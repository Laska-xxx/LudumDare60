namespace Machines.Minigames.PumpItUp
{
    public readonly struct NoteData
    {
        public readonly float Time;
        public readonly int Lane;

        public NoteData(float time, int lane)
        {
            Time = time;
            Lane = lane;
        }
    }
}