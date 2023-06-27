using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    public class Lokaty
    {
        public long? Id_Lokaty { get; set; }
        public double Oprocentowanie { get; set; }
        public int Czas { get; set; }
        public int Skala { get; set; }
        public decimal Kwota { get; set; }
        public DateTime Data_zakupu { get; set; }
        public string Nazwa { get; set; }
        public double Podatek { get; set; }

        
        //public double ObliczZyskKoncowy()
        //{
        //    return Kwota * Math.Pow(1 + (Oprocentowanie / (Kapitalizacja * 100) * (1 - (PodatekBelki / 100))), CzasTrwania * Kapitalizacja);
        //}
    }
}
