using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompositBridgeBuilder
{
    class ComBridge
    {
        // Fields
        public double Width;
        public double MBeamDist;
        public double HBeamDist;
        public double PlateThick;

        public double MainBeamFactor;
        public double LiveLoadFactor;
        public double PlateUnitWeight;
        public double AsphaltThick;
        public double CurbLengthWeight;
        public double HeighTemp;
        public double LowTemp;
        public double DeltHeighTemp;
        public double DeltLowTemp;
        public double WindPressure;


        // Propertys
        List<double> SpanList { get;}
        public double Nspan
        {
            get
            {
                return SpanList.Count;
            }
        }
        public double Length
        {
            get
            {
                return SpanList.Sum();
            }
        }
        
        
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
            SectList = new ObservableCollection<IBSection>();
            SectSplit = new ObservableCollection<SplitTuple>();
        }

        /// <summary>
        /// 解析桥跨序列
        /// </summary>
        /// <param name="splist"></param>
        public void ReadSpanList(string splist)
        {
            if (splist.StartsWith("示例："))
            {
                splist = string.Concat(splist.Skip(3));
            }
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
        public static void String2Double(ref double dataDouble, string dataText,double transFactor=1.0)
        {
            if (dataText.StartsWith("示例："))
            {                
                dataText = string.Concat(dataText.Skip(3));
            }
                
            
            try
            {
                dataDouble = double.Parse(dataText)*transFactor;
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



        public static void String2Temp(ref double heigh,ref double low,string dataText)
        {
            if (dataText.StartsWith("示例："))
            {
                dataText = string.Concat(dataText.Skip(3));
            }
            try
            {
                string[] ss = dataText.Split('/');
                low = double.Parse(ss[0]);
                heigh = double.Parse(ss[1]);
            }
            catch
            {
                throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
            }
            if (low >= heigh)
            {
                throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
            }
        }

        void GenerateModel()
        {

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
        /// <summary>
        /// 输出组合
        /// </summary>
        void WriteGroup()
        {

        }
        /// <summary>
        /// 输出截面
        /// </summary>
        void WriteSection()
        {
            using (FileStream fs = new FileStream(OutputMctPath, FileMode.Open, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("*SECTION");
                    foreach(IBSection ibs in SectList)
                    {
                        sw.WriteLine(string.Format("{0},DBUSER,{1},CT,1,0,0,0,1,{2},YES,H,2,{3:3F}," +
                            "{4:3F},{5:3F},{6:3F},{7:3F},{8:3F},0,0,0,0\n",ibs.ID,ibs.Name,0,ibs.H1,ibs.H2,ibs.H3,ibs.T1,ibs.T2,ibs.T3));
                    }
                    sw.Flush();
                }
            }
        }

        void WriteNodeElement()
        {

        }

        void WriteConstraint()
        {

        }

        void WriteLoad()
        {

        }





        /// <summary>
        /// 保存输出CBH文件
        /// </summary>
        /// <param name="filename"></param>
        public void SaveAs(string filename)
        {
            //string cwd = Environment.CurrentDirectory;

            if (!filename.EndsWith(".cbh"))
            {
                filename += ".cbh";
            }
            using (FileStream fs = new FileStream(Path.Combine(filename), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("#Layout");
                    sw.WriteLine(string.Join("+",SpanList.Select(t=>t/1000.0)));
                    sw.WriteLine(Width/1000.0);
                    sw.WriteLine(MBeamDist/1000.0);
                    sw.WriteLine(HBeamDist/1000.0);
                    sw.WriteLine(MBeamSRank);
                    sw.WriteLine(HBeamSRank);
                    sw.WriteLine(CRank);
                    sw.WriteLine("#CrossSection");
                    if (SectList.Count != 0)
                    {
                        foreach (IBSection s in SectList)
                        {
                            sw.WriteLine(s.GetString());
                        }
                    }
                    sw.WriteLine("#Splitlist");
                    if (SectSplit.Count != 0)
                    {
                        foreach (SplitTuple st in SectSplit)
                        {
                            sw.WriteLine(string.Concat(st.SectID, ",", st.Length));
                        }
                    }
                    sw.WriteLine("#Thickness");
                    sw.WriteLine(PlateThick);
                    sw.WriteLine(AsphaltThick);
                    sw.WriteLine("#Loads");
                    sw.WriteLine(MainBeamFactor);
                    sw.WriteLine(LiveLoadFactor);
                    sw.WriteLine(PlateUnitWeight);
                    sw.WriteLine(CurbLengthWeight);
                    sw.WriteLine(string.Concat(HeighTemp,",",LowTemp));
                    sw.WriteLine(string.Concat(DeltHeighTemp, ",", DeltLowTemp));
                    sw.WriteLine(WindPressure);
                    sw.WriteLine("#Construction");
                    sw.WriteLine("#MidasOutput");
                    sw.WriteLine("#AnsysOutput");
                    sw.Flush();
                }
            }
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

}
