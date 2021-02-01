using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SchetsEditor
{   public class SchetsControl : UserControl
    {   private Schets schets;
        private Color penkleur;

        public Color PenKleur
        { get { return penkleur; }
        }
        public Schets Schets
        { get { return schets;   }
        }
        public SchetsControl()
        {   this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }
        private void teken(object o, PaintEventArgs pea)
        {   schets.Teken(pea.Graphics);
        }
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }
        public Graphics MaakBitmapGraphics()
        {
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }
        public void Schoon(object o, EventArgs ea)
        {   schets.Schoon();
            this.Invalidate();
        }

        public List<GetekendElement> KrijgLijst()
        {
            return schets.GetekendeElementen;
        }
        // Methode om zo alle regels uit het txt bestand te kunnen tekenen
        public void Toevoegen(string tekening)
        {
            // Parse de belangrijke variabelen
            string[] v = tekening.Split();
            string soort = v[0];
            Point startpunt = new Point(int.Parse(v[1]), int.Parse(v[2]));
            Point eindpunt = new Point(int.Parse(v[3]), int.Parse(v[4]));
            Color kleur = Color.FromArgb(int.Parse(v[5]));
            
            // Check of de soort geen tekst type is
            if (soort != "tekst")
            {
                // Check vervolgens welk type het wel is
                // Voeg een element van dat type toe aan de lijst GetekendeElementen
                if (soort == "rechthoek")
                {
                    Rechthoek rechthoek = new Rechthoek(startpunt, eindpunt, kleur);
                    schets.GetekendeElementen.Add(rechthoek);
                }
                else if (soort == "volrechthoek")
                {
                    VolRechthoek volrechthoek = new VolRechthoek(startpunt, eindpunt, kleur);
                    schets.GetekendeElementen.Add(volrechthoek);
                }
                else if (soort == "ovaal")
                {
                    Ovaal ovaal = new Ovaal(startpunt, eindpunt, kleur);
                    schets.GetekendeElementen.Add(ovaal);
                }
                else if (soort == "volovaal")
                {
                    VolOvaal volovaal = new VolOvaal(startpunt, eindpunt, kleur);
                    schets.GetekendeElementen.Add(volovaal);
                }
                else if (soort == "lijn")
                {
                    Lijn lijn = new Lijn(startpunt, eindpunt, kleur);
                    schets.GetekendeElementen.Add(lijn);
                }
            }
            // Controleer of het type tekst is
            else if (soort == "tekst")
            {
                // Parse dan de string met karakters
                String letter = v[6];
                // Voeg deze toe aan de lijst met getekende elementen
                Tekst tekst = new Tekst(startpunt, eindpunt, kleur, letter);
                schets.GetekendeElementen.Add(tekst);
            }

        }

        public void Roteer(object o, EventArgs ea)
        {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }
        public void VeranderKleur(object obj, EventArgs ea)
        {   string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {   string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
    }
}
