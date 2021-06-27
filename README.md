# OrleansWebServer
Represents a web server based on MS Orleans framework and intended for parallelizing requests processing to different grains and SILO's 

# Usage
For executing your requests you should make several steps:
1. Define Request/Response classes (for example, MyRequest, MyResponse)
2. Define your grains and interface for it (for example, MyGrain, IMyGrain) 
3. In a startup.cs you should register your request/response/grai in a such way:
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddOrleansWebServerForRequest<MyRequest, NyResponse, MyGrain>(Configuration);
    }

4. Your grain should inherit from Orleans Grain class and implement your defined interface
5. Your grain interface should inherit from IWebServerBackendGrain<MyRequest, MyResponse> interface
 