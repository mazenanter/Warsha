using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.DTOs
{
    public class WorkshopRegisterRequest
    {
        public string Name { get;  set; } = null!;
        public string Email { get;  set; }  = null!;

        public string Phone { get;  set; }  = null!;

        public string Address { get;  set; } = null!;

        public string Password { get; set; } = null!;

      
    }
}
