using Microsoft.AspNetCore.Mvc;
using PatientsAPI.Dtos;
using PatientsAPI.Models;
using System.Text.RegularExpressions;

namespace PatientsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PatientController : Controller
    {
        private readonly PatienDbContext _patienDbContext;

        public PatientController(PatienDbContext patienDbContext)
        {
            _patienDbContext = patienDbContext;
        }

        /// <summary>
        /// Return patient by Id.
        /// </summary>
        /// <param name="patienId"></param>
        /// <returns>PatientDto</returns>
        /// <response code="200">Returns finded patient.</response>
        /// <response code="404">If the patient not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{patienId:Guid}")]
        public async Task<ActionResult<PatientDto>> GetById(Guid patienId)
        {
            var patient = await _patienDbContext.Patients.FindAsync(patienId);
            if (patient == null) return NotFound(); 
            return new PatientDto(patient);
        }

        /// <summary>
        /// Create new Patient.
        /// </summary>
        /// <param name="patientDto"></param>
        /// <returns></returns>
        /// <response code="200">Patient successfully created.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatientDto patientDto)
        {
            await _patienDbContext.Patients.AddAsync(new Patient(patientDto));
            await _patienDbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Update patient by Id.
        /// </summary>
        /// <param name="patientDto"></param>
        /// <returns></returns>
        /// <response code="200">Patient successfully updated.</response>
        /// <response code="404">If the patient not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PatientDto patientDto)
        {
            var patient = await _patienDbContext.Patients.FindAsync(patientDto.Name.Id);
            if (patient == null) return NotFound();
            patient.UpdatePatient(patientDto);
            _patienDbContext.Patients.Update(patient);
            await _patienDbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Delete patient by Id.
        /// </summary>
        /// <param name="patienId"></param>
        /// <returns></returns>
        /// <response code="200">Patient successfully deleted.</response>
        /// <response code="404">If the patient not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{patienId:Guid}")]
        public async Task<IActionResult> Delete(Guid patienId)
        {
            var patient = await _patienDbContext.Patients.FindAsync(patienId);
            if (patient == null) return NotFound();
            _patienDbContext.Patients.Remove(patient);
            await _patienDbContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Return all patients with matching birthDate.
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns>PatientDto[]</returns>
        /// <response code="200">Return all patients with matching birthDate or empty array.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult<IEnumerable<PatientDto>> GetPatients([FromQuery] string[] birthDate)
        {
            var forFiltration = _patienDbContext.Patients.AsQueryable();

            foreach(var date in birthDate)
            {
                forFiltration = AddFilter(forFiltration, date);
            }
            return forFiltration.Select(p => new PatientDto(p)).ToArray();
        }

        private IQueryable<Patient> AddFilter(IQueryable<Patient> forFiltration, string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return forFiltration;
            }

            // 6 - minimal valid length = 2 symbols for prefix and 4 symbols for year
            if (date.Length < 6)
            {
                throw new Exception("Invalid birthDate format");
            }

            var onlyYear = Regex.IsMatch(date, onlyYearPattern);
            var onlyMonth = Regex.IsMatch(date, onlyMonthPattern);
            var onlyDay = Regex.IsMatch(date, onlyDayPattern);
            var onlyMinutes = Regex.IsMatch(date, onlyMinutesPattern);
            var onlySeconds = Regex.IsMatch(date, onlySecondsPattern);

            var parsedDate = ParseDate(date.Substring(2));

            switch(date.Substring(0, 2).ToLower()) {
                case "eq":
                case "ap":
                    forFiltration = onlySeconds ? forFiltration.Where(p => p.BirthDate >= parsedDate && p.BirthDate < parsedDate.AddSeconds(1))
                        : onlyMinutes ? forFiltration.Where(p => p.BirthDate >= parsedDate && p.BirthDate < parsedDate.AddMinutes(1))
                            : onlyDay ? forFiltration.Where(p => p.BirthDate >= parsedDate && p.BirthDate < parsedDate.AddDays(1))
                                : onlyMonth ? forFiltration.Where(p => p.BirthDate >= parsedDate && p.BirthDate < parsedDate.AddMonths(1))
                                    : onlyYear ? forFiltration.Where(p => p.BirthDate >= parsedDate && p.BirthDate < parsedDate.AddYears(1))
                                        : forFiltration.Where(p => p.BirthDate == parsedDate);
                    break;

                case "ne":
                    forFiltration = onlySeconds ? forFiltration.Where(p => p.BirthDate < parsedDate || p.BirthDate >= parsedDate.AddSeconds(1))
                        : onlyMinutes ? forFiltration.Where(p => p.BirthDate < parsedDate || p.BirthDate >= parsedDate.AddMinutes(1))
                            : onlyDay ? forFiltration.Where(p => p.BirthDate < parsedDate || p.BirthDate >= parsedDate.AddDays(1))
                                : onlyMonth ? forFiltration.Where(p => p.BirthDate < parsedDate || p.BirthDate >= parsedDate.AddMonths(1))
                                    :onlyYear ? forFiltration.Where(p => p.BirthDate < parsedDate || p.BirthDate >= parsedDate.AddYears(1))
                                        : forFiltration.Where(p => p.BirthDate != parsedDate);
                    break;

                case "gt":
                case "sa":
                    forFiltration = onlySeconds ? forFiltration.Where(p => p.BirthDate > parsedDate.AddSeconds(1))
                        : onlyMinutes ? forFiltration.Where(p => p.BirthDate > parsedDate.AddMinutes(1))
                            : onlyDay ? forFiltration.Where(p => p.BirthDate > parsedDate.AddDays(1))
                                : onlyMonth ? forFiltration.Where(p => p.BirthDate > parsedDate.AddMonths(1))
                                    :onlyYear ? forFiltration.Where(p => p.BirthDate > parsedDate.AddYears(1)) 
                                        : forFiltration.Where(p => p.BirthDate == parsedDate);
                    break;

                case "lt":
                case "eb":
                    forFiltration = forFiltration.Where(p => p.BirthDate < parsedDate);
                    break;

                case "ge":
                    forFiltration = forFiltration.Where(p => p.BirthDate >= parsedDate);
                    break;

                case "le":
                    forFiltration =onlySeconds ? forFiltration.Where(p => p.BirthDate <= parsedDate.AddSeconds(1))
                        : onlyMinutes ? forFiltration.Where(p => p.BirthDate <= parsedDate.AddMinutes(1))
                            : onlyDay ? forFiltration.Where(p => p.BirthDate <= parsedDate.AddDays(1))
                                : onlyMonth ? forFiltration.Where(p => p.BirthDate <= parsedDate.AddMonths(1))
                                    : onlyYear ? forFiltration.Where(p => p.BirthDate <= parsedDate.AddYears(1))
                                        : forFiltration.Where(p => p.BirthDate <= parsedDate);
                    break;
            }
            return forFiltration;
        }

        private DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, dateFormats, System.Globalization.CultureInfo.InvariantCulture);
        }

        static string[] dateFormats = { 
            "yyyy",
            "yyyy-MM",
            "yyyy-MM-dd",
            "yyyy-MM-ddThh:mmK",
            "yyyy-MM-ddThh:mm:ssK",
            "yyyy-MM-ddThh:mm:ss.ssssK",
        };

        static string onlyYearPattern = @"^..\d\d\d\d.*";
        static string onlyMonthPattern = @"^..\d\d\d\d-\d\d.*";
        static string onlyDayPattern = @"^..\d\d\d\d-\d\d-\d\d.*";
        static string onlyMinutesPattern = @"^..\d\d\d\d-\d\d-\d\dT\d\d:\d\d.*";
        static string onlySecondsPattern = @"^..\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d.*";

    }
}
