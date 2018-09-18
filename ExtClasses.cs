using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositBridgeBuilder
{
    class Node
    {
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
    }
    class Beam
    {
        public int ID { get; set; }
        public Node Ni { get; set; }
        public Node Nj { get; set; }
        public Node Nk { get; set; }
        public int SecN { get; set; }
        public int MatN { get; set; }
        public double LocX
        {
            get
            {
                return (Ni.X + Nj.X) * 0.5;
            }
        }
        public double LocY
        {
            get
            {
                return (Ni.Y + Nj.Y) * 0.5;
            }
        }
        public string Xdata { get; set; }
    }

    class Plate
    {
        public int ID { get; set; }
        public Node Ni { get; set; }
        public Node Nj { get; set; }
        public Node Nk { get; set; }
        public Node Nl { get; set; }

        public Plate(int id,Node i,Node j,Node k, Node l)
        {
            ID = id;
            Ni = i;
            Nj = j;
            Nk = k;
            Nl = l;
        }

    }

    class IBSection
    {
        public int ID { set; get; }
        public string Name { get; set; }
        public double H1 { set; get; }
        public double H2 { set; get; }
        public double H3 { set; get; }
        public double T1 { set; get; }
        public double T2 { set; get; }
        public double T3 { set; get; }
        public string GetString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", ID, Name, H1, H2, H3, T1, T2, T3);
        }
    }
    class SplitTuple
    {
        public int SectID { set; get; }
        public double Length { set; get; }
    }
    enum CMethod { YiCi, ZhuKua }
    enum MDVersion { Md15, Md17 }
    enum Concrete { C50, C55 }
    enum Steel { Q345, Q420 }

    class Ext
    {
        public static List<double> Arange(double st, double ed, double step)
        {
            List<double> res = new List<double>();

            res.Add(st);
            while (res.Last() < ed)
            {
                res.Add(res.Last() + step);
            }
            return res;
        }
    }

}
