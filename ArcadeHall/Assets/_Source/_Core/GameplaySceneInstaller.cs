using Core;
using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallClasses();
    }

    private void InstallClasses()
    {
        Container.Bind<InputReader>().AsSingle();
    }
}