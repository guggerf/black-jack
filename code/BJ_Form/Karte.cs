using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ_Form
{
    public class Karte
    {
        // Karten initialisieren
        private int nummer;
        private int typ;
        private int decknummer;
        public static readonly string[] KARTENTYPEN = { "herz", "ecken", "schaufel", "kreuz" };
        // Konstante für Übersicht
        public const int KARTEN_NUMMER_ASS = 1;
        public const int KARTEN_NUMMER_BAUER = 11;
        public const int KARTEN_NUMMER_DAME = 12;
        public const int KARTEN_NUMMER_KOENIG = 13;
        public const int KARTEN_WERT_ASS = 11;
        public const int KARTEN_WERT_ASS_ALS_EINS = 1;
        public const int KARTEN_WERT_BILD = 10;
        
        // Konstruktor
        public Karte(int nummer, int typ, int decknummer)
        {
            this.nummer = nummer;
            this.typ = typ;
            this.decknummer = decknummer;
        }              
        // Funktionen mit Karten
        // Kartennummer ausgeben
        public int gibKartenNummer()
        {
            return this.nummer;
        }
        // Kartentyp ausgeben
        public int gibKartenTyp()
        {
            return this.typ;
        }
        // Kartenwert ausgeben
        // wenn in der () true steht so wird ein
        // Ass als 1 gewertet, wenn false
        // wirds als 11
        public int gibKartenWert(bool assAlsEins)
        {
            if (this.nummer == KARTEN_NUMMER_ASS)
            {
                if (assAlsEins)
                {
                    return KARTEN_WERT_ASS_ALS_EINS;
                }
                else
                {
                    return KARTEN_WERT_ASS;
                }
            }
            else if (this.nummer < KARTEN_NUMMER_BAUER)
            {
                return this.nummer;
            }
            else
            {
                return KARTEN_WERT_BILD;
            }
        }
        // Karten Typ als String ausgeben
        // Wurde in einem Array abgespeichert
        public string gibKartenTypAlsString()
        {
            return KARTENTYPEN[this.typ];
        }
        // Decknummer von Karte ausgeben
        // <debug only>
        public int gibDeckNummer()
        {
            return this.decknummer;
        }
        // Karte anzeigen
        // <debug only>
        public void zeigeKarte()
        {
            Console.WriteLine(gibKartenTypAlsString() + " " + gibKartenNummer() + " aus Deck " + gibDeckNummer());
        }
    }
}
