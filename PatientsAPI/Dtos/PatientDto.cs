using PatientsAPI.Enums;
using PatientsAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PatientsAPI.Dtos
{
    /// <summary>
    /// Dto for patient entity.
    /// </summary>
    public class PatientDto
    {
        [Required]
        public Name Name { get; set; }

        public Gender Gender { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        public Active Active { get; set; }

        public PatientDto() { }

        public PatientDto(Patient patient) {
            Name = new Name()
            {
                Id = patient.Id,
                Use = patient.Use,
                Family = patient.Family,
                Given = JsonSerializer.Deserialize<string[]>(patient.Given) ?? new string[] { },
            };
            Gender = patient.Gender;
            BirthDate = patient.BirthDate;
            Active = patient.Active;
        }
    }

    /// <summary>
    /// Dto for name entity.
    /// </summary>
    public class Name
    {
        public Guid Id { get; set; }

        public string Use { get; set; }

        [Required]
        public string Family { get; set; }

        public string[] Given { get; set; }

    }
}
