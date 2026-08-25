using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.DTOs
{
    public class WorkshopLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
