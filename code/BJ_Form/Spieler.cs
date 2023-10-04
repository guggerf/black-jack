using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ_Form
{
    public class Spieler
    {
        // Spieler initialisieren
        protected string name;
        protected List<Karte> hand;

        // Konstruktor
        public Spieler(string name)
        {
            this.name = name;           
            hand = new List<Karte>();
        }
        // Funktionen mit Spieler
        // Namen für Spieler vergeben
        public void setName(string name)
        {
            this.name = name;
        }
        // Aktueller Name von Spieler ausgeben
        public string gibName()
        {
            return this.name;
        }
        // Neue Karte wird der Hand (Gambler oder Dealer) hinzugefügt
        public void addKarte(Karte k)
        {
            hand.Add(k);
        }
        // Zeige aktuelle Hand des Spielers (Gambler oder Dealer)
        public List<Karte> gibHand()
        {
            return hand;
        }
        // Auswertung ob jemand gewonnen hat, wenn false, geht Spiel weiter
        public bool hatGewonnen()
        {
            return gibKartenSumme() == Game.GEWINNKARTENSUMME;
        }
        // Summer der Karen in der Hand ausgeben
        public int gibKartenSumme()
        {
            int summe = 0;
            foreach (Karte k in hand)
            {
                // Zuerst wird geschaut ob die Karte ein Ass ist
                if (k.gibKartenNummer() == Karte.KARTEN_NUMMER_ASS)
                {
                    // Wenn die Karte ein Ass ist so wird geschaut ob man die Gewinnkartensumme schon überschritten hat
                    if (summe+k.gibKartenWert(false) > Game.GEWINNKARTENSUMME)
                    {
                        summe += k.gibKartenWert(true); // wenn nicht dann soll dieses Ass als elf zählen
                    }
                    else
                    {
                        summe += k.gibKartenWert(false); // wenn doch, dann nur als eins (jedes 2. Ass wird automatisch zu 1)
                    }
                }
                else
                {
                    summe += k.gibKartenWert(false); // Wenn kein Ass, so Summe aller Karten ausgeben
                }
            }
            return summe; // Summe zurückgeben
        }
        // Alle Karten in der Liste aktuelleKarten löschen
        public void clearKarten()
        {
            hand.Clear();
        }
    }
}
