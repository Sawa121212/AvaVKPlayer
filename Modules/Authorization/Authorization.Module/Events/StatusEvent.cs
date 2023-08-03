using Prism.Events;

namespace Authorization.Module.Events
{
    /// <summary>
    /// Событие изменения статуса.
    /// </summary>
    public class AuthorizeEvent : PubSubEvent<AuthorizeEvent>
    {
        /// <summary>
        /// Событие изменения статуса.
        /// </summary>
        public AuthorizeEvent()
        {
        }
    }
}