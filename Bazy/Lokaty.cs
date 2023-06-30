using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
     public enum kapitalizacjaodesetek
    { Jednorazowa, Roczna, Miesięczna, Dzienna }
    public class Lokaty
    {
        public long Id_Lokaty;
        public double Oprocentowanie;
        public decimal Kwota;
        public double Podatek;
        public DateTime Data_zakupu;
        public DateTime Data_Zakończenia;
        public string Nazwa;
        public kapitalizacjaodesetek Kapitalizacjaodesetek;
        
    }
}
