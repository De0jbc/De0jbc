using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class test
    {
         [FunctionName("Ping_Pong")]
        public static void Ping_Pong([EntityTrigger] IDurableEntityContext ctx ,ILogger log)
        {           
          switch (ctx.OperationName.ToLowerInvariant())
          {
          case "addping":
             ctx.SetState(ctx.GetState<int>() + ctx.GetInput<int>());             
             ctx.SignalEntity(new EntityId(nameof(Ping_Pong), "destinationId"), "addpong",1);             
             log.LogInformation($"Signal Completed.");
             break;
          case "addpong":
             ctx.SetState(ctx.GetState<int>() + ctx.GetInput<int>());                          
             ctx.SignalEntity(new EntityId(nameof(Ping_Pong), "sourceId"), "addping",1);     
             log.LogInformation($"Signal Completed.");             
             break;
          case "get":             
             break;
          }
           
         }         
       
        [FunctionName("RunOrchestrator")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
          var sourceEntity = new EntityId(nameof(Ping_Pong), "sourceId");
          var destinationEntity = new EntityId(nameof(Ping_Pong), "destinationId");    

   // No operations can be performed on either the source or
    // destination accounts until the locks are released.
          for (int i = 0; i < 0; i++)
             {       
            await context.CallEntityAsync(sourceEntity, "addping",1);
            await context.CallEntityAsync(destinationEntity, "addpong",-1);
            }            
        }

 
        [FunctionName("PingPong_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            
            //string instanceId = await starter.StartNewAsync("RunOrchestrator", null);

            //log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            //return starter.CreateCheckStatusResponse(req, instanceId);
            return null;
            
        }
    }
}