using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestCoreConsoleApp
{
    class Program
    {
        private const string JsonMessage = "{\"product\":[{\"name\":\"Product A\",\"price\": \"32\"},{\"name\": \"Product B\",\"price\": \"27\"}]}";
        private const string XmlMessage = "<products><product name=\"Product A\" price=\"32\" /><product name=\"Product B\" price=\"27\" /></products>";
        private const string CustomMessage = "||product|Product A|32||product|Product B|27||";
        private const string TextMessage = "Just a plain text message.";

        static async Task Main(string[] args)
        {
            const string qUrl = "https://sqs.us-east-1.amazonaws.com/066612722316/JD596.fifo";
            // Create the Amazon SQS client
            var sqsClient = new AmazonSQSClient();

            //// (could verify that the queue exists)
            //// Send some example messages to the given queue
            //// A single message
            //await SendMessage(sqsClient, qUrl, JsonMessage);

            //// A batch of messages
            //var batchMessages = new List<SendMessageBatchRequestEntry>
            //{
            //    new SendMessageBatchRequestEntry("xmlMsg", XmlMessage),
            //    new SendMessageBatchRequestEntry("customeMsg", CustomMessage),
            //    new SendMessageBatchRequestEntry("textMsg", TextMessage)
            //};

            //await SendMessageBatch(sqsClient, args[0], batchMessages);

            //// Let the user send their own messages or quit
            //await InteractWithUser(sqsClient, args[0]);

            // Delete all messages that are still in the queue
            await DeleteAllMessages(sqsClient, qUrl);
        }

        //
        // Method to put a message on a queue
        // Could be expanded to include message attributes, etc., in a SendMessageRequest
        private static async Task SendMessage(IAmazonSQS sqsClient, string qUrl, string messageBody)
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = qUrl,
                MessageBody = messageBody,
                MessageGroupId = "RaNdOm123MeSsaGeId",
                MessageDeduplicationId = Guid.NewGuid().ToString(),
                //MessageAttributes = SqsMessageTypeAttribute.CreateAttributes<T>()
            };

            SendMessageResponse responseSendMsg = await sqsClient.SendMessageAsync(sendMessageRequest);

            Console.WriteLine($"Message added to queue\n  {qUrl}");
            Console.WriteLine($"HttpStatusCode: {responseSendMsg.HttpStatusCode}");
        }


        /// <summary>
        ///  Method to delete all messages from the queue
        /// </summary>
        /// <param name="sqsClient"></param>
        /// <param name="qUrl"></param>
        /// <returns></returns>
        private static async Task DeleteAllMessages(IAmazonSQS sqsClient, string qUrl)
        {
            Console.WriteLine($"\nPurging messages from queue\n  {qUrl}...");
            PurgeQueueResponse responsePurge = await sqsClient.PurgeQueueAsync(qUrl);
            Console.WriteLine($"HttpStatusCode: {responsePurge.HttpStatusCode}");
        }
    }
}
