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
        ComBridge curBridge = new ComBridge();
        ObservableCollection<IBSection> SectionList = new ObservableCollection<IBSection>();

        
        
        public MainWindow()
        {
         
            

            InitializeComponent();
            WidhtTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            SplistTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);
            MBeamDistTB.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseLeftButtonDown), true);

            //InitDataBinding();

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


    }

        

    



}
