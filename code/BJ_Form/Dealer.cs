using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ_Form
{
    public class Dealer : Spieler
    {
         // Dealer initialisieren
        private int limit;

        // Konstruktor
        public Dealer(string name, int limit) : base(name)
        {
            this.limit = limit;
        }
        // Funktionen min Dealer
        // Limit für Dealer setzten, er zieht keine Karte mehr wenn seine Kartensumme = Limit ist ( default = 17))
        public void setLimit(int limit)
        {
            this.limit = limit;
        }
        // Limit von Dealer ausgeben
        public int gibLimit()
        {
            return limit;
        }
        // Nur die Erste Karte vom Dealer ausgeben
        public Karte gibtSichtbareKarte()
        {
            return this.hand[0];
        }
    }
}
