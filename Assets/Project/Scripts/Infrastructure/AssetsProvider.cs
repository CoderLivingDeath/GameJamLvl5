using UnityEngine;

public class AssetsProvider
{
    public GameObject SettingsPopupPrefab => _assets.SettingsPopupPrefab;

    public GameObject JournalPopupPrefab => _assets.JournalPopupPrefab;

    private AssetsSO _assets;

    public AssetsProvider(AssetsSO assets)
    {
        _assets = assets;
    }
}