using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    public class Obligacje
    {
        public string Nazwa { get; set; }
        public long Id_Obligacji { get; set; }
        public double Oprecentowanie { get; set; }
        public long Długość_Inwestycji { get; set; }
        public long Liczba_Jednostek { get; set; }
        public decimal Kwota_Jednostki { get; set; }
        public long Skala { get; set; }
        public DateTime Data_zakupu { get; set; }
        public double Podatek { get; set; }
        
    }
}
