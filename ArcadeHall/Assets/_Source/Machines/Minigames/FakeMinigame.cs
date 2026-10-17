using DG.Tweening;
using Machines;
using UnityEngine;

public class FakeMinigame : ArcadeMachine
{
    [SerializeField] private float duration;
    [SerializeField] private int minTickets;
    [SerializeField] private int maxTickets;

    protected override void StartGame()
    {
        Debug.Log($"{name}: game started");

        DOVirtual.DelayedCall(duration, () =>
        {
            int tickets = Random.Range(minTickets, maxTickets + 1);
            Debug.Log($"{name}: game finished, {tickets} tickets");
            FinishGame(tickets);
        })
            .SetId(this);
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
