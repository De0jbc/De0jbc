export class Counter {
  constructor(state, env) {
    this.state = state;
    // `blockConcurrencyWhile()` ensures no requests are delivered until
    // initialization completes.
    this.state.blockConcurrencyWhile(async () => {
        let stored = await this.state.storage.get("value");
        this.value =  0;
    })
  }
  // Handle HTTP requests from clients.
  async fetch(request) {
    // Apply requested action.
    let url = new URL(request.url);    
	let currentValue =0;
	this.val = await this.state.storage.get("value");      
    switch (url.pathname) {
    case "/increment":      
	  //currentValue =++this.value;
	  currentValue = this.val;
      ++this.value;	 
      await this.state.storage.put("value", this.value);
      break;
	case "/reset":  
       currentValue=0; 
       this.value=0;	  	  
      await this.state.storage.put("value", this.value);
      break;
    case "/decrement":      
	  currentValue =--this.value;
      await this.state.storage.put("value", this.value);
      break;	
    case "/":
      // Just serve the current value. No storage calls needed!
      break;
    default:
      return new Response("Not found", {status: 404});
    }    
    return new Response(currentValue);
  }
}