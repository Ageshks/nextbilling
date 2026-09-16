using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BizStock.Infrastructure.Persistence;

/// <summary>Unit of work with ambient transaction support (nested calls join the outer transaction).</summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _ambient;

    /// <summary>Creates the unit of work.</summary>
    public UnitOfWork(AppDbContext context) => _context = context;

    /// <inheritdoc />
    public IAppDbContext Context => _context;

    /// <inheritdoc />
    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default)
    {
        if (_ambient is not null)
        {
            // Join the ambient transaction; commit is owned by the outermost caller.
            return await action(ct);
        }

        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        _ambient = tx;
        try
        {
            var result = await action(ct);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return result;
        }
        finally
        {
            _ambient = null;
        }
    }

    /// <inheritdoc />
    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
    {
        if (_ambient is not null)
        {
            await action(ct);
            return;
        }

        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        _ambient = tx;
        try
        {
            await action(ct);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        finally
        {
            _ambient = null;
        }
    }
}
