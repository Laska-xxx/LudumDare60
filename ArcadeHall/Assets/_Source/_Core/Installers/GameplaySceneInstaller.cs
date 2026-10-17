using Core;
using Session;
using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallClasses();
        InstallSignals();
    }

    private void InstallClasses()
    {
        Container.Bind<SessionManager>().AsSingle();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<TokensChangedSignal>();
        Container.DeclareSignal<TicketsChangedSignal>();
    }
}