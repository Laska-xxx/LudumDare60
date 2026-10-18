namespace Session
{
    public struct TokensChangedSignal
    {
        public int NewCount;
    }

    public struct TicketsChangedSignal
    {
        public int NewCount;
    }

    public struct SessionEndedSignal
    {
        public int Tickets;
        public bool IsNewRecord;
    }
}