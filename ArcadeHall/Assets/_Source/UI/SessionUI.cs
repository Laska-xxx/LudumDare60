using DG.Tweening;
using Session;
using TMPro;
using UnityEngine;
using Zenject;

public class SessionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ticketsText;
    [SerializeField] private TextMeshProUGUI tokensText;
    [SerializeField] private float ticketsCountDuration = 0.5f;

    private SignalBus _signalBus;
    private int _displayedTickets;

    [Inject]
    private void Init(SignalBus signalBus, SessionManager sessionManager)
    {
        _signalBus = signalBus;

        _displayedTickets = sessionManager.Tickets;
        SetTicketsText(_displayedTickets);
        SetTokensText(sessionManager.Tokens);

        _signalBus.Subscribe<TicketsChangedSignal>(UpdateTickets);
        _signalBus.Subscribe<TokensChangedSignal>(UpdateTokens);
    }

    private void OnDisable()
    {
        DOTween.Kill(this);

        if (_signalBus != null)
        {
            _signalBus.TryUnsubscribe<TicketsChangedSignal>(UpdateTickets);
            _signalBus.TryUnsubscribe<TokensChangedSignal>(UpdateTokens);
        }
    }

    private void UpdateTickets(TicketsChangedSignal signal)
    {
        DOTween.Kill(this);
        DOTween.To(() => _displayedTickets, x =>
        {
            _displayedTickets = x;
            SetTicketsText(x);
        }, signal.NewCount, ticketsCountDuration)
            .SetId(this);
    }

    private void UpdateTokens(TokensChangedSignal signal)
    {
        SetTokensText(signal.NewCount);
    }

    private void SetTicketsText(int value)
    {
        ticketsText.text = value.ToString("D4");
    }

    private void SetTokensText(int value)
    {
        tokensText.text = value.ToString();
    }
}
