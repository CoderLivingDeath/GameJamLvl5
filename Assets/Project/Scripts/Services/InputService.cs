using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using GameJamLvl5.Project.Infrastructure.EventBus;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class InputService : IDisposable
{
    private readonly InputSystem_Actions _inputActions;
    private readonly InputSubscribersContainer _subscribers;

    private bool _disposed;

    public InputService(InputSystem_Actions inputActions, InputSubscribersContainer subscribers)
    {
        _subscribers = subscribers ?? throw new ArgumentNullException(nameof(subscribers));
        _inputActions = inputActions ?? throw new ArgumentNullException(nameof(inputActions));

        Enable();

        foreach (var item in _subscribers)
        {
            Debug.Log(item);
        }
    }

    public void Enable()
    {
        _inputActions.Enable();
        _subscribers.Enable();
    }

    public void Disable()
    {
        _inputActions.Disable();
        _subscribers.Disable();
    }

    public void Dispose()
    {
        if (_disposed) return;

        _subscribers.Dispose();
        _inputActions.Dispose();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(InputService));
    }
}

public class InputSubscriber : IDisposable
{
    public enum CallType
    {
        OnStarted,
        OnPerformed,
        OnCanceled
    }

    public InputSubscriber(InputAction action, Action<InputAction.CallbackContext> callback, CallType type)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        Type = type;
    }

    public InputAction Action { get; }
    public Action<InputAction.CallbackContext> Callback { get; }
    public CallType Type { get; }

    public bool IsActive => _isActive;
    public bool IsDisposed { get; private set; }
    private bool _isActive;

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

    public void Dispose()
    {
        if (IsDisposed) return;

        Disable();
        IsDisposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(InputSubscriber));
    }
}

/// <summary>
/// Контейнер для управления подписчиками на события InputAction.
/// Позволяет подписывать, отписывать и перечислять подписчиков.
/// </summary>
public class InputSubscribersContainer : IEnumerable<KeyValuePair<string, InputSubscriber>>, IDisposable
{
    public bool IsActive => _isActive;
    private bool _isActive = false;

    public Dictionary<string, InputSubscriber> Subscribers => _subscribers;
    private readonly Dictionary<string, InputSubscriber> _subscribers = new();

    private bool _disposed;

    public InputSubscribersContainer() { }

    /// <summary>
    /// Получение или установка подписчика по ключу.
    /// При установке старый подписчик отписывается и заменяется на новый.
    /// </summary>
    // public InputSubscriber this[string key]
    // {
    //     get
    //     {
    //         if (TryFormatKey(key, out var formatedKey))
    //         {
    //             if (_subscribers.TryGetValue(formatedKey, out var value))
    //                 return value;

    //             throw new KeyNotFoundException($"No InputSubscriber with key '{formatedKey}'.");
    //         }
    //         else
    //         {
    //             throw new KeyNotFoundException($"Invalid key format '{key}'.");
    //         }
    //     }
    //     set
    //     {
    //         ThrowIfDisposed();
    //         if (TryFormatKey(key, out var formatedKey))
    //         {

    //             if (_subscribers.ContainsKey(formatedKey))
    //             {
    //                 _subscribers[formatedKey].Dispose();
    //             }
    //             _subscribers[formatedKey] = value ?? throw new ArgumentNullException(nameof(value));
    //         }
    //         else
    //         {
    //             throw new KeyNotFoundException($"Invalid key format '{key}'.");
    //         }
    //     }
    // }

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
    public void Subscribe(string key, InputSubscriber subscriber)
    {
        ThrowIfDisposed();
        var formatedKey = BuildKeyPath(key, subscriber.Type);

        if (_subscribers.ContainsKey(formatedKey)) throw new InvalidOperationException($"such a key already exists: {formatedKey}");
        if (subscriber == null)
            throw new ArgumentNullException(nameof(subscriber));

        // Если есть старый подписчик — освободить его ресурсы
        if (_subscribers.TryGetValue(formatedKey, out var oldSubscriber))
        {
            oldSubscriber.Dispose();
        }
        _subscribers[formatedKey] = subscriber;
    }

    /// <summary>
    /// Добавить одного подписчика.
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

    public void SubscribeStarted(string key, InputAction action, Action<InputAction.CallbackContext> callback)
        => Subscribe(key, action, callback, InputSubscriber.CallType.OnStarted);

    public void SubscribePerformed(string key, InputAction action, Action<InputAction.CallbackContext> callback)
        => Subscribe(key, action, callback, InputSubscriber.CallType.OnPerformed);

    public void SubscribeCanceled(string key, InputAction action, Action<InputAction.CallbackContext> callback)
        => Subscribe(key, action, callback, InputSubscriber.CallType.OnCanceled);

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
    /// Отключить и удалить подписчика по ключу и (опционально) типу вызова.
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

    private string BuildKeyPath(string key, InputSubscriber.CallType callType)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new InputKeyFormatKeyException("Key is null or empty");

        var builder = new InputKeyBuilder();

