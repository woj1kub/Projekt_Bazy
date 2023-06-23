using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    public class KontoOszczędnościowe
    {
        public long Id_KontaOszczędnościowego { get; set; }
        public DateTime Data_Założenia { get; set; }
        public decimal Kwota { get; set; }
        public double Oprecentowanie { get; set; }
        public DateTime Data_Wypłaty_Odsetek { get; set; }
        public double Podatek { get; set; }
        public string Nazwa { get; set; }
    }
}
