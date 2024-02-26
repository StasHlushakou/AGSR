using Newtonsoft.Json;
using PatientsAPI.Dtos;
using PatientsAPI.Enums;

namespace ConsoleGeneratePatients
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                for (int i = 0; i < 100; i++)
                {
                    var content = new PatientDto()
                    {
                        Name = new Name()
                        {
                            Use = $"official{i}",
                            Family = $"Иванов{i}",
                            Given = new string[] { $"Иван{i}", $"Иванович{i}" },
                        },
                        Gender = i % 4.0 == 0 ? Gender.Unknown : i % 4.0 == 1 ? Gender.Male : i % 4.0 == 2 ? Gender.Female : Gender.Other,
                        BirthDate = new DateTime(2000 + i, 6, 15),
                        Active = i % 2 == 0 ? Active.True : Active.False,
                    };

                    string json = JsonConvert.SerializeObject(content);
                    StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    using var response = client.PostAsync("http://localhost:8002/api/Patient", httpContent).GetAwaiter().GetResult();
                }

            }
        }
    }
}
