using FluentValidation;
using Photex.Core.Contracts.Requests;

namespace Phtotex.Api.Validators
{
    public class AddCatalogueRequestValidator : AbstractValidator<AddCatalogueRequest>
    {
        public AddCatalogueRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty();
        }
    }
}
