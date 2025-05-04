using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

namespace Server_test
{
    public struct colors
    {
        public int red;
        public int green;
        public int blue;
    }
    public partial class Map : Form
    {
        public Map(List<Client> clients)
        {
            InitializeComponent();
            int[,] locates = new int[710, 460];
            foreach(var client in clients)
            {
                Vector2 vector = client.galaxy.location;
                locates[(int)vector.x, (int)vector.y]++;
            }
        }

        public colors[] CreateColorBuffer(int width, int height)
        {
            colors[] buffer = new colors[width * height];
            for (int i = 0; i < width * height; i++)
            {
                buffer[i].red = 0;
                buffer[i].green = 0;
                buffer[i].blue = 0;
            }

            return buffer;
        }

        unsafe public Bitmap CreateColorBitMap(int width, int height, colors[] buffer)
        {
            var data = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var value = buffer[y * width + x];

                    data[(y * 4) * width + (x * 4 + 0)] = (byte)buffer[y * width + x].red;
                    data[(y * 4) * width + (x * 4 + 1)] = (byte)buffer[y * width + x].green;
                    data[(y * 4) * width + (x * 4 + 2)] = (byte)buffer[y * width + x].blue;
                    data[(y * 4) * width + (x * 4 + 3)] = 0;
                }
            }

            Bitmap image;

            unsafe
            {
                image = new Bitmap(width, height, PixelFormat.Format32bppRgb);

                BitmapData bmpData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.WriteOnly, image.PixelFormat);

                Marshal.Copy(data, 0, bmpData.Scan0, data.Length);

                image.UnlockBits(bmpData);
            }

            System.GC.Collect(0, GCCollectionMode.Forced);
            System.GC.WaitForFullGCComplete();

            return image;
        }

        public void LocationChange()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int width = 701;
            int height = 449;

            colors[] buff = CreateColorBuffer(width, height);

            Bitmap bit = CreateColorBitMap(width, height, buff);

            pictureBox1.Image = bit;
        }
    }
}