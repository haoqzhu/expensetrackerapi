using ExpenseTrackerAPI.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTrackerAPI.Repositories
{
  public class CosmosDbRepository : IExpenseRepository
  {
    private readonly Container _container;

    public CosmosDbRepository(CosmosClient cosmosClient, string databaseName, string containerName)
    {
      _container = cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
      var response = await _container.CreateItemAsync(expense, new PartitionKey(expense.UserId));
      return response.Resource;
    }

    public async Task<Expense> GetByIdAsync(string id, string userId)
    {
      try
      {
        var response = await _container.ReadItemAsync<Expense>(id, new PartitionKey(userId));
        return response.Resource;
      }
      catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        return null;
      }
    }

    public async Task<IEnumerable<Expense>> GetAllAsync(string userId)
    {
      var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
        .WithParameter("@userId", userId);

      var iterator = _container.GetItemQueryIterator<Expense>(query);
      var results = new List<Expense>();

      while (iterator.HasMoreResults)
      {
        var response = await iterator.ReadNextAsync();
        results.AddRange(response);
      }

      return results;
    }

    public async Task<Expense> UpdateAsync(Expense expense)
    {
      var response = await _container.ReplaceItemAsync(expense, expense.Id, new PartitionKey(expense.UserId));
      return response.Resource;
    }

    public async Task DeleteAsync(string id, string userId)
    {
      await _container.DeleteItemAsync<Expense>(id, new PartitionKey(userId));
    }

    public async Task<IEnumerable<Expense>> GetByCategoryAsync(string userId, string category)
    {
      var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId AND c.category = @category")
        .WithParameter("@userId", userId)
        .WithParameter("@category", category);

      var iterator = _container.GetItemQueryIterator<Expense>(query);
      var results = new List<Expense>();

      while (iterator.HasMoreResults)
      {
        var response = await iterator.ReadNextAsync();
        results.AddRange(response);
      }

      return results;
    }
  }
}