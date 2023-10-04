using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ_Form
{
    public class Gambler : Spieler
    {
        // Spieler initialisieren
        private int guthaben;
        private int einsatz;
        private int letzterGewinn = 0;
        
        // konstruktor
        public Gambler(string name, int guthaben, int einsatz) : base(name)
        {
            this.guthaben = guthaben;
            this.einsatz = einsatz;
        }
        // Funktionen mit Gambler
        // Gutehaben setzten
        public void setGuthaben(int guthaben)
        {
            this.guthaben = guthaben;
        }
        // Gutehaben ausgeben
        public int gibGuthaben()
        {
            return guthaben;
        }
        // Einsatz setzten
        public void setEinsatz(int einsatz)
        {
            this.einsatz = einsatz;
        }
        // Aktuellen Einsatz ausgeben
        public int gibEinsatz()
        {
            return einsatz;
        }
        // Einsatz abziehen am Anfang eines Spieles
        public void einsatzAbziehen()
        {
            this.guthaben -= this.einsatz;
            letzterGewinn = -this.einsatz; // Variable für den Gewinn/Verlust Thread
        }
        // Gewonnen BlackJack: Einsatz dreifach zurück
        public void gewinneBJ()
        {
            this.guthaben += 3 * this.einsatz;
            letzterGewinn = 3 * this.einsatz; // Variable für den Gewinn/Verlust Thread
        }
        // Gewonnen: Einsatz doppelt zurück
        public void gewinneEinsatz()
        {
            this.guthaben += 2 * this.einsatz;
            letzterGewinn = 2 * this.einsatz; // Variable für den Gewinn/Verlust Thread
        }
        // Unentschieden: Einsatz zurück
        public void einsatzZurueck()
        {
            this.guthaben += this.einsatz;
            letzterGewinn = this.einsatz;
        }
        // Methode um die Variable für den Gewinn/Verlust Thread auszulesen
        public int gibLetztenGewinn()
        {
            return this.letzterGewinn;
        }
    }
}
