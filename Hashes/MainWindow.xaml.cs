using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using DevExpress.Xpf.Core;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.VisualBasic;


namespace Hashes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : DXWindow
    {
        BitmapImage bm1; List<Int64> rez;
        List<Cluster> ListOfClusters = new List<Cluster>();
        List<Myimage> imglist; List<CheckBox> checklist;
        Myimage m;
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(List<CheckBox> checklist)
        {
            this.checklist = checklist;
            InitializeComponent();
        }

        public void CheckHash(RadioButton rb1, RadioButton rb2, RadioButton rb3, RadioButton rb4, BitmapImage bm1, Myimage m)
        {
            if (rb1.IsChecked == true) m.Hash = PerceptiveHash.aHash(bm1);
            if (rb2.IsChecked == true) m.Hash = PerceptiveHash.pHash(bm1);
            if (rb3.IsChecked == true) m.Hash = PerceptiveHash.dHash(bm1);
            if (rb4.IsChecked == true) m.Hash = PerceptiveHash.gHash(bm1);
        }
        public void GetDirectories(string path)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                m = new Myimage { ImagePath = fileInfo.FullName, Hash = 0, DirectoryPath = "" };
                Uri uriSource1 = new Uri(fileInfo.FullName);
                bm1 = new BitmapImage();
                bm1.BeginInit();
                bm1.CacheOption = BitmapCacheOption.OnLoad;
                bm1.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bm1.UriSource = uriSource1;
                bm1.EndInit();
                CheckHash(rb1, rb2, rb3, rb4, bm1, m);
                string[] dirs = Directory.GetDirectories(path);
                imglist = new List<Myimage>();
                foreach (var t in dirs)
                {
                    string[] fullfilesPath = Directory.GetFiles(t, "*.jpg", SearchOption.AllDirectories);
                    fullfilesPath = fullfilesPath.Concat(Directory.GetFiles(t, "*.png", SearchOption.AllDirectories)).ToArray();
                    fullfilesPath = fullfilesPath.Concat(Directory.GetFiles(t, "*.bmp", SearchOption.AllDirectories)).ToArray();
                    fullfilesPath = fullfilesPath.Concat(Directory.GetFiles(t, "*.jpeg", SearchOption.AllDirectories)).ToArray();
                    List<Myimage> temp = new List<Myimage>();
                    for (int i = 0; i < fullfilesPath.Length; i++)
                    {
                        temp.Add(new Myimage { ImagePath = fullfilesPath[i], Hash = 0, DirectoryPath = t });
                        Uri uriSource = new Uri(temp[i].ImagePath);
                        bm1 = new BitmapImage();
                        bm1.BeginInit();
                        bm1.CacheOption = BitmapCacheOption.OnLoad;
                        bm1.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bm1.UriSource = uriSource;
                        bm1.EndInit();
                        CheckHash(rb1, rb2, rb3, rb4, bm1, temp[i]);
                    }
                    imglist.AddRange(temp);
                }
                Stolp st = new Stolp(imglist, dirs);
                 st.Check();
             // to check quality
             //   st.Quality();
              
               m.DirectoryPath = st.Recognize(m);
               MessageBox.Show(m.DirectoryPath);
            }
            else { MessageBox.Show("Please choose a picture"); }
        }



        public void FindSimilar(Myimage m)
        {
            wrp.Children.Clear();
            string s = Interaction.InputBox("Enter % of similarity");
            double perc = int.Parse(s);
            perc = Math.Round(perc * 0.64);
            List<Myimage> similarList = new List<Myimage>();
            for (int j = 0; j < imglist.Count; j++)
            {
                Int64 ham = PerceptiveHash.hamming(m.Hash, imglist[j].Hash);
                if (ham <= (64 - perc))
                {
                    similarList.Add(imglist[j]);
                }
            }
            for (int i = 0; i < similarList.Count; i++)
            {
                Uri uriSource = new Uri(similarList[i].ImagePath);
                bm1 = new BitmapImage();
                bm1.BeginInit();
                bm1.UriSource = uriSource;
                bm1.EndInit();
                System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                im.Source = bm1; im.Width = 100; im.Height = 75;
                im.MouseEnter += new MouseEventHandler(OnMouseEnterHandler);
                im.MouseLeave += new MouseEventHandler(OnMouseLeaveHandler);
                ToolTip tooltip = new ToolTip { Content = similarList[i].ImagePath };
                im.ToolTip = tooltip;
                im.Margin = new Thickness(5, 5, 5, 5);
                wrp.Children.Add(im); scrollv.ScrollToBottom();
            }
        }

        public void LoadFiles(string path)
        {
            string[] fullfilesPath = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);
            fullfilesPath = fullfilesPath.Concat(Directory.GetFiles(path, "*.png", SearchOption.AllDirectories)).ToArray();
            fullfilesPath = fullfilesPath.Concat(Directory.GetFiles(path, "*.bmp", SearchOption.AllDirectories)).ToArray();
            fullfilesPath = fullfilesPath.Concat(Directory.GetFiles(path, "*.jpeg", SearchOption.AllDirectories)).ToArray();
            imglist = new List<Myimage>();
            prbar.Maximum = fullfilesPath.Length; wrp.Children.Clear();
            for (int i = 0; i < fullfilesPath.Length; i++)
            {
                imglist.Add(new Myimage { ImagePath = fullfilesPath[i], Hash = 0, DirectoryPath = "" });
                Uri uriSource = new Uri(imglist[i].ImagePath);
                bm1 = new BitmapImage();
                bm1.BeginInit();
                bm1.CacheOption = BitmapCacheOption.OnLoad;
                bm1.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bm1.UriSource = uriSource;
                bm1.EndInit();
                CheckHash(rb1, rb2, rb3, rb4, bm1, imglist[i]);
                System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                im.Source = bm1; im.Width = 100; im.Height = 75;
                im.MouseEnter += new MouseEventHandler(OnMouseEnterHandler);
                im.MouseLeave += new MouseEventHandler(OnMouseLeaveHandler);
                ToolTip tooltip = new ToolTip { Content = imglist[i].ImagePath };
                im.ToolTip = tooltip;
                im.Margin = new Thickness(5, 5, 5, 5);
                wrp.Children.Add(im); scrollv.ScrollToBottom();
                prbar.Dispatcher.Invoke(() => prbar.Value = i + 1, DispatcherPriority.Background);

            }
        }

        public void FindGroup()
        {
            rez = new List<Int64>();
            string s = Interaction.InputBox("Enter % of similarity");
            double perc = int.Parse(s);
            perc = Math.Round(perc * 0.64);
            ListOfClusters.Clear();
            for (int k = 0; k < imglist.Count; k++)
            {
                ListOfClusters.Add(new Cluster());
            }
            List<Myimage> tmp = new List<Myimage>();

            for (int i = 0; i < imglist.Count; i++)
            {
                prbar.Dispatcher.Invoke(() => prbar.Value++, DispatcherPriority.Background);
                int p = 0;
                for (int k = 0; k < ListOfClusters.Count; k++)
                {
                    if (ListOfClusters[k].ImgInCluster.Contains(imglist[i])) p++;
                }
                if (p == 0)
                { ListOfClusters[i].ImgInCluster.Add(imglist[i]); tmp.Add(imglist[i]); }
                for (int j = 0; j < imglist.Count; j++)
                {
                    Int64 ham = PerceptiveHash.hamming(imglist[i].Hash, imglist[j].Hash);
                    if ((ham <= (64 - perc)) && (i != j) && (!tmp.Contains(imglist[j])))
                    {
                        ListOfClusters[i].ImgInCluster.Add(imglist[j]);
                        tmp.Add(imglist[j]);
                    }
                }
            }
        }

        private void btnf_Click(object sender, RoutedEventArgs e)
        {
            string s = txtbox.Text;
            LoadFiles(s);
            prbar.Value = 0;
        }

        private void btnclass_Click(object sender, RoutedEventArgs e)
        {
            string s = txtbox.Text;
            if (s.Length == 0) { MessageBox.Show("Please enter the path before classification"); }
            else GetDirectories(s);
        }

        internal void OnMouseEnterHandler(object sender, MouseEventArgs e)
        {
            (sender as System.Windows.Controls.Image).Width = 150;
            (sender as System.Windows.Controls.Image).Height = 125;
        }

        internal void OnMouseLeaveHandler(object sender, MouseEventArgs e)
        {
            (sender as System.Windows.Controls.Image).Width = 100;
            (sender as System.Windows.Controls.Image).Height = 75;
        }


        internal void btndel_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < checklist.Count; i++)
            {
                if (checklist[i].IsChecked == true)
                {
                    string path = ((BitmapImage)(checklist[i].Content as System.Windows.Controls.Image).Source).UriSource.LocalPath;
                    (checklist[i].Content as System.Windows.Controls.Image).Source = null;
                    checklist[i].Content = null;
                    checklist[i].Visibility = Visibility.Collapsed;
                    File.Delete(path);
                }
            }
        }


        private void btnfind_Click(object sender, RoutedEventArgs e)
        {
            if ((imglist == null) || (imglist.Count == 0)) MessageBox.Show("Please enter the path and click LoadFiles");
            else
            {
                prbar.Maximum = imglist.Count + ListOfClusters.Count;
                FindGroup(); int check = 0;
                for (int i = 0; i < ListOfClusters.Count; i++)
                {
                    prbar.Dispatcher.Invoke(() => prbar.Value++, DispatcherPriority.Background);
                    if (ListOfClusters[i].ImgInCluster.Count > 1)
                    {
                        check++;
                    }
                }
                if (check == 0) MessageBox.Show("Similar images are not found!");
                else
                {
                    checklist = new List<CheckBox>(); int c = 0;
                    Window1 wind = new Window1(); int x = 0;
                    RowDefinition gridRow1 = new RowDefinition();
                    wind.grd2.RowDefinitions.Add(gridRow1);
                    Button btndel = new Button();
                    btndel.Height = 30; btndel.Content = "Delete chosen files";
                    btndel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    btndel.Click += btndel_Click;
                    Grid.SetRow(btndel, x); x++;
                    wind.grd2.Children.Add(btndel);
                    for (int i = 0; i < ListOfClusters.Count; i++)
                    {
                        if (ListOfClusters[i].ImgInCluster.Count > 1)
                        {
                            WrapPanel wp = new WrapPanel();
                            for (int j = 0; j < ListOfClusters[i].ImgInCluster.Count; j++)
                            {
                                System.Windows.Controls.Image im = new System.Windows.Controls.Image();
                                Uri uriSource = new Uri(ListOfClusters[i].ImgInCluster[j].ImagePath);
                                BitmapImage imgTemp = new BitmapImage();
                                imgTemp.BeginInit();
                                imgTemp.CacheOption = BitmapCacheOption.OnLoad;
                                imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                                imgTemp.UriSource = uriSource;
                                imgTemp.EndInit();
                                im.Source = imgTemp;
                                //im.Source = new BitmapImage(new Uri(ListOfClusters[i].ImgInCluster[j].ImagePath));
                                im.Width = 100; im.MouseEnter += new MouseEventHandler(OnMouseEnterHandler);
                                im.Height = 75; im.MouseLeave += new MouseEventHandler(OnMouseLeaveHandler);
                                ToolTip tooltip = new ToolTip { Content = ListOfClusters[i].ImgInCluster[j].ImagePath };
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

                    wind.Show();
                }
                prbar.Value = 0;
            }
        }

        private void btnclust_Click(object sender, RoutedEventArgs e)
        {
            if ((imglist == null) || (imglist.Count == 0)) MessageBox.Show("Please enter the path and click LoadFiles");
            else
            {
                Clusterization cl = new Clusterization(imglist);
                cl.clusterDend();
            }
        }

        private void btnsimilar_Click(object sender, RoutedEventArgs e)
        {
            if ((imglist == null) || (imglist.Count == 0)) MessageBox.Show("Please enter the path and click LoadFiles");
            else
            {
                Myimage mm = new Myimage();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    mm.ImagePath = fileInfo.FullName;
                    Uri uriSource = new Uri(fileInfo.FullName);
                    bm1 = new BitmapImage();
                    bm1.BeginInit();
                    bm1.UriSource = uriSource;
                    bm1.EndInit();
                    CheckHash(rb1, rb2, rb3, rb4, bm1, mm);
                    FindSimilar(mm);
                    prbar.Value = 0;
                }
                else { MessageBox.Show("Please choose a picture"); }
            }
        }
    }
}
