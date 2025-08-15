using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace GameJamLvl5.Project.Scripts.Services.InputService
{
    /// <summary>
    /// Container for managing subscribers to InputAction events.
    /// Allows subscribing, unsubscribing and enumerating subscribers.
    /// </summary>
    public class InputSubscribersContainer : IEnumerable<KeyValuePair<string, InputSubscriber>>, IDisposable
    {
        public bool IsActive => _isActive;
        private bool _isActive = false;

        public Dictionary<string, InputSubscriber> Entries => _subscribers;
        private readonly Dictionary<string, InputSubscriber> _subscribers = new();

        private bool _disposed;

        public InputSubscribersContainer() { }

        /// <summary>
        /// Enables all subscribers in the container.
        /// </summary>
        public void Enable()
        {
            ThrowIfDisposed();
            if (IsActive) return;

            foreach (var item in _subscribers)
            {
                if (item.Value.IsActive) return;
                item.Value.Enable();
            }

            _isActive = true;
        }

        /// <summary>
        /// Disables all subscribers in the container.
        /// </summary>
        public void Disable()
        {
            ThrowIfDisposed();
            if (!IsActive) return;

            foreach (var item in _subscribers)
            {
                if (!item.Value.IsActive) return;
                item.Value.Disable();
            }

            _isActive = false;
        }
        
        /// <summary>
        /// Subscribes an InputSubscriber under a specific key.
        /// </summary>
        public void Subscribe(string key, InputSubscriber subscriber)
        {
            ThrowIfDisposed();
            var formatedKey = BuildKeyPath(key, subscriber.Type);

            if (_subscribers.ContainsKey(formatedKey)) throw new InvalidOperationException($"such a key already exists: {formatedKey}");
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            // If an old subscriber exists – dispose it
            if (_subscribers.TryGetValue(formatedKey, out var oldSubscriber))
            {
                oldSubscriber.Dispose();
            }
            _subscribers[formatedKey] = subscriber;
        }

        /// <summary>
        /// Subscribes a single subscriber with an action and callback.
        /// </summary>
        public void Subscribe(string key, InputAction action, Action<InputAction.CallbackContext> callback, InputSubscriber.CallType callType)
        {
            ThrowIfDisposed();
            var formattedKey = BuildKeyPath(key, callType);

            if (_subscribers.ContainsKey(formattedKey)) throw new InvalidOperationException($"such a key already exists: {formattedKey}");
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            var subscriber = new InputSubscriber(action, callback, callType);
            _subscribers.Add(formattedKey, subscriber);
        }

        /// <summary>
        /// Subscribes to the InputAction started event.
        /// </summary>
        public void SubscribeStarted(string key, InputAction action, Action<InputAction.CallbackContext> callback)
            => Subscribe(key, action, callback, InputSubscriber.CallType.OnStarted);

        /// <summary>
        /// Subscribes to the InputAction performed event.
        /// </summary>
        public void SubscribePerformed(string key, InputAction action, Action<InputAction.CallbackContext> callback)
            => Subscribe(key, action, callback, InputSubscriber.CallType.OnPerformed);

        /// <summary>
        /// Subscribes to the InputAction canceled event.
        /// </summary>
        public void SubscribeCanceled(string key, InputAction action, Action<InputAction.CallbackContext> callback)
            => Subscribe(key, action, callback, InputSubscriber.CallType.OnCanceled);

        /// <summary>
        /// Unsubscribes and removes the subscriber by key.
        /// </summary>
        public void Unsubscribe(string key)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (_subscribers.TryGetValue(key, out var subscriber))
            {
                subscriber.Dispose();
                _subscribers.Remove(key);
            }
        }

        /// <summary>
        /// Disables and removes subscriber by key and (optionally) call type.
        /// </summary>
        public void Unsubscribe(string key, InputSubscriber.CallType callType)
        {
            ThrowIfDisposed();

            var formatedKey = BuildKeyPath(key, callType);
            if (_subscribers.TryGetValue(formatedKey, out var subscriber))
            {
                subscriber.Dispose();
                _subscribers.Remove(formatedKey);
            }

        }

        public void UnsubscribeStarted(string key) => Unsubscribe(key, InputSubscriber.CallType.OnStarted);
        public void UnsubscribePerformed(string key) => Unsubscribe(key, InputSubscriber.CallType.OnPerformed);
        public void UnsubscribeCanceled(string key) => Unsubscribe(key, InputSubscriber.CallType.OnCanceled);

        /// <summary>
        /// Builds a composite key string by combining the input key and call type.
        /// </summary>
        private string BuildKeyPath(string key, InputSubscriber.CallType callType)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InputKeyFormatKeyException("Key is null or empty");

            var builder = new InputKeyBuilder();

            // Split the key by '/' and append segments to the builder
            var segments = key.Split('/');

            foreach (var segment in segments)
            {
                // Spaces and empty segments are handled by the builder
                builder.Append(segment);
            }

            builder.SetCallType(callType);

            // ToString() will throw if the format is invalid
            return builder.ToString();
        }

        /// <summary>
        /// Disposes all subscribers and clears the container.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            foreach (var subscriber in _subscribers.Values)
            {
                subscriber.Dispose();
            }
            _subscribers.Clear();
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(InputSubscribersContainer));
        }

        public IEnumerator<KeyValuePair<string, InputSubscriber>> GetEnumerator()
        {
            return _subscribers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
