﻿using System;
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
            // Zorgt dat elk element in de lijst GetekendeElementen getekend kan worden
            foreach(GetekendElement tekening in GetekendeElementen)
            {
                tekening.Teken(gr);
            }
        }
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            // Zorgt dat de lijst met getekende elementen ook leeg is
            GetekendeElementen.Clear();

        }


        // Methode die alle elementen roteert en ook zo opslaat
        public void Roteer()
        {
            // Controleert eerst of de lijst niet leeg is
            if (GetekendeElementen != null)
            {
                int i, sx, sy, ex, ey;

                //Bepaal positie meest recente element
                i = GetekendeElementen.Count - 1;
                GetekendElement tekening = GetekendeElementen[i];

                sx = tekening.Startpunt.X;
                sy = tekening.Startpunt.Y;
                ex = tekening.Eindpunt.X;
                ey = tekening.Eindpunt.Y;

                // Wisselt voor het meest recente element de x en y punten om
                tekening.Startpunt = new Point(sy, sx);
                tekening.Eindpunt = new Point(ey, ex);
            }
        }
    }
}
