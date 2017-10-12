////////////////////////////////////////////////////////
// Copyright (c) 2017 Sameer Khandekar                //
// License: MIT License.                              //
////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace GarageDoorRESTAPI.Storage
{
    /// <summary>
    /// Wrapper utility for queue
    /// </summary>
    internal class QueueUtil
    {
        /// <summary>
        /// Constructor creates client
        /// </summary>
        public QueueUtil()
        {
            _queue = CreateCloudQueueClient();
        }

        /// <summary>
        /// Given the string, creates queue message and adds to queue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task AddToQueue(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var cloudMessage = new CloudQueueMessage(id);

            await _queue.AddMessageAsync(cloudMessage);
            Debug.WriteLine($"Message Q count {_queue.ApproximateMessageCount}");
        }

        /// <summary>
        /// Wait for message by polling
        /// </summary>
        /// <param name="id"></param>
        /// <param name="milliSecTimeout"></param>
        /// <returns></returns>
        internal async Task<string> WaitForMessage(string id, int milliSecTimeout)
        {
            // validate parameter
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            // ensure that timeout is valid
            if (milliSecTimeout < PollIntervalMSec)
            {
                throw new ArgumentOutOfRangeException($"Value of {nameof(milliSecTimeout)} must be greater than {PollIntervalMSec}");
            }

            var ticksNow = DateTime.Now.Ticks;
            var endTicks = ticksNow + (milliSecTimeout* 10000);

            // wait in the loop till timeout or the message shows up in the queue
            while (DateTime.Now.Ticks < endTicks)
            {
                // get the message
                var message = await _queue.GetMessageAsync();
                Debug.WriteLine(message?.AsString);
               if (message != null)
                {
                    // if it is the message that the method was waiting for
                    // delete it from the queue and return the message
                    if (message.AsString?.StartsWith(id) ?? false)
                    {
                        await _queue.DeleteMessageAsync(message);
                        return message.AsString;
                    }
                }

                await Task.Yield();
                await Task.Delay(PollIntervalMSec);
            }

            return string.Empty;
        }

        /// <summary>
        /// This is called in the start to ensure that queue is created
        /// </summary>
        /// <returns></returns>
        internal static bool EnsureCreated()
        {
            try
            {
                var queue = CreateCloudQueueClient();
                queue.CreateIfNotExists();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// creates client
        /// </summary>
        /// <returns></returns>
        private static CloudQueue CreateCloudQueueClient()
        {
            var connString = ConfigurationManager.AppSettings[ConnectionStringKey];
            var storageClient = CloudStorageAccount.Parse(connString);
            return storageClient.CreateCloudQueueClient().GetQueueReference(QueueName);
        }

        private CloudQueue _queue;

        internal const string QueueName = "YOUR QUEUE NAME";
        internal const string ConnectionStringKey = "AzureStorageConnectionKey";

        private const int PollIntervalMSec = 10;
    }
}