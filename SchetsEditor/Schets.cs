using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;

        // Lijst waarin de getekende elementen opgeslagen worden
        public List<GetekendElement> GetekendeElementen = new List<GetekendElement>();
        
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
        }
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
            foreach(GetekendElement tekening in GetekendeElementen)
            {
                tekening.Teken(gr);
            }
        }
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            GetekendeElementen.Clear();

        }
        public void Opslaan()
        { }
        public void Openen()
        { }
        public void Roteer()
        {
/*            foreach (GetekendElement tekening in GetekendeElementen)
            {
                Point oudstartpunt = tekening.Startpunt();
                tekening.VeranderStartpunt(tekening.Eindpunt());
                tekening.VeranderEindpunt(oudstartpunt);

            }*/
            /*bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);*/
        }
    }
}
