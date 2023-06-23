using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    public class Akcje
    {
        public long Id_akcji {  get; set; }
        public string Walor { get; set; }
        public DateTime Data_Zakupu { get; set; }
        public long Liczba_Jednestek { get; set; }
        public string Nazwa { get; set; }
    }
}
