using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIPKS_3.Classes
{
    public class Student
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Group { get; set; } = null!;

        public int Course { get; set; }

        public string StudyForm { get; set; } = null!;

        public DateTime AdmissionDate { get; set; }

        public bool IsActive { get; set; }
    }

    public class User
    {
        public byte UserId { get; set; }

        public string? Username { get; set; }

        public string? UserLogin { get; set; }

        public string? UserPassword { get; set; }

        public byte? UserRole { get; set; }

        public virtual Role? UserRoleNavigation { get; set; }
    }

    public class Role
    {
        public byte RoleId { get; set; }

        public string? RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }

    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Login { get; set; }

        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime Expiration { get; set; }
    }

    public class ChangeRoleRequest
    {
        public string Login { get; set; }
        public int NewRole { get; set; }
    }
}
