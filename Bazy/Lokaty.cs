using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
     public enum kapitalizacjaOdsetek
    { Jednorazowa, Roczna, Miesieczna, Dzienna }

    public class Lokaty
    {
        public long Id_Lokaty;
        public double Oprocentowanie;
        public decimal Kwota;
        public double Podatek;
        public DateTime Data_zakupu;
        public DateTime Data_zakończenia;
        public string Nazwa;
        public kapitalizacjaOdsetek Kapitalizacjaodesetek;

        public decimal ObliczZysk() // TODO: brak podatku - trzeba go dodać
        {
            decimal zysk = 0;

            switch (Kapitalizacjaodesetek)
            {
                case kapitalizacjaOdsetek.Jednorazowa:
                    zysk = Kwota * (decimal)Oprocentowanie;
                    break;
                case kapitalizacjaOdsetek.Roczna:
                    zysk = Kwota * (decimal)Math.Pow(1 + Oprocentowanie, Data_zakończenia.Year - Data_zakupu.Year);
                    break;
                case kapitalizacjaOdsetek.Miesieczna:
                    int iloscMiesiecy = (Data_zakończenia.Year - Data_zakupu.Year) * 12 + Data_zakończenia.Month - Data_zakupu.Month;
                    double oprocentowanieMiesieczne = Oprocentowanie / 12;
                    zysk = Kwota * (decimal)Math.Pow(1 + oprocentowanieMiesieczne, iloscMiesiecy);
                    break;
                case kapitalizacjaOdsetek.Dzienna:
                    int iloscDni = (int)(Data_zakończenia - Data_zakupu).TotalDays;
                    double oprocentowanieDzienne = Oprocentowanie / 365;
                    zysk = Kwota * (decimal)Math.Pow(1 + oprocentowanieDzienne, iloscDni);
                    break;
            }

            return zysk - Kwota;
        }

    }
}
