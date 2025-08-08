using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using GameJamLvl5.Project.Infrastructure.EventBus.Subscribers;
using GameJamLvl5.Project.Scripts.Services.InputService;

namespace GameJamLvl5.Project.Infrastructure.Factory
{
    public class CodeGeneratedInputServiceFactory : IFactory<InputService>
    {
        public const string INPUT_KEY_GAMEPLAY_MOVEMENT = "gameplay/movement";

        public const string INPUT_KEY_GAMEPLAY_INTERACT = "gameplay/interact";

        public const string INPUT_KEY_GAMEPLAY_MOUSE_POSITION = "gameplay/mouse/position";

        public const string INPUT_KEY_UI_MOUSE_POSITION = "ui/mouse/position";

        public const string INPUT_KEY_UI_UP = "ui/up";

        public const string INPUT_KEY_UI_DOWN = "ui/down";

        public const string INPUT_KEY_UI_LEFT = "ui/left";

        public const string INPUT_KEY_UI_RIGHT = "ui/right";

        public const string INPUT_KEY_UI_MOUSE_LEFTCLICK = "ui/mouse/left_click";

        public const string INPUT_KEY_UI_MOUSE_RIGHTCLICK = "ui/mouse/right_click";

        private DiContainer _container;

        public CodeGeneratedInputServiceFactory(DiContainer container)
        {
            _container = container;
        }

        public InputService Create()
        {
            EventBus.EventBus eventBus = _container.Resolve<EventBus.EventBus>();
            InputSystem_Actions actions = _container.Resolve<InputSystem_Actions>();

            InputSubscribersContainer subscribersContainer = CreateSubscribers(actions, eventBus);

            InputService service = new(actions, subscribersContainer);
            return service;
        }

        private InputSubscribersContainer CreateSubscribers(InputSystem_Actions actions, EventBus.EventBus eventBus)
        {
            InputSubscribersContainer container = new InputSubscribersContainer();

            #region Gameplay

            container.SubscribePerformed(INPUT_KEY_GAMEPLAY_MOVEMENT, actions.Gameplay.Movement, Gameplay_Movement_Handler);
            container.SubscribeCanceled(INPUT_KEY_GAMEPLAY_MOVEMENT, actions.Gameplay.Movement, Gameplay_Movement_Handler);

            container.SubscribePerformed(INPUT_KEY_GAMEPLAY_INTERACT, actions.Gameplay.Interact, Gameplay_Interact_Handler);
            container.SubscribePerformed(INPUT_KEY_GAMEPLAY_MOUSE_POSITION, actions.Gameplay.MousePosition, Gameplay_MousePosition_Handler);

            void Gameplay_Movement_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValue<Vector2>();
                eventBus.RaiseEvent<IGameplay_MovementEventHandler>(h => h.HandleMovement(value));
            }

            void Gameplay_Interact_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValueAsButton();
                eventBus.RaiseEvent<IGameplay_InteractEventHandler>(h => h.HandleInteract(value));
            }

            void Gameplay_MousePosition_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValue<Vector2>();
                eventBus.RaiseEvent<IGameplay_Mouse_PositionEventHandler>(h => h.HandleMousePosition(value));
            }
            #endregion

            #region UI
            container.SubscribePerformed(INPUT_KEY_UI_UP, actions.UI.Up, UI_Up_Handler);
            container.SubscribePerformed(INPUT_KEY_UI_DOWN, actions.UI.Down, UI_Down_Handler);
            container.SubscribePerformed(INPUT_KEY_UI_LEFT, actions.UI.Left, UI_Left_Handler);
            container.SubscribePerformed(INPUT_KEY_UI_RIGHT, actions.UI.Right, UI_Right_Handler);
            container.SubscribePerformed(INPUT_KEY_UI_MOUSE_POSITION, actions.UI.MousePosition, UI_Mouse_Position_Handler);
            container.SubscribePerformed(INPUT_KEY_UI_MOUSE_LEFTCLICK, actions.UI.MouseLeftClick, UI_Mouse_LeftClick_Handler);
            container.SubscribePerformed(INPUT_KEY_UI_MOUSE_RIGHTCLICK, actions.UI.MouseRightClick, UI_Mouse_RightClick_Handler);

            void UI_Up_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValueAsButton();
                eventBus.RaiseEvent<IUI_UpEventHandler>(h => h.HandleUp(value));
            }

            void UI_Down_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValueAsButton();
                eventBus.RaiseEvent<IUI_DownEventHandler>(h => h.HandleDown(value));
            }

            void UI_Left_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValueAsButton();
                eventBus.RaiseEvent<IUI_LeftEventHandler>(h => h.HandleLeft(value));
            }

            void UI_Right_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValueAsButton();
                eventBus.RaiseEvent<IUI_RightEventHandler>(h => h.HandleRight(value));
            }

            void UI_Mouse_Position_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValue<Vector2>();
                eventBus.RaiseEvent<IUI_MousePositionEventHandler>(h => h.HandleMousePosition(value));
            }

            void UI_Mouse_LeftClick_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValueAsButton();
                eventBus.RaiseEvent<IUI_MouseLeftClickEventHandler>(h => h.HandleMouseLeftClick(value));
            }

            void UI_Mouse_RightClick_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValueAsButton();
                eventBus.RaiseEvent<IUI_MouseRightClickEventHandler>(h => h.HandleMouseRightClick(value));
            }
            #endregion
            return container;
        }
    }
}