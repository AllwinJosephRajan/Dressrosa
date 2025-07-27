using Dressrosa.Dto;
using Dressrosa.Model;
using Dressrosa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Converter
{
    public class TokenRequestConverter : BaseConverter<TokenRequestDto, TokenRequest>
    {
        private readonly PasswordHasher _hasher;
        //model to dto convert
        public TokenRequestConverter(PasswordHasher haser)
        {
            _hasher = haser;
        }
        public override TokenRequest Convert(TokenRequestDto source)
        {
            return new TokenRequest
            {
                EmailAddressOrUserName = source.EmailAddressOrUserName,
                Password = source.Password
            };
        }
        //Dto to model convert
        public override TokenRequestDto Convert(TokenRequest source)
        {
            return new TokenRequestDto
            {
                EmailAddressOrUserName = source.EmailAddressOrUserName,
                Password = source.Password
            };
        }
    }
}
