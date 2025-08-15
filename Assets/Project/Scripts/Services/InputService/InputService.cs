using System;
using System.Linq;

namespace GameJamLvl5.Project.Scripts.Services.InputService
{
    /// <summary>
    /// Provides centralized management for input actions and their subscribers.
    /// Handles enabling, disabling and disposing of input actions and their event subscribers.
    /// </summary>
    public class InputService : IDisposable, IInputService
    {
        private readonly InputSystem_Actions _inputActions;
        private readonly InputSubscribersContainer _subscribers;

        public event Action OnEnabled;
        public event Action OnDisabled;

        public event Action<string> OnEnabledBy;
        public event Action<string> OnDisabledBy;

        private bool _disposed;

        /// <summary>
        /// Constructs the InputService with provided input actions and subscribers container.
        /// Automatically enables input and subscriptions.
        /// </summary>
        /// <param name="inputActions">Generated input action asset.</param>
        /// <param name="subscribers">Container managing input subscribers.</param>
        /// <exception cref="ArgumentNullException">Thrown if arguments are null.</exception>
        public InputService(InputSystem_Actions inputActions, InputSubscribersContainer subscribers)
        {
            _subscribers = subscribers ?? throw new ArgumentNullException(nameof(subscribers));
            _inputActions = inputActions ?? throw new ArgumentNullException(nameof(inputActions));

            Enable();
        }

        public void Enable(string filter)
        {
            _subscribers
                .Where(pair => pair.Key.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(pair =>
                {
                    pair.Value.Enable();
                });
            OnEnabledBy?.Invoke(filter);
        }

        /// <summary>
        /// Enables the input actions and all subscribers.
        /// </summary>
        public void Enable()
        {
            ThrowIfDisposed();
            _inputActions.Enable();
            _subscribers.Enable();
            OnEnabled?.Invoke();
        }

        public void Disable(string filter)
        {
            _subscribers
                .Where(pair => pair.Key.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(pair => pair.Value.Disable());

            OnDisabledBy?.Invoke(filter);
        }

        /// <summary>
        /// Disables the input actions and all subscribers.
        /// </summary>
        public void Disable()
        {
            ThrowIfDisposed();
            _inputActions.Disable();
            _subscribers.Disable();
            OnDisabled?.Invoke();
        }

        /// <summary>
        /// Disposes the input actions and subscriber container, freeing resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _subscribers.Dispose();
            _inputActions.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Throws an exception if the service has already been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InputService));
        }
    }
}
