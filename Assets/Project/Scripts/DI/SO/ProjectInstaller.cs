using UnityEngine;
using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Infrastructure.Factory;
using Zenject;

[CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<InputSystem_Actions>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<EventBus>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InputService>().FromFactory<InputService, CodeGeneratedInputServiceFactory>().AsSingle().NonLazy();
    }
}