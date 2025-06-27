using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Models
{
    public class Token
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }

        public string? Email { get; set; }     
        public string? Phone { get; set; }
    }
}
