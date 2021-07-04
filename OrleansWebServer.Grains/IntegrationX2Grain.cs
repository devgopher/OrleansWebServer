using Orleans;
using OrleansWebServer.Grains.Models;
using System;
using System.Threading.Tasks;

namespace OrleansWebServer.Grains
{
    public class IntegrationX2Grain : Grain, IX2IntegrationGrain
    {
        public async Task<IntegralX2Response> Execute(IntegralX2Request request, GrainCancellationToken cancellationToken = default)
        {
            //IsBusy = true;
            var response = new IntegralX2Response();
            double pr1 = 0.0;
            double pr2 = request.Accuracy * 100;
            double delta = Math.Abs(request.X1 - request.X0) / 100.0;

            while (Math.Abs(pr1-pr2) > request.Accuracy)
            {
                if (cancellationToken != null)
                    if (cancellationToken.CancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine("CANCELLED!");
                        //IsBusy = false;
                        return response;
                    }

                var x = request.X0;
                var y = 0.0;
                pr1 = pr2;

                while (x < request.X1) {
                    if (cancellationToken != null)
                        if (cancellationToken.CancellationToken.IsCancellationRequested)
                        {
                            Console.WriteLine("CANCELLED!");
                            //IsBusy = false;
                            return response;
                        }
                    y += x*x*delta;
                    x += delta;
                }
                
                delta /= 2.0;
                pr2 = y;
                response.Result = pr2;

                Console.WriteLine($"approx result: {pr2}");
            }
            response.Result = pr2;

            //IsBusy = false;
            
            return response;
        }
    }
}
