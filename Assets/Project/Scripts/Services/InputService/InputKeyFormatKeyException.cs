using System;
namespace GameJamLvl5.Project.Scripts.Services.InputService
{
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
}