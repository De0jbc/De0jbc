using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Diagnostics;

namespace Company.Function
{
    public static class TestTrigger
    {
        [FunctionName("TestTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get","post")] HttpRequest req,
            [DurableClient] IDurableEntityClient client, 
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");                
            /*
                var parallelTasks = new List<Task>();
                var parallelEntityId = new List<EntityId>();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for  (int j = 0; j < 1; j++) {
                    var entityId = new EntityId(nameof(TestEntities.Counter), "myCounter"+j); 
                    parallelEntityId.Add(entityId);
                }
                foreach (EntityId a in parallelEntityId) {
                    //Task task = client.SignalEntityAsync(entityId, "Add", 1);
                     for (int i = 0; i < 1; i++)
                    {
                       await client.SignalEntityAsync(a, "add",1);                                          
                    }   
                }
                 var elapsed_time = stopwatch.Elapsed;             
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                elapsed_time.Hours, elapsed_time.Minutes, elapsed_time.Seconds,
                elapsed_time.Milliseconds / 10);          
                log.LogInformation($"+++++++++++++++End Trigger for 10 Entities and 10 of signals(10 per Entity):"+elapsedTime);             
                return null;
            */
            return null;   
 
        }
    }
}
