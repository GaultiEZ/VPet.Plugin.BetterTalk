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
using System.Timers;
using System.Text.Json;

using System.Windows.Navigation;
using System.Windows.Shapes;
using VPet_Simulator.Windows.Interface;
using System.Windows.Threading;
using System.Reflection;
using LinePutScript.Localization.WPF;
using Newtonsoft.Json;

namespace VPet.Plugin.BetterTalk
{
    /// <summary>
    /// ClockOrTimerControler.xaml 的交互逻辑
    /// </summary>
    public partial class ClockOrTimerControler : UserControl, IDisposable
    {

        TimeState tst;
        private string timeHour;
        private string timeMinute;
        private string timeSecond;
        private string timeWeek;
        private bool isclock;

        public ClockOrTimerControler(TimeState tst)
        {
            isclock = tst.isclock;
            InitializeComponent();
            this.tst = tst;
            timeHour = this.tst.timeHour;
            timeMinute=this.tst.timeMinute;
            timeWeek = this.tst.timeWeek;
            timeSecond = this.tst.timeSecond;
            Opacity = 1.0;
            TimeText.Text = $"{timeHour}  :  {timeMinute}";
            DayText.Text = timeWeek;
            this.tst.UpdateUI += Tst_UpdateUI;            
            CheckIsClockOrTimer(isclock);
        }

        private void Tst_UpdateUI()
        {
            Dispatcher.Invoke(() =>
            {
                Opacity = 0.5;
                Enable = false;
                tst.CancelSign = true;
            });
        }

        private void CheckIsClockOrTimer(bool isclock)
        {
            if (isclock)
            {
                tst.RunClock();
            }
            else
            {
                tst.RunTimer();
            }
        }


        bool Enable { get; set; } = true;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Enable) { 

                Opacity = 1.0; Enable = true;tst.CancelSign = false;
                if (isclock)
                {
                    tst.RunClock();
                }
                else
                {
                    tst.RunTimer();
                }
            }
            else
            {
                Opacity = 0.5;
                Enable = false;

                tst.CancelSign = true;
            }

        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (isclock)
            {
                BetterTalk.clockTalk.ClockList.Items.Remove(this);

                Dispose(); 
            }
            else
            {
                BetterTalk.clockTalk.ClockListP2.Items.Remove(this);

                Dispose();
            }
        }    
        public void Dispose()
        {
            
            tst.CancelSign = true ;
            
        }

    }
    public class TimeState
    {
        BetterTalk Plugin;

        public string timeHour;
        public string timeMinute;
        public string timeSecond;
        public string timeWeek;
        public string message;
        public bool isclock;
        public int totaltime;
        System.Timers.Timer timer;
        public event Action UpdateUI;
        [JsonProperty]
        public bool CancelSign { set; get; }

        public TimeState(BetterTalk mp,string hour, string minute, string messages, bool IsClock,bool isEnable,string secondsOrweek = "0")
        {
            Plugin = mp;
            timeHour = hour;
            timeMinute = minute;
            timeSecond = secondsOrweek;
            timeWeek = secondsOrweek;
            message = messages;
            isclock = IsClock;
            CancelSign = isEnable;
        }

        public void RunClock() 
        {
            int nowHour = DateTime.Now.Hour;
            int nowMinute = DateTime.Now.Minute;
            if (timeWeek == "每一天".Translate())
            {
                Task.Run(async () =>
                {
                    while (!(nowHour == int.Parse(timeHour) && nowMinute == int.Parse(timeMinute)))
                    {
                        if (CancelSign) // 确保控件未被禁用且未被标记为删除
                        {
                            break;
                        }
                        nowHour = DateTime.Now.Hour;
                        nowMinute = DateTime.Now.Minute;
                        await Task.Delay(100);
                        continue;
                        // 等待一小段时间再次检查，避免密集循环

                    }
                    if (!CancelSign)
                    {
                        Plugin.CheckIsSleeping(message);
                    }
                });
            }
            else
            {
                string nowWeek = DateTime.Now.Date.DayOfWeek.ToString();
                Task.Run(async () =>
                {
                    while (!(nowHour == int.Parse(timeHour) && nowMinute == int.Parse(timeMinute) && nowWeek == timeWeek + "day"))
                    {
                        if (CancelSign) // 确保控件未被禁用且未被标记为删除
                        {
                            break;
                        }
                        nowHour = DateTime.Now.Hour;
                        nowMinute = DateTime.Now.Minute;
                        nowWeek = DateTime.Now.Date.DayOfWeek.ToString();
                        await Task.Delay(100);
                        // 等待一小段时间再次检查，避免密集循环


                    }
                    if (!CancelSign)
                    {

                        Plugin.CheckIsSleeping(message);
                    }
                });
            }
        }

        public void RunTimer() 
        {
            if (!CancelSign)
            {
                totaltime = (int.Parse(timeHour) * 3600 + int.Parse(timeMinute) * 60 + int.Parse(timeSecond)) * 1000;
                timer = new System.Timers.Timer(totaltime);
                timer.Elapsed += Timer_Elapsed;
                timer.Start();

            }
        }
        public event Action ClockOn;
        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (!CancelSign)
            {
                Plugin.CheckIsSleeping(message);
                CancelSign = true;
                timer.Stop();
                timer.Dispose();
                UpdateUI.Invoke();
                ClockOn?.Invoke();
            }
        }
    }
}