using System;
using System.Collections.Generic;


namespace Hashes
{
   public class Cluster
    {
        public List<Myimage> ImgInCluster = new List<Myimage>();
    }
    class Iteration
    {
        public List<Cluster> Clusters { get; set; }
    }
}
