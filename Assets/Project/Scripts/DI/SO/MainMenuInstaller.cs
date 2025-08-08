using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MainMenuInstaller", menuName = "Installers/MainMenuInstaller")]
public class MainMenuInstaller : ScriptableObjectInstaller<MainMenuInstaller>
{
    public override void InstallBindings()
    {
    }
}