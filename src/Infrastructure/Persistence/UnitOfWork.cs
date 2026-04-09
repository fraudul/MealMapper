using Application.Abstractions;
using Application.Abstractions.Data;

namespace Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    //private readonly ApplicationDbContext _context;
    private readonly IApplicationDbContext _dbContext;
    public UnitOfWork(IApplicationDbContext dbContext) => _dbContext = dbContext;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        => _dbContext.SaveChangesAsync(cancellationToken);

    // Transaction методы (реализуй позже, если понадобится)
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RollbackAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Dispose() => _dbContext.Dispose();
}
