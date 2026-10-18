using Core;
using Session;
using UnityEngine;
using Zenject;

public class SessionManager
{
    public int Tokens { get; private set; } = 10;
    public int Tickets { get; private set; }
    public bool IsEnded { get; private set; }

    private readonly SignalBus _signalBus;
    private int _pendingRibbons;

    [Inject]
    public SessionManager(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public bool TrySpendToken()
    {
        if (Tokens <= 0)
            return false;

        Tokens--;
        _signalBus.Fire(new TokensChangedSignal { NewCount = Tokens });
        return true;
    }

    public void AddTickets(int amount)
    {
        if (amount <= 0)
            return;

        Tickets += amount;
        _signalBus.Fire(new TicketsChangedSignal { NewCount = Tickets });
    }

    public void RegisterRibbon()
    {
        _pendingRibbons++;
    }

    public void CollectRibbon(int tickets)
    {
        _pendingRibbons--;
        AddTickets(tickets);
        CheckSessionEnd();
    }

    public void CheckSessionEnd()
    {
        if (IsEnded || Tokens > 0 || _pendingRibbons > 0)
            return;

        IsEnded = true;
        bool isNewRecord = BestScore.TrySave(Tickets);
        _signalBus.Fire(new SessionEndedSignal { Tickets = Tickets, IsNewRecord = isNewRecord });
    }
}
