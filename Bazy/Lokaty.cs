using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    internal class Lokaty
    {
        long Id_Lokaty { get; set; }
        double Kwota { get; set; }
        double Oprocentowanie { get; set; }
        int CzasTrwania { get; set; }
        int Kapitalizacja { get; set; }
        DateTime CzasZalozenia { get; set; }

        double PodatekBelki = 19;

        public Lokaty(double kwota, double oprocentowanie, int okres, int kapitalizacja, DateTime czasZalozenia)
        {
            Kwota = kwota;
            Oprocentowanie = oprocentowanie;
            CzasTrwania = okres;
            Kapitalizacja = kapitalizacja;
            CzasZalozenia = czasZalozenia;
        }

        public double ObliczZyskKoncowy()
        {
            return Kwota * Math.Pow(1 + (Oprocentowanie / (Kapitalizacja * 100) * (1 - PodatekBelki / 100)), CzasTrwania * Kapitalizacja);
        }
    }
}
