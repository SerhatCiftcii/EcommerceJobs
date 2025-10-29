

using ECommerceSolution.Core.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace ECommerceSolution.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

    
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

       
        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        //  Repository'ler DI (Dependency Injection) ile doğrudan Servislere sağlanacaktır.
    }
}