using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projekt3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox.SelectedIndex = 2;
            profileCombo.SelectedIndex = 0;
            iluminantCombo.SelectedIndex = 0;
            InsertDefaultImage();
            FillGroupBoxNames();
            LoadIluminantInfo();
            LoadProfileInfo();
            label11.Text = "Redukcja do K kolorów: " + KTrackBar.Value;
        }

        private void SeparateChannels(object sender, EventArgs e)
        {
            FillGroupBoxNames();
            if (comboBox.SelectedItem.ToString() == "LAB")
            {
                SeparateIntoLAB();
            }
            else if (comboBox.SelectedItem.ToString() == "HSV")
            {
                SeparateIntoHSV();
            }
            else if (comboBox.SelectedItem.ToString() == "YCbCr")
            {
                SeparateIntoYCBCR();
            }
        }
        private void InsertNewImages(Bitmap one, Bitmap two, Bitmap three)
        {
            pictureBox1.Image = new Bitmap(one, new Size(pictureBox1.Width, pictureBox1.Height));
            pictureBox2.Image = new Bitmap(two, new Size(pictureBox2.Width, pictureBox2.Height));
            pictureBox3.Image = new Bitmap(three, new Size(pictureBox3.Width, pictureBox3.Height));
        }
        private (double,double,double) GetRGBInPixel(int x, int y, Bitmap original, bool fraction=false)
        {
            Color color = original.GetPixel(x, y);
            if(fraction) return (color.R / 255.0, color.G / 255.0, color.B / 255.0);
            return (color.R, color.G, color.B);
        }
        private void SeparateIntoYCBCR()
        {
            Bitmap one = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap two = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap three = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap original = new Bitmap(originalPictureBox.Image);
            for(int x=0;x<originalPictureBox.Width;x++)
            {
                for(int y = 0; y < originalPictureBox.Height; y++)
                {
                    (double r, double g, double b) = GetRGBInPixel(x, y, original,true);
                    double Y = 0.299 * r + 0.587 * g + 0.114 * b;
                    double Cb = (b - Y) / 1.772 + 0.5;
                    double Cr = (r - Y) / 1.402 + 0.5;
                    one.SetPixel(x, y, Color.FromArgb((int)Math.Round(Y * 255, 0), (int)Math.Round(Y * 255, 0), (int)Math.Round(Y * 255, 0)));
                    two.SetPixel(x, y, Color.FromArgb(127, (int)Math.Round((1-Cb) * 255, 0), (int)Math.Round(Cb * 255, 0)));
                    three.SetPixel(x, y, Color.FromArgb((int)Math.Round(Cr * 255, 0), (int)Math.Round((1-Cr) * 255, 0), 127));
                }
            }
            InsertNewImages(one, two, three);
        }
        private void SeparateIntoHSV()
        {
            Bitmap one = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap two = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap three = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap original = new Bitmap(originalPictureBox.Image);
            for (int x = 0; x < originalPictureBox.Width; x++)
            {
                for (int y = 0; y < originalPictureBox.Height; y++)
                {
                    (double r, double g, double b) = GetRGBInPixel(x, y, original, true);
                    double v = Math.Max(Math.Max(r, g), b);
                    double min = Math.Min(Math.Min(r, g), b);
                    double h = 0;
                    double s = 0;
                    if (v != min)
                    {
                        if (r == v) h = (360.0 + 60 * ((g - b) / (v - min))) % 360;
                        if (g == v) h = (120.0 + 60 * ((b - r) / (v - min))) % 360;
                        if (b == v) h = (240.0 + 60 * ((r - g) / (v - min))) % 360;
                    }
                    h = h / 360.0 * 255.0;
                    if (h < 0) h += 255;
                    if (v != 0)
                    {
                        s = ((v - min) * 255.0) / v;
                    }
                    v *= 255;
                    one.SetPixel(x, y, Color.FromArgb((int)Math.Round(h, 0), (int)Math.Round(h, 0), (int)Math.Round(h, 0)));
                    two.SetPixel(x, y, Color.FromArgb((int)Math.Round(s, 0), (int)Math.Round(s, 0), (int)Math.Round(s, 0)));
                    three.SetPixel(x, y, Color.FromArgb((int)Math.Round(v, 0), (int)Math.Round(v, 0), (int)Math.Round(v, 0)));
                }
            }
            InsertNewImages(one, two, three);
        }
        private void SeparateIntoLAB()
        {
            Bitmap one = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap two = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap three = new Bitmap(originalPictureBox.Width, originalPictureBox.Height);
            Bitmap original = new Bitmap(originalPictureBox.Image);
            double Yw = 1.0, Xw = (double)whiteX.Value / (double)whiteY.Value, Zw = (1.0 - (double)whiteY.Value - (double)whiteX.Value) / (double)whiteY.Value, k = 903.3;
            for (int x = 0; x < originalPictureBox.Width; x++)
            {
                for (int y = 0; y < originalPictureBox.Height; y++)
                {
                    (double, double, double) color = GetRGBInPixel(x, y, original, false);
                    (double X, double Y, double Z) = GetXYZFromRGB(color);
                    double L = 116 * Math.Pow(Y / Yw, 1.0 / 3.0) - 16;
                    if (Y / Yw <= 0.008856) L = k * Y / Yw;
                    double A = 500 * (Math.Pow(X / Xw, 1.0 / 3.0) - Math.Pow(Y / Yw, 1.0 / 3.0));
                    double B = 200 * (Math.Pow(Y / Yw, 1.0 / 3.0) - Math.Pow(Z / Zw, 1.0 / 3.0));
                    A += 127;
                    B += 127;
                    L /= 100.0;
                    A /= 255.0;
                    B /= 255.0;
                    if (B > 1) B = 1;
                    if (L > 1) L = 1;
                    if (A > 1) A = 1;
                    if (B < 0) B = 0;
                    if (L < 0) L = 0;
                    if (A < 0) A = 0;
                    one.SetPixel(x, y, Color.FromArgb((int)Math.Round(L * 255, 0), (int)Math.Round(L * 255, 0), (int)Math.Round(L * 255, 0)));
                    two.SetPixel(x, y, Color.FromArgb((int)Math.Round(A * 255, 0), (int)Math.Round((1 - A) * 255, 0), 127));
                    three.SetPixel(x, y, Color.FromArgb((int)Math.Round(B * 255, 0), 127, (int)Math.Round((1 - B) * 255, 0)));
                }
            }
            InsertNewImages(one, two, three);
        }
        private (double,double,double) GetXYZFromRGB((double R, double G, double B) color)
        {

            color.R = Math.Pow(color.R / 255.0, (double)gamma.Value);
            color.G = Math.Pow(color.G / 255.0, (double)gamma.Value);
            color.B = Math.Pow(color.B / 255.0, (double)gamma.Value);

            double yw = (double)whiteY.Value, xw = (double)whiteX.Value, zw = 1.0 - yw - xw;
            double xr = (double)redX.Value, yr = (double)redY.Value, zr = 1.0 - xr - yr;
            double xg = (double)greenX.Value, yg = (double)greenY.Value, zg = 1.0 - xg - yg;
            double xb = (double)blueX.Value, yb = (double)blueY.Value, zb = 1.0 - xb - yb;
            double Yw = 1.0, Xw = xw / yw, Zw = zw / yw;

            double det = xr * yg * zb + xg * yb * zr + yr * zg * xb - xb * yg * zr - xg * yr * zb - xr * yb * zg;
            //macierz doepelnien
            double[,] D = new double[,]{
            { yg * zb - yb * zg,-(yr*zb-zr*yb),yr*zg-zr*yg },
            {-(xg*zb-zg*xb),xr*zb-zr*xb,-(xr*zg-zr*xg) },
            { xg*yb-xb*yg,-(xr*yb-xb*yr),xr*yg-xg*yr} };
            //macierz odwrotna
            (double x, double y, double z) r = (D[0, 0] / det, D[1, 0] / det, D[2, 0] / det);
            (double x, double y, double z) g = (D[0, 1] / det, D[1, 1] / det, D[2, 1] / det);
            (double x, double y, double z) b = (D[0, 2] / det, D[1, 2] / det, D[2, 2] / det);

            //wyliczenie S z równania
            double Sr = r.x * Xw + r.y * Yw + r.z * Zw;
            double Sb = b.x * Xw + b.y * Yw + b.z * Zw;
            double Sg = g.x * Xw + g.y * Yw + g.z * Zw;

            //RGB->XYZ
            xr *= Sr;
            yr *= Sr;
            zr *= Sr;
            yg *= Sg;
            xg *= Sg;
            zg *= Sg;
            xb *= Sb;
            yb *= Sb;
            zb *= Sb;

            double X = color.R * xr + color.G * xg + color.B * xb;
            double Y = color.R * yr + color.G * yg + color.B * yb;
            double Z = color.R * zr + color.G * zg + color.B * zb;
            return (X, Y, Z);
        }
        private void FillGroupBoxNames()
        {
            if (comboBox.SelectedItem.ToString() == "LAB")
            {
                groupBox1.Text = "L";
                groupBox2.Text = "A";
                groupBox3.Text = "B";
            }
            else if (comboBox.SelectedItem.ToString() == "HSV")
            {
                groupBox1.Text = "H";
                groupBox2.Text = "S";
                groupBox3.Text = "V";
            }
            else if (comboBox.SelectedItem.ToString() == "YCbCr")
            {
                groupBox1.Text = "Y";
                groupBox2.Text = "Cb";
                groupBox3.Text = "Cr";
            }
        }
        private void InsertDefaultImage()
        {
            originalPictureBox.Image = new Bitmap(Image.FromFile("../../../default_image.png"), new Size(originalPictureBox.Width, originalPictureBox.Height));
        }

        private void PickImage(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...";
            dialog.InitialDirectory = @"C:\";
            dialog.Title = "Select a file with image.";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                originalPictureBox.Image = new Bitmap(Image.FromFile(dialog.FileName), new Size(originalPictureBox.Width, originalPictureBox.Height));
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
            }
        }

        private void ProfileChanged(object sender, EventArgs e) => LoadProfileInfo();
        private void LoadProfileInfo()
        {
            if (profileCombo.SelectedItem.ToString() == "sRGB")
            {
                redX.Value = 0.64M;
                redY.Value = 0.33M;
                greenX.Value = 0.3M;
                greenY.Value = 0.6M;
                blueX.Value = 0.15M;
                blueY.Value = 0.06M;
            }
            else if (profileCombo.SelectedItem.ToString() == "Adobe RGB")
            {
                redX.Value = 0.64M;
                redY.Value = 0.33M;
                greenX.Value = 0.21M;
                greenY.Value = 0.71M;
                blueX.Value = 0.15M;
                blueY.Value = 0.06M;
            }
            else if (profileCombo.SelectedItem.ToString() == "CIE RGB")
            {
                redX.Value = 0.73M;
                redY.Value = 0.265M;
                greenX.Value = 0.274M;
                greenY.Value = 0.717M;
                blueX.Value = 0.167M;
                blueY.Value = 0.07M;
            }
            else if (profileCombo.SelectedItem.ToString() == "Wide Gamut")
            {
                redX.Value = 0.7347M;
                redY.Value = 0.265300M;
                greenX.Value = 0.1152M;
                greenY.Value = 0.8284M;
                blueX.Value = 0.1566M;
                blueY.Value = 0.0177M;
            }
        }
        private void LoadIluminantInfo()
        {
            if (iluminantCombo.SelectedItem.ToString() == "D65")
            {
                whiteX.Value = 0.3127M;
                whiteY.Value = 0.32902M;
            }
            else if (iluminantCombo.SelectedItem.ToString() == "D55")
            {
                whiteX.Value = 0.33242M;
                whiteY.Value = 0.34743M;
            }
            else if (iluminantCombo.SelectedItem.ToString() == "D75")
            {
                whiteX.Value = 0.29902M;
                whiteY.Value = 0.31485M;
            }
        }

        private void IluminantChanged(object sender, EventArgs e) => LoadIluminantInfo();

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillGroupBoxNames();
            if (comboBox.SelectedItem.ToString() != "LAB")
            {
                whiteX.Enabled = false;
                whiteY.Enabled = false;
                greenX.Enabled = false;
                greenY.Enabled = false;
                blueY.Enabled = false;
                blueX.Enabled = false;
                redX.Enabled = false;
                redY.Enabled = false;
                iluminantCombo.Enabled = false;
                profileCombo.Enabled = false;
                gamma.Enabled = false;
            }
            else
            {
                whiteX.Enabled = true;
                whiteY.Enabled = true;
                greenX.Enabled = true;
                greenY.Enabled = true;
                blueY.Enabled = true;
                blueX.Enabled = true;
                redX.Enabled = true;
                redY.Enabled = true;
                iluminantCombo.Enabled = true;
                profileCombo.Enabled = true;
                gamma.Enabled = true;
            }
        }

        private void SaveOutput(object sender, EventArgs e)
        {
            if(pictureBox1.Image!=null && pictureBox2.Image != null && pictureBox3.Image != null)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.Personal;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(dialog.SelectedPath + "\\" + groupBox1.Text+".png");
                    pictureBox2.Image.Save(dialog.SelectedPath + "\\" + groupBox2.Text+".png");
                    pictureBox3.Image.Save(dialog.SelectedPath + "\\" + groupBox3.Text+".png");
                }
            }
        }
        private void colorReduction()
        {
            List<Color> colors = GetKpopularColors();
            Bitmap original = new Bitmap(originalPictureBox.Image);
            for (int x = 0; x < originalPictureBox.Width; x++)
            {
                for (int y = 0; y < originalPictureBox.Height; y++)
                {
                    Color c = original.GetPixel(x, y);
                    Color winner = colors[0];
                    foreach(Color item in colors)
                    {
                        if (getDistance(winner, c) > getDistance(item, c))
                        {
                            winner = item;
                            if (getDistance(item, c) == 0) break;
                        }
                    }
                    original.SetPixel(x, y, winner);
                }
            }
            originalPictureBox.Image = new Bitmap(original, new Size(originalPictureBox.Width, originalPictureBox.Height));
        }
        private double getDistance(Color a, Color b)
        {
            return Math.Sqrt((a.R - b.R) * (a.R - b.R) + (a.G - b.G) * (a.G - b.G) + (a.B - b.B) * (a.B - b.B));
        }
        private List<Color> GetKpopularColors()
        {
            int K = (int)KTrackBar.Value;
            Dictionary<Color, int> dict = new Dictionary<Color, int>();
            Bitmap original = new Bitmap(originalPictureBox.Image);
            for (int x = 0; x < originalPictureBox.Width; x++)
            {
                for(int y = 0; y < originalPictureBox.Height; y++)
                {
                    Color c = original.GetPixel(x, y);
                    if (!dict.ContainsKey(c))
                    {
                        dict.Add(c, 0);
                    }
                    dict[c]++;
                }
            }
            List<Color> res = new List<Color>();
            List<KeyValuePair<Color,int>> myList = dict.ToList();

            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            int i = 0;
            foreach((Color c,int j) in myList)
            {
                if (i == K) break;
                res.Add(c);
                i++;
            }
            return res;
        }

        private void SetKValue(object sender, EventArgs e)
        {
            label11.Text = "Redukcja do K kolorów: " + KTrackBar.Value;
        }

        private void ToDeafultImage(object sender, EventArgs e)
        {
            InsertDefaultImage();
        }

        private void ReductColors(object sender, EventArgs e)
        {
            colorReduction();
        }
    }
}
