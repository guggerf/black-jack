using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualBasic;

namespace BJ_Form
{
    public partial class BJGUI : Form
    {
        // Game initialisieren damit man es überall verwenden kann
        private Game bj;
        private bool kannDealerKartenZeigen = false;

        // neuen Thread erstellen für die Anzeige gewinn/verlust
        private Thread aktuellerThread = null;

        // Standard Initialisierung für Forms
        public BJGUI()
        {
            InitializeComponent();
        }
        // Das Form so laden das startNewGame() gleich ausgeführt wird
        private void frShown(object sender, EventArgs e)
        {
            startNewGame();
        }
        // Ein neues Spiel starten
        private void startNewGame()
        {
            // Alles Löschen
            clearAll();
            // neue Daten annehmen
            string eingabeName = "Name"; // Name vorgeben, der dann geändert werden soll
            string eingabeGuthaben = "150"; // Guthaben vorgeben, das dann geändert werden soll
            // Ein Dialog anzeigen mit zwei Eingabefelder für Namen und Guthaben
            DialogResult result1 = ShowInputDialog(this, "Bitte Spieldaten eingeben", ref eingabeName, "Name", ref eingabeGuthaben, "Guthaben");

            // Erstellung eines Objekt Game
            int guthaben = Convert.ToInt32(eingabeGuthaben);
            int einsatz = Game.STARTEINSATZ;

            // Game erstellen mit 6 Decks und eingegebenen Daten
            bj = new Game(6, eingabeName, guthaben, einsatz);

            // Ausgabe in Form
            laSpielername.Text = bj.gambler.gibName();
            laGuthaben.Text = bj.gambler.gibGuthaben().ToString();
            laEinsatz.Text = bj.gambler.gibEinsatz().ToString();

            // Button "Stand" deaktivieren und erhöhen/verringern-Buttons anzeigen
            btStand.Enabled = false;
            enablePictureBox(piIncEinsatz);
            enablePictureBox(piDecEinsatz);

            // neuen (leeren) Spielstand anzeigen
            spielstandAnzeigen();
        }
        // Alles reseten für neues Game
        private void clearAll()
        {
            // alle Spielvariablen zurücksetzen
            kannDealerKartenZeigen = false;
            // Formdaten in Ausgangszustand setzten
            laEinsatz.Text = " ";
            laGuthaben.Text = " ";
            laKartensummeDealer.Text = " ";
            laKartensummeSpieler.Text = " ";
            laKartensummeDealer.Visible = false;
            laResultat.Visible = false;
            btHit.Text = "Spiel starten";
            btHit.Enabled = true;
        }
        // Button für Einsatz um 5 zu verringern
        private void piDecEinsatz_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(laEinsatz.Text) > Game.MIN_ERHOEHUNGSSCHRITT)
            {
                laEinsatz.Text = (Convert.ToInt32(laEinsatz.Text) - Game.MIN_ERHOEHUNGSSCHRITT).ToString();
                bj.gambler.setEinsatz(Convert.ToInt32(laEinsatz.Text));
            }
        }
        // Button (DoppelKlick) für Einsatz um 20 zu verringern
        private void piDecEinsatz_DoubleClick(object sender, EventArgs e)
        {
            if (Convert.ToInt32(laEinsatz.Text) > Game.MAX_ERHOEHUNGSSCHRITT)
            {
                laEinsatz.Text = (Convert.ToInt32(laEinsatz.Text) - Game.MAX_ERHOEHUNGSSCHRITT).ToString();
                bj.gambler.setEinsatz(Convert.ToInt32(laEinsatz.Text));
            }
        }
        // Button für Einsatz um 5 zu erhöhen
        private void piIncEinsatz_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(laEinsatz.Text) < Convert.ToInt32(laGuthaben.Text))
            {
                laEinsatz.Text = (Convert.ToInt32(laEinsatz.Text) + Game.MIN_ERHOEHUNGSSCHRITT).ToString();
                bj.gambler.setEinsatz(Convert.ToInt32(laEinsatz.Text));
            }            
        }
        // Button (DoppelKlick) für Einsatz um 20 zu erhöhen
        private void piIncEinsatz_DoubleClick(object sender, EventArgs e)
        {
            if (Convert.ToInt32(laEinsatz.Text) < (Convert.ToInt32(laGuthaben.Text) - Game.MAX_ERHOEHUNGSSCHRITT))
            {
                laEinsatz.Text = (Convert.ToInt32(laEinsatz.Text) + Game.MAX_ERHOEHUNGSSCHRITT).ToString();
                bj.gambler.setEinsatz(Convert.ToInt32(laEinsatz.Text));
            }
        }
        // Button für Spielstart oder Hit
        private void btHit_Click(object sender, EventArgs e)
        {
            //Wenn der ButtonText "Start" heisst führt ein Klick ein neues Spiel aus
            if (btHit.Text == "Spiel starten")
            {
                // Hat der Gambler noch genug Guthaben?
                // Wenn nicht, Spiel abbrechen
                if (bj.gambler.gibGuthaben() < bj.gambler.gibEinsatz())
                {
                    if (bj.gambler.gibGuthaben() == 0)
                    {
                        // kann nichts mehr machen, er hat wirklich verloren
                        laResultat.Visible = true;
                        laResultat.Text = "Please insert coin";
                        laKartensummeDealer.Visible = false;
                        btStand.Enabled = false;
                        btHit.Enabled = false;
                        disablePictureBox(piIncEinsatz);
                        disablePictureBox(piDecEinsatz);
                        spielstandAnzeigen();
                        return;
                    }
                    else if (bj.gambler.gibEinsatz() > Game.MIN_ERHOEHUNGSSCHRITT)
                    {
                        // Einsatz auf Minimum setzen
                        bj.gambler.setEinsatz(Game.MIN_ERHOEHUNGSSCHRITT);
                        laEinsatz.Text = bj.gambler.gibEinsatz().ToString();
                    }
                }
                // Form vorbereiten für neue Runde
                laResultat.Visible = false;
                laKartensummeDealer.Visible = false;
                btStand.Enabled = true;
                kannDealerKartenZeigen = false;

                // ButtonText umbenenen in Hit
                btHit.Text = "Hit";
                // Einsatz kann nicht mehr verändert werden
                disablePictureBox(piIncEinsatz);
                disablePictureBox(piDecEinsatz);

                string resultat = bj.spielAblauf();
                // falls jemand gewonnen hat oder unentschieden ist, können die
                // Dealer-Karten angezeigt werden
                if (resultat != null)
                {
                    kannDealerKartenZeigen = true;
                }
                ausgabeResultat(resultat);                
                spielstandAnzeigen();
                gewinnVerlustAnzeigen();
            }
            // Wenn der ButtonText "Hit" ist wird neue Karte verteilt
            else if (btHit.Text == "Hit")
            {
                //ziehekarte
                bj.zieheKarteSpieler(bj.gambler);              
                spielstandAnzeigen();

                string resultat = bj.hitCheck();

                // falls jemand gewonnen hat oder unentschieden ist können die
                // Dealer-Karten angezeigt werden
                if (resultat != null)
                {
                    kannDealerKartenZeigen = true;
                }
                ausgabeResultat(resultat);
                spielstandAnzeigen();
            }
        }
        // Button für Stand
        private void btStand_Click(object sender, EventArgs e)
        {
            // 2. Karte von Dealer anzeigen und
            // Die kartensumme vom Dealer anzeigen
            kannDealerKartenZeigen = true;
            laKartensummeDealer.Visible = true;
            spielstandAnzeigen();
            // Resultat anzeigen
            ausgabeResultat(bj.standCheck());
            spielstandAnzeigen();
        }
        // Spielstand in GUI anzeigen
        private void spielstandAnzeigen()
        {
            // Listen initialisieren
            List<Karte> gamblerHand = bj.gambler.gibHand();
            List<Karte> dealerHand = bj.dealer.gibHand();
            // Array erstellen für die 6 Pictureboxen vom Dealer
            PictureBox[] piDealer = { piDealerkarte1, piDealerkarte2, piDealerkarte3, piDealerkarte4, piDealerkarte5, piDealerkarte6, piDealerkarte7 };
            PictureBox[] piGambler = { piGamblerKarte1, piGamblerKarte2, piGamblerKarte3, piGamblerKarte4, piGamblerKarte5, piGamblerKarte6, piGamblerKarte7 };
            // Karten anzeigen
            dealerKartenAnzeigen(dealerHand, piDealer);
            gamblerKartenAnzeigen(gamblerHand, piGambler);
            // Kartensumme anzeigen
            if (bj.gambler.gibHand().Count > 0)
            {
                laKartensummeSpieler.Text = bj.gambler.gibKartenSumme().ToString();
                laKartensummeDealer.Text = bj.dealer.gibKartenSumme().ToString();
            }
            else // Wenn keine Karten vorhanden sind, keine Summe anzeigen
            {
                laKartensummeSpieler.Text = null;
                laKartensummeDealer.Text = null;
            }
        }
        // Dealer Karten anzeigen (nur eine oder nach Stand alle)
        private void dealerKartenAnzeigen(List<Karte> dealerHand, PictureBox[] piAllCards)
        {
            // Dealer-Karten anzeigen
            if (kannDealerKartenZeigen) // hat der Spiler gestandet oder hat jemand gewonnen/verloren
            {
                // zuerst alle Karten verstecken
                for (int i = 0; i < piAllCards.Length; i++) //Falls nicht benütze Karten in weiteren Games noch auf visible = true sind
                {
                    piAllCards[i].Visible = false;
                }
                // Karten anzeigen
                for (int i = dealerHand.Count - 1; i >= 0; i--) //Rückwärts damit die überlappung der Karten stimmt, also neuste = TOP
                {
                    Karte k = dealerHand[i];
                    // Code für Anzeigen von Grafiken
                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    Stream _imageStream = _assembly.GetManifestResourceStream("BJ_Form.images." + k.gibKartenTypAlsString() + k.gibKartenNummer() + ".png");
                    // Karte an Stelle i anzeigen
                    piAllCards[i].Visible = true;
                    piAllCards[i].Image = new Bitmap(_imageStream);
                    this.Controls.Add(piAllCards[i]);                }
            }
            else
            {
                // zuerst alle Karten verstecken
                for (int i = 0; i < piAllCards.Length; i++) //Falls nicht benütze Karten in weiteren Games noch auf visible = true sind
                {
                    piAllCards[i].Visible = false;
                }
                // nur die erste Karte anzeigen, falls es schon welche anzuzeigen gibt
                if (dealerHand.Count > 0)
                {
                    Karte k = dealerHand[0];
                    // Code für Anzeigen von Grafiken
                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    Stream _imageStream = _assembly.GetManifestResourceStream("BJ_Form.images." + k.gibKartenTypAlsString() + k.gibKartenNummer() + ".png");
                    // Karte an Stelle [0] anzeigen (erste karte von Dealer)
                    piAllCards[0].Visible = true;
                    piAllCards[0].Image = new Bitmap(_imageStream);
                    this.Controls.Add(piAllCards[0]);
                    // Karte an Stelle [3] anzeigen (anstat die zweite karte anzuzeigen, wird eine Kartenrückseite dargestellt)
                    _imageStream = _assembly.GetManifestResourceStream("BJ_Form.images.back.png");
                    piAllCards[3].Visible = true;
                    piAllCards[3].Image = new Bitmap(_imageStream);
                    this.Controls.Add(piAllCards[3]);
                }
            }
        }
        // Karten von Gambler anzeigen
        private void gamblerKartenAnzeigen(List<Karte> gamblerHand, PictureBox[] piAllCards)
        {
            // zuerst alle Karten verstecken
            for (int i = 0; i < piAllCards.Length; i++) //Falls nicht benütze Karten in weiteren Games noch auf visible = true sind
            {
                piAllCards[i].Visible = false;
            }
            // Karten anzeigen
            for (int i = gamblerHand.Count - 1; i >= 0; i--) //Rückwärts damit die überlappung der Karten stimmt, also neuste = TOP
            {
                Karte k = gamblerHand[i];
                // Code für Anzeige von Grafik
                Assembly _assembly = Assembly.GetExecutingAssembly();
                Stream _imageStream = _assembly.GetManifestResourceStream("BJ_Form.images." + k.gibKartenTypAlsString() + k.gibKartenNummer() + ".png");
                // Karte an Stelle i anzeigen
                piAllCards[i].Visible = true;
                piAllCards[i].Image = new Bitmap(_imageStream);
                this.Controls.Add(piAllCards[i]);
            }
        }
        // Methode um Gewinner festzulegen
        public void ausgabeResultat(string resultat)
        {
            if (resultat == "Unentschieden")
            {
                laResultat.Text = "Unentschieden";
            }
            else if (resultat == "Spieler")
            {
                laResultat.Text = "Spieler hat gewonnen";
            }
            else if (resultat == "Dealer")
            {
                laResultat.Text = "Dealer hat gewonnen";
            }
            else if (resultat == "Blackjack")
            {
                laResultat.Text = "Black Jack!";
            }
            else
            {
                return;
            }
            // Text für Gewinner / Verlierer anzeigen
            // Form auf neues Spiel vorbereiten
            // Einsatz erhöhen und verringern freigeben
            laResultat.Visible = true;
            btStand.Enabled = false;
            laKartensummeDealer.Visible = true;
            enablePictureBox(piIncEinsatz);
            enablePictureBox(piDecEinsatz);
            btHit.Text = "Spiel starten";
            gewinnVerlustAnzeigen();
        }
        // Methode, die einen Eingabedialog anzeigt, gefunden mit Google auf
        // http://stackoverflow.com/questions/97097/what-is-the-c-sharp-version-of-vb-nets-inputdialog
        // etwas umgebaut
        private static DialogResult ShowInputDialog(Form parentForm, string titel, ref string input1, string inputText1, ref string input2, string inputText2)
        {
            System.Drawing.Size size = new System.Drawing.Size(300, 130);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = titel;

            System.Windows.Forms.Label label1 = new Label();
            label1.Size = new System.Drawing.Size(size.Width - 10, 20);
            label1.Location = new System.Drawing.Point(5, 5);
            label1.Text = inputText1;
            inputBox.Controls.Add(label1);

            System.Windows.Forms.TextBox textBox1 = new TextBox();
            textBox1.Size = new System.Drawing.Size(size.Width - 10, 25);
            textBox1.Location = new System.Drawing.Point(5, 25);
            textBox1.Text = input1;
            inputBox.Controls.Add(textBox1);

            System.Windows.Forms.Label label2 = new Label();
            label2.Size = new System.Drawing.Size(size.Width - 10, 20);
            label2.Location = new System.Drawing.Point(5, 50);
            label2.Text = inputText2;
            inputBox.Controls.Add(label2);

            System.Windows.Forms.TextBox textBox2 = new TextBox();
            textBox2.Size = new System.Drawing.Size(size.Width - 10, 25);
            textBox2.Location = new System.Drawing.Point(5, 70);
            textBox2.Text = input2;
            inputBox.Controls.Add(textBox2);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 100, 95);
            inputBox.Controls.Add(okButton);

            inputBox.AcceptButton = okButton;
            // Achtung absichtlich kein Cancle Button vorhanden

            // Die Eingabeaufforderung zentrieren
            inputBox.StartPosition = FormStartPosition.Manual;
            Point location = new Point();
            location.X = parentForm.Left + (parentForm.Width / 2) - inputBox.Width / 2;
            location.Y = parentForm.Top + (parentForm.Height / 2) - inputBox.Height / 2;
            inputBox.Location = location;

            DialogResult result = inputBox.ShowDialog();

            input1 = textBox1.Text;
            input2 = textBox2.Text;
            return result;
        }
        // Methode für die PictureButtons anzuzeigen
        // und komplett undurchsichtig machen
        private void enablePictureBox(PictureBox box)
        {
            box.Enabled = true;

            // Jedes Pixel des Bildes holen und den Alpha-Kanal auf 255 (komplett undurchsichtig)
            // http://www.codeproject.com/Questions/198575/C-Opacity-Of-Picturebox
            Bitmap pic = new Bitmap(box.Image);
            for (int w = 0; w < pic.Width; w++)
            {
                for (int h = 0; h < pic.Height; h++)
                {
                    Color c = pic.GetPixel(w, h);
                    Color newC = Color.FromArgb(255, c);
                    pic.SetPixel(w, h, newC);
                }
            }
            box.Image = pic;
        }
        // Methode für die PictureButtons auszuschalten
        // und 20% durchsichtig machen
        private void disablePictureBox(PictureBox box)
        {
            box.Enabled = false;

            // Jedes Pixel des Bildes holen und den Alpha-Kanal auf 50 (20% durchsichtig)
            // http://www.codeproject.com/Questions/198575/C-Opacity-Of-Picturebox
            Bitmap pic = new Bitmap(box.Image);
            for (int w = 0; w < pic.Width; w++)
            {
                for (int h = 0; h < pic.Height; h++)
                {
                    Color c = pic.GetPixel(w, h);
                    Color newC = Color.FromArgb(50, c);
                    pic.SetPixel(w, h, newC);
                }
            }
            box.Image = pic;
        }                
        // Menu-Eintrag "Beenden"
        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Wollen Sie Black Jack wirklich beenden?", "Beenden", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Close(); // Fenster wird geschlossen
            }
        }
        // Menu-Eintrag "Regeln"
        private void meRegeln_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Achtung\nSplit, Doubledown und Insurance\nsind nicht ausprogrammiert!\n\nDer Dealer spielt soft 17", "Regeln", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Menu-Entrag "New Game"
        private void meNewGame_Click(object sender, EventArgs e)
        {
            startNewGame();
        }
        // zeige Gewinn/Verlust an
        private void gewinnVerlustAnzeigen()
        {
            // wenn der Gewinn/Verlust schon abgezogen wurde, dann nichts machen
            if (Convert.ToInt32(laGuthaben.Text) == bj.gambler.gibGuthaben())
            {
                return;
            }
            if (this.aktuellerThread != null) // Wenn ein aktueller Thread am laufen ist, soll dieser abgebrochen werden und der neue starten
            {
                this.aktuellerThread.Abort();
            }
            // Neuer Thread starten
            aktuellerThread = new Thread(new ThreadStart(this.guiUpdaten));
            aktuellerThread.Start();
            while (!aktuellerThread.IsAlive) ;
        }
        // Anzeigeparameter für neuen Thread (Gewinn/Verlust)
        private void guiUpdaten()
        {
            int guthabenVorAnimation = Convert.ToInt32(laGuthaben.Text);
            int letzterGewinn = bj.gambler.gibLetztenGewinn();
            Color farbe = Color.Red; // rot wenn Verlust abgezogen wird
            if (letzterGewinn > 0)
            {
                farbe = Color.Green; // grün wenn Gewinn addiert wird
            }
            // Den Thread für 2 Sekunden anzeigen
            animationAnzeigen(true, farbe, letzterGewinn, guthabenVorAnimation + letzterGewinn);
            Thread.Sleep(2000);
            // danach wieder ausblenden (Sichtbarkeit = false)
            animationAnzeigen(false, farbe, letzterGewinn, guthabenVorAnimation + letzterGewinn);
        }
        // Einen Thread machen für ein sauberes Update im GUI
        // Diese Information haben wir nach langem hin und her im Internet gefunden
        // und auf unseren Code abgestummen
        // http://msdn.microsoft.com/en-us/library/ms171728%28v=vs.85%29.aspx
        // Delegate sagt dem Hauptthread das da noch ein weiterer Thread ist und er diesen bei Gelegenheit abarbeiten soll
        delegate void AnimationAnzeigenCallback(bool sichtbarkeit, Color farbe, int gewinnVerlust, int guthaben);
        
        private void animationAnzeigen(bool sichtbarkeit, Color farbe, int gewinnVerlust, int guthaben)
        {
			// Es wird mit InvokeRequest die ID der Threads verglichen
            // Wenn der Hauptthread und der neue Thread verschieden sind
            // wird true ausgegeben und der Thread wird bei Gelegenheit gestartet
			if (this.lbGewinnVerlust.InvokeRequired)
			{	
				AnimationAnzeigenCallback d = new AnimationAnzeigenCallback(animationAnzeigen);
                this.Invoke(d, new object[] { sichtbarkeit, farbe, gewinnVerlust, guthaben });
			}
            else
            {
                this.lbGewinnVerlust.Visible = sichtbarkeit;
                this.lbGewinnVerlust.ForeColor = farbe;
                if (gewinnVerlust > 0)
                {
                    this.lbGewinnVerlust.Text = "+" + gewinnVerlust; // Bei Gewinn soll ein + vor den Betrag
                }
                else
                {
                    this.lbGewinnVerlust.Text = gewinnVerlust.ToString();
                }
                this.laGuthaben.Text = guthaben.ToString();
            }
        }
        // Menu-Eintrag "Credits" anzeigen
        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           MessageBox.Show("Credits\n\nProjekt Black Jack\n\nTEKO B-TEL-12-T-A       \nDaniel Frieden\nFabian Gugger\nJonas Bettschen", "Credits", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        // Menu-Eintrag "Spielanleitung"
        private void spielanleitungToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Spielanleitung Black Jack\n\n\nWenn die Spielerdaten eingegeben wurden, kann der Einsatz verändert werden.\nWenn dann 'Spiel Starten' gedrückt wird, so verteilt der Dealer dem Spieler und sich zwei Karten,\njedoch wird nur die erste Karte vom Dealer sichtbar.\nJetzt kann der Spieler entscheiden ob er no weitere Karten möchte ('Hit') oder ob er so die Runde beenden will ('Stand')\nDas Ziel ist es, näher an 21 zu kommen als der Dealer.\nWenn der Spieler beim 'Hit' aber über 21 kommt so hat er verloren.\nWenn bei den ersten Karten ein Ass und eine Bildkarte oder eine 10 hat, het der Spieler Black Jack und somit den dreifachen Einsatz gewonnen, das selbe gilt für den Dealer.\nWenn der Spieler nach 'Hit' genau 21 hat so hat er automatisch gewonnen und der dopelte Einsatz wird ihm ausbezahlt.\nBei Gleichstand ist Unentschieden und der Speiler erhält seinen Einsatz zurück.\nDer Dealer zieht nur Karten solange er 17 oder weniger hat.\n\n\nViel Erfolg!!!", "Credits", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
