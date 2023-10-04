using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BJ_Form
{
    public class Game
    {

        // Game initialisieren
        private int anzahldecks;        
        private Deck theDeck = null;
        public Gambler gambler;
        public Dealer dealer;
        // Konstante für übersicht
        public const int DECK_GROESSE = 52;
        public const int ANFANGSKARTEN = 2;
        public const int GEWINNKARTENSUMME = 21;
        public const int STARTEINSATZ = 5;
        public const int MIN_ERHOEHUNGSSCHRITT = 5;
        public const int MAX_ERHOEHUNGSSCHRITT = 20;


        // Konstruktor
        public Game(int anzahlDecks, string gamblerName, int gamblerGuthaben, int gamblerEinsatz)
        {
            this.theDeck = new Deck(anzahlDecks);
            this.anzahldecks = anzahlDecks;

            gambler = new Gambler(gamblerName, gamblerGuthaben, gamblerEinsatz);
            dealer = new Dealer("Dealer", 17);
            
        }
        // Funktionen in Game
        // Gibt aktuelle Daten von Sieler aus
        public void datenAusgeben()
        {
            gambler.gibName();            
        }
        // Spiel wird gestartet
        public void spielStart()
        {
            // Kontrolle ob Gambler noch genug Guthaben hat
            while (gambler.gibGuthaben() >= gambler.gibEinsatz())
            {
                spielAblauf();
            }
        }
        // Ablauf des Spielvorganges
        public string spielAblauf()
        {
            // Die Hand des Gamblers und des Deales löschen
            gambler.clearKarten();
            dealer.clearKarten();

            // Einsatz abziehen
            gambler.einsatzAbziehen();

            // die ersten Karten verteilen
            ersteKarten();

            // hat bereits jemand gewonnen?
            if (istGewinnerVorhanden())
            {
                if (gambler.hatGewonnen() && dealer.hatGewonnen())
                {
                    gambler.einsatzZurueck();
                    return "Unentschieden";
                }
                else if (gambler.hatGewonnen())
                {
                    gambler.gewinneBJ();
                    return "Blackjack";
                }
                else
                {
                    return "Blackjack";
                }
                // Ende der Runde
            }
            return null;
        } 
        // Wenn Hit gedrückt wird, wird hitCheck ausgeführt
        public string hitCheck()
        {
                if (spielerHatVerloren(gambler))
                {
                    // Ende der Runde
                    return "Dealer";
                }
                else if (istGewinnerVorhanden())
                {
                    gambler.gewinneEinsatz();
                    // Ende der Runde
                    return "Spieler";
                }
                else
                {
                    // weiter im Spiel-Loop
                    return null;
                }
        }
        // Wenn stand gedrückt wird, wird standCheck ausgeführt
        public string standCheck()
        {
            // Dealer nimmt Karten bis zu seiner Limite auf
            while (dealer.gibKartenSumme() < dealer.gibLimit())
            {
                zieheKarteSpieler(dealer);
            }

            // Hat der Dealer verloren?
            if (dealer.gibKartenSumme() > Game.GEWINNKARTENSUMME)
            {
                gambler.gewinneEinsatz();
                // Ende der Runde
                return "Spieler";
            }
            int dealerKartenSumme = dealer.gibKartenSumme();
            int gamblerKartenSumme = gambler.gibKartenSumme();
            int dealerAbstand = Game.GEWINNKARTENSUMME - dealerKartenSumme;
            int gamblerAbstand = Game.GEWINNKARTENSUMME - gamblerKartenSumme;

            // wer hat gewonnen?
            if (gamblerAbstand < dealerAbstand)
            {
                gambler.gewinneEinsatz();
                // Ende der Runde
                return"Spieler";
            }
            else if (gamblerAbstand > dealerAbstand)
            {
                // Ende der Runde
                return "Dealer";
            }
            else
            {
                gambler.einsatzZurueck();
                // Ende der Runde
                return "Unentschieden";
            }
        }
        // Check ob Spieler verloren hat (über 21)
        public bool spielerHatVerloren(Spieler spieler)
        {
            return spieler.gibKartenSumme() > Game.GEWINNKARTENSUMME;
        }
        // Ziehe eine random Karte aus der Liste aller Karten
        public Karte zieheKarte()
        {
            Random r = new Random();

            // hat es noch genug Karten im Deck? (mehr als 51?)
            // wenn nicht, 5 neue Decks hinzufügen
            if (theDeck.alleKarten.Count < Game.DECK_GROESSE)
            {
                Deck neuesDeck = new Deck(5);
                theDeck.alleKarten.AddRange(neuesDeck.alleKarten);
            }
            // Zufällig eine Karte aus der Liste alleKarten ziehen
            int zufall = r.Next(0, theDeck.alleKarten.Count);
            Karte k = theDeck.alleKarten[zufall];
            // gezogenen Karte der Liste entfernen damit diese nicht noch einmal gezogen werden kann
            theDeck.alleKarten.Remove(k);

            return k;
        }
        // Game zieht Karte für Spieler
        public void zieheKarteSpieler(Spieler spieler)
        {
            Karte k = zieheKarte();
            spieler.addKarte(k);
        }
        // Dem Gambler und dem Dealer je 2 Karten in die Hand abfüllen
        public void ersteKarten()
        {
            for (int i = 0; i < ANFANGSKARTEN; i++)
            {
                zieheKarteSpieler(gambler);
            }
            for (int i = 0; i < ANFANGSKARTEN; i++)
            {
                zieheKarteSpieler(dealer);
            }
        }
        // Check ob jemand gewonnen hat
        public bool istGewinnerVorhanden()
        {
            return gambler.hatGewonnen() || dealer.hatGewonnen();
        } 
    }
}
