using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJamLvl5.Project.Infrastructure.EventBus
{
    /// <summary>
    /// Central event bus that manages subscriptions and event dispatching for global subscribers.
    /// </summary>
    public class EventBus
    {
        // Dictionary mapping subscriber types to their subscriber lists
        private Dictionary<Type, SubscribersList<IGlobalSubscriber>> s_Subscribers
            = new Dictionary<Type, SubscribersList<IGlobalSubscriber>>();

        /// <summary>
        /// Subscribes a global subscriber to all event interfaces it implements.
        /// </summary>
        /// <param name="subscriber">Subscriber instance to add.</param>
        public void Subscribe(IGlobalSubscriber subscriber)
        {
            // Get all subscribed event interface types implemented by the subscriber
            List<Type> subscriberTypes = EventBusHelper.GetSubscriberTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (!s_Subscribers.ContainsKey(t))
                {
                    s_Subscribers[t] = new SubscribersList<IGlobalSubscriber>();
                }
                s_Subscribers[t].Add(subscriber);
            }
        }

        /// <summary>
        /// Unsubscribes a global subscriber from all event interfaces it implements.
        /// </summary>
        /// <param name="subscriber">Subscriber instance to remove.</param>
        public void Unsubscribe(IGlobalSubscriber subscriber)
        {
            // Get all subscribed event interface types implemented by the subscriber
            List<Type> subscriberTypes = EventBusHelper.GetSubscriberTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (s_Subscribers.ContainsKey(t))
                    s_Subscribers[t].Remove(subscriber);
            }
        }

        /// <summary>
        /// Raises an event for all subscribers of the given subscriber interface type.
        /// Calls the provided action on each subscriber.
        /// </summary>
        /// <typeparam name="TSubscriber">Subscriber interface to raise event for.</typeparam>
        /// <param name="action">Action to invoke on subscribers.</param>
        public void RaiseEvent<TSubscriber>(Action<TSubscriber> action)
            where TSubscriber : class, IGlobalSubscriber
        {
            if (!s_Subscribers.TryGetValue(typeof(TSubscriber), out SubscribersList<IGlobalSubscriber> subscribers) || subscribers == null)
            {
                // No subscribers - simply exit without calling the action
                return;
            }

            subscribers.Executing = true;
            foreach (IGlobalSubscriber subscriber in subscribers.List)
            {
                try
                {
                    // Invoke action on the subscriber cast to the specific subscriber interface
                    action.Invoke(subscriber as TSubscriber);
                }
                catch (Exception e)
                {
                    // Log any exception thrown by a subscriber to prevent breaking event dispatching
                    Debug.LogError(e);
                }
            }
            subscribers.Executing = false;

            // Cleanup removed subscribers after event dispatching finishes
            subscribers.Cleanup();
        }
    }
}
