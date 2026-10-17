using Session;
using UnityEngine;
using Zenject;

public class SessionManager
{
    public int Tokens { get; private set; } = 10;
    public int Tickets { get; private set; }

    private readonly SignalBus _signalBus;

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
}
