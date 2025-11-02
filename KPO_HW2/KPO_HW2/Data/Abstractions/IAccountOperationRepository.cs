using KPO_HW2.Data.Models;

namespace KPO_HW2.Data.Abstractions;

internal interface IAccountOperationRepository : IRepository<AccountOperation, AccountOperationId>
{
}