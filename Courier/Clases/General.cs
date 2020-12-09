using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Courier.Clases
{
    public class General
    {
       
    }
    public class Retorno
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public decimal CantBultos { get; set; }
        public bool BodegaCliente { get; set; }
        public double bul { get; set; }
        public double pall { get; set; }
    }
}