
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace PacodelaCruz.AzureFunctions.Logging
{
    public static class ProcessOrder
    {
        /// <summary>
        /// Sample Function to show structured and correlated logging on Azure Functions using ILogger.
        /// Triggered by a message on a Service Bus Queue.
        /// </summary>
        /// <param name="inMessage"></param>
        /// <param name="correlationId"></param>
        /// <param name="log"></param>
        [FunctionName("ProcessOrder")]
        public static void Run(
                [ServiceBusTrigger("orders", Connection = "ServiceBus:ConnectionString")] string inMessage,
                string correlationId,
                ILogger log)
        {

            dynamic order = JsonConvert.DeserializeObject(inMessage);
            string orderNumber = order?.orderNumber;

            try
            {
                Process(order);

                log.LogInformation(new EventId((int)LoggingConstants.EventId.ProcessingSucceeded),
                                    LoggingConstants.Template,
                                    LoggingConstants.EventId.ProcessingSucceeded.ToString(),
                                    LoggingConstants.EntityType.Order.ToString(),
                                    orderNumber,
                                    LoggingConstants.Status.Succeeded.ToString(),
                                    correlationId,
                                    LoggingConstants.TrackingEventType.Subscriber.ToString(),
                                    "");
            }
            catch (InvalidDataException ex)
            {
                log.LogError(new EventId((int)LoggingConstants.EventId.ProcessingFailedInvalidData),
                                    LoggingConstants.Template,
                                    LoggingConstants.EventId.ProcessingFailedInvalidData.ToString(),
                                    LoggingConstants.EntityType.Order.ToString(),
                                    orderNumber,
                                    LoggingConstants.Status.Failed.ToString(),
                                    correlationId,
                                    LoggingConstants.TrackingEventType.Subscriber.ToString(),
                                    $"Invalid Data. {ex.Message}");
            
                throw;
            }
            catch (Exception ex)
            {
                log.LogError(new EventId((int)LoggingConstants.EventId.ProcessingFailedUnhandledException),
                                    LoggingConstants.Template,
                                    LoggingConstants.EventId.ProcessingFailedUnhandledException.ToString(),
                                    LoggingConstants.EntityType.Order.ToString(),
                                    orderNumber,
                                    LoggingConstants.Status.Failed.ToString(),
                                    correlationId,
                                    LoggingConstants.TrackingEventType.Subscriber.ToString(),
                                    $"An unhandled exception occurred. {ex.Message}");

                throw;
            }
        }

        /// <summary>
        /// Simulate a processing of an order which can throw exceptions in certain conditions
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private static void Process(object order)
        {
            Random random = new Random();
            int randomOutput = random.Next(0, 5);
            if (randomOutput == 4)
            {
                throw new InvalidDataException("Required fields are null or empty");
            }
            else if (randomOutput == 1)
            {
                throw new Exception("Catasfrofic failure!!!");
            }
        }
    }
}
