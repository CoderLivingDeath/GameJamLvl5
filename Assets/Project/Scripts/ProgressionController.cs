using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameJamLvl5.Project.Infrastructure.EventBus;
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
    private AssetsProvider _assetsProvider;

    [Inject]
    private PlayerBehaviour _player;

    [Inject]
    private EventBus _eventBus;

    private int interactionCount = 0;
    private int currentLelv = 1;
    private bool isGameOver = false;

    [Inject] private SoundManager soundManager;

    public async UniTask HandleItemInteraction(string itemId)
    {
        soundManager.SoundVolume = 0;
        _inputService.Disable();
        try
        {
            Item item = _dataService.ItemsData.items.Where((item) => item.id == itemId).First();

            Sprite Sprite = _assetsProvider.GetItemSprite(itemId);
            if (Sprite == null)
            {
                Debug.LogError("sprite is null");
                return;
            }

            // Сбор оригинальных пар
            List<SelectionOption> options = new List<SelectionOption>()
{
    new SelectionOption(item.meanings.cult.text, item.meanings.cult.tag),
    new SelectionOption(item.meanings.doctor.text, item.meanings.doctor.tag),
    new SelectionOption(item.meanings.island.text, item.meanings.island.tag),
    new SelectionOption(item.meanings.fail.text, item.meanings.fail.tag),
};

            // Перемешиваем список
            for (int i = 0; i < options.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, options.Count);
                SelectionOption temp = options[i];
                options[i] = options[randomIndex];
                options[randomIndex] = temp;
            }

            // Теперь options[0..3] — перемешанные пары текст-тег:
            string SelectionOneText = options[0].Text;
            string OneSelectTag = options[0].Tag;

            string SelectionTwoText = options[1].Text;
            string TwoSelectTag = options[1].Tag;

            string SelectionThreeText = options[2].Text;
            string ThreeSelectTag = options[2].Tag;

            string SelectionFourText = options[3].Text;
            string FourSelectTag = options[3].Tag;

            PerceptionSelectionView.SetupContext setupContext = new(Sprite, SelectionOneText, SelectionTwoText, SelectionThreeText, SelectionFourText,
            OneSelectTag, TwoSelectTag, ThreeSelectTag, FourSelectTag);

            _gameplayUIService.SetupPrecepririonSelectionView(setupContext);

            var scope = _gameplayUIService.ShowPrecepririonSelectionView();

            string selectedTag = await scope.AwaitForSelect();

            if (selectedTag == "fail")
            {
                Debug.Log("Ты умер");
                _gameplayUIService.ClosePrecepririonSelectionView();

                //TODO: скример
                isGameOver = true;
                _gameplayUIService.ShowGameOverView().Forget();
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

            interactionCount += 1;
            await UniTask.WaitForSeconds(1);
            if (interactionCount > 2)
            {
                await HandleNextLevel();
                interactionCount = 0;
            }
            else
            {
                var context = new JournalPopupView.ShowContext(false);
                var journalAwait = _gameplayUIService.ShowJournalPopup(context);
                await journalAwait.AwaitShow();
                Debug.Log("1");
                _eventBus.RaiseEvent<IProgressionEventHandler>(h => h.HandleProgressionEvent("journalFullScreen"));
            }

        }
        finally
        {
            if (!isGameOver)
                _inputService.Enable();
            _eventBus.RaiseEvent<IProgressionEventHandler>(h => h.HandleProgressionEvent("playerFree"));
        }
    }

    public async UniTask HandleNextLevel()
    {
        if (currentLelv == 1)
        {
            var context = new JournalPopupView.ShowContext(true);
            var journalAwait = _gameplayUIService.ShowJournalPopup(context);
            await journalAwait.AwaitShow();
            _player.transform.position = _gameplaySceneAssets.L2SpawnPoint.position;
            _cameraController.SetNewBoundingShape(_gameplaySceneAssets.L2CameraBounds);
            currentLelv = 2;
            return;
        }
        if (currentLelv == 2)
        {
            var context = new JournalPopupView.ShowContext(true);
            var journalAwait = _gameplayUIService.ShowJournalPopup(context);
            await journalAwait.AwaitShow();
            _player.transform.position = _gameplaySceneAssets.L3SpawnPoint.position;
            _cameraController.SetNewBoundingShape(_gameplaySceneAssets.L3CameraBounds);
            currentLelv = 3;
            return;
        }

        throw new Exception("АААА БЛЯТЬ!!!111");
    }
}

public struct SelectionOption
{
    public string Text;
    public string Tag;

    public SelectionOption(string text, string tag)
    {
        Text = text;
        Tag = tag;
    }
}