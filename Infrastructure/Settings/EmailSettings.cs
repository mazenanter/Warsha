using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Settings
{
    public class EmailSettings
    {
         public string SenderName { get; set; } = default!;
    public string SenderEmail { get; set; } = default!;
    public string SmtpServer { get; set; } = default!;
    public int Port { get; set; }
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    }
}
