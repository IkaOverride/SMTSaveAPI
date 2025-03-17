using SMTSaveAPI.API.Managers;
using System;

namespace SMTSaveAPI.API.SavedValue
{
    /// <summary>
    /// Represents a value that is saved and loaded with the game's save system.
    /// </summary>
    /// <typeparam name="T">The type of the stored value.</typeparam>
    public class SavedValue<T> : ISavedValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavedValue{T}"/> class.
        /// </summary>
        /// <param name="key">A unique identifier for the saved value.</param>
        /// <param name="defaultValue">The default value assigned when no saved data exists.</param>
        /// <param name="persistent">Indicates whether the value should persist even if it is no longer registered.</param>
        public SavedValue(string key, T defaultValue = default, bool persistent = false)
        {
            Value = defaultValue;
            Persistent = persistent;
            if (CustomSaveManager.SavedValues.ContainsKey(key))
                throw new ArgumentException("A saved value with the same key already exists: " + key);
            CustomSaveManager.SavedValues.Add(key, this);
        }

        /// <summary>
        /// Gets or sets the stored value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the stored value as an object (for interface compatibility).
        /// </summary>
        object ISavedValue.Value
        {
            get => Value;
            set => Value = (T) value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this saved value should persist even if it is no longer registered by a mod or system.
        /// </summary>
        public bool Persistent { get; set; }
    }
}
