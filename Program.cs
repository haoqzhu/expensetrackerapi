using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;
using ExpenseTrackerAPI.Repositories;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();

		services.AddSingleton((s) =>
		{
			var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
			return new CosmosClient(connectionString);
		});

		services.AddSingleton<IExpenseRepository>((s) =>
		{
			var cosmosClient = s.GetRequiredService<CosmosClient>();
			var databaseName = Environment.GetEnvironmentVariable("CosmosDBDatabaseName") ?? "ExpenseTrackerDB";
			var containerName = Environment.GetEnvironmentVariable("CosmosDBContainerName") ?? "Expenses";
			return new CosmosDbRepository(cosmosClient, databaseName, containerName);
		});
	})
	.Build();

host.Run();