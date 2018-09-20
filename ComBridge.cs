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
        public int NMBeam;
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
        readonly List<Node> NodeList;
        readonly List<Beam> BeamList;
        readonly List<Plate> PlateList;
        int Num_Node;
        int Num_Elem;

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
        public ObservableCollection<SplitTuple> SectSplitList;

        











        public ComBridge()
        {
            SpanList = new List<double>();
            SectList = new ObservableCollection<IBSection>();
            SectSplitList = new ObservableCollection<SplitTuple>();
            Num_Elem = 1;
            Num_Node = 1;
            NodeList = new List<Node>();
            BeamList = new List<Beam>();
            PlateList = new List<Plate>();

            OutputMctPath = "test.mct";
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
        /// 解析整数
        /// </summary>
        /// <param name="dataInt"></param>
        /// <param name="dataText"></param>
        public static void String2Int(ref int dataInt,string dataText)
        {
            if (dataText.StartsWith("示例："))
            {
                dataText = string.Concat(dataText.Skip(3));
            }


            try
            {
                dataInt = int.Parse(dataText) ;
            }
            catch
            {
                dataInt = 0;
                throw new ArgumentException(string.Format("\"{0}\" 解析错误", dataText));
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





        /// <summary>
        /// 配置模型
        /// </summary>
        public void GenerateModel()
        {
            List<double> baselist = new List<double>() { 0.0};
            List<double> midlist = new List<double>();
            foreach (double i in SpanList)
            {
                baselist.Add(baselist.Last() + i);
            }
            for (int i = 0; i < baselist.Count-1; i++)
            {
                midlist.Add((baselist[i] + baselist[i + 1]) * 0.5);
            }
            List<double> xlist = Ext.Arange(0, baselist.Last(), 1000);
            List<double> ylist = new List<double>() { 0.0 };
            List<double> mlist = new List<double>();
            List<double> slist = new List<double>();
            //List<double> hllist = new List<double>();
            List<double> stageloc = new List<double>() { 0.0 };

            ylist.Add(500.0);
            ylist.Add(Width-500);
            ylist.Add(Width);
            double tmpside = 0.5 * (Width - (NMBeam - 1) * MBeamDist);            
            for(int i = 0; i < NMBeam; i++)
            {
                ylist.Add(tmpside + i * MBeamDist);
                mlist.Add(tmpside + i * MBeamDist);
            }

            var bb = from va in mlist select va - 0.5 * Width;
            mlist = bb.ToList();

            foreach (double yy in mlist)
            {
                if (yy != mlist.Last())
                {
                    slist.Add(yy + 0.5 * MBeamDist);
                }
            }
            var aa = from va in ylist select va - 0.5 * Width;
            ylist = aa.ToList();
            foreach (double yy in slist)
            {
                ylist.Add(yy);
            }
            ylist.Distinct();
            ylist.Sort();



            //hllist = ylist.ToList();
            //hllist.Remove(hllist.Last());
            //hllist.Remove(hllist.First());            
            foreach(double tt in SpanList)
            {
                stageloc.Add(stageloc.Last() + tt);
            }


            // 桥面板
            foreach(double xx0 in xlist)
            {
                foreach(double yy0 in ylist)
                {
                    Node item = new Node(Num_Node,xx0,yy0,0.0);
                    NodeList.Add(item);
                    Num_Node++;
                }
            }
            for (int ii = 0; ii < xlist.Count-1; ii++)
            {
                for (int jj = 0; jj < ylist.Count-1; jj++)
                {
                    var n1 = from node in NodeList where (node.X == xlist[ii]) && (node.Y== ylist[jj]) select node;
                    var n2 = from node in NodeList where (node.X == xlist[ii+1]) && (node.Y == ylist[jj]) select node;
                    var n3 = from node in NodeList where (node.X == xlist[ii+1]) && (node.Y == ylist[jj+1]) select node;
                    var n4 = from node in NodeList where (node.X == xlist[ii]) && (node.Y == ylist[jj+1]) select node;
                    Plate item = new Plate(Num_Elem, n1.First(), n2.First(), n3.First(), n4.First(),Ext.Stageclass2(stageloc,xlist[ii]+500.0));
                    PlateList.Add(item);
                    Num_Elem++;
                }
            }

            // 钢梁
            double x0 = 0;
            Node nk = new Node(0, 0, 0, 100000000);
            for (int ii = 0; ii < SectSplitList.Count; ii++)
            {
                for(int kk = 0; kk < SectSplitList[ii].Length; kk++)
                {
                    for (int jj = 0; jj < NMBeam; jj++)
                    {
                        var n1 = from node in NodeList where ((node.X == x0) && (node.Y == mlist[jj]) && (node.Z == 0.0)) select node;
                        var n2 = from node in NodeList where ((node.X == x0+1000.0) && (node.Y == mlist[jj]) && (node.Z == 0.0)) select node;
                        Beam item = new Beam(Num_Elem, n1.First(), n2.First(),nk , SectSplitList[ii].SectID,2, Ext.Stageclass(stageloc,x0));
                        BeamList.Add(item);
                        Num_Elem++;
                    }
                    x0 += 1000;
                }
            }
            foreach(double xx0 in xlist)
            {
                if (xx0 == xlist.Last())
                {
                    break;
                }
                foreach(double yy0 in slist)
                {
                    var n1 = from node in NodeList where ((node.X == xx0) && (node.Y == yy0) && (node.Z == 0.0)) select node;
                    var n2 = from node in NodeList where ((node.X == xx0+1000) && (node.Y == yy0) && (node.Z == 0.0)) select node;
                    int mid = (int)(slist.Count * 0.5);
                    Beam item;
                    if (yy0 == slist[mid]&& slist.Count%2!=0)
                    {
                        item = new Beam(Num_Elem, n1.First(), n2.First(), nk, 10,3, Ext.Stageclass(stageloc, xx0));

                    }
                    else
                    {
                        item = new Beam(Num_Elem, n1.First(), n2.First(), nk, 5, 3, Ext.Stageclass(stageloc, xx0));
                    }
                    BeamList.Add(item);
                    Num_Elem++;
                }
            }
            foreach (double xx0 in xlist)
            {
                for(int jj = 0; jj < mlist.Count - 1; jj++)
                {                    
                    var n1 = from node in NodeList where ((node.X == xx0) && (node.Y == mlist[jj]) && (node.Z == 0.0)) select node;
                    var n2 = from node in NodeList where ((node.X == xx0) && (node.Y == mlist[jj+1]) && (node.Z == 0.0)) select node;
                    Beam item;
                    int secn=0;
                    if (xx0 == xlist.First() || xx0 == xlist.Last())
                    {
                        secn = 9;
                    }
                    else if (baselist.Contains(xx0)) 
                    {
                        secn = 8;
                    }
                    else if (midlist.Contains(xx0)&& xx0%HBeamDist==0.0)
                    {
                        secn = 7;
                    }
                    else if (midlist.Contains(xx0+ HBeamDist*0.5) && xx0 % HBeamDist == 0.0)
                    {
                        secn = 7;
                    }
                    else if (midlist.Contains(xx0 - HBeamDist*0.5) && xx0 % HBeamDist == 0.0)
                    {
                        secn = 7;
                    }
                    else if (xx0 % HBeamDist == 0.0)
                    {
                        secn = 6;
                    }
                    else
                    {
                        continue;
                    }
                    item = new Beam(Num_Elem, n1.First(), n2.First(), nk, secn, 3, Ext.Stageclass(stageloc, xx0));
                    BeamList.Add(item);
                    Num_Elem++;
                }
            }

            ;
        }





        /// <summary>
        /// 输出MCT开头
        /// </summary>
        public void WriteBegin()
        {
            Dictionary<Concrete, double> ConcE = new Dictionary<Concrete, double>();
            ConcE.Add(Concrete.C50, 3.45e4);
            ConcE.Add(Concrete.C55, 3.55e4);
        
            using (FileStream fs = new FileStream(OutputMctPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("*UNIT");
                    sw.WriteLine("N, mm, KJ, C");
                    sw.WriteLine("*MATERIAL");
                    sw.WriteLine("1,CONC,{0}+, 0,0,,C,NO,0.05,2,{1},0.2,1.0000e-5,{2},0", Enum.GetName(typeof(Concrete), CRank), ConcE[CRank], CUnitWeight);
                    sw.WriteLine("2,STEEL,{0}+,0,0,,C,NO,0.02,2,2.06e5,0.3,1.2000e-5,{1:E3},0", Enum.GetName(typeof(Steel), MBeamSRank),SUnitWeight);
                    sw.WriteLine("3,STEEL,{0},0,0,,C,NO,0.02,2,2.06e5,0.3,1.2000e-5,7.698e-5,0", Enum.GetName(typeof(Steel), HBeamSRank));
                    sw.Flush();
                }
            }

        }
        /// <summary>
        /// 输出组合
        /// </summary>
        public void WriteGroup()
        {

        }
        /// <summary>
        /// 输出截面
        /// </summary>
        public void WriteSection()
        {
            using (FileStream fs = new FileStream(OutputMctPath, FileMode.Open, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs,Encoding.Default))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("*SECTION");
                    foreach(IBSection ibs in SectList)
                    {
                        sw.WriteLine(string.Format("{0},DBUSER,{1},CT,0,0,0,0,0,{2},YES,H,2,{3:F3}," +
                            "{4:F3},{5:F3},{6:F3},{7:F3},{8:F3},0,0,0,0", ibs.ID,ibs.Name,0,ibs.H3,ibs.H2,ibs.T3,ibs.T2,ibs.H1,ibs.T1));
                    }
                    sw.Flush();
                }
            }
        }

        public void WriteNodeElement()
        {
            using (FileStream fs = new FileStream(OutputMctPath, FileMode.Open, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("*NODE");
                    foreach (Node nd in NodeList)
                    {
                        sw.WriteLine(string.Format("{0},{1:F3},{2:F3},{3:F3}",nd.ID,nd.X,nd.Y,nd.Z));
                    }
                    sw.Flush();

                    sw.WriteLine("*ELEMENT");
                    foreach(Plate pt in PlateList)
                    {
                        sw.WriteLine(string.Format("{0},PLATE,1,1,{1},{2},{3},{4},1,0", pt.ID, pt.Ni.ID, pt.Nj.ID, pt.Nk.ID, pt.Nl.ID));
                    }
                    foreach (Beam bm in BeamList)
                    {
                        sw.WriteLine(string.Format("{0},BEAM,{1},{2},{3},{4},0,0",bm.ID,bm.MatN,bm.SecN,bm.Ni.ID,bm.Nj.ID));
                    }

                }
            }

        }

        public void WriteConstraint()
        {

        }

        public void WriteLoad()
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
                    if (SectSplitList.Count != 0)
                    {
                        foreach (SplitTuple st in SectSplitList)
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










}
