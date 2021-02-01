using System;
using System.Drawing;

namespace SchetsEditor
{
    // De abstracte klasse die de basis vormt voor alle onderstaande elementen
    public abstract class GetekendElement
    {
        // De variabele die gebruikt worden om een getekend element aan te maken
        protected Point startpunt, eindpunt;
        protected Color kleur;
        protected string letter;
        protected Brush kwast;
        public String soort;

        public Point Startpunt
        { 
            get { return startpunt; }
            set { startpunt = value; }
        }
        public Point Eindpunt
        {
            get { return eindpunt; }
            set { eindpunt = value; }
        }

        public virtual void Teken(Graphics gr)
        { kwast = new SolidBrush(kleur); }

        public abstract bool Geraakt(Point p);

    }

    public class Tekst : GetekendElement
    {
        public Tekst(Point s, Point e, Color c, string k)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
            letter = k;
        }
        public override string ToString()
        {
            return $"tekst {startpunt.X} {startpunt.Y} {eindpunt.X} {eindpunt.Y} {kleur.ToArgb()} {letter}";
        }
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            Font font = new Font("Tahoma", 40);
            gr.DrawString(letter, font, kwast,
                                  startpunt, StringFormat.GenericTypographic);
            //gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, eindpunt.X, eindpunt.Y);
        }
        // Controleert of een letter geraakt is
        // Kijkt of het geklikte punt binnen de rechthoek van een letter zit
        public override bool Geraakt(Point p)
        {
            return (p.X >= startpunt.X && p.X <= eindpunt.X && p.Y >= startpunt.Y && p.Y <= eindpunt.Y);
        }
    }

    public class Rechthoek : GetekendElement
    {
        // Constructor methode om een nieuwe rechthoek aan te maken
        public Rechthoek(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }

        // Methode die aangeeft hoe een rechthoek getekend kan worden
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.DrawRectangle(TweepuntTool.MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }

        // De rechthoek is geraakt als de afstand van het punt tot het kader minder dan  +/- 5 is
        public override bool Geraakt(Point p)
        {
            bool linkx, rechtsx, boveny, ondery;
            // Absolute waarden zijn nodig omdat breedte en lengte in dit geval positief zijn
            int breedte = Math.Abs(startpunt.X - eindpunt.X);
            int lengte = Math.Abs(startpunt.Y - eindpunt.Y);

            linkx = (p.X >= startpunt.X - 5 && p.X <= startpunt.X + 5) && (p.Y >= startpunt.Y && p.Y <= startpunt.Y + lengte);
            rechtsx = (p.X >= eindpunt.X - 5 && p.X <= eindpunt.X + 5) && (p.Y >= startpunt.Y && p.Y <= startpunt.Y + lengte);
            boveny = (p.Y >= startpunt.Y - 5 && p.Y <= startpunt.Y + 5) && (p.X >= startpunt.X && p.X <= startpunt.X + breedte);
            ondery = (p.Y >= eindpunt.Y - 5 && p.Y <= eindpunt.Y + 5) && (p.X >= startpunt.X && p.X <= startpunt.X + breedte);

            return linkx || rechtsx || boveny || ondery;
        }
        // Zorgt dat een rechthoek weergegeven kan worden als een string
        public override string ToString()
        {
            return $"rechthoek {startpunt.X} {startpunt.Y} {eindpunt.X} {eindpunt.Y} {kleur.ToArgb()}";
        }

    }

    public class VolRechthoek : GetekendElement
    {
        // Constructor methode om een nieuwe volle rechthoek aan te maken
        public VolRechthoek(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }

        // De volle rechthoek is geraakt als de er in de rechthoek geklikt wordt
        // Controleert of het geklikte punt in het volle vierkant zit
        public override bool Geraakt(Point p)
        {
            return (p.X >= startpunt.X && p.X <= eindpunt.X && p.Y >= startpunt.Y && p.Y <= eindpunt.Y);
        }

        // Methode die aangeeft hoe een volle rechthoek getekend moet worden
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }

        // Zorgt dat een volle rechthoek weergegeven kan worden als een string
        public override string ToString()
        {
            return $"volrechthoek {startpunt.X} {startpunt.Y} {eindpunt.X} {eindpunt.Y} {kleur.ToArgb()}";
        }
    }

    public class Ovaal : GetekendElement
    {
        // Constructor methode om een nieuwe ovaal aan te maken
        public Ovaal(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }

        // Methode die aangeeft hoe een ovaal getekend kan worden
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.DrawEllipse(TweepuntTool.MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }

        // Berekent de relatieve straal van punt tot middelpunt en kijkt of tussen de 0.6 en 0.4 zit
        // TODO betere uitleg van relatieve straal
        // bron: http://www.hhofstede.nl/modules/cirkel.htm
        public override bool Geraakt(Point p)
        {
            double relatievestraal;
            double xmidden, ymidden;
            double breedte = Math.Abs(startpunt.X - eindpunt.X);
            double lengte = Math.Abs(startpunt.Y - eindpunt.Y);
            xmidden = (double)Math.Min(startpunt.X, eindpunt.X) + (breedte / 2);
            ymidden = (double)Math.Min(startpunt.Y, eindpunt.Y) + (lengte / 2);
            relatievestraal = ((double)Math.Pow((p.X - xmidden), 2) / Math.Pow((double)breedte, 2)) 
                + ((double)Math.Pow((p.Y - ymidden), 2) / Math.Pow((double)lengte, 2));
            return Math.Sqrt(relatievestraal) <= 0.6 &&  Math.Sqrt(relatievestraal) >= 0.4;
        }

        // Zorgt dat een ovaal weergegeven kan worden als een string
        public override string ToString()
        {
            return $"ovaal {startpunt.X} {startpunt.Y} {eindpunt.X} {eindpunt.Y} {kleur.ToArgb()}";
        }
    }

    public class VolOvaal : GetekendElement
    {
        // Constructor methode om een nieuwe volle ovaal aan te maken
        public VolOvaal(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }

        // Methdoe die aangeeft hoe een volle ovaal getekend kan worden
        public override void Teken(Graphics gr)
        {
            base.Teken(gr);
            gr.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
        }

        // Berekent de relatieve straal van punt tot middelpunt en kijkt of die kleiner is dan 0.5
        // bron: http://www.hhofstede.nl/modules/cirkel.htm
        public override bool Geraakt(Point p)
        {
            double relatievestraal;
            double xmidden, ymidden;
            double breedte = Math.Abs(startpunt.X - eindpunt.X);
            double lengte = Math.Abs(startpunt.Y - eindpunt.Y);
            xmidden = (double)Math.Min(startpunt.X, eindpunt.X) + (breedte / 2);
            ymidden = (double)Math.Min(startpunt.Y, eindpunt.Y) + (lengte / 2);
            relatievestraal = ((double)Math.Pow((p.X - xmidden), 2) / Math.Pow((double)breedte, 2)) 
                + ((double)Math.Pow((p.Y - ymidden), 2) / Math.Pow((double)lengte, 2));
            return Math.Sqrt(relatievestraal) <= 0.5;
        }

        // Zorgt dat een volovaal weergegeven kan worden als een string
        public override string ToString()
        {
            return $"volovaal {startpunt.X} {startpunt.Y} {eindpunt.X} {eindpunt.Y} {kleur.ToArgb()}";
        }
    }

    public class Lijn : GetekendElement
    {
        // Constructor methode om een nieuwe lijn aan te maken
        public Lijn(Point s, Point e, Color c)
        {
            startpunt = s;
            eindpunt = e;
            kleur = c;
        }

        // Methode die aangeeft hoe een lijn getekend kan worden
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
            if (eindpunt.X - startpunt.X != 0)
                a = (double)(eindpunt.Y - startpunt.Y) / (double)(eindpunt.X - startpunt.X);
            else a = 0;
            b = eindpunt.Y - (a * eindpunt.X);
            afstand = Math.Abs(a * p.X - p.Y + b) / Math.Sqrt(a * a + 1);
            return (a * p.X + b == p.Y || afstand <= 5);
        }

        // Zorgt dat een lijn weergegeven kan worden als een string
        public override string ToString()
        {
            return $"lijn {startpunt.X} {startpunt.Y} {eindpunt.X} {eindpunt.Y} {kleur.ToArgb()}";
        }
    }


}
