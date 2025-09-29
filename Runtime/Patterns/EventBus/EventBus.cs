using System;

namespace LazyCoder.Core
{
    /// <summary>
    /// This is the simplest an fastest implementation for the event bus pattern.
    /// Since this is static bus DO NOT USE it when you are making a package,
    /// Since it may conflict with user project.
    ///
    /// It only makes sense to use it inside the project you maintain and own.
    /// </summary>
    /// <typeparam name="T">Event Type.</typeparam>
    public static class EventBus<T> where T : IEvent
    {
        private static Action<T> s_action = delegate { };

        /// <summary>
        /// Subscribes listener to a certain event type.
        /// </summary>
        /// <param name="listener">Listener instance.</param>
        public static void Subscribe(Action<T> listener)
        {
            s_action += listener;
        }

        /// <summary>
        /// Unsubscribes listener to a certain event type.
        /// </summary>
        /// <param name="listener">Listener instance.</param>
        public static void Unsubscribe(Action<T> listener)
        {
            s_action -= listener;
        }

        /// <summary>
        /// Posts and event.
        /// </summary>
        /// <param name="event">An event instance to post.</param>
        public static void Post(T @event)
        {
            s_action.Invoke(@event);
        }
    }
}
