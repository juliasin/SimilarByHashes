using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hashes
{
    class Dist { public double[][] distt1; }
    class Stolp
    {
        public int index;
        public List<Myimage> fullobjects;
        public List<List<Myimage>> objects;
        public List<Myimage> etal1;
        public List<Myimage> NotRec1;
        string[] dirpaths;

        public class Arrays
        {
            public double[][] dist;
            public double[][] W;
            public double[] minn; public double[][] minn1;
            public List<Dist> d;

            public Arrays(Stolp parent, List<Myimage> ob, List<List<Myimage>> obs)
            {
                minn = new double[ob.Count];
                minn1 = new double[obs.Count][];
                W = new double[obs.Count][];
                d = new List<Dist>();
                for (int i = 0; i < obs.Count; i++)
                {
                    d.Add(new Dist());
                }


                if (ob.Count != 0)
                {
                    dist = new double[ob.Count][];
                    for (int i = 0; i < ob.Count; i++) { dist[i] = new double[ob.Count]; }
                    parent.DistAtOneClass(ob.Count, dist, ob);
                    //min = new double[ob.Count];
                    for (int i = 0; i < ob.Count; i++) { minn[i] = parent.MinAtTwo(dist, ob.Count, ob.Count, i); }
                }

                List<double[][]> distt1 = new List<double[][]>();

                for (int i = 0; i < obs.Count; i++)
                {
                    if ((ob.Count != 0) && (obs[i].Count != 0))
                    {
                        d[i].distt1 = new double[ob.Count][];
                        // distt1[i] = new double[ob.Count][];
                        for (int j = 0; j < ob.Count; j++) { d[i].distt1[j] = new double[obs[i].Count]; }
                        parent.DistBetwTwoClasses(ob.Count, obs[i].Count, d[i].distt1, ob, obs[i]);
                        minn1[i] = new double[ob.Count];
                        for (int j = 0; j < ob.Count; j++) { minn1[i][j] = parent.MinAtTwo(d[i].distt1, ob.Count, obs[i].Count, j); }
                        W[i] = new double[ob.Count];
                        for (int j = 0; j < ob.Count; j++) { W[i][j] = minn[j] / minn1[i][j]; }
                    }
                }
            }

        }


        public Stolp(List<Myimage> t, string[] paths)
        {
            fullobjects = new List<Myimage>(t);
            etal1 = new List<Myimage>();
            NotRec1 = new List<Myimage>();
            // print = new List<string>();
            dirpaths = paths;
            ListImages();
        }
        public void StartQuality(List<Myimage> t)
        {
            fullobjects = new List<Myimage>(t);           
            etal1 = new List<Myimage>();
            NotRec1 = new List<Myimage>();            
            ListImages();
        }

        static public void Sort(List<Myimage> lst)
        {
            Random rnd = new Random();
            int n = lst.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                Myimage value = lst[k];
                lst[k] = lst[n];
                lst[n] = value;
            }
        }

        public void Quality()
        {
            List<Myimage> exam = new List<Myimage>();
            List<Myimage> teach = new List<Myimage>();
            double c1 = Math.Truncate(0.7 * fullobjects.Count);
            int[] mstk = new int[20]; double pr = 0;
            double[] p = new double[20]; List<Myimage> tmp = new List<Myimage>(fullobjects);
            for (int k = 0; k < 20; k++)
            {
                Sort(fullobjects); teach.Clear(); exam.Clear();
                for (int i = 0; i < c1; i++) { teach.Add(fullobjects[i]); }
                for (int i = Convert.ToInt32(c1); i < fullobjects.Count; i++) { exam.Add(fullobjects[i]); }
                StartQuality(teach);
                Check();

                for (int i = 0; i < exam.Count; i++)
                {
                    if (Equals(exam[i].DirectoryPath, Recognize(exam[i])) != true) mstk[k]++;
                                       
                }
                p[k] = mstk[k] * 1.0 / exam.Count;
                double y = 100 - p[k] * 100;
                MessageBox.Show(y.ToString());
                fullobjects=new List<Myimage>(tmp);
            }

            double sum = 0;
            for (int i = 0; i < 20; i++) sum += p[i];
            pr = sum / 20 * 100;
            MessageBox.Show("Wrong = " + pr.ToString() + "%");
            MessageBox.Show("Correct = " + (100 - pr).ToString() + "%");
        }


        private void ListImages()
        {
            objects = new List<List<Myimage>>();
            for (int i = 0; i < dirpaths.Length; i++) objects.Add(new List<Myimage>());
            for (int i = 0; i < dirpaths.Length; i++)
            {
                for (int j = 0; j < fullobjects.Count; j++)
                {
                    if (Equals(fullobjects[j].DirectoryPath, dirpaths[i])) { objects[i].Add(fullobjects[j]); }
                }
            }
        }



        public void DistAtOneClass(int n, double[][] arr, List<Myimage> temp)
        {
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    { if (j != k) { arr[j][k] = PerceptiveHash.hamming(temp[j].Hash, temp[k].Hash); } }
                }
            }
        }

        public void DistBetwTwoClasses(int n1, int n2, double[][] arr, List<Myimage> temp1, List<Myimage> temp2)
        {
            for (int j = 0; j < n1; j++)
            {
                for (int k = 0; k < n2; k++)
                {
                    { arr[j][k] = PerceptiveHash.hamming(temp1[j].Hash, temp2[k].Hash); }
                }
            }
        }

        public double MinAtTwo(double[][] arr, int n1, int n2, int ind)
        {
            double m = 0;
            for (int i = 0; i < n1; i++)
            {
                if (i == ind)
                {
                    /*if (ind == 0)
                        m = arr[i][1];
                    else*/
                        m = arr[i][0];
                    for (int j = 0; j < n2; j++)
                    {
                        if (n1 == n2) { if ((i != j) && (arr[i][j] < m)) m = arr[i][j]; }
                        if (n1 != n2) { if (arr[i][j] < m) m = arr[i][j]; }
                    }
                }
            }
            return m;
        }

        public double MaxW(double[] arr)
        {
            int c = 0; index = -1;
            double m = arr[0];
            for (int i = 0; i < arr.Length; i++) { if (m < arr[i]) { m = arr[i]; c = i; } }
            index = c; return m;
        }

        public string Recognize(Myimage X)
        {
            Myimage k = new Myimage();
            double min = PerceptiveHash.hamming(X.Hash, etal1[0].Hash);
            for (int i = 0; i < etal1.Count; i++)
            {
                Int64 h = PerceptiveHash.hamming(X.Hash, etal1[i].Hash);
                if (min >= h) { min = h; k = etal1[i]; }
            }
            return k.DirectoryPath;
        }

        public void Check()
        {
            Counting(); NotRec1.Clear();
            for (int i = 0; i < fullobjects.Count; i++)
            {
                if (Equals(fullobjects[i].DirectoryPath, Recognize(fullobjects[i])) != true)

                {
                    NotRec1.Add(fullobjects[i]);
                }
            }

            if (NotRec1.Count != 0)
            { fullobjects.Clear(); fullobjects.AddRange(NotRec1); ListImages(); Check(); }
        }

        public List<List<Myimage>> Copy(List<List<Myimage>> set)
        {
            List<List<Myimage>> tmp = new List<List<Myimage>>();
            for (int i = 0; i < set.Count; i++)
            {
                tmp.Add(new List<Myimage>());
                for (int j = 0; j < set[i].Count; j++)
                {
                    tmp[i].Add(new Myimage());
                    tmp[i][j].DirectoryPath = set[i][j].DirectoryPath;
                    tmp[i][j].ImagePath = set[i][j].ImagePath;
                    tmp[i][j].Hash = set[i][j].Hash;
                }
            }
            return tmp;
        }

        public void Counting()
        {
            double[][] maxW = new double[dirpaths.Length][];
            for (int i = 0; i < dirpaths.Length; i++) maxW[i] = new double[dirpaths.Length];
            for (int i = 0; i < dirpaths.Length; i++)
            {
                List<List<Myimage>> tmp = new List<List<Myimage>>();
                tmp = Copy(objects);
                tmp.RemoveAt(i);
                Arrays a = new Arrays(this, objects[i], tmp);
                for (int j = 0; j < tmp.Count; j++)
                {
                    if (a.W[j] != null)
                        maxW[i][j] = MaxW(a.W[j]);
                    if (index<objects[i].Count)
                    {
                        if (!(etal1.Contains(objects[i][index])))
                            etal1.Add(objects[i][index]);
                    }
                }
            }

            for (int i = 0; i < dirpaths.Length; i++)
            {
                List<List<Myimage>> tmp = new List<List<Myimage>>();
                tmp = Copy(objects);
                tmp.RemoveAt(i);
                int y = 0;
                foreach (var z in tmp)
                {
                    if (z.Count != 0) y++;
                }
                if ((objects[i].Count != 0) && (y == 0)) { etal1.Add(objects[i][0]); }
            }

        }
    }
}
