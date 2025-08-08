using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "LoadingInstaller", menuName = "Installers/LoadingInstaller")]
public class LoadingInstaller : ScriptableObjectInstaller<LoadingInstaller>
{
    public override void InstallBindings()
    {
    }
}