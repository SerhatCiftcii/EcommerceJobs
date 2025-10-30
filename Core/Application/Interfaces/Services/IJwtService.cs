using ECommerceSolution.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceSolution.Core.Application.Interfaces.Services
{
    // JWT token oluşturma işlemleri için sözleşme
    public interface IJwtService
    {
        // Kullanıcı nesnesini alıp bir JWT string'i döndürür.
        string GenerateToken(User user);
    }
}
