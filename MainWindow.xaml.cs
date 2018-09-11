using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
        
        
        public MainWindow()
        {
            InitializeComponent();

            WidhtTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            SplistTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            MBeamDistTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            MBeamFactorTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            HuLanTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            CPlateTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            LiQingTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);            

            InitSectData();
        }



        void InitSectData()
        {
            IBSection ps1;
            ps1 = new IBSection { ID = 1, Name = "标准主梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 2, Name = "边跨主梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 3, Name = "边跨主梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 4, Name = "边支座主梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 5, Name = "小纵梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 6, Name = "普通横梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 7, Name = "跨中横梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 8, Name = "中支座横梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 9, Name = "边支座横梁" };
            SectionList.Add(ps1);
            ps1 = new IBSection { ID = 10, Name = "中纵梁" };
            SectionList.Add(ps1);
            SectionDataGird.ItemsSource = SectionList;

            SplitTuple st1;
            st1 = new SplitTuple { SectID = 1, Length = curBridge.Length/1000 };
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
                ComBridge.String2Double(ref curBridge.Width, WidhtTB.Text);
                ComBridge.String2Double(ref curBridge.MBeamDist, MBeamDistTB.Text);
                ComBridge.String2Enum(ref curBridge.CRank, ConcClass.Text);
                ComBridge.String2Enum(ref curBridge.MBeamSRank, MSteelClass.Text);
                ComBridge.String2Enum(ref curBridge.HBeamSRank, HSteelClass.Text);                
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
            }
            else
            {
                curBridge.SectSplit = SplitList;
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
        // Open






    }







}
