using UnityEngine;
using Template.Project.Infrastructure.EventBus;
using Template.Project.Infrastructure.Factory;
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