using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    class SplitTuple
    {
        public int SectID { set; get; }
        public double Length { set; get; }
    }




    enum CMethod { YiCi, ZhuKua }
    enum MDVersion { Md15, Md17 }
    enum Concrete { C50,C55}
    enum Steel { Q345, Q420 }


    class ComBridge
    {
        public double Width;
        public double MBeamDist;
        // public
        List<double> SpanList { get;}
        public double Nspan { get; }
        public double Length
        {
            get
            {
                return SpanList.Sum();
            }
        }
        
        public double HBeamDist { set; get; }
        public double PlateThick { get; }
        public double ExtendLength { get; }
        public string OutputMctPath { get; }
        public double CUnitWeight { get; }
        public double SUnitWeight { get; }

        public Concrete CRank; // 混凝土标号
        public Steel MBeamSRank; // 主钢材级别
        public Steel HBeamSRank;// 副钢材级别
        public double RH { get; }//相对湿度百分值

        public CMethod ConsMethod { set; get; }
        public MDVersion MidasVersion { set; get; }




        // private
        public ObservableCollection<IBSection> SectList;
        public ObservableCollection<SplitTuple> SectSplit;


        public ComBridge()
        {
            SpanList = new List<double>();

        }

        /// <summary>
        /// 解析桥跨序列
        /// </summary>
        /// <param name="splist"></param>
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
                    SpanList.Clear();
                    throw new ArgumentException("桥跨序列");
                }
                
            }
            
        }

        /// <summary>
        /// 解析单一浮点数
        /// </summary>
        /// <param name="dataDouble"></param>
        /// <param name="dataText"></param>
        public static void String2Double(ref double dataDouble, string dataText)
        {
            try
            {
                dataDouble = double.Parse(dataText);
            }
            catch
            {
                dataDouble = 0;
                throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
            }
        }
        /// <summary>
        /// 解析枚举
        /// </summary>
        /// <param name="enum"></param>
        /// <param name="dataText"></param>
        public static void String2Enum(ref Concrete @enum,string dataText)
        {
            try
            {
                if (dataText.StartsWith("C"))
                {
                    @enum = (Concrete)Enum.Parse(typeof(Concrete), dataText);
                }
                else
                {
                    throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
                }
            }
            catch
            {
                throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
            }
        }
        public static void String2Enum(ref Steel @enum, string dataText)
        {
            try
            {
               if (dataText.StartsWith("Q"))
                {
                    @enum = (Steel)Enum.Parse(typeof(Steel), dataText);
                }
                else
                {
                    throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
                }
            }
            catch
            {
                throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
            }
        }





        /// <summary>
        /// 输出MCT开头
        /// </summary>
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
