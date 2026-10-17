using Zenject;

namespace Core
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallClasses();
        }

        private void InstallClasses()
        {
            Container.Bind<InputReader>().AsSingle();
            Container.Bind<CursorController>().AsSingle();
        }
    }
}