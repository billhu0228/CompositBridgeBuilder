using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CompositBridgeBuilder
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        // 
        ComBridge curBridge = new ComBridge();
        ObservableCollection<IBSection> SectionList = new ObservableCollection<IBSection>();
        ObservableCollection<SplitTuple> SplitList = new ObservableCollection<SplitTuple>();

        Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog()
        {
            InitialDirectory = Environment.CurrentDirectory,
            RestoreDirectory = true,
            Filter = "组合梁模型文件(*.cbh)|*.cbh|所有文件(*.*)|*.*",
        };
        Microsoft.Win32.SaveFileDialog sf = new Microsoft.Win32.SaveFileDialog()
        {
            InitialDirectory= Environment.CurrentDirectory,
            RestoreDirectory = true,
            Filter = "组合梁模型文件(*.cbh)|*.cbh",
            DefaultExt="cbh",
        };
       
        

        
        public MainWindow()
        {
            InitializeComponent();

            WidhtTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            SplistTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            MBeamDistTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            HBeamDistTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            MBeamFactorTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            HuLanTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            CPlateTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            LiQingTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            LiveLoadTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            TempTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            DeltTemp.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            WindTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            ThickTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);


            InitSectData();
        }



        void InitSectData()
        {
            IBSection ps1;
            ps1 = new IBSection { ID = 1, Name = "标准主梁", H1 = 700, H2 = 600, H3 = 1800, T1 = 32, T2 = 24, T3 = 18 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 2, Name = "边跨主梁", H1 = 700, H2 = 600, H3 = 1800, T1 = 50, T2 = 32, T3 = 18 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 3, Name = "边跨主梁", H1 = 700, H2 = 600, H3 = 1800, T1 = 50, T2 = 32, T3 = 18 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 4, Name = "边支座主梁", H1 = 700, H2 = 600, H3 = 1800, T1 = 62, T2 = 36, T3 = 18 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 5, Name = "小纵梁", H1 = 300, H2 = 300, H3 = 300, T1 = 15, T2 = 15, T3 = 10 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 6, Name = "普通横梁", H1 = 300, H2 = 300, H3 = 700, T1 = 24, T2 = 24, T3 = 13 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 7, Name = "跨中横梁", H1 = 400, H2 = 400, H3 = 1100, T1 = 24, T2 = 24, T3 = 14 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 8, Name = "中支座横梁", H1 = 400, H2 = 400, H3 = 1100, T1 = 24, T2 = 24, T3 = 16 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 9, Name = "边支座横梁", H1 = 400, H2 = 1060, H3 = 1500, T1 = 24, T2 = 28, T3 = 16 };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 10, Name = "中纵梁", H1 = 300, H2 = 500, H3 = 300, T1 = 16, T2 = 20, T3 =10 };
            SectionList.Add(ps1);
            SectionDataGird.ItemsSource = SectionList;

            SplitTuple st1;
            st1 = new SplitTuple { SectID = 1, Length = 8 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 2, Length = 20 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 1, Length = 8 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 4, Length = 8 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 1, Length = 33 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 3, Length = 6 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 1, Length = 33 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 4, Length = 8 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 1, Length = 8 };
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 2, Length = 20};
            SplitList.Add(st1);

            st1 = new SplitTuple { SectID = 1, Length = 8 };
            SplitList.Add(st1);

            SplitDataGird.ItemsSource = SplitList;

        }
 
        

        private new void MouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            TextBox curTB = sender as TextBox;
            if (curTB.Text == curTB.Tag.ToString())
            {
                curTB.Text = "";
            }            
        }


        private new void LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox curTB = sender as TextBox;
            if (curTB.Text == "")
            {
                curTB.Text = curTB.Tag.ToString();
            }            
        }


        // 整体布置
        private void Tab1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                curBridge.ReadSpanList(SplistTB.Text);
                RemoveExample(ref SplistTB);
                ComBridge.String2Double(ref curBridge.Width, WidhtTB.Text,1000.0);
                RemoveExample(ref WidhtTB);
                ComBridge.String2Double(ref curBridge.MBeamDist, MBeamDistTB.Text,1000.0);
                RemoveExample(ref MBeamDistTB);
                ComBridge.String2Double(ref curBridge.HBeamDist, HBeamDistTB.Text,1000.0);
                RemoveExample(ref HBeamDistTB);

                ComBridge.String2Enum(ref curBridge.CRank, ConcClass.Text);
                ComBridge.String2Enum(ref curBridge.MBeamSRank, MSteelClass.Text);
                ComBridge.String2Enum(ref curBridge.HBeamSRank, HSteelClass.Text);
                this.ShowMessageAsync("整体布置 输入成功.", "");
            }
            catch(Exception err)
            {
                MessageBox.Show(string.Format("{0} : 输入参数错误，请参考示例.", err.Message), "ERROR");
            }            
        }

        // 断面布置
        private void Tab2_Click(object sender, RoutedEventArgs e)
        {
            curBridge.SectList = SectionList;
            
            if (SplitList.Sum(t => t.Length) != curBridge.Length/1000)
            {
                MessageBox.Show(string.Format("截面布置与总长不符 : 请检查."), "ERROR");
                return;
            }
            else
            {
                curBridge.SectSplit = SplitList;
            }

            try
            {
                ComBridge.String2Double(ref curBridge.PlateThick, ThickTB.Text,1000.0);
                this.ShowMessageAsync("断面布置 输入成功.","");
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0} : 输入参数错误，请参考示例.", err.Message), "ERROR");
            }

        }

        private void SplitDataGird_InitializingNewItem(object sender,InitializingNewItemEventArgs e)
        {
            ((SplitTuple)e.NewItem).SectID = 1;
            ((SplitTuple)e.NewItem).Length = curBridge.Length/1000.0 - SplitList.Sum(t => t.Length);            
        }
   






        // 荷载
        private void Tab3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComBridge.String2Double(ref curBridge.MainBeamFactor, MBeamFactorTB.Text);
                ComBridge.String2Double(ref curBridge.LiveLoadFactor, LiveLoadTB.Text);
                ComBridge.String2Double(ref curBridge.PlateUnitWeight, CPlateTB.Text,1.0/9.8);
                ComBridge.String2Double(ref curBridge.AsphaltThick, LiQingTB.Text);
                ComBridge.String2Double(ref curBridge.CurbLengthWeight, HuLanTB.Text);
                ComBridge.String2Double(ref curBridge.WindPressure, WindTB.Text, 1 / 1000.0);
                ComBridge.String2Temp(ref curBridge.HeighTemp, ref curBridge.LowTemp, TempTB.Text);
                ComBridge.String2Temp(ref curBridge.DeltHeighTemp, ref curBridge.DeltLowTemp, DeltTemp.Text);

                RemoveExample(ref MBeamFactorTB);
                RemoveExample(ref CPlateTB);                
                RemoveExample(ref LiveLoadTB);
                RemoveExample(ref LiveLoadTB);
                RemoveExample(ref LiQingTB);
                RemoveExample(ref HuLanTB);
                RemoveExample(ref WindTB);




                this.ShowMessageAsync("荷载输入成功.", "");
                
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0} : 输入参数错误，请参考示例.", err.Message), "ERROR");
            }

        }
        // 施工阶段
        private void Tab4_Click(object sender, RoutedEventArgs e)
        {

        }
        // Midas 输出
        private void Tab5_Click(object sender, RoutedEventArgs e)
        {

        }











        // SaveAs
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {

            if (sf.ShowDialog() == true)
            {
                curBridge.SaveAs(sf.FileName);
                this.ShowMessageAsync("保存成功!","");
            }

        }
        // Open
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            List<string> cont;
            string[] tmp;
            if (op.ShowDialog() == true)
            {                
                using (FileStream fs = new FileStream(op.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        sr.BaseStream.Seek(0, SeekOrigin.Begin);
                        tmp = sr.ReadToEnd().Split( '\n' );
                    }
                }
                cont= tmp.Select(t=>t.TrimEnd('\r')).ToList();
                for (int ii = 0; ii < cont.Count; ii++)
                {
                    string line = cont[ii];
                    if (line.StartsWith("#"))
                    {
                        switch (line)
                        {
                            case "#Layout":
                                {
                                    SplistTB.Text = cont[ii + 1];
                                    WidhtTB.Text = cont[ii + 2];
                                    MBeamDistTB.Text = cont[ii + 3];
                                    HBeamDistTB.Text = cont[ii + 4];
                                    MSteelClass.Text = cont[ii + 5];
                                    HSteelClass.Text = cont[ii + 6];
                                    ConcClass.Text = cont[ii + 7];
                                    break;
                                }
                            case "#CrossSection":
                                {
                                    break;
                                }
                            case "#Splitlist":
                                {
                                    break;
                                }
                            case "#Thickness":
                                {
                                    break;
                                }
                            case "#Loads":
                                {
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }

            }
        }


        private void RemoveExample(ref TextBox tb)
        {
            if (tb.Text.StartsWith("示例："))
            {
                tb.Text = string.Concat(tb.Text.Skip(3));
            }
        }




    }







}
