using cms.Core.Domain.Royalty;
using cms.Core.Models.Royalty;
using cms.Core.Models;
using cms.Core.SeedWorks;

namespace cms.Core.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction, Guid>
    {
        Task<PagedResult<TransactionDto>> GetAllPaging(string? userName, int fromMonth, int fromYear, int toMonth, int toYear, int pageIndex = 1, int pageSize = 10);
    }
}
