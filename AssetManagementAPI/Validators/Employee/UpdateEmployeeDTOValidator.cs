using FluentValidation;
using AssetManagementAPI.DTOs.Employee;

namespace AssetManagementAPI.Validators.Employee
{
    public class UpdateEmployeeDTOValidator
        : AbstractValidator<UpdateEmployeeDTO>
    {
        public UpdateEmployeeDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MinimumLength(3)
                .MaximumLength(50)
                .Matches(@"^[a-zA-Z\s]+$")
                .WithMessage("Name should contain only letters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(role =>
                    new[] { "Admin", "Employee" }.Contains(role))
                .WithMessage("Role must be Admin or Employee");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .Must(gender =>
                    new[] { "Male", "Female", "Other" }.Contains(gender))
                .WithMessage("Invalid gender");

            RuleFor(x => x.ContactNumber)
                .NotEmpty()
                .WithMessage("Contact number is required")
                .Matches(@"^[0-9]{10}$")
                .WithMessage("Contact number must contain exactly 10 digits");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required")
                .MinimumLength(5)
                .MaximumLength(200);
        }
    }
}