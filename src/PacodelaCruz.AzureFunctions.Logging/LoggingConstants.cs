namespace PacodelaCruz.AzureFunctions.Logging
{
    /// <summary>
    /// Contains constants and enums for consistent structured logging
    /// </summary>
    internal static class LoggingConstants
    {
        internal const string Template = "{Description}, {EntityType}, {EntityId}, {Status}, {CorrelationId}, {TrackingEventType}";

        internal enum EntityType
        {
            Order
        }
        internal enum TrackingEventType
        {
            Publisher,
            Subscriber
        }
        internal enum EventId
        {
            SubmissionSucceeded = 1000,
            SubmissionFailed = 1001,
            ProcessingSucceeded = 1100, 
            ProcessingFailedInvalidData = 1101,
            ProcessingFailedUnhandledException = 1102
        }

        internal enum Status
        {
            Succeeded,
            Failed
        }
    }
}
