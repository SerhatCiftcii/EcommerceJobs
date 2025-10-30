using Application.Interfaces.Services;
using ECommerceSolution.Core.Application.Interfaces.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceSolution.Infrastructure.Services
{
   
    public class PasswordHasher : IPasswordHasher
    {
        // Basit SHA256 Hashleme
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // Sağlanan şifreyi hash'le ve saklanan hash ile karşılaştır
            return HashPassword(providedPassword) == hashedPassword;
        }
    }
}