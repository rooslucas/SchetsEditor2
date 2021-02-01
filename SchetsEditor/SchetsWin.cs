using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO; // Nodig voor het opslaan en openen van files

namespace SchetsEditor
{
    public class SchetsWin : Form
    {   
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        // Methode om de schets op te slaan als txt file
        private void Opslaan(object o, EventArgs ea)
        {
            {
                SaveFileDialog dialoog = new SaveFileDialog();
                dialoog.Filter = "Alle files|*.*";
                dialoog.Title = "Opslaan";
                if (dialoog.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter stream = File.CreateText(dialoog.FileName + ".txt");
                    foreach (GetekendElement tekening in schetscontrol.KrijgLijst())
                        stream.WriteLine(tekening.ToString());
                    stream.Close();
                }
            }
        }

        // Methode om een txt file te openen en te tekenen als schets
        // Methode is public zodat deze ook in het hoofdscherm gebruikt kan worden
        public void Openen(object o, EventArgs ea)
        {
            OpenFileDialog dialoog = new OpenFileDialog();
            if (dialoog.ShowDialog() == DialogResult.OK)
            {
                StreamReader r = new StreamReader(dialoog.FileName);
                string regel;
                while ((regel = r.ReadLine()) != null)
                    schetscontrol.Toevoegen(regel);
                r.Close();
                schetscontrol.Invalidate();
            }
        }

            public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool()
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new OvaalTool()
                                    , new VolOvaalTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    };
            // De penkleuren waar de gebruiker uit kan kiezen // EXTRA TODO: meer kleuren!
            String[] deKleuren = { "Black", "Red", "Orange", "Yellow", "Green", "Blue"
                                 , "Pink", "Purple", "White"
                                 };
            String[] Blue = { "Blue", "Navy", "MidnightBlue", "RoyalBlue", "LightBlue", "Cyan" };
            String[] Red = { "Red", "OrangeRed", "Crimson", "DarkRed", "FireBrick", "Tomato" };
            String[] Black = { "Black", "Gray", "DarkSlateGray", "Silver", "DimGray", "LightSlateGray" };
            String[] Orange = { "Orange", "Chocolate", "DarkOrange", "SanyBrown", "Brown", "Tan" };
            String[] Green = { "Green", "LimeGreen", "DarkGreen", "Olive", "Chartreuse", "LightGreen" };
            String[] Yellow = { "Yellow", "Gold", "LightYellow", "Goldenrod", "LemonChiffon" };
            String[] Pink = { "Magenta", "DeepPink", "LightPink", "Coral", "MistyRose", "HotPink" };
            String[] Purple = { "Purple", "Indigo", "MediumPurple", "DarkViolet", "Plum", "Lavender" };
            String[] White = { "White" };

            String[][] deKleurSoorten = { Black, Red, Orange, Yellow, Green, Blue, Pink, Purple, White };

            this.ClientSize = new Size(700, 520);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                       {   vast=true;  
                                           huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisLos (schetscontrol, mea.Location);
                                           vast = false; 
                                       };
            schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                       {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                       };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren, deKleurSoorten);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void maakFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            // Voeg opslaan en openen opties toe aan het menu
            menu.DropDownItems.Add("Opslaan", null, this.Opslaan);
            menu.DropDownItems.Add("Openen", null, this.Openen);
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(47, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                // Bij resources zijn plaatjes van een ovaal en volle ovaal toegevoegd
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren, String[][] soorten)
        {   
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);
            
            Button b; Label l; ComboBox cbb, ccb;
            b = new Button(); 
            b.Text = "Clear";  
            b.Location = new Point(  0, 0); 
            b.Click += schetscontrol.Schoon; 
            paneel.Controls.Add(b);
            
            b = new Button(); 
            b.Text = "Rotate"; 
            b.Location = new Point( 80, 0); 
            b.Click += schetscontrol.Roteer; 
            paneel.Controls.Add(b);
            
            l = new Label();  
            l.Text = "Penkleur:"; 
            l.Location = new Point(180, 3); 
            l.AutoSize = true;               
            paneel.Controls.Add(l);
            
            cbb = new ComboBox(); cbb.Location = new Point(240, 0); 
            cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);

            // Maak een extra ComboBox aan om uitgebreide kleuropties toe te voegen
            ccb = new ComboBox(); ccb.Location = new Point(400, 0);
            ccb.DropDownStyle = ComboBoxStyle.DropDownList;
            ccb.SelectedValueChanged += schetscontrol.VeranderKleur;
            ccb.Items.Clear();
            foreach (string sk in soorten[0])
                ccb.Items.Add(sk);
            ccb.SelectedIndex = 0;
            paneel.Controls.Add(ccb);
        }
    }
}
