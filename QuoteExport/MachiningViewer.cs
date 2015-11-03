using QuoteExport.Dimensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuoteExport
{
    public partial class MachiningViewer : Form
    {
        Part part;
        string filename = "";
        Bitmap buffer;
        public MachiningViewer(string filepath)
        {
            InitializeComponent();

            this.loadFileAndDisplay(filepath);
        }

        private void loadFileAndDisplay(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            FileInfo fi = new FileInfo(file);
            this.filename = fi.Name.Substring(0, fi.Name.LastIndexOf("."));

            this.part = new Part();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sr = new StreamReader(fs, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    readSeq(line, sr, part);
                }
            }

            drawPart(part);
        }

        private void readSeq(string line, StreamReader sr, Part part)
        {
            if (line.StartsWith("BorderSequence"))
            {
                part.Border = BorderSeq.LoadSeq(line);
            }
            else if (line.StartsWith("VdrillSequence"))
            {
                part.Vdrillings.Add(VdrillSeq.LoadSeq(line));
            }
            else if (line.StartsWith("HDrillSequence"))
            {
                part.Hdrillings.Add(HdrillSeq.LoadSeq(line));
            }
            else if (line.StartsWith("RouteSetMillSequence"))
            {
                RouteSeq rs = new RouteSeq(line);
                while (((line = sr.ReadLine()) != null)
                    && (line.StartsWith("RouteSequence")))
                {
                    rs.AddRoute(line);
                }
                part.Routes.Add(rs);

                if (line != null)
                { readSeq(line, sr, part); }
            }
            else if (line.StartsWith("EndSequence"))
            {
                return;
            }
        }
        private void getEdgeBanding(Part part, out string eU, out string eB, out string eL, out string eR)
        {
            if (part.Border.MachinePoint == "1" || part.Border.MachinePoint == "7M")
            {
                eU = part.Border.Edge4;
                eB = part.Border.Edge3;
                eL = part.Border.Edge1;
                eR = part.Border.Edge2;
                return;
            }
            if (part.Border.MachinePoint == "4" || part.Border.MachinePoint == "6M")
            {
                eU = part.Border.Edge3;
                eB = part.Border.Edge4;
                eL = part.Border.Edge1;
                eR = part.Border.Edge2;
                return;
            }
            if (part.Border.MachinePoint == "5" || part.Border.MachinePoint == "3M")
            {
                eU = part.Border.Edge3;
                eB = part.Border.Edge4;
                eL = part.Border.Edge2;
                eR = part.Border.Edge1;
                return;
            }
            if (part.Border.MachinePoint == "8" || part.Border.MachinePoint == "2M")
            {
                eU = part.Border.Edge4;
                eB = part.Border.Edge3;
                eL = part.Border.Edge2;
                eR = part.Border.Edge1;
                return;
            }
            if (part.Border.MachinePoint == "2" || part.Border.MachinePoint == "4M")
            {
                eU = part.Border.Edge1;
                eB = part.Border.Edge2;
                eL = part.Border.Edge4;
                eR = part.Border.Edge3;
                return;
            }
            if (part.Border.MachinePoint == "6" || part.Border.MachinePoint == "8M")
            {
                eU = part.Border.Edge2;
                eB = part.Border.Edge1;
                eL = part.Border.Edge3;
                eR = part.Border.Edge4;
                return;
            }
            if (part.Border.MachinePoint == "3" || part.Border.MachinePoint == "1M")
            {
                eU = part.Border.Edge1;
                eB = part.Border.Edge2;
                eL = part.Border.Edge3;
                eR = part.Border.Edge4;
                return;
            }
            if (part.Border.MachinePoint == "7" || part.Border.MachinePoint == "5M")
            {
                eU = part.Border.Edge2;
                eB = part.Border.Edge1;
                eL = part.Border.Edge4;
                eR = part.Border.Edge3;
                return;
            }

            throw new Exception("Unknown machine point!");
        }

        private void drawPart(Part part)
        {
            if (panel1.Width == 0 || panel1.Height == 0)
                return;

            buffer = new Bitmap(panel1.Width, panel1.Height);
            using (Graphics g = Graphics.FromImage(buffer))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, panel1.Width, panel1.Height);

                Drawer drawer = new Drawer(g, part, panel1.Width, panel1.Height);
                drawer.Draw();
            }

            panel1.BackgroundImage = buffer;
        }
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.Width < 200 || this.Height < 100)
                return;

            this.panel1.Width = this.Width - 200;
            this.panel1.Height = this.Height - 100;

            if (part != null)
            {
                drawPart(part);
            }
        }
    }
}
