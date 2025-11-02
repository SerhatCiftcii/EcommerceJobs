// ECommerceSolution.Infrastructure/Persistence/Repositories/UserRepository.cs

using ECommerceSolution.Core.Application.Interfaces.Repositories;
using ECommerceSolution.Core.Domain.Entities;
using ECommerceSolution.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ECommerceSolution.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            // E-posta benzersiz olmalı, FirstOrDefaultAsync kullanmak güvenli
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRegistrationDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // startDate dahil, endDate'in bir sonraki gününe kadar (yani endDate günü dahil)
            return await _context.Users
                                 .Where(u => u.CreatedAt >= startDate && u.CreatedAt < endDate.AddDays(1))
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }
}