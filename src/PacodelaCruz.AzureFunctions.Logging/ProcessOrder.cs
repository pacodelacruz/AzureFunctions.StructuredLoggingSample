
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
        /// Simulates the Order processing and logs events accordingly. 
        /// </summary>
        /// <param name="inMessage">Expects an order in the JSON format, with the 'orderNumber' property</param>
        /// <param name="correlationId">Gets the CorrelationId property from the Service Bus Message</param>
        /// <param name="log"></param>
        [FunctionName("ProcessOrder")]
        public static void Run(
                [ServiceBusTrigger("orders", Connection = "ServiceBus:ConnectionString")] string inMessage,
                string correlationId,
                int deliveryCount,
                string messageId, 
                ILogger log)
        {
            dynamic order = JsonConvert.DeserializeObject(inMessage);
            string orderNumber = order?.orderNumber;

            try
            {
                Process(order);

                //Log a success event if there was no exception. 
                log.LogInformation(new EventId((int)LoggingConstants.EventId.ProcessingSucceeded),
                                    LoggingConstants.Template,
                                    LoggingConstants.EventId.ProcessingSucceeded.ToString(),
                                    LoggingConstants.EntityType.Order.ToString(),
                                    orderNumber,
                                    LoggingConstants.Status.Succeeded.ToString(),
                                    correlationId,
                                    LoggingConstants.CheckPoint.Subscriber.ToString(),
                                    "");
            }
            catch (InvalidDataException ex)
            {
                //Log an error for the corresponding exception type
                log.LogError(new EventId((int)LoggingConstants.EventId.ProcessingFailedInvalidData),
                                    LoggingConstants.Template,
                                    LoggingConstants.EventId.ProcessingFailedInvalidData.ToString(),
                                    LoggingConstants.EntityType.Order.ToString(),
                                    orderNumber,
                                    LoggingConstants.Status.Failed.ToString(),
                                    correlationId,
                                    LoggingConstants.CheckPoint.Subscriber.ToString(),
                                    $"Invalid Data. {ex.Message}");
            
                throw;
            }
            catch (Exception ex)
            {
                //Log an error for an unexcepted exception
                log.LogError(new EventId((int)LoggingConstants.EventId.ProcessingFailedUnhandledException),
                                    LoggingConstants.Template,
                                    LoggingConstants.EventId.ProcessingFailedUnhandledException.ToString(),
                                    LoggingConstants.EntityType.Order.ToString(),
                                    orderNumber,
                                    LoggingConstants.Status.Failed.ToString(),
                                    correlationId,
                                    LoggingConstants.CheckPoint.Subscriber.ToString(),
                                    $"An unexcepted exception occurred. {ex.Message}");

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
