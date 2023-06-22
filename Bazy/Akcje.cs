using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    class Akcje
    {
        long Id_akcji {  get; set; }
        string Walor { get; set; }
        DateTime Data_Zakupu { get; set; }
        short Liczba_Jednestek { get; set; }
        string Nazwa { get; set; }
    }
}
