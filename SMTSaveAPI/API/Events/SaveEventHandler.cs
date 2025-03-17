namespace SMTSaveAPI.API.Events
{
    /// <summary>
    /// Handles events related to the saving and loading process of saved data.
    /// Provides events that can be subscribed to when saving or loading occurs.
    /// </summary>
    public static class SaveEventHandler
    {
        /// <summary>
        /// Event that gets triggered right before the custom save data is saved to the file.
        /// Subscribers will be notified just before the save operation begins.
        /// </summary>
        public static Event Saving { get; set; } = new();

        /// <summary>
        /// Event that gets triggered right after the custom save data is loaded from the file.
        /// Subscribers will be notified immediately after the load operation completes.
        /// </summary>
        public static Event Loaded { get; set; } = new();

        /// <summary>
        /// Invokes the <see cref="Saving"/> event, triggered right before the custom save data is saved to the file.
        /// This method is called when the saving process is about to begin.
        /// </summary>
        internal static void OnSaving() => Saving.Invoke();

        /// <summary>
        /// Invokes the <see cref="Loaded"/> event, triggered right after the custom save data is loaded from the file.
        /// This method is called when the loading process has completed.
        /// </summary>
        internal static void OnLoaded() => Loaded.Invoke();
    }
}
