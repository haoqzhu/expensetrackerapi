using ExpenseTrackerAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTrackerAPI.Repositories
{
  public interface IExpenseRepository
  {
    Task<Expense> CreateAsync(Expense expense);
    Task<Expense> GetByIdAsync(string id, string userId);
    Task<IEnumerable<Expense>> GetAllAsync(string userId);
    Task<Expense> UpdateAsync(Expense expense);
    Task DeleteAsync(string id, string userId);
    Task<IEnumerable<Expense>> GetByCategoryAsync(string userId, string category);
  }
}