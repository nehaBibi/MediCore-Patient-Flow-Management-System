using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace MediCorePatientFlow.Services
{
    public class AuthService
    {
        // Simple in-memory users for demo purposes
        private readonly List<(string Username, string Password, string Role)> _users = new()
        {
            ("reception","pass","Receptionist"),
            ("nurse","pass","Nurse"),
            ("doctor","pass","Doctor")
        };

        public bool ValidateCredentials(string username, string password, out string role)
        {
            var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
            role = user == default ? string.Empty : user.Role;
            return user != default;
        }
    }
}
