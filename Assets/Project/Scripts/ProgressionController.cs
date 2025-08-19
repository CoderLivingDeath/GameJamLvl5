using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameJamLvl5.Project.Infrastructure.EventBus;
using GameJamLvl5.Project.Scripts.Services.InputService;
using UnityEngine;
using Zenject;

public class ProgressionController
{
    #region Injected Services

    [Inject] private GameplayUIService _gameplayUIService;
    [Inject] private DataService _dataService;
    [Inject] private InputService _inputService;
    [Inject] private CameraController _cameraController;
    [Inject] private GameplaySceneAssets _gameplaySceneAssets;
    [Inject] private AssetsProvider _assetsProvider;
    [Inject] private PlayerBehaviour _player;
    [Inject] private EventBus _eventBus;
    [Inject] private SoundManager soundManager;
    [Inject] private GameplayUIViewsProvider gameplayUIViewsProvider;
    [Inject] private ScreamersView screamersView;
    [Inject] private SceneManagerService sceneManagerService;

    [Inject] private FinalBgView finalBgView;

    #endregion

    #region State

    private int interactionCount = 0;
    private int currentLevel = 1;
    private bool isGameOver = false;

    #endregion

    #region Public API

    public async UniTask HandleFinal(string final)
    {
        switch (final)
        {
            case "cult":
                _gameplaySceneAssets.MusicSource.resource = _gameplaySceneAssets.Ktulhu;
                _gameplaySceneAssets.MusicSource.Play();
                finalBgView.cultAnimationObject.SetActive(true);
                break;
            case "doc":
                _gameplaySceneAssets.MusicSource.resource = _gameplaySceneAssets.doctor;
                _gameplaySceneAssets.MusicSource.Play();
                finalBgView.image.sprite = finalBgView.Doc;
                break;
            case "island":
                _gameplaySceneAssets.MusicSource.resource = _gameplaySceneAssets.island;
                _gameplaySceneAssets.MusicSource.Play();
                finalBgView.image.sprite = finalBgView.Island;
                break;
        }

        _inputService.Disable("gameplay");
        await ShowJournalWithSticker();
        finalBgView.gameObject.SetActive(true);
    }

    private async UniTask ShowJournalWithSticker()
    {
        var tone = _dataService.GetMaxTon();
        switch (tone)
        {
            case "cult":
                gameplayUIViewsProvider.JournalPopupView.Sticker_cult.gameObject.SetActive(true);
                break;
            case "doctor":
                gameplayUIViewsProvider.JournalPopupView.Sticker_doc.gameObject.SetActive(true);
                break;
            case "island":
                gameplayUIViewsProvider.JournalPopupView.Sticker_island.gameObject.SetActive(true);
                break;
            case "neutral":
                Debug.LogError("Тон: Нейтральный");
                break;
            default:
                Debug.LogError($"Неизвестный тон: {tone}");
                break;
        }

        JournalPopupView.ShowContext context = new(true);
        var scope = _gameplayUIService.ShowJournalPopup(context);
        await scope.AwaitShow();
    }

    /// <summary>
    /// Обрабатывает взаимодействие с предметом.
    /// </summary>
    public async UniTask HandleItemInteraction(string itemId)
    {

        try
        {
            Item item = _dataService.ItemsData.items.FirstOrDefault(i => i.id == itemId);
            if (item == null)
            {
                Debug.LogError($"Item with id '{itemId}' not found.");
                return;
            }

            Sprite sprite = _assetsProvider.GetItemSprite(itemId);
            if (sprite == null)
            {
                Debug.LogError("Sprite is null.");
                return;
            }

            List<SelectionOption> options = BuildSelectionOptions(item);
            ShuffleOptions(options);

            PerceptionSelectionView.SetupContext setupContext = CreateSetupContext(sprite, options);
            _gameplayUIService.SetupPrecepririonSelectionView(setupContext);

            var scope = _gameplayUIService.ShowPrecepririonSelectionView();
            string selectedTag = await scope.AwaitForSelect();

            _eventBus.RaiseEvent<IProgressionEventHandler>(h => h.HandleProgressionEvent(itemId + "_event_preceptionSelected"));

            if (selectedTag == "fail")
            {
                _eventBus.RaiseEvent<IProgressionEventHandler>(h => h.HandleProgressionEvent("event_fail"));
                _eventBus.RaiseEvent<IProgressionEventHandler>(h => h.HandleProgressionEvent(itemId + "_event_fail"));
                _gameplayUIService.ClosePrecepririonSelectionView();

                isGameOver = true;
                JournalPopupView.ShowContext context = new(true);
                var showScope = _gameplayUIService.ShowJournalPopup(context);
                _gameplaySceneAssets.JournalSource.Play();

                gameplayUIViewsProvider.JournalPopupView.BloackCloseFor(_gameplaySceneAssets.JournalSource.clip.length).Forget();
                await showScope.AwaitShow();

                screamersView.gameObject.SetActive(true);

                GameObject screamer = null;
                    
                switch (itemId)
                {
                    case "l1_rorsharh":
                        screamer = screamersView.RorshahScreamer.gameObject;
                        break;
                    case "l1_piplls":
                        screamer = screamersView.PipplesScreamer.gameObject;
                        break;
                    case "l1_statue":
                        screamer = screamersView.StatuyaScreamer.gameObject;
                        break;
                    case "l2_painting":
                        screamer = screamersView.PaintingScreamer.gameObject;
                        break;
                    case "l2_clock":
                        screamer = screamersView.ClockScreamer.gameObject;
                        break;
                    case "l2_mirror":
                        screamer = screamersView.MirrorScreamer.gameObject;
                        
                        break;
                }

                if (screamer != null)
                {
                    screamer.SetActive(true);
                }
                
                await showScope.AwaitClose();
                screamer.GetComponent<AudioSource>().enabled = true;
                await UniTask.WaitForSeconds(screamer.GetComponent<AudioSource>().clip.length);
                sceneManagerService.RestartGameplayLevelAsync().Forget();
                _inputService.Enable();

                return;
            }

            HandleTagSelection(item, selectedTag);
            _gameplayUIService.ClosePrecepririonSelectionView();

            interactionCount++;
            await UniTask.WaitForSeconds(1);

            if (interactionCount > 2)
            {
                await HandleNextLevel();
                interactionCount = 0;
            }
            else
            {
                await ShowJournalPopup(itemId);
            }

        }
        finally
        {
            if (!isGameOver)
                _inputService.Enable("gameplay");
        }
    }

