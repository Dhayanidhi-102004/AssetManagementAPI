using FluentValidation;
using AssetManagementAPI.DTOs.Asset;

namespace AssetManagementAPI.Validators.Asset
{
    public class UpdateAssetDTOValidator
        : AbstractValidator<UpdateAssetDTO>
    {
        public UpdateAssetDTOValidator()
        {
            RuleFor(x => x.AssetName)
                .NotEmpty()
                .WithMessage("Asset name is required")
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required")
                .MinimumLength(3);

            RuleFor(x => x.SerialNumber)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.PurchaseDate)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Purchase date cannot be future date");

        }
    }
}