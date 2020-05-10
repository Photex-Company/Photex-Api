using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Photex.Core.Contracts.Requests;

namespace Phtotex.Api.Validators
{
    public class UploadImageRequestValidator : AbstractValidator<UploadImageRequest>
    {
        public UploadImageRequestValidator()
        {
            RuleFor(x => x.CatalogueId)
                .GreaterThan(0);

            RuleFor(x => x.Image)
                .NotNull()
                .NotEmpty();
        }
    }
}
