using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
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
using System.Text.Json;
using Newtonsoft.Json;

namespace VPet.Plugin.BetterTalk
{
    /// <summary>
    /// ClockTalk2.xaml 的交互逻辑
    /// </summary>
    public partial class ClockTalk2 : Window
    {
        bool turnpage = false;
        string[] hours = new string[25];
        string[] minutes = new string[61];
        string[] seconds = new string[61];
        string[] weeks { set; get; } = new[] { "每一天".Translate(), "Mon", "Tues", "Wednes", "Thurs", "Fri", "Satur", "Sun" };
        BetterTalk plugin;
        TextBox tb;
        TextBox tb2;
        public ClockTalk2(BetterTalk Plugin)
        {
            
            InitializeComponent();  
            FullString();
            LoadCombo();
            HoursComboP2.Text = "0";
            MinsComboP2.Text="0";
            SecondComboP2.Text="0";
            this.Closing += ClockTalk2_Closing;
            turnToPage1();
            TextboxTimer.Text = TextboxTimer.Text.Translate();
            TextboxTimerP2.Text = TextboxTimerP2.Text.Translate();
            plugin = Plugin;

        }

        private void ClockTalk2_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }


        private void FullString()
        {
            minutes[0] = "0";
            hours[0] = "0";
            seconds[0] = "0";

            for (int i = 1; i < 61; i++)
            {
                minutes[i] = (i).ToString();
            }
            for (int a = 1; a < 25; a++)
            {
                hours[a] = (a).ToString();
            }
            for (int b = 1; b < 61; b++)
            {
                seconds[b] = (b).ToString();
            }
        }

        void LoadCombo()
        {
            foreach (string s in hours) { HoursCombo.Items.Add(s); }
            foreach (string s in minutes) { MinsCombo.Items.Add(s); }
            foreach (string s in weeks) { WeekCombo.Items.Add(s); }

            foreach (string s in hours) { HoursComboP2.Items.Add(s); }
            foreach (string s in minutes) { MinsComboP2.Items.Add(s); }
            foreach (string s in seconds) { SecondComboP2.Items.Add(s); }
        }
       

        private void TimerAddButton_Click(object sender, RoutedEventArgs e)
        {

            if (HoursCombo.Text == "" || MinsCombo.Text == "" || WeekCombo.Text == "")
            {
                MessageBox.Show("请先设置闹钟！".Translate());
            }
            else
            {
                TimeState tst = new(plugin, HoursCombo.Text, MinsCombo.Text, TextboxTimer.Text, true, false,WeekCombo.Text);
                ClockOrTimerControler coc = new ClockOrTimerControler(tst);

                ClockList.Items.Add(coc);

            }
            
        }

        private void TimerButton_Click(object sender, RoutedEventArgs e)
        {
            TimerButton.Opacity = 0.6;
            ClockButton.Opacity = 1;
            turnToPage2();
        }

        private void ClockButton_Click(object sender, RoutedEventArgs e)
        {
            ClockButton.Opacity = 0.6;
            TimerButton.Opacity = 1;
            turnToPage1();
        }
        private void turnToPage1()
        {
            Dispatcher.Invoke(() =>
            {
                Page2.Visibility = Visibility.Collapsed;
                NtextDaysP2.Visibility = Visibility.Collapsed;
                HoursComboP2.Visibility = Visibility.Collapsed;
                MinsComboP2.Visibility = Visibility.Collapsed;
                SecondComboP2.Visibility = Visibility.Collapsed;
                TimerAddButtonP2.Visibility = Visibility.Collapsed;
                ClockListP2.Visibility = Visibility.Collapsed;
                TextboxTimerP2.Visibility = Visibility.Collapsed;
                NtextP2.Visibility = Visibility.Collapsed;

                Page1.Visibility = Visibility.Visible;
                NtextDays.Visibility = Visibility.Visible;
                HoursCombo.Visibility = Visibility.Visible;
                MinsCombo.Visibility = Visibility.Visible;
                WeekCombo.Visibility = Visibility.Visible;
                TimerAddButton.Visibility = Visibility.Visible;
                ClockList.Visibility = Visibility.Visible;
                TextboxTimer.Visibility = Visibility.Visible;
                Ntext.Visibility = Visibility.Visible;
            });

        }
        private void turnToPage2()
        {
            Dispatcher.Invoke(() =>
            {
            Page2.Visibility = Visibility.Visible;
            NtextDaysP2.Visibility = Visibility.Visible;
            HoursComboP2.Visibility = Visibility.Visible;
            MinsComboP2.Visibility = Visibility.Visible;
            SecondComboP2.Visibility = Visibility.Visible;
            TimerAddButtonP2.Visibility = Visibility.Visible;
            ClockListP2.Visibility = Visibility.Visible;
            TextboxTimerP2.Visibility = Visibility.Visible;
            NtextP2.Visibility = Visibility.Visible;

            Page1.Visibility = Visibility.Collapsed;
            NtextDays.Visibility = Visibility.Collapsed;
            HoursCombo.Visibility = Visibility.Collapsed;
            MinsCombo.Visibility = Visibility.Collapsed;
            WeekCombo.Visibility = Visibility.Collapsed;
            TimerAddButton.Visibility = Visibility.Collapsed;
            ClockList.Visibility = Visibility.Collapsed;
            TextboxTimer.Visibility = Visibility.Collapsed;
            Ntext.Visibility = Visibility.Collapsed;
            });
        }

        private void TimerAddButtonP2_Click(object sender, RoutedEventArgs e)
        {
            if (HoursComboP2.Text == "0"&&MinsComboP2.Text == "0" && SecondComboP2.Text == "0")
            {
                MessageBox.Show("不能添加一个0秒的倒计时噢！".Translate());
            }
            else
            {
                TimeState tst = new(plugin, HoursComboP2.Text, MinsComboP2.Text, TextboxTimerP2.Text, false, false,SecondComboP2.Text);
                ClockOrTimerControler coc = new ClockOrTimerControler(tst);

                ClockListP2.Items.Add(coc);


            }            
        }


        private void TextboxTimer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextboxTimer.Text = "";
        }
        private void TextboxTimerP2_MouseDown(object sender,MouseButtonEventArgs e)
        {
            TextboxTimerP2.Text = "";
        }

    }
}
