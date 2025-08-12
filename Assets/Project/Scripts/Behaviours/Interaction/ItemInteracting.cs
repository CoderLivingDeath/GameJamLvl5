using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace GameJamLvl5.Project.Scripts.behaviours.Interaction.InteractableHandlers
{
    [InteractableComponent]
    public class ItemInteracting : InteractableHandlerBehaviourBase
    {
        public string ItemId;

        public bool isWasInteracted;

        [Inject]
        private GameplayUIService _gameplayUIService;

        [Inject]
        private ProgressionController _progressionController;

        public override void HandleInteract(InteractionContext context)
        {
            if (isWasInteracted == true) return;
            _progressionController.HandleItemInteraction(ItemId).Forget();
        }
    }
}
