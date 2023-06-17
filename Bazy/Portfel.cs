using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bazy
{
    public class Portfel : INotifyPropertyChanged
    {
        private Int64? portfeleId;
        private string? nazwa;
        private decimal wartosc;
        public Portfel(Int64? portfelId, string? nazwa, decimal wartość)
        {
            this.Nazwa = nazwa;
            this.Wartosc = wartość;
            this.PortfeleId = portfelId;
        }
        public Portfel() { }
        public Int64? PortfeleId
        {
            get { return portfeleId; }
            set { 
                portfeleId = value; 
                OnPropertyChanged(nameof(PortfeleId));
                }
        }

        public string? Nazwa 
        {
            get { return nazwa; }
            set { nazwa = value; OnPropertyChanged(nameof(Nazwa));}
        }

        public decimal Wartosc 
        {
            get { 
                return wartosc;
            }
            set {
                wartosc = value; OnPropertyChanged(nameof(Wartosc));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
       
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
