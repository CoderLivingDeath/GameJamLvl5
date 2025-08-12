using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ProgressionController
{
    [Inject]
    private GameplayUIService _gameplayUIService;

    [Inject]
    private DataService _dataService;

    private int interactionCount = 0;

    public async UniTask HandleItemInteraction(string itemId)
    {
        //Item item = _dataService.ItemsData.items.Where((item) => item.id == itemId).First();

        interactionCount += 1;
        Sprite Sprite = default;
        string SelectionOneText = "item-1";
        string SelectionTwoText = "item-2";
        string SelectionThreeText = "item-3";
        string SelectionFourText = "item-4";
        string OneSelectId = "item-select-1";
        string TwoSelectId = "item-select-2";
        string ThreeSelectId = "item-select-3";
        string FourSelectId = "item-select-4";

        PerceptionSelectionView.SetupContext setupContext = new(Sprite, SelectionOneText, SelectionTwoText, SelectionThreeText, SelectionFourText,
        OneSelectId, TwoSelectId, ThreeSelectId, FourSelectId);

        _gameplayUIService.SetupPrecepririonSelectionView(setupContext);

        var scope = _gameplayUIService.ShowPrecepririonSelectionView();

        string id = await scope.AwaitForSelect();
        Debug.Log(id);

        _gameplayUIService.ClosePrecepririonSelectionView();

        if (interactionCount > 2)
        {
            await UniTask.WaitForSeconds(1);
            var context = new JournalPopupView.ShowContext(false);
            _gameplayUIService.ShowJournalPopup(context);
        }
    }
}
