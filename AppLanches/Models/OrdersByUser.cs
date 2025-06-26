using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Models
{
    public class OrdersByUser
    {
        public int Id { get; set; }
        public decimal Total { get; set; }  //PedidoTotal
        public DateTime OrderDate { get; set; }

    }
}
