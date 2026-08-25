using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.DTOs
{
    public class WorkshopRegisterRequest
    {
        public string Name { get;  set; } 
        public string Phone { get;  set; } 
        public string Address { get;  set; }
        public string Password { get; set; }
      
    }
}
