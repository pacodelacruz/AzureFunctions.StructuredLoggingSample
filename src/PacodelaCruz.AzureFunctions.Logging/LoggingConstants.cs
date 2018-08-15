namespace PacodelaCruz.AzureFunctions.Logging
{
    /// <summary>
    /// Contains constants and enums for consistent structured logging
    /// </summary>
    internal static class LoggingConstants
    {
        // Template for consisted structured logging accross multiple functions, each field is described below: 
        // EventDescription is a short description of the Event being logged. 
        // EntityType: Business Entity Type being processed: e.g. Order, Shipment, etc.
        // EntityId: Id of the Business Entity being processed: e.g. Order Number, Shipment Id, etc. 
        // Status: Status of the Log Event, e.g. Succeeded, Failed, Discarded.
        // CorrelationId: Unique identifier of the message that can be processed by more than one component. 
        // TrackingEventType: To classify and be able to correlate tracking events.
        // Description: A detailed description of the log event. 
        internal const string Template = "{EventDescription}, {EntityType}, {EntityId}, {Status}, {CorrelationId}, {TrackingEventType}, {Description}";

        internal enum EntityType
        {
            Order,
            Shipment
        }
        internal enum TrackingEventType
        {
            Publisher,
            Subscriber
        }
        /// <summary>
        /// Enumeration of all different EventId that can be used for logging
        /// </summary>
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
            Failed,
            Discarded
        }
    }
}
