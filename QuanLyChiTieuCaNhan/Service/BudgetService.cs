using AutoMapper;
using QuanLyChiTieuCaNhan.DTO.Budget;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository;

namespace QuanLyChiTieuCaNhan.Service
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetDto>> GetBudgetsByUserAsync(string userId);
        Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto dto, string userId);
        Task<bool> UpdateBudgetAsync(int id, UpdateBudgetDto dto, string userId);
        Task<bool> DeleteBudgetAsync(int id, string userId);
        Task<BudgetDto> GetBudgetDetailsAsync(int id, string userId);
        Task<decimal> CalculateActualSpendingAsync(int budgetId, string userId);
    }
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IExpenseTransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public BudgetService(IBudgetRepository budgetRepository, IExpenseTransactionRepository transactionRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BudgetDto>> GetBudgetsByUserAsync(string userId)
        {
            var budgets = await _budgetRepository.GetBudgetsByUserIdAsync(userId);
            var budgetDtos = _mapper.Map<IEnumerable<BudgetDto>>(budgets);

            foreach (var budgetDto in budgetDtos)
            {
                budgetDto.ActualSpending = await CalculateActualSpendingAsync(budgetDto.Id, userId);
                budgetDto.IsOverBudget = budgetDto.ActualSpending > budgetDto.AmountLimit;
            }

            return budgetDtos;
        }

        public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto dto, string userId)
        {
            var budget = _mapper.Map<Budget>(dto);
            budget.UserId = userId;

            var createdBudget = await _budgetRepository.AddAsync(budget);
            return _mapper.Map<BudgetDto>(createdBudget);
        }

        public async Task<bool> UpdateBudgetAsync(int id, UpdateBudgetDto dto, string userId)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            if (budget == null || budget.UserId != userId || budget.IsDeleted)
                return false;

            _mapper.Map(dto, budget);
            return await _budgetRepository.UpdateAsync(budget);
        }

        public async Task<bool> DeleteBudgetAsync(int id, string userId)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            if (budget == null || budget.UserId != userId || budget.IsDeleted)
                return false;

            budget.IsDeleted = true;
            return await _budgetRepository.UpdateAsync(budget);
        }

        public async Task<BudgetDto> GetBudgetDetailsAsync(int id, string userId)
        {
            var budget = await _budgetRepository.GetBudgetWithDetailsAsync(id, userId);
            if (budget == null) return null;

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            budgetDto.ActualSpending = await CalculateActualSpendingAsync(id, userId);
            budgetDto.IsOverBudget = budgetDto.ActualSpending > budgetDto.AmountLimit;

            return budgetDto;
        }

        public async Task<decimal> CalculateActualSpendingAsync(int budgetId, string userId)
        {
            var transactions = await _transactionRepository.GetTransactionsByBudgetIdAsync(budgetId, userId);
            return transactions.Sum(t => t.Amount);
        }
    }
}
