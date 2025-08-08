using System;
using UnityEngine.InputSystem;

namespace GameJamLvl5.Project.Scripts.Services.InputService
{
    /// <summary>
    /// Represents a subscriber to a specific InputAction event with a callback and call type.
    /// Manages subscription lifecycle (enable/disable) and disposal.
    /// </summary>
    public class InputSubscriber : IDisposable
    {
        /// <summary>
        /// Type of callback event to subscribe to on an InputAction.
        /// </summary>
        public enum CallType
        {
            OnStarted,
            OnPerformed,
            OnCanceled
        }

        /// <summary>
        /// Creates a new subscriber for the given InputAction, callback and call type.
        /// </summary>
        /// <param name="action">InputAction to subscribe to.</param>
        /// <param name="callback">Delegate called on the specified event.</param>
        /// <param name="type">Type of event to listen for.</param>
        /// <exception cref="ArgumentNullException">Thrown if action or callback is null.</exception>
        public InputSubscriber(InputAction action, Action<InputAction.CallbackContext> callback, CallType type)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
            Type = type;
        }

        /// <summary>
        /// The InputAction this subscriber is associated with.
        /// </summary>
        public InputAction Action { get; }

        /// <summary>
        /// The callback delegate triggered by the subscribed event.
        /// </summary>
        public Action<InputAction.CallbackContext> Callback { get; }

        /// <summary>
        /// The type of event this subscriber listens to (started, performed, canceled).
        /// </summary>
        public CallType Type { get; }

        /// <summary>
        /// Returns true if the subscriber is currently active (subscribed).
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// Returns true if the subscriber has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private bool _isActive;

        /// <summary>
        /// Enables the subscription by attaching the callback to the InputAction event specified by CallType.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the subscriber has been disposed.</exception>
        public void Enable()
        {
            ThrowIfDisposed();
            if (_isActive) return;

            switch (Type)
            {
                case CallType.OnStarted:
                    Action.started += Callback;
                    break;
                case CallType.OnPerformed:
                    Action.performed += Callback;
                    break;
                case CallType.OnCanceled:
                    Action.canceled += Callback;
                    break;
            }

            _isActive = true;
        }

        /// <summary>
        /// Disables the subscription by detaching the callback from the InputAction event.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the subscriber has been disposed.</exception>
        public void Disable()
        {
            ThrowIfDisposed();
            if (!_isActive) return;

            switch (Type)
            {
                case CallType.OnStarted:
                    Action.started -= Callback;
                    break;
                case CallType.OnPerformed:
                    Action.performed -= Callback;
                    break;
                case CallType.OnCanceled:
                    Action.canceled -= Callback;
                    break;
            }

            _isActive = false;
        }

        /// <summary>
        /// Disposes the subscriber, disabling it and marking as disposed.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            Disable();
            IsDisposed = true;
        }

        /// <summary>
        /// Throws ObjectDisposedException if the subscriber has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(InputSubscriber));
        }
    }
}
