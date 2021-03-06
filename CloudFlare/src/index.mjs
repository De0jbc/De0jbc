// Worker
export { Counter } from './counter.mjs'

export default {
  async fetch(request, env) {

    return await handleRequest(request, env);
  }
}

async function handleRequest(request, env) {
  let timeElapsed = Date.now();
  let today = new Date(timeElapsed);
  let txt  =" Start Test Init URL " +request.url+"\n, URL"+today.toISOString()+"\n";  	
  let id = env.COUNTER.idFromName("A");
  let obj = env.COUNTER.get(id);
  let resp =await obj.fetch(request.url);
  let count =await resp.text();  
  let numbersignals =800;
  timeElapsed = Date.now();
  today = new Date(timeElapsed);
  //txt  =txt+ " Start Test Init Val " +count+"\n, Init TimeStamp"+today.toISOString()+"\n";  
  let readdo = new Date(Date.now());
  
  for (let i = 1; i < numbersignals+1; i++) {	    
    today = new Date(Date.now());
	//txt = txt +"Signal" +i+ " Start " + today.toISOString();
    resp = await obj.fetch("https://javierberlinches.javier-berlinches.workers.dev/increment");
    count = await resp.text();
    today = new Date(Date.now());
	//txt = txt +", end " + today.toISOString();
	//txt =txt+", val "+count+"\n";
  } 
    
  let fin = new Date(Date.now());
  txt  =txt+" End Test " +fin.toISOString()+ "," ; 
  txt  =txt+" Throughput " + numbersignals/((Date.now() - timeElapsed)/1000)+ " Total signal "+numbersignals+", signals per second \n" ;   
  return new Response(txt);
}
