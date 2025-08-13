using UnityEngine;

public class AssetsProvider
{
    private AssetsSO _assets;
    public Sprite GetItemSprite(string itemId)
    {
        return itemId switch
        {
            "l1_rorsharh" => _assets.l1_rorsharh,
            "l1_piplls" => _assets.l1_piplls,
            "l1_statue" => _assets.l1_statue,
            "l2_painting" => _assets.l2_painting,
            "l2_clock" => _assets.l2_clock,
            "l2_mirror" => _assets.l2_mirror,
            "l3_book" => _assets.l3_book,
            "l3_journal" => _assets.l3_journal,
            "l3_map" => _assets.l3_map,
            _ => null  // или выбросить исключение, если нужно
        };
    }

    public AssetsProvider(AssetsSO assets)
    {
        _assets = assets;
    }
}