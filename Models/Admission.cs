using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediCorePatientFlow.Models
{
    public class Admission
    {
        [Key]
        public int AdmissionID { get; set; }

        [Required]
        public int PatientID { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorID { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        [StringLength(50)]
        public string WardName { get; set; } = string.Empty;

        [StringLength(10)]
        public string? BedNumber { get; set; }

        public DateTime AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        [StringLength(50)]
        public string? AdmissionStatus { get; set; }

        public string? TreatmentNotes { get; set; }
    }
}
