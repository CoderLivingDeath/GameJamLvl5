using UnityEngine;

namespace GameJamLvl5.Project.Scripts.behaviours.Interaction.InteractableHandlers
{
    [InteractableComponent]
    public class DebugInteracting : InteractableHandlerBehaviourBase
    {
        public override void HandleInteract(InteractionContext context)
        {
            Debug.Log(this.ToString() + " has interacted");
        }
    }
}
