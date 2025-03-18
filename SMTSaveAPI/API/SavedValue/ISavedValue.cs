using System;

namespace SMTSaveAPI.API.SavedValue
{
    /// <summary>
    /// Represents a generic saved value that can be stored and retrieved from the game's save system.
    /// </summary>
    public interface ISavedValue
    {
        /// <summary>
        /// Gets the default value assigned when no saved data exists.
        /// This value is used as a fallback if no previous save data is found.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Gets or sets the stored value as an object.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Gets the type of the stored value.
        /// This indicates the underlying data type used for serialization and retrieval.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this saved value should persist even if it is no longer registered by a mod or system.
        /// </summary>
        bool Persistent { get; set; }
    }
}
