using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    class KontoOszczędnościowe
    {
        long Id_KontaOszczędnościowego { get; set; }
        DateOnly Data_Założenia { get; set; }
        decimal Kwota { get; set; }
        double Oprecentowanie { get; set; }
        DateOnly Data_Wypłaty_Odsetek { get; set; }
        double Podatek { get; set; }
        string Nazwa { get; set; }
    }
}
