using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserAPI.Apps.ManageApp.DTOs.AccountDtos
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
