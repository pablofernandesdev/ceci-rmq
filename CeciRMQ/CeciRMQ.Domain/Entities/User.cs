﻿using System.Collections.Generic;

namespace CeciRMQ.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int RoleId { get; set; }

        public bool Validated { get; set; }

        public bool ChangePassword { get; set; }

        public virtual Role Role { get; set; }

        public ICollection<RefreshToken> RefreshToken { get; set; }

        public ICollection<RegistrationToken> RegistrationToken { get; set; }

        public ICollection<ValidationCode> ValidationCode { get; set; }

        public ICollection<Address> Address { get; set; }
    }
}
