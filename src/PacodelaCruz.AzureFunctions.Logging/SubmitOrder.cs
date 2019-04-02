
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PacodelaCruz.AzureFunctions.Logging
{
    public static class SubmitOrder
    {
        /// <summary>
        /// Sample Function to show structured and correlated logging on Azure Functions using ILogger.
        /// Triggered by an Http post and drops a message into a Service Bus message queue.
        /// Checks whether an order is valid and logs an event accordingly
        /// </summary>
        /// <param name="req">Expects an order in the JSON format, with the 'orderNumber' property</param>
        /// <param name="outMessage"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("SubmitOrder")]
        public static async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order")]
                HttpRequest req, 
                [ServiceBus("orders", Connection = "ServiceBus:ConnectionString" , EntityType = EntityType.Queue)] IAsyncCollector<Message> outMessages,
                ILogger log)
        {
            string orderAsJson = new StreamReader(req.Body).ReadToEnd();
            dynamic order = JsonConvert.DeserializeObject(orderAsJson);
            string orderNumber = order?.orderNumber;
            string correlationId = Guid.NewGuid().ToString();

            if (IsOrderValid(order))
            {
                Message outMessage = new Message(Encoding.ASCII.GetBytes(orderAsJson))
                {
                    // Set the Service Bus Message CorrelationId property for correlation in the subscriber function
                    CorrelationId = correlationId
                };
                await outMessages.AddAsync(outMessage);

                log.LogInformation(new EventId((int)LoggingConstants.EventId.SubmissionSucceeded),
                                    LoggingConstants.Template,
                                    LoggingConstants.EventId.SubmissionSucceeded.ToString(),
                                    LoggingConstants.EntityType.Order.ToString(),
                                    orderNumber,
                                    LoggingConstants.Status.Succeeded.ToString(),
                                    correlationId,
                                    LoggingConstants.CheckPoint.Publisher.ToString(),
                                    "");

                return new OkResult();
            }
            else
            {
                log.LogError(new EventId((int)LoggingConstants.EventId.SubmissionFailed), 
                                    LoggingConstants.Template, 
                                    LoggingConstants.EventId.SubmissionFailed.ToString(), 
                                    LoggingConstants.EntityType.Order.ToString(), 
                                    orderNumber, 
                                    LoggingConstants.Status.Failed.ToString(), 
                                    correlationId, 
                                    LoggingConstants.CheckPoint.Publisher.ToString(), 
                                    "Order is not valid and cannot be sent for processing.");
                
                return new BadRequestObjectResult("Order is not Valid");
            }
        }

        /// <summary>
        /// Returns whether an Order is Valid or not based on a random number
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private static bool IsOrderValid(object order)
        {
            Random random = new Random();
            return random.Next(0, 5) == 4 ? false : true;
        }
    }
}
