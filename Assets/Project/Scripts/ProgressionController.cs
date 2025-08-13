using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameJamLvl5.Project.Scripts.Services.InputService;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class ProgressionController
{
    [Inject]
    private GameplayUIService _gameplayUIService;

    [Inject]
    private DataService _dataService;

    [Inject]
    private InputService _inputService;

    [Inject]
    private CameraController _cameraController;

    [Inject]
    private GameplaySceneAssets _gameplaySceneAssets;

    [Inject]
    private PlayerBehaviour _player;

    private int interactionCount = 0;
    private int currentLelv = 1;

    public async UniTask HandleItemInteraction(string itemId)
    {
        _inputService.Disable();
        try
        {
            Item item = _dataService.ItemsData.items.Where((item) => item.id == itemId).First();

            interactionCount += 1;
            // TODO: сделять
            Sprite Sprite = default;

            // TODO: рандомить
            string SelectionOneText = item.meanings.cult.text;
            string SelectionTwoText = item.meanings.doctor.text;
            string SelectionThreeText = item.meanings.island.text;
            string SelectionFourText = item.meanings.fail.text;

            string OneSelectTag = item.meanings.cult.tag;
            string TwoSelectTag = item.meanings.doctor.tag;
            string ThreeSelectTag = item.meanings.island.tag;
            string FourSelectTag = item.meanings.fail.tag;

            PerceptionSelectionView.SetupContext setupContext = new(Sprite, SelectionOneText, SelectionTwoText, SelectionThreeText, SelectionFourText,
            OneSelectTag, TwoSelectTag, ThreeSelectTag, FourSelectTag);

            _gameplayUIService.SetupPrecepririonSelectionView(setupContext);

            var scope = _gameplayUIService.ShowPrecepririonSelectionView();

            string selectedTag = await scope.AwaitForSelect();

            if (selectedTag == "fail")
            {
                Debug.Log("Ты умер");
                _gameplayUIService.ClosePrecepririonSelectionView();
                return;
            }

            RootEnum tagEnum = (RootEnum)Enum.Parse(typeof(RootEnum), selectedTag, true);

            _dataService.AddRootTag(tagEnum);

            string tone = _dataService.GetMaxTon();

            if (tone == "neutral")
            {
                tone = selectedTag;
            }

            string newEntry = item.meanings.GetTone(tone);
            newEntry = $"[{tone}]\n" + newEntry;
            Debug.Log(newEntry);
            _gameplayUIService.AddEntryInJournalPopup(newEntry);

            _gameplayUIService.ClosePrecepririonSelectionView();

            await UniTask.WaitForSeconds(1);
            if (interactionCount > 2)
            {
                await HandleNextLevel();
            }
            else
            {
                var context = new JournalPopupView.ShowContext(false);
                await _gameplayUIService.ShowJournalPopup(context);
            }

        }
        finally
        {
            _inputService.Enable();
        }
    }

    public async UniTask HandleNextLevel()
    {
        if (currentLelv == 1)
        {
            var context = new JournalPopupView.ShowContext(true);
            await _gameplayUIService.ShowJournalPopup(context);
            _player.transform.position = _gameplaySceneAssets.L2SpawnPoint.position;
            _cameraController.SetNewBoundingShape(_gameplaySceneAssets.L2CameraBounds);
            return;
        }
        if (currentLelv == 2)
        {
            var context = new JournalPopupView.ShowContext(true);
            await _gameplayUIService.ShowJournalPopup(context);
            _player.transform.position = _gameplaySceneAssets.L3SpawnPoint.position;
            _cameraController.SetNewBoundingShape(_gameplaySceneAssets.L3CameraBounds);
            return;
        }

        throw new Exception("АААА БЛЯТЬ!!!111");
    }
}
