using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Hashes
{
    static class PerceptiveHash
    {
       static public UInt64 aHash(BitmapImage img)
        {
            Bitmap bmp = BitmapImage2Bitmap(img);
            bmp = resizeImage(bmp, new System.Drawing.Size(8, 8));
            bmp = GreyScalling(bmp);
            double[,] k = Matrix(bmp);
            BitArray bits = SetOfBits1(k);
            UInt64 hash = Hash(bits);
            return hash;
        }

       static public UInt64 pHash(BitmapImage img)
        {
            Bitmap bmp = BitmapImage2Bitmap(img);
            bmp = resizeImage(bmp, new System.Drawing.Size(32, 32));
            bmp = GreyScalling(bmp);
            double[,] k = Matrix(bmp);
            double[,] dct = DCT(k);
            double[,] reduce = Reduce(dct, 8);
            BitArray bits = SetOfBits1(reduce);
            UInt64 hash = Hash(bits);
            return hash;
        }

        static public UInt64 dHash(BitmapImage img)
        {
            Bitmap bmp = BitmapImage2Bitmap(img);
            bmp = resizeImage(bmp, new System.Drawing.Size(9, 8));
            bmp = GreyScalling(bmp);
            double[,] k = Matrix(bmp);
            double[,] difmatr = differenceMatrix(k);
            BitArray bits = SetOfBitsDHash(difmatr);
            UInt64 hash = Hash(bits);
            return hash;
        }

        static public UInt64 gHash(BitmapImage img)
        {
            Bitmap bmp = BitmapImage2Bitmap(img);
            bmp = resizeImage(bmp, new System.Drawing.Size(32, 32));
            bmp = GreyScalling(bmp);
            double[,] k = Matrix(bmp);
            BitArray bits = SetOfBitsGHash(k);
            UInt64 hash = Hash(bits);
            return hash;
        }

        static public BitArray SetOfBitsGHash(double[,] d)
        {
            int s = d.GetLength(0);
            double[] sumcols = new double[s];
            for (int j = 0; j < s; j++)
            {
                for (int i = 0; i < s; i++)
                {
                    sumcols[j] += d[i, j];
                }
            }
            double[] sumrows = new double[s];
            for (int i = 0; i < s; i++)
            {
                for (int j = 0; j < s; j++)
                {
                    sumrows[i] += d[i, j];
                }
            }
            BitArray arr = new BitArray(64);
            int k = 63;
            for (int i = 0; i < sumcols.Length - 1; i++)
            {
                if (sumcols[i] > sumcols[i + 1]) arr[k] = true; else arr[k] = false;
                k--;
            }
            if (sumcols[sumcols.Length - 1] > sumcols[0]) arr[k] = true; else arr[k] = false;
            k--;
            for (int i = 0; i < sumrows.Length - 1; i++)
            {
                if (sumrows[i] > sumrows[i + 1]) arr[k] = true; else arr[k] = false;
                k--;
            }
            if (sumrows[sumrows.Length - 1] > sumrows[0]) arr[k] = true; else arr[k] = false;
            k--;

            return arr;
        }

        static public double[,] differenceMatrix(double[,] d)
        {
            double[,] difmatr = new double[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    difmatr[i, j] = d[i, j + 1] - d[i, j];
                }
            }
            return difmatr;
        }

        static public BitArray SetOfBitsDHash(double[,] y)
        {
            BitArray arr = new BitArray(8 * 8); int k = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (y[i, j] > 0) arr[k] = true; else arr[k] = false;
                    k++;
                }
            }
            return arr;
        }


        static public double[,] DCT(double[,] matrix)
        {
            int size = matrix.GetLength(0);
            double[,] resultmatr = new double[size, size];
            for (int u = 0; u < size; u++)
            {
                for (int v = 0; v < size; v++)
                {
                    double result = 0d;
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            result += (alpha(i) * alpha(j) * Math.Cos(((Math.PI * u) / (2 * size)) * (2 * i + 1)) * Math.Cos(((Math.PI * v) / (2 * size)) * (2 * j + 1)) * matrix[i, j]);
                        }
                    }
                    resultmatr[u, v] = Math.Round((result * 2 / Math.Sqrt(size * size)) * (1d / size + 1d / size));
                }
            }
            return resultmatr;
        }


        static private double alpha(int u)
        {
            if (u == 0)
                return 1 / Math.Sqrt(2);
            else
                return 1;
        }

        static public double[,] Reduce(double[,] resultmatr, int newsize)
        {
            // double[,] tmp = new double[oldsize, oldsize];
            double[,] reducematrix = new double[newsize, newsize];
            for (int i = 0; i < newsize; i++)
            {
                for (int j = 0; j < newsize; j++)
                {
                    reducematrix[i, j] = resultmatr[i, j];
                }
            }
            return reducematrix;
        }

        static public Int64 hamming(UInt64 x, UInt64 y)
        {
            Int64 dist = 0;
            UInt64 val = x ^ y;

            // Count the number of bits set
            while (val != 0)
            {
                // A bit is set, so increment the count and clear the bit
                dist++;
                val &= val - 1;
            }

            // Return the number of differing bits
            return dist;
        }


        static public BitArray SetOfBits1(double[,] cl)
        {
            int size = cl.GetLength(0);
            double avg = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    avg += cl[i, j];
                }
            }
            avg = avg / (size * size);
            BitArray arr = new BitArray(size * size); int k = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (cl[i, j] < avg) arr[k] = false; else arr[k] = true;
                    k++;
                }
            }
            return arr;
        }

        static public UInt64 Hash(BitArray arr)
        {
            var bytes = new byte[8];
            arr.CopyTo(bytes, 0);
            UInt64 hash = BitConverter.ToUInt64(bytes, 0);
            return hash;
        }

        static public Bitmap resizeImage(Bitmap imgToResize, System.Drawing.Size size)
        {
            return new Bitmap(imgToResize, size);
        }

        static public Bitmap GreyScalling(Bitmap c)
        {
            Bitmap d = new Bitmap(c.Width, c.Height);
            for (int i = 0; i < c.Width; i++)
            {
                for (int x = 0; x < c.Height; x++)
                {
                    System.Drawing.Color oc = c.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    System.Drawing.Color nc = System.Drawing.Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    d.SetPixel(i, x, nc);
                }
            }
            return d;
        }

        static public double[,] Matrix(Bitmap b)
        {
            System.Drawing.Color[,] clrs = new System.Drawing.Color[b.Height, b.Width];
            double[,] d = new double[b.Height, b.Width];
            for (int i = 0; i < b.Height; i++)
                for (int j = 0; j < b.Width; j++)
                {
                    clrs[i, j] = b.GetPixel(j, i);
                    d[i, j] = clrs[i, j].B;
                }
            return d;

        }
        static public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }
    }
}
