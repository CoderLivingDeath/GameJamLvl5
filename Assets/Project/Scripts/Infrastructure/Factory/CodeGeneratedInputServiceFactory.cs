using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Template.Project.Infrastructure.EventBus;
using Zenject;
using Template.Project.Infrastructure.EventBus.Subscribers;

namespace Template.Project.Infrastructure.Factory
{
    public class CodeGeneratedInputServiceFactory : IFactory<InputService>
    {
        public const string INPUT_KEY_MOVEMENT = "movement";
        public const string INPUT_KEY_INTERACT = "interact";
        public const string INPUT_KET_MOUSETRACK = "mouseTrack";

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

            InputService service = new(eventBus, actions, subscribersContainer);
            return service;
        }

        private InputSubscribersContainer CreateSubscribers(InputSystem_Actions actions, EventBus.EventBus eventBus)
        {
            InputSubscribersContainer container = new InputSubscribersContainer();

            // подписка событий
            container.SubscribePerformed(INPUT_KET_MOUSETRACK, actions.Gameplay.MouseTrack, MouseTrack_Handler);

            // методы обработчики событий
            void MouseTrack_Handler(InputAction.CallbackContext context)
            {
                var value = context.ReadValue<Vector2>();
                eventBus.RaiseEvent<IMouseTrackEventHandler>(h => h.HandleMouseTrack(value));
            }

            return container;
        }
    }
}