        // Разбиваем ключ на сегменты по '/' и добавляем в билдер
        var segments = key.Split('/');

        foreach (var segment in segments)
        {
            // Пробелы и пустые сегменты обработает билдера
            builder.Append(segment);
        }

        builder.SetCallType(callType);

        // Метод ToString() выбросит исключение, если формат неверный
        return builder.ToString();
    }

    /// <summary>
    /// Освобождает все ресурсы всех подписчиков и очищает контейнер.
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

public static class InputKeyFormatter
{
    private static readonly Regex pattern = new Regex(@"^([^/]+)(/[^/]+)*$", RegexOptions.Compiled);

    /// <summary>
    /// Проверяет, соответствует ли строка шаблону "сегмент/сегмент/..."
    /// </summary>
    public static bool IsValid(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;
        return pattern.IsMatch(input);
    }

    /// <summary>
    /// Приводит строку к формату "сегмент/сегмент/..." (без пробелов, с нижним регистром).
    /// Возвращает отформатированную строку или null, если формат некорректен.
    /// </summary>
    public static string Format(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var segments = input.Split('/');

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i] = segments[i].Trim();

            if (string.IsNullOrEmpty(segments[i]))
                return null; // пустой сегмент — некорректный формат
        }

        string formatted = string.Join("/", segments).ToLowerInvariant();

        if (!IsValid(formatted))
            return null;

        return formatted;
    }

    /// <summary>
    /// Приводит строку к формату "название/тип_вызова" (без пробелов, нижний регистр, разделитель '/')
    /// и проверяет, соответствует ли она этому формату.
    /// </summary>
    /// <param name="input">Входная строка.</param>
    /// <param name="result">Преобразованная строка в требуемом формате, если успешно.</param>
    /// <returns>True, если строка приведена к виду "название/тип_вызова", иначе false.</returns>
    public static bool TryFormatKey(string input, out string result)
    {
        result = InputKeyFormatter.Format(input);
        return result != null;
    }
}

public class InputKeyBuilder
{
    private readonly List<string> _segments = new List<string>();
    private InputSubscriber.CallType? _callType;

    private const string CALLTYPE_STARTED = "started";
    private const string CALLTYPE_PERFORMED = "performed";
    private const string CALLTYPE_CANCELED = "canceled";

    /// <summary>
    /// Добавляет сегмент в ключ.
    /// </summary>
    /// <param name="segment">Сегмент ключа (например, часть названия).</param>
    /// <returns>Текущий экземпляр билдера (для цепочного вызова).</returns>
    /// <exception cref="ArgumentNullException">Если segment пустой или whitespace.</exception>
    public InputKeyBuilder Append(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
            throw new ArgumentNullException(nameof(segment), "Segment cannot be null or whitespace.");

        // Нормализация: убрать пробелы, привести к нижнему регистру
        string normalized = segment.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(normalized))
            throw new ArgumentException("Segment is empty after normalization.", nameof(segment));

        _segments.Add(normalized);
        return this;
    }

    /// <summary>
    /// Устанавливает тип вызова.
    /// </summary>
    /// <param name="type">Тип вызова.</param>
    /// <returns>Текущий экземпляр билдера (для цепочного вызова).</returns>
    public InputKeyBuilder SetCallType(InputSubscriber.CallType type)
    {
        _callType = type;
        return this;
    }

    /// <summary>
    /// Строит итоговый ключ в формате "название/тип_вызова".
    /// </summary>
    /// <returns>Строка ключа.</returns>
    /// <exception cref="InvalidOperationException">Если не добавлено ни одного сегмента или не установлен тип вызова.</exception>
    public override string ToString()
    {
        if (_segments.Count == 0)
            throw new InvalidOperationException("No segments have been added to the key.");

        if (!_callType.HasValue)
            throw new InvalidOperationException("CallType has not been set.");

        string baseKey = string.Join("/", _segments);

        if (!InputKeyFormatter.IsValid(baseKey))
            throw new InputKeyFormatKeyException($"Key format is invalid: '{baseKey}'");

        string callTypeStr = _callType.Value switch
        {
            InputSubscriber.CallType.OnStarted => CALLTYPE_STARTED,
            InputSubscriber.CallType.OnPerformed => CALLTYPE_PERFORMED,
            InputSubscriber.CallType.OnCanceled => CALLTYPE_CANCELED,
            _ => throw new ArgumentOutOfRangeException(nameof(_callType), "Unknown CallType value")
        };

        return $"{baseKey}/{callTypeStr}";
    }

    /// <summary>
    /// Пример сброса (очистки) билдера для повторного использования.
    /// </summary>
    public void Clear()
    {
        _segments.Clear();
        _callType = null;
    }
}



public class InputKeyFormatKeyException : Exception
{
    public InputKeyFormatKeyException()
    {
    }

    public InputKeyFormatKeyException(string message) : base(message)
    {
    }

    public InputKeyFormatKeyException(string message, Exception inner) : base(message, inner)
    {
    }
}