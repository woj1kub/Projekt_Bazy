using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    class Obligacje
    {
        string Nazwa { get; set; }
        long Id_Obligacji { get; set; }
        long Oprecentowanie { get; set; }
        long Długość_Inwestycji { get; set; }
        long Liczba_Jednostek { get; set; }
        decimal Kwota_Jednostki { get; set; }
        long Skala { get; set; }
        DateTime Data_zakupu { get; set; }
        double Podatek { get; set; }

        Obligacje(string nazwa,long Oprecentowanie, long Długość_Inwestycji, long Liczba_Jednostek, decimal Kwota_Jednostki, long Skala, DateTime Data_zakupu, double Podatek) 
        {
            this.Nazwa = nazwa;
            this.Data_zakupu = Data_zakupu;
            this.Podatek = Podatek;
            this.Skala = Skala;
            this.Oprecentowanie = Oprecentowanie;
            this.Długość_Inwestycji = Długość_Inwestycji;
            this.Liczba_Jednostek = Liczba_Jednostek;
            this.Kwota_Jednostki= Kwota_Jednostki;
        }
    }
}
