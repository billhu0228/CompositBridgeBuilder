using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositBridgeBuilder
{
    public class IBSection
    {
        public int ID { set; get; }
        public string Name { get; set; }
        public double H1 { set; get; }
        public double H2 { set; get; }
        public double H3 { set; get; }
        public double T1 { set; get; }
        public double T2 { set; get; }
        public double T3 { set; get; }
    }


    struct IB_Section
    {
        public int ID { set; get; }
        public string Name { get; set; }
        public double H1, H2, H3, T1, T2, T3;
        public IB_Section(int id,string name,double h1,double h2, double h3,double t1,double t2,double t3)
        {
            ID = id;
            Name = name;
            H1 = h1; H2 = h2; H3 = h3; T1 = t1; T2 = t2; T3 = t3;
        }
    }

    enum CMethod { YiCi, ZhuKua }
    enum MDVersion { Md15, Md17 }
    enum Concrete { C50,C55}
    enum Steel { Q345, Q420 }
    class ComBridge
    {
        // public
        List<double> SpanList { get;}
        double Nspan { get; }

        double Width { set; get; }
        double MBeamDist { set; get; }
        double HBeamDist { set; get; }
        double PlateThick { get; }
        double ExtendLength { get; }
        string OutputMctPath { get; }
        double CUnitWeight { get; }
        double SUnitWeight { get; }
        
        Concrete CRank { set; get; } // 混凝土标号
        Steel MBeamSRank { set; get; } // 主钢材级别
        Steel HBeamSRank { set; get; } // 副钢材级别
        double RH { get; }//相对湿度百分值：80
        
        CMethod ConsMethod { set; get; }
        MDVersion MidasVersion { set; get; }



        
        // private
        IB_Section[] SectList=new IB_Section[6];
        List<double[]> SectSplit;


        public ComBridge()
        {
            SpanList = new List<double>();

        }


        public void ReadSpanList(string splist)
        {
            string[] slist = splist.Split('+');
            foreach (string s in slist)
            {
                try
                {
                    SpanList.Add(double.Parse(s) * 1000.0);
                }
                catch
                {
                    throw;
                }
                
            }
        }


        void WriteBegin()
        {
            Dictionary<Concrete, double> ConcE = new Dictionary<Concrete, double>();
            ConcE.Add(Concrete.C50, 3.45e4);
            ConcE.Add(Concrete.C55, 3.55e4);
        
            using (FileStream fs = new FileStream(OutputMctPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("*UNIT");
                    sw.WriteLine("N, mm, KJ, C");
                    sw.WriteLine("*MATERIAL");
                    sw.WriteLine("1,CONC,{0}+, 0,0,,C,NO,0.05,2,{1},0.2,1.0000e-5,{2},0", Enum.GetName(typeof(Concrete), CRank), ConcE[CRank], CUnitWeight);
                    sw.WriteLine("2,STEEL,{0}+,0,0,,C,NO,0.02,2,2.06e5,0.3,1.2000e-5,{1:3e},0", Enum.GetName(typeof(Steel), MBeamSRank),SUnitWeight);
                    sw.WriteLine("3,STEEL,{0},0,0,,C,NO,0.02,2,2.06e5,0.3,1.2000e-5,7.698e-5,0", Enum.GetName(typeof(Steel), HBeamSRank));
                    sw.Flush();
                }
            }

        }

        

    }
}
