using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace Company.Function
{
    public static class TestEntities
    {
        
        [FunctionName("Counter")]
        public static void Counter([EntityTrigger] IDurableEntityContext ctx ,ILogger log)
        { 
          switch (ctx.OperationName.ToLowerInvariant())
          {
          case "add":
             ctx.SetState(ctx.GetState<int>() + ctx.GetInput<int>());             
             break;
          case "reset":
            ctx.SetState(0);            
            break;
          case "get":
            ctx.Return(ctx.GetState<int>());
            break;
          }
         }
        
        [FunctionName("TestEntities")]        
        public static async Task<List<string>> OchestratorEntities(
            [OrchestrationTrigger] IDurableOrchestrationContext context,ILogger log)
                    {                               
          var parallelTasks = new List<Task>();
          var outputs = new List<string>();
          outputs.Add("Start Orchestrator");        
          context.SetCustomStatus("Start Orchestrator");

          var stopwatch = new Stopwatch();
          stopwatch.Start();   
        
          var entityId = new EntityId(nameof(Counter), "myCounter"); 
          int number_wait = 1000;
          for  (int j = 0; j < number_wait; j++) {              
             Task task= context.CallEntityAsync(entityId, "add",1);
             parallelTasks.Add(task);
          }        
          await Task.WhenAll(parallelTasks);
          var elapsed_time = stopwatch.ElapsedMilliseconds;  
          int currentval = await context.CallEntityAsync<int>(entityId, "get");                          
          outputs.Add("End Orchestrator "+context.InstanceId+" ;"+number_wait+" signals current val ;" +currentval+" ,elapsed time ;"+elapsed_time+"; ms.");       
          context.SetCustomStatus("End Orchestrator ;"+context.InstanceId+","+number_wait+" signals current val ;" +currentval+" ,elapsed time ;"+elapsed_time+" ms." );
          log.LogInformation("******End Orchestrator ;"+context.InstanceId+","+number_wait+" signals current val ;" +currentval+" ,elapsed time ;"+elapsed_time+" ms." );             

          return outputs;  
        }


        [FunctionName("SubOrchestratorEntities")]
        public static async Task<List<string>> RunSubOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,ILogger log)
        { 
          var parallelTasks = new List<Task>();        
          var outputs = new List<string>();
          var stopwatch = new Stopwatch();
          stopwatch.Start();   
          int number_orchestrators = 10;
          for  (int j = 0; j < number_orchestrators; j++) {                         
           Task task = context.CallSubOrchestratorAsync("TestEntities",null);
           parallelTasks.Add(task);
          }       
          return outputs;            
        }
        

        [FunctionName("TestEntities_HttpStart")]

        public static async Task<HttpResponseMessage> TestEntities_HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient context,
            ILogger log            
           )
       {             
        
          string instanceId = await context.StartNewAsync("TestEntities", null);
          DurableOrchestrationStatus durableOrchestrationStatus = await context.GetStatusAsync(instanceId);
          string stat = durableOrchestrationStatus.CustomStatus.ToString();          
         

          return context.CreateCheckStatusResponse(req, instanceId);          
        }   

        [FunctionName("Counter_HttpStart")]

        public static async Task<HttpResponseMessage> Counter_HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get","post")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client         
           )
       {    
        var entityId = new EntityId(nameof(Counter), "myCounter");     
        Task task= client.SignalEntityAsync(entityId, "add",1);           
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        return httpResponseMessage;          
        } 

        [FunctionName("SubOrchestrator_HttpStart")]

        public static async Task<HttpResponseMessage> SubOrchestrator_HttpStart(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get","post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient context,
            ILogger log       
           )
       {    
         string instanceId = await context.StartNewAsync("SubOrchestratorEntities", null);
          DurableOrchestrationStatus durableOrchestrationStatus = await context.GetStatusAsync(instanceId);
          string stat = durableOrchestrationStatus.CustomStatus.ToString();          
          return context.CreateCheckStatusResponse(req, instanceId);       
        }         

    }
}