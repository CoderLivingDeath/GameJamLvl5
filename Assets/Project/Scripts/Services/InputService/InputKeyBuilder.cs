using System;
using System.Collections.Generic;

namespace GameJamLvl5.Project.Scripts.Services.InputService
{
    /// <summary>
    /// Builder class to construct composite input keys consisting of multiple segments and a call type.
    /// </summary>
    public class InputKeyBuilder
    {
        private const string CALLTYPE_STARTED = "started";
        private const string CALLTYPE_PERFORMED = "performed";
        private const string CALLTYPE_CANCELED = "canceled";

        private readonly List<string> _segments = new List<string>();
        private InputSubscriber.CallType? _callType;

        /// <summary>
        /// Adds a segment to the key.
        /// </summary>
        /// <param name="segment">Key segment (e.g., part of a name).</param>
        /// <returns>The current builder instance (to allow method chaining).</returns>
        /// <exception cref="ArgumentNullException">Thrown if segment is null or whitespace.</exception>
        public InputKeyBuilder Append(string segment)
        {
            if (string.IsNullOrWhiteSpace(segment))
                throw new ArgumentNullException(nameof(segment), "Segment cannot be null or whitespace.");

            // Normalize: trim whitespace and convert to lowercase
            string normalized = segment.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(normalized))
                throw new ArgumentException("Segment is empty after normalization.", nameof(segment));

            _segments.Add(normalized);
            return this;
        }

        /// <summary>
        /// Sets the call type of the key.
        /// </summary>
        /// <param name="type">The call type.</param>
        /// <returns>The current builder instance (to allow method chaining).</returns>
        public InputKeyBuilder SetCallType(InputSubscriber.CallType type)
        {
            _callType = type;
            return this;
        }

        /// <summary>
        /// Builds the final key string in the format "name/call_type".
        /// </summary>
        /// <returns>The composed key string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no segments are added or call type is not set.</exception>
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
        /// Clears the builder for reuse.
        /// </summary>
        public void Clear()
        {
            _segments.Clear();
            _callType = null;
        }
    }
}
