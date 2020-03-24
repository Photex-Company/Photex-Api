using FluentValidation;
using Photex.Core.Contracts.Requests;

namespace Phtotex.Api.Validators
{
    public class UpdateImageRequestValidator : AbstractValidator<UpdateImageRequest>
    {
        public UpdateImageRequestValidator()
        {
            When(x => x.CatalogueSpecified, () =>
                RuleFor(x => x.Catalogue)
                    .NotNull()
                    .NotEmpty());
        }
    }
}
