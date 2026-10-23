using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ExpenseTrackerAPI.Models;
using ExpenseTrackerAPI.Repositories;
using System.Net;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ExpenseTrackerAPI.Functions
{
	public class ExpenseFunctions
	{
		private readonly ILogger _logger;
		private readonly IExpenseRepository _repository;

		public ExpenseFunctions(ILoggerFactory loggerFactory, IExpenseRepository repository)
		{
			_logger = loggerFactory.CreateLogger<ExpenseFunctions>();
			_repository = repository;
		}

		[Function("CreateExpense")]
		public async Task<HttpResponseData> CreateExpense(
			[HttpTrigger(AuthorizationLevel.Function, "post", Route = "expenses")] HttpRequestData req)
		{
			_logger.LogInformation("Creating a new expense.");

			try 
			{
				// Read the request body
				string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
				var expense = JsonSerializer.Deserialize<Expense>(requestBody, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				});

				if (expense == null)
				{
					var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
					await badRequest.WriteStringAsync("Invalid expense data");
					return badRequest;
				}

				// Validate required fields
				if (string.IsNullOrEmpty(expense.UserId) || expense.Amount <- 0 || string.IsNullOrEmpty(expense.Category))
				{
					var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
					await badRequest.WriteStringAsync("UserId, Amount, andCategory are required");
					return badRequest;
				}

				// Create the expense
				var createdExpense = await _repository.CreateAsync(expense);

				var response = req.CreateResponse(HttpStatusCode.Created);
				await response.WriteAsJsonAsync(createdExpense);
				return response;
			}
			catch (Exception ex) {
				_logger.LogError($"Error creating expense: {ex.Message}");
				var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
				await errorResponse.WriteStringAsync("An error occurred while creating the expense");
				return errorResponse;
			}
		}

		[Function("GetExpenses")]
    public async Task<HttpResponseData> GetExpenses(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "expenses")] HttpRequestData req)
    {
      _logger.LogInformation("Getting all expenses");

      try
      {
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.Url.Query);
        var userId = query["userId"];

        if (string.IsNullOrEmpty(userId))
        {
          var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
          await badRequest.WriteStringAsync("UserId query parameter is required");
          return badRequest;
        }

        var expenses = await _repository.GetAllAsync(userId);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(expenses);
        return response;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting expenses: {ex.Message}");
        var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
        await errorResponse.WriteStringAsync("An error occurred while retrieving expenses");
        return errorResponse;
      }
    }

    [Function("GetExpenseById")]
		public async Task<HttpResponseData> GetExpenseById(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "expenses/{id}")] HttpRequestData req,
      string id)
		{
			_logger.LogInformation($"Getting expense with id: {id}");

			try
			{
				var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.Url.Query);
        var userId = query["userId"];

				if (string.IsNullOrEmpty(userId))
				{
					var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
					await badRequest.WriteStringAsync("UserId query parameter is required");
					return badRequest;
				}

				var expense = await _repository.GetByIdAsync(id, userId);

				if (expense == null)
				{
					var notFound = req.CreateResponse(HttpStatusCode.NotFound);
					await notFound.WriteStringAsync($"Expense with id {id} not found");
					return notFound;
				}

				var response = req.CreateResponse(HttpStatusCode.OK);
				await response.WriteAsJsonAsync(expense);
				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error getting expense: {ex.Message}");
				var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
				await errorResponse.WriteStringAsync("An error occurred while retrieving the expense");
				return errorResponse;
			}
		}

    [Function("UpdateExpense")]
    public async Task<HttpResponseData> UpdateExpense(
      [HttpTrigger(AuthorizationLevel.Function, "put", Route = "expenses/{id}")] HttpRequestData req,
      string id)
    {
      _logger.LogInformation($"Updating expense with id: {id}");

      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var expense = JsonSerializer.Deserialize<Expense>(requestBody, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        if (expense == null)
        {
          var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
          await badRequest.WriteStringAsync("Invalid expense data");
          return badRequest;
        }

        // Get userId from query parameter
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.Url.Query);
        var userId = query["userId"];

        if (string.IsNullOrEmpty(userId))
        {
          var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
          await badRequest.WriteStringAsync("UserId query parameter is required");
          return badRequest;
        }

        // Check if expense exists
        var existingExpense = await _repository.GetByIdAsync(id, userId);
        if (existingExpense == null)
        {
          var notFound = req.CreateResponse(HttpStatusCode.NotFound);
          await notFound.WriteStringAsync($"Expense with id {id} not found");
          return notFound;
        }

        // Update the expense
        expense.Id = id;
        expense.UserId = userId;
        var updatedExpense = await _repository.UpdateAsync(expense);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(updatedExpense);
        return response;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error updating expense: {ex.Message}");
        var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
        await errorResponse.WriteStringAsync("An error occurred while updating the expense");
        return errorResponse;
      }
    }

    [Function("DeleteExpense")]
    public async Task<HttpResponseData> DeleteExpense(
      [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "expenses/{id}")] HttpRequestData req,
      string id)
    {
      _logger.LogInformation($"Deleting expense with id: {id}");

      try
      {
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.Url.Query);
        var userId = query["userId"];

        if (string.IsNullOrEmpty(userId))
        {
          var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
          await badRequest.WriteStringAsync("UserId query parameter is required");
          return badRequest;
        }

         // Check if expense exists
        var existingExpense = await _repository.GetByIdAsync(id, userId);
        if (existingExpense == null)
        {
          var notFound = req.CreateResponse(HttpStatusCode.NotFound);
          await notFound.WriteStringAsync($"Expense with id {id} not found");
          return notFound;
        }

        await _repository.DeleteAsync(id, userId);

        var response = req.CreateResponse(HttpStatusCode.NoContent);
        return response;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error deleting expense: {ex.Message}");
        var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
        await errorResponse.WriteStringAsync("An error occurred while deleting the expense");
        return errorResponse;
      }
    }

    [Function("GetExpenseSummary")]
    public async Task<HttpResponseData> GetExpenseSummary(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "summary")] HttpRequestData req)
    {
      _logger.LogInformation("Getting expense summary");

      try
      {
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(req.Url.Query);
        var userId = query["userId"];

        if (string.IsNullOrEmpty(userId))
        {
          var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
          await badRequest.WriteStringAsync("UserId query parameter is required");
          return badRequest;
        }

         var expenses = await _repository.GetAllAsync(userId);
        
        // Calculate summary by category
        var summary = expenses
          .GroupBy(e => e.Category)
          .Select(g => new
          {
            Category = g.Key,
            TotalAmount = g.Sum(e => e.Amount),
            Count = g.Count(),
            AverageAmount = g.Average(e => e.Amount)
          })
          .OrderByDescending(s => s.TotalAmount)
          .ToList();

        var totalSpent = expenses.Sum(e => e.Amount);

        var result = new
        {
          TotalSpent = totalSpent,
          ExpenseCount = expenses.Count(),
          CategoryBreakdown = summary
        };

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error getting expense summary: {ex.Message}");
        var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
        await errorResponse.WriteStringAsync("An error occurred while retrieving the expense summary");
        return errorResponse;
      }
    }
	}
}