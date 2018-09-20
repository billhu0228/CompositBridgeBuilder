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
        public Node( int id, double x0,double y0,double z0)
        {
            ID = id;
            X = x0;
            Y = y0;
            Z = z0;
        }
        
    }
    class Beam
    {
        public int ID { get; set; }
        public Node Ni { get; set; }
        public Node Nj { get; set; }
        public Node Nk { get; set; }
        public int SecN { get; set; }
        public int MatN { get; set; }
        public string Gr { get; set; }

        public Beam(int id,Node ni,Node nj,Node nk,int secnum,int matnum,string group)
        {
            ID = id;
            Ni = ni;
            Nj = nj;
            Nk = nk;
            SecN = secnum;
            MatN = matnum;
            Gr = group;
        }


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
        public string Gr { set; get; }

        public Plate(int id,Node i,Node j,Node k, Node l,string group)
        {
            ID = id;
            Ni = i;
            Nj = j;
            Nk = k;
            Nl = l;
            Gr = group;
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
        public int Length { set; get; }
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




        public static string Stageclass(List<double> loclist, double x0)
        {
            loclist.Add(x0);
            loclist.Sort();
            int rr = loclist.IndexOf(x0);
            return string.Format("第{0}跨钢梁", rr);
        }

        public static string Stageclass2(List<double> loclist,double x0)
        {
            loclist.Add(x0);
            loclist.Sort();
            int rr = loclist.IndexOf(x0);
            return string.Format("第{0}跨混凝土板",rr);
        }













    }

}
