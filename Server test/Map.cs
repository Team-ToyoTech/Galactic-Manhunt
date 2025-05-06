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
using System.Net.Security;

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
        public Map(List<Client>clients, List<Galaxy>galaxies)
        {
            InitializeComponent();
            int[,] robLocates = new int[710, 460];
            int[,] copLocates = new int[710, 460];
            bool[,] visit = new bool[710, 460];
            List<Tuple<int, int>> galaxyLocates = new List<Tuple<int, int>>();
            List<Tuple<int,int>>galaxyLocates2 = new List<Tuple<int, int>>();

            foreach(var galaxy in galaxies)
            {
                Vector2 vector = galaxy.Location;
                galaxyLocates2.Add(new Tuple<int,int>((int)vector.x, (int)vector.y));
            }

            foreach(var client in clients)
            {
                Vector2 vector = client.galaxy.location;
                if (client.GetType() == Client.PlayerType.cop) copLocates[(int)vector.x, (int)vector.y]++;
                else if (client.GetType() == Client.PlayerType.robber) robLocates[(int)vector.x, (int)vector.y]++;
                if (!visit[(int)vector.x, (int)vector.y])
                {
                    visit[(int)vector.x, (int)vector.y] = true;
                    galaxyLocates.Add(new Tuple<int,int>((int)vector.x, (int)vector.y));
                }   
            }

            int width = 701;
            int height = 449;

            colors[] buff = CreateColorBuffer(width, height);

            foreach (var galaxy in galaxyLocates2)
            {
                int x = galaxy.Item1;
                int y = galaxy.Item2;

                buff[(x * 4) * height + y].blue = 0;
                buff[(x * 4) * height + y].red = 0;
                buff[(x * 4) * height + y].green = 0;
            }

            for(int i = 0; i < galaxyLocates.Count; i++)
            {
                int x = galaxyLocates[i].Item1;
                int y= galaxyLocates[i].Item2;
                

                if (copLocates[x,y] > 0)
                {
                    buff[(x * 4) * height + y].blue = 254;
                }
                if (robLocates[x,y] > 0)
                {
                    buff[(x * 4) * height + y].red = 254;
                }
            }

            Bitmap bit = CreateColorBitMap(
                width, height, buff);            

            pictureBox1.Image = bit;

        }



        public colors[] CreateColorBuffer(int width, int height)
        {
            colors[] buffer = new colors[width * height];
            for (int i = 0; i < width * height; i++)
            {
                buffer[i].red = 254;
                buffer[i].green = 254;
                buffer[i].blue = 254;
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

        


    }
}
