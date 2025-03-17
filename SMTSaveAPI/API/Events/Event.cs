using System;
using System.Collections.Generic;

namespace SMTSaveAPI.API.Events
{
    /// <summary>
    /// Represents a custom event that allows subscribing to and invoking actions.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// A list of actions that have been subscribed to this event.
        /// </summary>
        internal List<Action> Subscribed = [];

        /// <summary>
        /// Allows adding a new subscriber to the event using the '+' operator.
        /// </summary>
        /// <param name="ev">The current event instance.</param>
        /// <param name="onInvoked">The action to be invoked when the event is triggered.</param>
        /// <returns>The event instance with the newly added subscriber.</returns>
        public static Event operator +(Event ev, Action onInvoked)
        {
            ev.Subscribe(onInvoked);
            return ev;
        }

        /// <summary>
        /// Adds a new subscriber to the event. The subscriber will be called when the event is invoked.
        /// </summary>
        /// <param name="onInvoked">The action to be invoked when the event is triggered.</param>
        public void Subscribe(Action onInvoked)
            => Subscribed.Add(onInvoked);

        /// <summary>
        /// Invokes all subscribed actions. All actions in the Subscribed list will be executed.
        /// </summary>
        internal void Invoke()
        {
            foreach (Action subscribed in Subscribed)
                subscribed.Invoke();
        }
    }
}
