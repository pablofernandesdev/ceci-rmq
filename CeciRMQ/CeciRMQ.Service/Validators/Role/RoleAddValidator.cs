using CeciRMQ.Domain.DTO.Role;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CeciRMQ.Service.Validators.Role
{
    public class RoleAddValidator : AbstractValidator<RoleAddDTO>
    {
        public RoleAddValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Please enter the name role.")
                .NotNull().WithMessage("Please enter the name role.");
        }
    }
}