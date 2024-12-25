using System;

namespace AxeEngine.Editor
{
    public struct ActorEvent
    {
        public Type EventType { get; }
        public int ActorId { get; }
        public string PropertyName { get; }
        public string PropertyValue { get; }
        public string GameObjectName { get; }
        public string Time { get; }
        public string StackTrace { get; }

        public ActorEvent(Type eventType, int actorId, string propertyName, string propertyValue, string gameObjectName, DateTime dateTime, string stackTrace)
        {
            EventType = eventType;
            ActorId = actorId;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            GameObjectName = gameObjectName;
            StackTrace = stackTrace;
            Time = dateTime.ToLongTimeString();
        }

        public enum Type
        {
            Added,
            Removed,
            Replaced
        }
    }
}