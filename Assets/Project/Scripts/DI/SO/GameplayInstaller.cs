using Unity.Cinemachine;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameplayInstaller", menuName = "Installers/GameplayInstaller")]
public class GameplayInstaller : ScriptableObjectInstaller<GameplayInstaller>
{

    [SerializeField]
    private AssetsSO _assetsSO;
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerBehaviour>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CinemachineCamera>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CinemachineConfiner2D>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameplaySceneAssets>().FromComponentInHierarchy().AsSingle().NonLazy();

        //views
        Container.BindInterfacesAndSelfTo<SettingsPopupView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<JournalPopupView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameplayView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PerceptionSelectionView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameOverView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DebugView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FinalBgView>().FromComponentInHierarchy().AsSingle().NonLazy();

        // providers
        Container.BindInterfacesAndSelfTo<AssetsSO>().FromInstance(_assetsSO).AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AssetsProvider>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameplayUIViewsProvider>().AsSingle();

        // services
        Container.BindInterfacesAndSelfTo<SceneManagerService>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameplayUIService>().AsSingle();
        Container.BindInterfacesAndSelfTo<DataService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SoundManager>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ProgressionController>().AsSingle().NonLazy();
    }
}