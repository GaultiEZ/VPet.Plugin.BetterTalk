using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using VPet_Simulator.Windows.Interface;

namespace VPet.Plugin.BetterTalk
{
    /// <summary>
    /// ClockTalk2.xaml 的交互逻辑
    /// </summary>
    public partial class ClockTalk2 : Window
    {
        string[] hours = new string[24];
        string[] minutes = new string[61];
        string[] weeks = new[] { "每一天".Translate(),"Mon", "Tues", "Wednes", "Thurs", "Fri", "Satur", "Sun" };
        MainPlugin plugin;
        public ClockTalk2(MainPlugin Plugin)
        {
            plugin = Plugin;
            InitializeComponent();  
            FullString();
            LoadCombo();
            this.Closing += ClockTalk2_Closing;
        }

        private void ClockTalk2_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void FullString()
        {
            for (int i = 0; i < 60; i++)
            {
                minutes[i] = (i+1).ToString();
            }
            for (int a = 0; a < 24; a++)
            {
                hours[a] = (a+1).ToString();
            }
        }

        void LoadCombo()
        {
            foreach (string s in hours) { HoursCombo.Items.Add(s); }
            foreach (string s in minutes) { MinsCombo.Items.Add(s); }
            foreach (string s in weeks) { WeekCombo.Items.Add(s); }
        }

        private void TimerAddButton_Click(object sender, RoutedEventArgs e)
        {
            ClockOrTimerControler coc = new ClockOrTimerControler(plugin,HoursCombo.Text,MinsCombo.Text,WeekCombo.Text,TextboxTimer.Text);
           
            ClockList.Items.Add(coc);
            
        }

        
    }
}
