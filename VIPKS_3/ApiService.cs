using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VIPKS_3
{
    public class ApiService : IDisposable
    {
        public readonly HttpClient _httpClient;
        private const string baseUrl = "https://localhost:7181/api/";
        public static string _token;
        public static bool IsAdmin = false;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            SetToken(_token);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }


        public async Task<List<Student>> GetStudentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Students");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Student>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return new List<Student>();
            }
        }

        public async Task<Student> GetStudentAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Students/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Student>();
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            var response = await _httpClient.PostAsJsonAsync($"Students", student);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Student>();
        }

        public async Task UpdateStudentAsync(int id, Student student)
        {
            var response = await _httpClient.PutAsJsonAsync($"Students/{id}", student);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpCourseStudentsAsync()
        {
            var response = await _httpClient.PutAsync($"Students", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveStudentAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Students/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            var loginRequest = new { Login = login, Password = password };

            var json = JsonConvert.SerializeObject(loginRequest);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseString);

                _token = authResponse.Token;

                SetToken(_token);

                IsAdmin = authResponse.Role == "admin" ? true : false;

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string username, string login, string password)
        {
            var response =
            await _httpClient.PostAsync
            (
                $"Auth/register",
                new StringContent
                (
                    JsonConvert.SerializeObject
                    (
                        new { Username = username, Login = login, Password = password, }
                    ),
                    Encoding.UTF8,
                    "application/json"
                )
            );
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ChangeUserRoleAsync(string login, int newRole)
        {
            try
            {
                var request = new ChangeRoleRequest
                {
                    Login = login,
                    NewRole = newRole
                };

                var response = await _httpClient.PostAsJsonAsync("Auth/change-role", request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else return false;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Users");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<User>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User> GetUserAsync(byte id)
        {
            var response = await _httpClient.GetAsync($"Users/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<User>();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync($"Users", user);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<User>();
        }

        public async Task UpdateUserAsync(byte id, User user)
        {
            var response = await _httpClient.PutAsJsonAsync($"Users/{id}", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveUserAsync(byte id)
        {
            var response = await _httpClient.DeleteAsync($"Users/{id}");
            response.EnsureSuccessStatusCode();
        }
    }



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

    public class ChangeRoleResponse
    {
        public string Message { get; set; }
        public int NewRole { get; set; }
    }
}

