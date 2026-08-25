using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.DTOs
{
    public class ClientLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
