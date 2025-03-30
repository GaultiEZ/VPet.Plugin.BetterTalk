using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using System.Windows.Navigation;
using System.Windows.Shapes;
using VPet_Simulator.Windows.Interface;
using System.Windows.Threading;

namespace VPet.Plugin.BetterTalk
{
    /// <summary>
    /// ClockOrTimerControler.xaml 的交互逻辑
    /// </summary>
    public partial class ClockOrTimerControler : UserControl, IDisposable
    {
        MainPlugin MainPlugin;
        private string timeHour;
        private string timeMinute;
        private string timeWeek;
        private string message;

        private bool IsDeleteOrDisable;

        public ClockOrTimerControler(MainPlugin mp, string hour, string minute, string week, string messages)
        {
            MainPlugin = mp;
            InitializeComponent();
            ClockHandle += ClockOrTimerControler_ClockHandle;
            timeHour = hour;
            timeMinute = minute;
            timeWeek = week;
            this.message = messages;
            Opacity = 1.0;
            TimeText.Text = $"{timeHour}  :  {timeMinute}";
            DayText.Text = timeWeek;
            RunClock();
        }

        private void ClockOrTimerControler_ClockHandle()
        {
            MainPlugin.MW.Main.SayRnd(message);
        }

        bool Enable { get; set; } = true;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Enable) { Opacity = 1.0; Enable = true;IsDeleteOrDisable = false; }
            else
            {
                Opacity = 0.5;
                Enable = false;
          
                IsDeleteOrDisable = true;
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            BetterTalk.clockTalk.ClockList.Items.Remove(this);
            Dispose(); // 手动调用 Dispose 方法
        }

        public event Action ClockHandle;
        void RunClock()
        {
            int nowHour = DateTime.Now.Hour;
            int nowMinute = DateTime.Now.Minute;
            string nowWeek = DateTime.Now.Date.DayOfWeek.ToString();
            

            
                Task.Run(async() =>
                {
                    while (!(nowHour == int.Parse(timeHour) && nowMinute == int.Parse(timeMinute) && nowWeek == timeWeek + "day"))
                    {
                        if (IsDeleteOrDisable) // 确保控件未被禁用且未被标记为删除
                        {
                            break;
                        }
                        nowHour = DateTime.Now.Hour;
                        nowMinute = DateTime.Now.Minute;
                        nowWeek = DateTime.Now.Date.DayOfWeek.ToString();
                        await Task.Delay(100);
                        // 等待一小段时间再次检查，避免密集循环
                        
                    }
                    ClockHandle?.Invoke();
                });
                
            
            
        }


        public void Dispose()
        {
            
            IsDeleteOrDisable = true;
            
        }
    }
}