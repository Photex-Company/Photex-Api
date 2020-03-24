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
            RuleFor(x => x.Catalogue)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Image)
                .NotNull()
                .NotEmpty();
        }
    }
}
