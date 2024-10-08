﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        // Voeg variabele eindpunt toe zodat het eindpunt van een object opgeslagen kan wroden
        protected Point startpunt, eindpunt;
        protected Brush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {   startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {   kwast = new SolidBrush(s.PenKleur);
            eindpunt = p;
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz =
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                // Dit stuk is overbodig geworden en toegepast in de methode Teken van Tekst
/*                gr.DrawString(tekst, font, kwast,
                                            this.startpunt, StringFormat.GenericTypographic);
                gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);*/
                //startpunt.X += (int)sz.Width;
                s.Invalidate();
                eindpunt.X = startpunt.X + (int)sz.Width;
                eindpunt.Y = startpunt.Y + (int)sz.Height;
                Tekst letter = new Tekst(startpunt, eindpunt, s.PenKleur, tekst);
                s.Schets.GetekendeElementen.Add(letter);
                startpunt.X += (int)sz.Width;
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {   s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            // Hiermee wordt de schets getekend
            s.MaakBitmapGraphics();
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(Graphics g, Point p1, Point p2);
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            // Voeg de rechthoek toe aan de lijst met getekende elementen
            Rechthoek rechthoek = new Rechthoek(startpunt, eindpunt, s.PenKleur);
            s.Schets.GetekendeElementen.Add(rechthoek);
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }
    
    public class VolRechthoekTool : TweepuntTool
    {
        public override string ToString() { return "vlak"; }
        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            // Voeg de volrechthoek op in de lijst met getekende elementen
            VolRechthoek volrechthoek = new VolRechthoek(startpunt, eindpunt, s.PenKleur);
            s.Schets.GetekendeElementen.Add(volrechthoek);
        }

        // Zorgt dat er een preview van een volle rechthoek zichtbaar is
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    // Tekent een ovaal in een rechthoek en is daarom een subklasse van 
    public class OvaalTool : TweepuntTool
    {
        public override string ToString() { return "omtrek"; }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            // Voegt de ovaal toe aan de lijst met getekende elementen
            Ovaal ovaal = new Ovaal(startpunt, eindpunt, s.PenKleur);
            s.Schets.GetekendeElementen.Add(ovaal);
        }

        // Laat een preview zien van een getekende ovaal
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
    }

    // Vult een ovaal in een rechthoek 
    public class VolOvaalTool : TweepuntTool
    {
        public override string ToString() { return "ovaal"; }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            // Voegt de volovaal toe aan de lijst met getekende elementen
            VolOvaal volovaal = new VolOvaal(startpunt, eindpunt, s.PenKleur);
            s.Schets.GetekendeElementen.Add(volovaal);
        }

        // Laat een preview zien van een getekende volle ovaal
        public override void Bezig(Graphics g, Point p1, Point p2)
        {
            g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        }

    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            // Slaat de lijn op in de lijst met getekende elementen
            Lijn lijn = new Lijn(startpunt, eindpunt, s.PenKleur);
            s.Schets.GetekendeElementen.Add(lijn);
        }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   g.DrawLine(MaakPen(this.kwast,3), p1, p2);
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            this.MuisLos(s, p);
            this.MuisVast(s, p);
        }

        // De Penstreken worden opgeslagen als losse lijnen en worden op dezelfde manier uitgegumd als een lijn
    }

    public class GumTool : PenTool
    {
        public override string ToString() { return "gum"; }

        // Het Nieuwe Gummen
        public override void MuisLos(SchetsControl s, Point p)
        {
            // Controleert of de lijst niet leeg is
            if (s.Schets.GetekendeElementen != null)
            {
                // Gaat van bovenste naar onderste element in de tekening
                for (int i = s.Schets.GetekendeElementen.Count - 1; i >= 0; i--)
                {
                    GetekendElement tekening = s.Schets.GetekendeElementen[i];

                    // Controleert of het getekende elment geraakt is, afhankelijk van het soort element
                    if (tekening.Geraakt(p))
                    {
                        // Element wordt verwijderd van de lijst en de lijst wordt opnieuw getekend
                        s.Schets.GetekendeElementen.Remove(tekening);
                        s.Invalidate();

                        // De loop wordt afgebroken zodat er niet per ongeluk teveel elemnten verwijderd worden
                        break;
                    }
                }

            }
        }
    }
}
