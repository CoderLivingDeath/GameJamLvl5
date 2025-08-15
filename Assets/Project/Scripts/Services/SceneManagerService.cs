using Cysharp.Threading.Tasks;
using Zenject;

public class SceneManagerService
{
    private ZenjectSceneLoader _zenjectSceneLoader;

    public SceneManagerService(ZenjectSceneLoader zenjectSceneLoader)
    {
        _zenjectSceneLoader = zenjectSceneLoader;
    }

    public async UniTask RestartGameplayLevelAsync()
    {
        await _zenjectSceneLoader.LoadSceneAsync(0,UnityEngine.SceneManagement.LoadSceneMode.Single).ToUniTask();
    }
}