using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Dto
{
    public class TokenRequestDto
    {
        public string GrantType { get; set; }
        public string EmailAddressOrUserName { get; set; }
        public string RefreshToken { get; set; }
        public string Password { get; set; }
    }
}
