namespace PetShop.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangeSync(CancellationToken cancellationToken = default);
}

