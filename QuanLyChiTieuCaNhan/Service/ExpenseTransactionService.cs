using AutoMapper;
using QuanLyChiTieuCaNhan.DTO.ExpenseTransaction;
using QuanLyChiTieuCaNhan.Models;
using QuanLyChiTieuCaNhan.Repository;

namespace QuanLyChiTieuCaNhan.Service
{
    public interface IExpenseTransactionService
    {
        Task<IEnumerable<ExpenseTransactionDto>> GetAllTransactionsByUserAsync(string userId);
        Task<(bool isOverBudget, decimal overAmount, ExpenseTransactionDto transaction)> CreateTransactionAsync(CreateExpenseTransactionDto dto, string userId);
        Task<bool> UpdateTransactionAsync(int id, UpdateExpenseTransactionDto dto, string userId);
        Task<bool> DeleteTransactionAsync(int id, string userId);
        Task<(bool isOverBudget, decimal overAmount)> CheckBudgetAsync(CreateExpenseTransactionDto dto, string userId);
        Task<IEnumerable<ExpenseTransactionDto>> GetTransactionsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ExpenseTransactionDto>> GetTransactionsByFilterAsync(string userId, DateTime? startDate, DateTime? endDate);

    }
    public class ExpenseTransactionService : IExpenseTransactionService
    {
        private readonly IExpenseTransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public ExpenseTransactionService(IExpenseTransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExpenseTransactionDto>> GetAllTransactionsByUserAsync(string userId)
        {
            var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ExpenseTransactionDto>>(transactions);
        }

        public async Task<(bool isOverBudget, decimal overAmount, ExpenseTransactionDto transaction)> CreateTransactionAsync(CreateExpenseTransactionDto dto, string userId)
        {
            var (isOverBudget, overAmount) = await CheckBudgetAsync(dto, userId);

            if (isOverBudget)
            {
                return (true, overAmount, null); // Trả về thông báo vượt ngân sách
            }

            var transaction = _mapper.Map<ExpenseTransaction>(dto);
            transaction.UserId = userId;
            transaction.CreatedDate = DateTime.UtcNow;

            var createdTransaction = await _transactionRepository.AddAsync(transaction);
            var fullTransaction = await _transactionRepository.GetTransactionWithCategoryAsync(createdTransaction.ExpenseTransactionId);

            return (false, 0, _mapper.Map<ExpenseTransactionDto>(fullTransaction));
        }


        public async Task<bool> UpdateTransactionAsync(int id, UpdateExpenseTransactionDto dto, string userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null || transaction.UserId != userId || transaction.IsDelete)
                return false;

            _mapper.Map(dto, transaction);
            transaction.UpdatedDate = DateTime.UtcNow;

            return await _transactionRepository.UpdateAsync(transaction);
        }

        public async Task<bool> DeleteTransactionAsync(int id, string userId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null || transaction.UserId != userId || transaction.IsDelete)
                return false;

            transaction.IsDelete = true;
            return await _transactionRepository.UpdateAsync(transaction);
        }
        public async Task<(bool isOverBudget, decimal overAmount)> CheckBudgetAsync(CreateExpenseTransactionDto dto, string userId)
        {
            var budget = await _transactionRepository.GetRelatedBudgetAsync(dto.CategoryId, dto.TransactionDate, userId);

            if (budget == null)
                return (false, 0); // Không có ngân sách liên quan, không kiểm tra

            var actualSpending = await _transactionRepository.GetActualSpendingAsync(
                dto.CategoryId, budget.StartDate, budget.EndDate, userId
            );

            var totalSpending = actualSpending + dto.Amount;

            if (totalSpending > budget.AmountLimit)
            {
                return (true, totalSpending - budget.AmountLimit); // Vượt ngân sách
            }

            return (false, 0); // Không vượt ngân sách
        }
        public async Task<IEnumerable<ExpenseTransactionDto>> GetTransactionsByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(userId, startDate, endDate);
            return _mapper.Map<IEnumerable<ExpenseTransactionDto>>(transactions);
        }
        public async Task<IEnumerable<ExpenseTransactionDto>> GetTransactionsByFilterAsync(string userId, DateTime? startDate, DateTime? endDate)
        {
            var transactions = await _transactionRepository.GetTransactionsByFilterAsync(userId, startDate, endDate);
            return _mapper.Map<IEnumerable<ExpenseTransactionDto>>(transactions);
        }


    }
}
