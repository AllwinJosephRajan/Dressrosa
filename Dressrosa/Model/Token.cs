using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dressrosa.Model
{
    public class Token
    {
        public string UserId { get; set; }
        public string Value { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime ExpiryTime { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime UpdatedDate { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreatedDate { get; set; }
    }
}
