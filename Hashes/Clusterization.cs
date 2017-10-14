//Clustering images with drawing dendrogram using hierarchical method

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hashes
{
    public struct Order
    {
        public List<int> set1 { get; set; }
        public List<int> set2 { get; set; }
        public double dist { get; set; }
    }

    class Clusterization
    {
        List<Iteration> iterations; Dendrogram dend;
        List<Cluster> ListOfClusters = new List<Cluster>();
        List<Myimage> imglist; List<CheckBox> checklist;
        ////
        int index1; List<List<int>> order; List<Order> orderlist;
        int index2; //List<Myimage> ListOfObj;
        double min; double max; int n;
        int RowsCount;
        int ColumnsCount; 

        public Clusterization(List<Myimage> imglist)
        {
            this.imglist = imglist;
        }


        public double MinValue(List<List<double>> m, out int i, out int j)
        {
            double min = m[0][1]; i = 0; j = 1;
            for (int k = 0; k < m[0].Count; k++)
            {
                for (int p = 0; p < m[0].Count; p++)
                {
                    if ((m[k][p] < min) && (k != p)) { min = m[k][p]; i = k; j = p; }
                }
            }
            return min;
        }

       
        public void RemoveRow(int ind, List<List<double>> m)
        {
            RowsCount--;
            m.RemoveAt(ind);
        }

        public void RemoveColumn(int ind, List<List<double>> m)
        {
            ColumnsCount--;
            foreach (var list in m)
            {
                list.RemoveAt(ind);
            }
        }

        public List<int> Copy(List<int> set)
        {
            List<int> tmp = new List<int>(set);
            return tmp;
        }

        public List<Cluster> Copy2(List<Cluster> set)
        {
            List<Cluster> tmp = new List<Cluster>(); int i = 0;
            foreach (var e in set)
            {
                tmp.Add(new Cluster());
                tmp[i].ImgInCluster.AddRange(e.ImgInCluster); i++;
            }
            return tmp;
        }

        public void PaintDend()
        {
            dend = new Dendrogram();
            dend.Height = imglist.Count * 30 + 100;
            dend.Width = max * 10 + 50;
            Line vertL = new Line();
            vertL.X1 = 20; vertL.X2 = 20;
            vertL.Y1 = imglist.Count * 30; vertL.Y2 = 10;
            vertL.Stroke = System.Windows.Media.Brushes.Black;
            dend.cnvs.Children.Add(vertL);
            Line horL = new Line();
            horL.X1 = 20; horL.X2 = 20 + max * 10;
            horL.Y1 = imglist.Count * 30; horL.Y2 = imglist.Count * 30;
            horL.Stroke = System.Windows.Media.Brushes.Black;
            dend.cnvs.Children.Add(horL);
            double h = (imglist.Count * 30 - 20) / imglist.Count;
            double h1 = (max * 10) / imglist.Count;
            double h2 = max / imglist.Count;
            var p = order.FindAll(i => i.Count != 0);
            Dictionary<int, double> dd = new Dictionary<int, double>();

            for (byte i = 0; i < imglist.Count; i++)
            {
                dd.Add(p[0][i], imglist.Count * 30 - (i + 1) * h);
                Line a = new Line();
                TextBlock txtb = new TextBlock();
                txtb.Height = 25; txtb.Width = 15;
                txtb.Text = p[0][i].ToString();
                a.X1 = 15; a.X2 = 25;
                a.Y1 = imglist.Count * 30 - (i + 1) * h;
                a.Y2 = imglist.Count * 30 - (i + 1) * h;
                a.Stroke = System.Windows.Media.Brushes.Black;
                Canvas.SetTop(txtb, imglist.Count * 30 - (i + 1) * h);
                Canvas.SetLeft(txtb, 10);
                dend.cnvs.Children.Add(a); dend.cnvs.Children.Add(txtb);
            }

            for (byte i = 0; i < imglist.Count; i++)
            {
                Line a = new Line();
                a.X1 = (20 + max * 10) - i * h1; a.X2 = (20 + max * 10) - i * h1;
                a.Y1 = imglist.Count * 30 - 5;
                a.Y2 = imglist.Count * 30 + 5;
                a.Stroke = System.Windows.Media.Brushes.Black;
                TextBlock txtb1 = new TextBlock();
                txtb1.Height = 25; txtb1.Width = 30;
                Canvas.SetTop(txtb1, imglist.Count * 30 + 10);
                Canvas.SetLeft(txtb1, max * 10 - i * h1);
                txtb1.Text = (Math.Round(max - i * h2, 2)).ToString();
                dend.cnvs.Children.Add(txtb1);
                dend.cnvs.Children.Add(a);
            }

            for (int i = 0; i < orderlist.Count; i++)
            {
                Line l1 = new Line(); Line l2 = new Line();
                if ((orderlist[i].set1.Count == 1) && (orderlist[i].set2.Count == 1))
                {
                    l1.X1 = 20; l2.X1 = 20;
                    l1.Y1 = dd[orderlist[i].set1[0]]; l2.Y1 = dd[orderlist[i].set2[0]];
                    l1.Y2 = (l1.Y1 + l2.Y1) / 2; l2.Y2 = (l1.Y1 + l2.Y1) / 2;
                    l1.X2 = l1.X1 + orderlist[i].dist * 10;
                    l2.X2 = l2.X1 + orderlist[i].dist * 10;
                    l1.Stroke = System.Windows.Media.Brushes.Red;
                    l2.Stroke = System.Windows.Media.Brushes.Red;
                    l1.StrokeThickness = 2; l2.StrokeThickness = 2;
                }
                else
                {
                    int size1 = orderlist[i].set1.Count;
                    int size2 = orderlist[i].set2.Count;
                    if (size1 == 1)
                    {
                        l1.X1 = 20;
                        l1.Y1 = dd[orderlist[i].set1[0]];
                        double w = Dend(0, l2, dd, l1, l2, orderlist[i].dist, orderlist[i].set1, orderlist[i].set2, orderlist, dend);
                    }
                    if (size2 == 1)
                    {
                        l1.X1 = 20;
                        l1.Y1 = dd[orderlist[i].set2[0]];
                        double w = Dend(0, l2, dd, l1, l2, orderlist[i].dist, orderlist[i].set2, orderlist[i].set1, orderlist, dend);
                    }
                    if ((size1 > 1) && (size2 > 1))
                    {
                        double w = Dend(0, l1, dd, l1, l2, orderlist[i].dist, orderlist[i].set2, orderlist[i].set1, orderlist, dend);
                        double w1 = Dend(w, l2, dd, l1, l2, orderlist[i].dist, orderlist[i].set1, orderlist[i].set2, orderlist, dend);
                    }
                }
                dend.cnvs.Children.Add(l1); dend.cnvs.Children.Add(l2);
            }
            dend.btnclust.Click += ButtonCluster_Click;
            dend.Show();
        }

        public double Dend(double t, Line dif, Dictionary<int, double> dd, Line l1, Line l2, double dist, List<int> set1, List<int> set2, List<Order> orderlist, Dendrogram dend)
        {
            double t1 = 0;
            l1.X2 = 20 + dist * 10;
            l2.X2 = 20 + dist * 10;
            l1.Stroke = System.Windows.Media.Brushes.Red;
            l2.Stroke = System.Windows.Media.Brushes.Red;
            l1.StrokeThickness = 2; l2.StrokeThickness = 2;
            var c = orderlist.FindAll(z => (z.set1.Count + z.set2.Count == set2.Count));
            int counter = 0; int tmp = 0;
            foreach (var k in c)
            {
                foreach (var d in set2)
                {
                    tmp = counter;
                    foreach (var z in k.set1)
                    {
                        if (d == z) counter++;
                    }
                    if (tmp == counter)
                    {
                        foreach (var z in k.set2)
                        {
                            if (d == z) counter++;
                        }
                    }
                }
                if (counter == set2.Count)
                {
                    dif.X1 = 20 + k.dist * 10;
                    foreach (var f in k.set1) t1 += dd[f];
                    foreach (var f in k.set2) t1 += dd[f];
                    dif.Y1 = t1 / (k.set1.Count + k.set2.Count);
                    if (t != 0)
                    {
                        l1.Y2 = (t1 + t) / (set2.Count + set1.Count); l2.Y2 = (t1 + t) / (set2.Count + set1.Count);
                    }
                    else
                    {
                        l1.Y2 = (t1 + l1.Y1) / (set2.Count + set1.Count); l2.Y2 = (t1 + l1.Y1) / (set2.Count + set1.Count);
                    }
                    break;
                }
            }
            return t1;
        }


        public void clusterDend()
        {
            ListOfClusters.Clear();
            iterations = new List<Iteration>();
            // ListOfObj = new List<Myimage>();
            // ListOfObj = imglist.GetRange(0, imglist.Count);
            for (int k = 0; k < imglist.Count; k++)
            {
                ListOfClusters.Add(new Cluster());
                ListOfClusters[k].ImgInCluster.Add(imglist[k]);
            }

            n = 1;
            double[,] data = new double[imglist.Count, imglist.Count];

            for (int i = 0; i < imglist.Count; i++)
            {
                for (int j = 0; j < imglist.Count; j++)
                {
                    data[i, j] = PerceptiveHash.hamming(imglist[i].Hash, imglist[j].Hash);
                }
            }

            RowsCount = imglist.Count;
            ColumnsCount = imglist.Count;
            List<List<double>> matrix = new List<List<double>>(RowsCount + 1);
            for (int i = 0; i < RowsCount; i++)
            {
                var list = new List<double>(ColumnsCount);
                for (int j = 0; j < ColumnsCount; j++)
                {
                    list.Add(data[i, j]);
                }
                matrix.Add(list);
            }
            var list1 = new List<double>(ColumnsCount);
            for (int j = 0; j < ColumnsCount; j++)
                list1.Add(j);
            matrix.Add(list1);
            order = new List<List<int>>();
            for (int i = 0; i < imglist.Count; i++)
            {
                order.Add(new List<int>());
                order[i].Add(i);
            }
            orderlist = new List<Order>();
            max = 0;
            iterations.Add(new Iteration { Clusters = Copy2(ListOfClusters) });

            while (ListOfClusters.FindAll(i => (i.ImgInCluster.Count != 0)).Count != n)
            {
                min = MinValue(matrix, out index1, out index2);
                if (min > max) max = min;
                double d1 = 0; double d2 = 0;

                for (int i = 0; i < ColumnsCount; i++)
                {
                    if (index1 != i)
                    {
                        d1 = matrix[index1][i];
                        d2 = matrix[index2][i];
                        matrix[index1][i] = (d1 + d2) / 2;
                        matrix[i][index1] = (d1 + d2) / 2;
                    }
                }
                orderlist.Add(new Hashes.Order { set1 = Copy(order[Convert.ToInt32(matrix[RowsCount][index1])]), set2 = Copy(order[Convert.ToInt32(matrix[RowsCount][index2])]), dist = min });
                order[Convert.ToInt32(matrix[RowsCount][index1])].AddRange(order[Convert.ToInt32(matrix[RowsCount][index2])]);
                order[Convert.ToInt32(matrix[RowsCount][index2])].Clear();
                ListOfClusters[Convert.ToInt32(matrix[RowsCount][index1])].ImgInCluster.AddRange(ListOfClusters[Convert.ToInt32(matrix[RowsCount][index2])].ImgInCluster);
                ListOfClusters[Convert.ToInt32(matrix[RowsCount][index2])].ImgInCluster.Clear();
                iterations.Add(new Iteration { Clusters = Copy2(ListOfClusters) });
                RemoveRow(index2, matrix);
                RemoveColumn(index2, matrix);

            }
            PaintDend();
        }

          private void ButtonCluster_Click(object sender, RoutedEventArgs e)
       {
           checklist = new List<CheckBox>(); int c = 0;
           Window1 wind = new Window1(); int x = 0;
           wind.grd2.Children.Clear();
           RowDefinition gridRow1 = new RowDefinition();
           wind.grd2.RowDefinitions.Add(gridRow1);
           Button btndel = new Button();
           btndel.Height = 30; btndel.Content = "Delete chosen files";
           btndel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
           MainWindow mw = new MainWindow(checklist);
           btndel.Click += mw.btndel_Click;
           Grid.SetRow(btndel, x); x++;
           wind.grd2.Children.Add(btndel);
           int number = int.Parse(dend.numbOfcluster.Text);
           iterations.Reverse();
           for (int i = 0; i < iterations[number - 1].Clusters.Count; i++)
           {
               if (iterations[number - 1].Clusters[i].ImgInCluster.Count >= 1)
               {
                   WrapPanel wp = new WrapPanel();
                   for (int j = 0; j < iterations[number - 1].Clusters[i].ImgInCluster.Count; j++)
                   {
                       System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                       Uri uriSource = new Uri(iterations[number - 1].Clusters[i].ImgInCluster[j].ImagePath);
                       BitmapImage imgTemp = new BitmapImage();
                       imgTemp.BeginInit();
                       imgTemp.CacheOption = BitmapCacheOption.OnLoad;
                       imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                       imgTemp.UriSource = uriSource;
                       imgTemp.EndInit();
                       im.Source = imgTemp;
                       im.Width = 100; im.MouseEnter += new MouseEventHandler(mw.OnMouseEnterHandler);
                       im.Height = 75; im.MouseLeave += new MouseEventHandler(mw.OnMouseLeaveHandler);
                       ToolTip tooltip = new ToolTip { Content = iterations[number - 1].Clusters[i].ImgInCluster[j].ImagePath };
                       im.ToolTip = tooltip;
                       im.Margin = new Thickness(5, 5, 5, 5);

                       checklist.Add(new CheckBox());
                       checklist[c].Content = im;
                       wp.Children.Add(checklist[c]); c++;
                   }
                   RowDefinition gridRow = new RowDefinition();
                   wind.grd2.RowDefinitions.Add(gridRow);
                   Grid.SetRow(wp, x); x++;
                   wind.grd2.Children.Add(wp);
               }
           }
           iterations.Reverse();
           wind.Show();
       }

    }
}
