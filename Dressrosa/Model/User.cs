using Dressrosa.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Core.Model
{
    public class User
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string? CreatedBy { get; set; }
        public bool DeleteBit { get; set; }
        public string? TenantId { get; set; }
        public List<UserRoleMappingDto>? UserRoleMapping { get; set; }
    }
}
