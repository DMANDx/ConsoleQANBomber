using NBomber.Configuration;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using System.Net.Http;

namespace ConsoleQANBomber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //GetMain();
            GetStep();
        }

        private static void GetMain() 
        {
            using var httpClient = new HttpClient();

            var scenario = Scenario.Create("hello_world_scenario", async context =>
            {
                var response = await httpClient.GetAsync("https://localhost:7021/OrderDetails");
                context.Logger.Fatal("my login");
                return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                //скорость инъекции (10 запросов в секунду)
                Simulation.Inject(rate: 10,                                  
                                  interval: TimeSpan.FromSeconds(1),                                  
        //продолжительность инъекции трафика (30 секунд)
        during: TimeSpan.FromSeconds(30))
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }

        private static void GetStep() 
        {
            var scenario = Scenario.Create("my_step_scenario", async context =>
            {
                using var httpClient = new HttpClient();

                var step1 = await Step.Run("login", context, async () =>
                {
                    //var response = await httpClient.GetAsync("https://localhost:7021/OrderDetails");
                    var response = await httpClient.GetAsync("https://www.google.com");
                    context.Logger.Information("my login");
                    await Task.Delay(1000);
                    return Response.Ok();
                });

                var step2 = await Step.Run("open_home_page", context, async () =>
                {
                    context.Logger.Information("my open home page");
                    var response = await httpClient.GetAsync("https://localhost:7021/OrderDetails");
                    await Task.Delay(1_000);
                    return Response.Ok();
                });

                var step3 = await Step.Run("logout", context, async () =>
                {
                    //your logic...
                    context.Logger.Information("my logout");
                    await Task.Delay(1_000);
                    return Response.Ok();
                }) ;

                return Response.Ok();

            }).WithLoadSimulations(
                //скорость инъекции (10 запросов в секунду)
                Simulation.Inject(rate: 10,
                                  interval: TimeSpan.FromSeconds(1),
                //продолжительность инъекции трафика (30 секунд)
                during: TimeSpan.FromSeconds(30))
            ); ;  NBomberRunner
                .RegisterScenarios(scenario)
                .Run(); ;
        }

    }
}
