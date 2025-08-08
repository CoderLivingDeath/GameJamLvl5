using System;
using System.Collections.Generic;
namespace GameJamLvl5.Project.Scripts.Services.InputService
{
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
}