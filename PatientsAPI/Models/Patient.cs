using PatientsAPI.Dtos;
using PatientsAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Xml.Linq;

namespace PatientsAPI.Models
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Use { get; set; }

        [Required]
        public string Family { get; set; }

        public string Given { get; set; }

        public Gender Gender { get; set; } = Gender.Unknown;

        [Required]
        public DateTime BirthDate { get; set; }

        public Active Active { get; set; } = Active.True;

        public Patient() { }

        public Patient (PatientDto patientDto)
        {
            Use = patientDto.Name.Use;
            Family = patientDto.Name.Family;
            Given = JsonSerializer.Serialize(patientDto.Name.Given);
            Gender = patientDto.Gender;
            BirthDate = patientDto.BirthDate;
            Active = patientDto.Active;
        }

        public void UpdatePatient(PatientDto patientDto)
        {
            this.Use = patientDto.Name.Use;
            this.Family = patientDto.Name.Family;
            this.Given = JsonSerializer.Serialize(patientDto.Name.Given);
            this.Gender = patientDto.Gender;
            this.BirthDate = patientDto.BirthDate;
            this.Active = patientDto.Active;
        }
    }
}
