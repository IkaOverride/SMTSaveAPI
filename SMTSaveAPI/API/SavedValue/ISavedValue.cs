namespace SMTSaveAPI.API.SavedValue
{
    /// <summary>
    /// Represents a generic saved value that can be stored and retrieved from the game's save system.
    /// </summary>
    public interface ISavedValue
    {
        /// <summary>
        /// Gets or sets the stored value as an object.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this saved value should persist even if it is no longer registered by a mod or system.
        /// </summary>
        bool Persistent { get; set; }
    }
}
