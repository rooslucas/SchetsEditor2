using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public abstract class GetekendElement
    {
        // De variabele die gebruikt worden om een getekend element aan te maken
        protected Point startpunt, eindpunt;
        protected Color kleur;
        protected char letter;
        protected Brush kwast;

        public virtual void Teken(Graphics gr)
        { kwast = new SolidBrush(kleur); }

        public abstract bool Geraakt(Point p);

        // Override methode ToString om de elementen op te kunnen slaan in een string
        public override string ToString()
        {
            return $"{startpunt} {eindpunt} {kleur}";
        }

    }
/*
    public class Tekst : GetekendElement
    {
        public void Teken(Graphics gr)
        {
*//*            if (c >= 32)
            {
                Graphics gr = SchetsControl.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz =
                gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                gr.DrawString(tekst, font, kwast,
                                              this.startpunt, StringFormat.GenericTypographic);
                // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);
                startpunt.X += (int)sz.Width;
                s.Invalidate();
            }*//*
        }
    }*/

    public class Rechthoek : GetekendElement
    {
        public Rechthoek(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.DrawRectangle(TweepuntTool.MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }

        // De rechthoek is geraakt als de afstand tot het kader minder dan  +/- 5 is
        public override bool Geraakt(Point p)
        {
            bool linkx, rechtsx, boveny, ondery;
            int breedte = Math.Abs(startpunt.X - eindpunt.X);
            int lengte = Math.Abs(startpunt.Y - eindpunt.Y);

            linkx = (p.X >= startpunt.X - 5 && p.X <= startpunt.X + 5) && (p.Y >= startpunt.Y && p.Y <= startpunt.Y + lengte);
            rechtsx = (p.X >= eindpunt.X - 5 && p.X <= eindpunt.X + 5) && (p.Y >= startpunt.Y && p.Y <= startpunt.Y + lengte);
            boveny = (p.Y >= startpunt.Y - 5 && p.Y <= startpunt.Y - 5) && (p.X >= startpunt.X && p.X <= startpunt.X + breedte);
            ondery = (p.Y >= eindpunt.Y - 5 && p.Y <= eindpunt.Y - 5) && (p.X >= startpunt.X && p.X <= startpunt.X + breedte);

            return linkx || rechtsx || boveny || ondery;
        }

    }

    public class VolRechthoek : GetekendElement
    {
        public VolRechthoek(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }

        // De volle rechthoek is geraakt als de er in de rechthoek geklikt wordt
        public override bool Geraakt(Point p)
        {
            int breedte = Math.Abs(startpunt.X - eindpunt.X);
            int lengte = Math.Abs(startpunt.Y - eindpunt.Y);
            return (p.X >= startpunt.X && p.X <= startpunt.X + breedte && p.Y >= startpunt.Y && startpunt.Y <= startpunt.Y + lengte);
        }

        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }
    }

    public class Ovaal : GetekendElement
    {
        public Ovaal(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.DrawEllipse(TweepuntTool.MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }

        // Berekent de relatieve straal van punt tot middelpunt en kijkt of tussen de 0.95 en 1.05 zit
        // bron: http://www.hhofstede.nl/modules/cirkel.htm
        public override bool Geraakt(Point p)
        {
            double relatievestraal;
            int xmidden, ymidden;
            int breedte = Math.Abs(startpunt.X - eindpunt.X) / 2;
            int lengte = Math.Abs(startpunt.Y - eindpunt.Y) / 2;
            xmidden = Math.Min(startpunt.X, eindpunt.X) + breedte;
            ymidden = Math.Min(startpunt.Y, eindpunt.Y) + lengte;
            relatievestraal = Math.Pow((p.X - xmidden) / breedte, 2) + Math.Pow((p.Y - ymidden) / lengte, 2);
            return relatievestraal >= 0.95 && relatievestraal <= 1.05;
        }
    }

    public class VolOvaal : GetekendElement
    {
        public VolOvaal(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }

        // Berekent de relatieve straal van punt tot middelpunt en kijkt of die kleiner is dan 1
        // bron: http://www.hhofstede.nl/modules/cirkel.htm
        public override bool Geraakt(Point p)
        {
            double relatievestraal;
            int xmidden, ymidden;
            int breedte = Math.Abs(startpunt.X - eindpunt.X) / 2;
            int lengte = Math.Abs(startpunt.Y - eindpunt.Y) / 2;
            xmidden = Math.Min(startpunt.X, eindpunt.X) + breedte;
            ymidden = Math.Min(startpunt.Y, eindpunt.Y) + lengte;
            relatievestraal = Math.Pow((p.X - xmidden) / breedte, 2) + Math.Pow((p.Y - ymidden) / lengte, 2);
            return relatievestraal <= 1.0;
        }

    }

    public class Lijn : GetekendElement
    {
        public Lijn(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.DrawLine(TweepuntTool.MaakPen(this.kwast, 3), startpunt, eindpunt);
        }

        // Controleert of de lijn geraakt is door te kijken of de afstand van punt tot lijn kleiner is dan 5
        // bron: https://www.basic-mathematics.com/distance-between-a-point-and-a-line.html

        public override bool Geraakt(Point p)
        {
            double afstand, a, b;
            a = (eindpunt.X - startpunt.X) / (eindpunt.Y - startpunt.Y);
            b = startpunt.Y - (a * startpunt.X);
            afstand = Math.Abs(a * p.X - p.Y + b) / Math.Sqrt(a * a + b * b);
            return (a * p.X + b == p.Y || afstand <= 5);
        }
    }
}
