using Player;
using Session;
using UnityEngine;
using Zenject;

namespace Core
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private PlayerInteractor playerInteractor;

        public override void InstallBindings()
        {
            InstallClasses();
            InstallSignals();
            InstallMonoClasses();
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

        private void InstallMonoClasses()
        {
            Container.Bind<PlayerInteractor>().FromInstance(playerInteractor).AsSingle();
        }
    }
}