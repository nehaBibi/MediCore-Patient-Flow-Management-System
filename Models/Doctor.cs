using System.ComponentModel.DataAnnotations;

namespace MediCorePatientFlow.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorID { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Specialization { get; set; }

        [StringLength(50)]
        public string? AssignedWard { get; set; }

        [StringLength(50)]
        public string? AvailabilityStatus { get; set; }

        public string? ConsultationScheduleJson { get; set; }

        public ICollection<Admission>? Admissions { get; set; }
    }
}
