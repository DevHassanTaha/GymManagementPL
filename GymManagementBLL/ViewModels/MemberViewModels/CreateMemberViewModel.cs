using GymManagementDAL.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.ViewModels.MemberViewModels
{
    public class CreateMemberViewModel
    {
        [Required(ErrorMessage ="Name is Required")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Please enter a valid name using letters and spaces only.")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone is Required")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [RegularExpression(@"^(010|011|012|015)\d{8}", ErrorMessage = "Phone Number Must Be a Valid Egypt Number.")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Date of Birth is Required")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is Required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Bulding Number is Required")]
        [Range(1, int.MaxValue, ErrorMessage = "Building Number must be a positive integer.")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "City is Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City Must Be Between 2 And 100 Characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "City can only contain letters and spaces.")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Street is Required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Street Must Be Between 2 And 150 Characters")]
        [RegularExpression(@"^[A-Za-z0-9\s]+$", ErrorMessage = "Street can only contain letters, numbers, and spaces.")]
        public string Street { get; set; } = null!;

        public HealthRecordViewModel HealthRecordViewModel { get; set; } = null!;

    }
}