    /// <summary>
    /// Переходит на следующий уровень.
    /// </summary>
    public async UniTask HandleNextLevel()
    {
        switch (currentLevel)
        {
            case 1:
                await MovePlayerToNextLevel(_gameplaySceneAssets.L2SpawnPoint.position, _gameplaySceneAssets.L2CameraBounds, 2);
                break;
            case 2:

                await HandleTherdLevel();
                break;
            default:
                throw new Exception("АААА БЛЯТЬ!!!111");
        }
    }

    public async UniTask HandleTherdLevel()
    {
        switch (_dataService.GetMaxTon())
        {
            case "cult":
                await MovePlayerToNextLevel(_gameplaySceneAssets.L3SpawnPoints[0].position, _gameplaySceneAssets.L3CameraBounds[0], 2);
                break;
            case "doctor":

                await MovePlayerToNextLevel(_gameplaySceneAssets.L3SpawnPoints[1].position, _gameplaySceneAssets.L3CameraBounds[1], 2);
                break;
            case "island":

                await MovePlayerToNextLevel(_gameplaySceneAssets.L3SpawnPoints[2].position, _gameplaySceneAssets.L3CameraBounds[2], 2);
                break;
            default:
                throw new Exception("АААА БЛЯТЬ!!!111");
        }

        //await MovePlayerToNextLevel(_gameplaySceneAssets.L3SpawnPoint.position, _gameplaySceneAssets.L3CameraBounds, 3);
    }

    #endregion

    #region Private Methods

    private List<SelectionOption> BuildSelectionOptions(Item item)
    {
        return new List<SelectionOption>
        {
            new SelectionOption(item.meanings.cult.text,   item.meanings.cult.tag),
            new SelectionOption(item.meanings.doctor.text, item.meanings.doctor.tag),
            new SelectionOption(item.meanings.island.text, item.meanings.island.tag),
            new SelectionOption(item.meanings.fail.text,   item.meanings.fail.tag)
        };
    }

    private void ShuffleOptions(List<SelectionOption> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, options.Count);
            (options[i], options[randomIndex]) = (options[randomIndex], options[i]);
        }
    }

    private PerceptionSelectionView.SetupContext CreateSetupContext(Sprite sprite, List<SelectionOption> options)
    {
        return new PerceptionSelectionView.SetupContext(
            sprite,
            options[0].Text, options[1].Text, options[2].Text, options[3].Text,
            options[0].Tag, options[1].Tag, options[2].Tag, options[3].Tag
        );
    }

    private void HandleTagSelection(Item item, string selectedTag)
    {
        RootEnum tagEnum = (RootEnum)Enum.Parse(typeof(RootEnum), selectedTag, true);
        _dataService.AddRootTag(tagEnum);

        string tone = _dataService.GetMaxTon();
        Debug.Log(selectedTag);

        if (tone == "neutral")
            tone = selectedTag;

        string text = null;
        if (selectedTag == "cult")
        {
            text = item.meanings.cult.text;
        }
        else if (selectedTag == "doctor")
        {
            text = item.meanings.doctor.text;
        }
        else if (selectedTag == "island")
        {
            text = item.meanings.island.text;
        }
        else
        {
            Debug.LogError("AAAAAA");
        }

        string newEntry = $"[{text}]\n{item.meanings.GetTone(selectedTag)}";
        Debug.Log(newEntry);
        _gameplayUIService.AddEntryInJournalPopup(newEntry);
    }

    private async UniTask ShowJournalPopup(string itemId)
    {
        var context = new JournalPopupView.ShowContext(false);
        var journalAwait = _gameplayUIService.ShowJournalPopup(context);
        await journalAwait.AwaitShow();

        _eventBus.RaiseEvent<IProgressionEventHandler>(h => h.HandleProgressionEvent(itemId + "_event_journalFullscreen"));
        _eventBus.RaiseEvent<IProgressionEventHandler>(h => h.HandleProgressionEvent("journalFullScreen"));
    }

    private async UniTask MovePlayerToNextLevel(Vector3 spawnPosition, PolygonCollider2D cameraBounds, int nextLevel)
    {
        var context = new JournalPopupView.ShowContext(true);
        var journalAwait = _gameplayUIService.ShowJournalPopup(context);
        await journalAwait.AwaitShow();

        _player.transform.position = spawnPosition;
        _cameraController.SetNewBoundingShape(cameraBounds);
        currentLevel = nextLevel;
    }


    #endregion

    #region Structs

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

    #endregion
}
