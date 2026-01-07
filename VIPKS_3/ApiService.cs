using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VIPKS_3.ModelsDB;

namespace VIPKS_3
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string baseUrl = "https://localhost:7181/api/";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
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
    }
}
