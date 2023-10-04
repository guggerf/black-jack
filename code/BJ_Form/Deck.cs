using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ_Form
{
    public class Deck
    {
        // Deck initialisieren
        public List<Karte> alleKarten = new List<Karte>();

        // Konstruktor
        // anzahl52erDecks wird von uns vorgegeben
        public Deck(int anzahl52erDecks)
        {
            this.decksErstellen(anzahl52erDecks);
        }
        // Funktionen mit Deck
        // Deck erstellen     
        public void decksErstellen(int anzahlDecks)
        {
            // Es werden so viele Decks erstellt wie 
            // in () eingegeben werden
            for(int j = 0; j < anzahlDecks; j++)
            {
                // Kartentyp wird festgelegt (4 verschiedene Typen)
                for (int i = 0; i < Karte.KARTENTYPEN.Length; i++)
                {
                    // Karten von Ass bis König erstellen (13 Karten)
                    for (int k = Karte.KARTEN_NUMMER_ASS; k <= Karte.KARTEN_NUMMER_KOENIG; k++)
                    {
                        // erstelle Karte gleich in die Liste alleKarten
                        alleKarten.Add(new Karte(k, i, j));
                    }
                }
            }
            // Mischt alle Karten in der Liste allekarten, damit besseres random ziehen ermöglicht wird
            // http://stackoverflow.com/questions/12180038/randomly-shuffle-a-list
            Random rand = new Random();
            alleKarten = alleKarten.OrderBy(c => rand.Next()).ToList();
        }
    }
}
