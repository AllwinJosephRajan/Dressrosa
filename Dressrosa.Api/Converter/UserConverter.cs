using Dressrosa.Api.Dto;
using Dressrosa.Api.Model;
using Dressrosa.API.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Api.Converter
{
    public class UserConverter : BaseConverter<UserDto, User>
    {
        public override User Convert(UserDto source)
        {
            return new User
            {
                Id = source.Id,
                FirstName = source.FirstName,
                LastName = source.LastName,
                UserName = source.UserName,
                Password = source.Password,
                EmailAddress = source.EmailAddress,
                PhoneNumber = source.PhoneNumber,
                CreatedOn = source.CreatedOn,
                UpdateOn = source.UpdateOn,
                CreatedBy = source.CreatedBy,
                DeleteBit = source.DeleteBit,
                UserRoleMapping = source.UserRoleMapping
            };
        }

        public override UserDto Convert(User source)
        {
            return new UserDto
            {
                Id = source.Id,
                FirstName = source.FirstName,
                LastName = source.LastName,
                UserName = source.UserName,
                Password = source.Password,
                EmailAddress = source.EmailAddress,
                PhoneNumber = source.PhoneNumber,
                CreatedOn = source.CreatedOn,
                UpdateOn = source.UpdateOn,
                CreatedBy = source.CreatedBy,
                DeleteBit = source.DeleteBit,
                UserRoleMapping = source.UserRoleMapping
            };
        }
    }
}
