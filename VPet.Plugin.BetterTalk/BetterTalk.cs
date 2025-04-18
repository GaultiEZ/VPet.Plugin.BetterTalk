using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using LinePutScript.Localization.WPF;
using Newtonsoft.Json;
using VPet_Simulator.Core;
using VPet_Simulator.Windows;
using VPet_Simulator.Windows.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VPet.Plugin.BetterTalk
{
    public class BetterTalk : MainPlugin
    {
        private List<char>? outputtext;

        private List<string>? touchHeadtext;

        private List<string>? touchBodytext;

        private List<string>? timertext;

        private List<string>? playtext;

        private List<string>? playjumptext;

        private List<string>? livetext;

        private List<string>? worktext;

        private List<string>? sleeptext;

        public MessageBar msgbar;

        public MainWindow mww;

        public DateTime RelsTime;

        private TalkSelect ts;

        private List<SelectText> textList;

        private HashSet<string> textSaid;

        private ComboBox tbTalk;

        private Button btn_Send;

        private ProgressBar PrograssUsed;

        private readonly char[] chars = new char[15]
        {
        ',', '，', '.', '。', '!', '！', '?', '？', '~', '~',
        ':', '：', '；', ';', '-'
        };

        private MenuItem menuItemRT;

        private MenuItem TrunTimer;

        private MenuItem OpenTimer;

        private MenuItem OffTimer;
        private MenuItem ClockTalkRT;

        private IntervalSet set;

        private CheckWindow CW;

        public UIElement UIE;

        public static ClockTalk2 clockTalk;
        //public static List<ClockOrTimerControler> cocList { set; get; } = new List<ClockOrTimerControler>();

        public System.Timers.Timer SayTimer;

        public override string PluginName => "BetterTalk";

        string[] setting = new string[8];

        public BetterTalk(IMainWindow mainwin) : base(mainwin)
        {
            touchHeadtext = new List<string>();
            touchBodytext = new List<string>();
            timertext = new List<string>();
            playjumptext = new List<string>();
            playtext = new List<string>();
            worktext = new List<string>();
            livetext = new List<string>();
            sleeptext = new List<string>();
            Application.Current.Dispatcher.Invoke(() => { clockTalk = new ClockTalk2(this); });


        }

        private void GetOutputtext()
        {
            Type msgType = typeof(MessageBar);
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo listProperty = msgType.GetField("outputtext", bindingFlags);
            if (listProperty != null)
            {
                outputtext = (List<char>)listProperty.GetValue(msgbar);
            }
            else
            {
                ShowError("出现错误 错误代码：UE7N");
            }
        }
        
        private void SetSetting()
        {
            setting = File.ReadAllLines(Environment.CurrentDirectory + @"\Setting.txt");
        }
        private void SetTimer()
        {
            SayTimer.Elapsed += SayTimer_Elapsed;
            SayTimer.Interval = double.Parse(setting[0]);
        }

        public void GetTalkSelectNumber()
        {
            Type type = typeof(TalkSelect);
            FieldInfo fieldInfo = type.GetField("textList", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                textList = (List<SelectText>)fieldInfo.GetValue(ts);
            }
            else
            {
                ShowError("出现错误 错误代码：UE6N");
            }
            FieldInfo fieldInfo2 = type.GetField("textSaid", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                textSaid = (HashSet<string>)fieldInfo2.GetValue(ts);
            }
            else
            {
                ShowError("出现错误 错误代码：UE5N");
            }
        }
        
        public void WriteSetFile()
        {

            File.WriteAllLines(Environment.CurrentDirectory + @"\Setting.txt",setting);
        }
        public void CheckIsTimerOpen()
        {
            if (setting[1]=="1")
            {
                SayTimer.Start();
            }
        }
        public void GetTalkSelect()
        {
            ref MainWindow val = ref mww;
            IMainWindow mW = base.MW;
            val = (MainWindow)(object)((mW is MainWindow) ? mW : null);
            ref TalkSelect val2 = ref ts;
            UIElement talkBox = mww.TalkBox;
            val2 = (TalkSelect)(object)((talkBox is TalkSelect) ? talkBox : null);
        }

        public void GetTbTalk()
        {
            Type type = typeof(TalkSelect);
            FieldInfo tbTalkFiled = type.GetField("tbTalk", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo btnFiled = type.GetField("btn_Send", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo puFiled = type.GetField("PrograssUsed", BindingFlags.Instance | BindingFlags.NonPublic);
            if (tbTalkFiled != null)
            {
                tbTalk = (ComboBox)tbTalkFiled.GetValue(ts);
                tbTalk.Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTalk.IsEditable = true;
                });
            }
            else
            {
                ShowError("出现错误 错误代码：UE4N");
            }
            if (btnFiled != null)
            {
                btn_Send = (Button)btnFiled.GetValue(ts);
            }
            else
            {
                ShowError("出现错误 错误代码：UE1N");
            }
            if (puFiled != null)
            {
                PrograssUsed = (ProgressBar)puFiled.GetValue(ts);
            }
            else
            {
                ShowError("出现错误 错误代码：UE2N");
            }
        }

        private void LoadPluginList(object sender, RoutedEventArgs e)
        {
            GetTalkSelect();
            if (ts != null)
            {
                GetTalkSelectNumber();
                GetTbTalk();
                GetEventShow();
                RelsSelect();
                MessageBox.Show("RealTalk: 代码加载完成".Translate());
            }
            else
            {
                ShowError("加载遇到问题 请确认当前是选项式聊天 错误代码：E1N 如果不知道怎么解决可以查看MOD创意工坊页面简介".Translate());
            }
            
            
        }

        public void DelSystemSentence()
        {
            try
            {
                mww.ClickTexts.Remove(new ClickText("你知道吗? 鼠标右键可以打开菜单栏"));
                mww.ClickTexts.Remove(new ClickText("你知道吗? 你可以在设置里面修改游戏的缩放比例"));
                mww.ClickTexts.Remove(new ClickText("想要宠物不乱动? 设置里可以设置智能移动或者关闭移动"));
                mww.ClickTexts.Remove(new ClickText("这游戏开发这么慢,都怪画师太咕了"));
                //ClickTexts.Add(new ClickText("有建议/游玩反馈? 来 菜单-系统-反馈中心 反馈吧"));
                mww.ClickTexts.Remove(new ClickText("长按脑袋拖动桌宠到你喜欢的任意位置"));
            }
            catch
            {
                MessageBox.Show("删除对话时进程中断 请反馈BetterTalk开发组".Translate());                
            }


        }
        public void GetEventShow()
        {
            Type type = typeof(VPet_Simulator.Core.ToolBar);
            EventInfo eventInfo = type.GetEvent("EventShow", BindingFlags.Instance | BindingFlags.Public);
            if (eventInfo != null)
            {
                MethodInfo methodInfo = eventInfo.GetRemoveMethod();
                MethodInfo handler = typeof(TalkSelect)!.GetMethod("RelsSelect");
                Delegate privateHandlerDelegate = Delegate.CreateDelegate(methodInfo.GetParameters()[0].ParameterType, ts, handler);
                //methodInfo.Invoke(base.MW.get_Main().ToolBar, new object[1] { privateHandlerDelegate });
            }
            else
            {
                ShowError("出现错误 错误代码：UE3N");
            }
        }

        public static string LoaddllPath(string dll)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] array = loadedAssemblies;
            foreach (Assembly assembly in array)
            {
                string assemblyName = assembly.GetName().Name;
                if (assemblyName == dll)
                {
                    string assemblyPath = assembly.Location;
                    string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
                    return Directory.GetParent(assemblyDirectory)!.FullName;
                }
            }
            return "";
        }

        public override void LoadPlugin()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    msgbar = (MessageBar)MW.Main.MsgBar;
                    SayTimer = new System.Timers.Timer();
                    CW = new CheckWindow();
                    mww = MW as MainWindow;
                });
                SetTouchTalk();
                SetTimerVPET();
                CreatSetFile();
                CheckFile();
                SetSetting();
                SetTimer();
                CheckIsTimerOpen();
                DelSystemSentence();
            });
        }
        public override void LoadDIY()
        {
            menuItemRT = new MenuItem
            {
                Header = "无限列表刷新".Translate()
            };
            menuItemRT.Click += LoadPluginList;
            TrunTimer = new MenuItem
            {
                Header = "定时讲话".Translate()
            };
            ClockTalkRT = new MenuItem
            {
                Header = "闹钟讲话".Translate()
            };
            
            OpenTimer = new MenuItem();
            OffTimer = new MenuItem();
            OpenTimer.Click += OpenTimer_Click;
            OffTimer.Click += OffTimer_Click;
            OpenTimer.Header = "打开计时器".Translate();
            OffTimer.Header = "关闭计时器".Translate();
            TrunTimer.Items.Add(OpenTimer);
            TrunTimer.Items.Add(OffTimer);

            MW.Main.ToolBar.MenuDIY.Items.Add(menuItemRT);
            MW.Main.ToolBar.MenuDIY.Items.Add(TrunTimer);
            MW.Main.ToolBar.MenuDIY.Items.Add(ClockTalkRT);
            ClockTalkRT.Click += ClockTalkRT_Click;
        }

        private void ClockTalkRT_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                clockTalk.Visibility = Visibility.Visible;
            });
            
        }

        public override void Setting()
        {
            string TargetPath = Environment.CurrentDirectory + @"\Setting.txt";
            Application.Current.Dispatcher.Invoke(delegate
            {
                set = new IntervalSet
                {
                    num = SayTimer.Interval
                };
                set.UpdateTimerInterval += UpdateInterval;
                set.num = SayTimer.Interval;
                set.TalkTimeSetnum.Text = set.num.ToString();
                set.textBlock.Text = set.textBlock.Text.Translate();
                set.ShowDialog();
            });
            setting[0] = SayTimer.Interval.ToString();
            WriteSetFile();

        }

        private void OffTimer_Click(object sender, RoutedEventArgs e)
        {
            SayTimer.Stop();
            setting[1] = "0";
            WriteSetFile();
        }

        private void OpenTimer_Click(object sender, RoutedEventArgs e)
        {
            SayTimer.Start();
            setting[1] = "1";
            WriteSetFile();
        }

        private void SayTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            
            if (MW.Main.State == Main.WorkingState.Nomal && MW.Main.DisplayType.Type != GraphInfo.GraphType.Sleep)
            {
                
                Random r = new Random();
                int index = r.Next(0, timertext.Count());
                MW.Main.SayRnd(timertext[index]);
            }
            if (MW.Main.State == Main.WorkingState.Work)
            {


                if (MW.Main.NowWork.NameTrans == "跳绳".Translate())
                {
                    
                    Random r = new Random();
                    int index = r.Next(0, playjumptext.Count());
                    MW.Main.SayRnd(playjumptext[index]);
                }

                if (MW.Main.NowWork.NameTrans == "直播".Translate())
                {
                    
                    Random r = new Random();
                    int index = r.Next(0, livetext.Count());
                    MW.Main.SayRnd(livetext[index]);
                }

                if (MW.Main.NowWork.NameTrans == "玩游戏".Translate())
                {
                    
                    Random r = new Random();
                    int index = r.Next(0, playtext.Count());
                    MW.Main.SayRnd(playtext[index]);
                }
                if (MW.Main.NowWork.Type == GraphHelper.Work.WorkType.Work && MW.Main.NowWork.NameTrans != "直播".Translate())
                {
                    
                    Random r = new Random();
                    int index = r.Next(0, worktext.Count());
                    MW.Main.SayRnd(worktext[index]);
                }
                
            }
            if (MW.Main.State == Main.WorkingState.Sleep)
            {
                
                
                Random r = new Random();
                int a = r.Next(0, 60);
                if (a == 8)
                {
                    int index = r.Next(0, sleeptext.Count());
                    MW.Main.SayRnd(sleeptext[index]);
                }
                

            }

        }

        private void SetTouchTalk()
        {
            MW.Main.Event_TouchBody += Main_Event_TouchBody;
            MW.Main.Event_TouchHead += Main_Event_TouchHead;
        }

        private void UpdateInterval()
        {
            SayTimer.Interval = set.num;
        }

        private void Main_Event_TouchBody()
        {
            Random random = new Random();
            int index = random.Next(0, touchBodytext.Count());
            if (MW.Main.Core.Save.Feeling <= MW.Main.Core.Save.FeelingMax / 3.0)
            {
                Random r = new Random();   
                int a = r.Next(0, 5);
                if (a == 1)
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        msgbar.Show(MW.Main.Core.Save.Name, touchBodytext[index].Translate());
                    });
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    msgbar.Show(MW.Main.Core.Save.Name, touchBodytext[index].Translate());
                });
            }
        }

        private void Main_Event_TouchHead()
        {
            Random random = new Random();
            int index = random.Next(0, touchHeadtext.Count());
            if (MW.Main.Core.Save.Feeling <= MW.Main.Core.Save.FeelingMax / 3.0)
            {
                Random r = new Random();
                int a = r.Next(0, 5);
                if (a == 1)
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        msgbar.Show(MW.Main.Core.Save.Name, touchHeadtext[index].Translate());
                    });
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    msgbar.Show(MW.Main.Core.Save.Name, touchHeadtext![index].Translate());
                });
            }
        }

        public static void CreatFlagFile()
        {
            string filePath = LoaddllPath("VPet.Plugin.BetterTalk") + @"\check.txt";
            string TargetPath = Environment.CurrentDirectory + @"\check.txt";
            string content = "";
            File.WriteAllText(filePath, content);
            if (!File.Exists(TargetPath))
            {
                File.Copy(filePath, TargetPath);
                File.WriteAllText(TargetPath, "");
            }
        }

        private void CheckFile()
        {
            string pathCheck = Environment.CurrentDirectory + @"\check.txt";
            if (!File.Exists(pathCheck))
            {
                Task.Run(delegate
                {
                    CW.Dispatcher.Invoke(delegate
                    {
                        CW.ShowDialog();
                    });
                });
            }
        }



        public void CreatSetFile()
        {         
            string talkingtxtFile = LoaddllPath("VPet.Plugin.BetterTalk") + @"\TouchHeadText.txt";
            string talkingtxtFileB = LoaddllPath("VPet.Plugin.BetterTalk") + @"\TouchBodyText.txt";
            string talkingtxtFileC = LoaddllPath("VPet.Plugin.BetterTalk") + @"\Setting.txt";
            string talkingtxtFileD = LoaddllPath("VPet.Plugin.BetterTalk") + @"\TimerText.txt";
            string talkingtxtFilePlay = LoaddllPath("VPet.Plugin.BetterTalk") + @"\PlayText.txt";
            string talkingtxtFileJump = LoaddllPath("VPet.Plugin.BetterTalk") + @"\JumpText.txt";
            string talkingtxtFileLive = LoaddllPath("VPet.Plugin.BetterTalk") + @"\LIVEtext.txt";
            string talkingtxtFileWork = LoaddllPath("VPet.Plugin.BetterTalk") + @"\WorkText.txt";
            string talkingtxtFileSleep = LoaddllPath("VPet.Plugin.BetterTalk") + @"\SleepText.txt";

            string path = Environment.CurrentDirectory + @"\TouchHeadText.txt";
            string path2 = Environment.CurrentDirectory + @"\TouchBodyText.txt";
            string path3 = Environment.CurrentDirectory + @"\Setting.txt";
            string path4 = Environment.CurrentDirectory + @"\TimerText.txt";
            string path5 = Environment.CurrentDirectory + @"\PlayText.txt";
            string path6 = Environment.CurrentDirectory + @"\JumpText.txt";
            string path7 = Environment.CurrentDirectory + @"\LIVEtext.txt";
            string path8 = Environment.CurrentDirectory + @"\WorkText.txt";
            string path9 = Environment.CurrentDirectory + @"\SleepText.txt";
            if (!File.Exists(path))
            {
                File.Copy(talkingtxtFile, path);
            }
            if (!File.Exists(path2))
            {
                File.Copy(talkingtxtFileB, path2);
            }
            if (!File.Exists(path3))
            {
                File.Copy(talkingtxtFileC, path3);
            }
            if (!File.Exists(path4))
            {
                File.Copy(talkingtxtFileD, path4);
            }
            if (!File.Exists(path5))
            {
                File.Copy(talkingtxtFilePlay, path5);
            }
            if (!File.Exists(path6))
            { 
                File.Copy(talkingtxtFileJump, path6);
            }
            if (!File.Exists(path7))
            {
                File.Copy(talkingtxtFileLive, path7);
            }
            if (!File.Exists(path8))
            {
                File.Copy(talkingtxtFileWork, path8);
            }
            if (!File.Exists(path9))
            {
                File.Copy(talkingtxtFileSleep, path9);
            }
            string[] array = File.ReadAllLines(path4);
            foreach (string item in array)
            {
                timertext!.Add(item);
            }
            string[] array2 = File.ReadAllLines(path);
            foreach (string item in array2)
            {
                touchHeadtext!.Add(item);
            }
            string[] array3 = File.ReadAllLines(path2);
            foreach (string item in array3)
            {
                touchBodytext!.Add(item);
            }
            string[] array4 = File.ReadAllLines(path5);
            foreach (string item in array4)
            {
                playtext!.Add(item);
            }
            string[] array5 = File.ReadAllLines(path6);
            foreach (string item in array5)
            {
                playjumptext!.Add(item);
            }
            string[] array6 = File.ReadAllLines(path7);
            foreach (string item in array6)
            {
                livetext!.Add(item);
            }
            string[] array7 = File.ReadAllLines(path8);
            foreach (string item in array7)
            {
                worktext!.Add(item);
            }

            string[] array8 = File.ReadAllLines(path9);
            foreach (string item in array8)
            {
                sleeptext!.Add(item);
            }
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "RealTalk 错误", MessageBoxButton.OK, MessageBoxImage.Hand);
        }

        private void SetTimerVPET()
        {
            msgbar.ShowTimer.Stop();
            msgbar.ShowTimer.Dispose();
            msgbar.ShowTimer = new System.Timers.Timer
            {
                Interval = 85.0
            };
            msgbar.EndTimer.Interval = 1.0;
            msgbar.ShowTimer.Elapsed += new ElapsedEventHandler(ShowTimer_Elapsed);
            msgbar.CloseTimer.Interval = 80;

        }


        private void RelsSelect()
        {
            if (RelsTime < DateTime.Now)
            {
                RelsTime = DateTime.Now.AddMinutes(10.0);
                List<SelectText> list = MW.SelectTexts.ToList();
                while (list.Count > 0 && textList.Count < list.Count)
                {
                    int sid = VPet_Simulator.Core.Function.Rnd.Next(list.Count);
                    SelectText select = list[sid];
                    list.RemoveAt(sid);
                    if (textList.Find((SelectText x) => x.Choose == select.Choose) == null && ((ICheckText)select).CheckState(MW.Main))
                    {
                        textList.Add(select);
                    }
                }
            }
            if (textList.Count > 0)
            {
                tbTalk.Items.Clear();
                foreach (SelectText item in textList)
                {
                    if (!textSaid.Contains(item.Choose))
                    {
                        tbTalk.Items.Add(item.Choose.Translate());
                    }
                }
                btn_Send.IsEnabled = true;
            }
            else
            {
                tbTalk.Items.Clear();
                tbTalk.Items.Add("没有可以说的话？请上报RealTalk开发者".Translate());
                btn_Send.IsEnabled = false;
            }
            double min = (RelsTime - DateTime.Now).TotalMinutes;
            double prograss = 1.0 - min / 10.0;
            if (prograss > 1.0)
            {
                prograss = 1.0;
            }
            else if (prograss < 0.0)
            {
                prograss = Math.Min(1.0, Math.Max(0.0, min % 10.0)) / 2.0;
            }
            PrograssUsed.Value = prograss;
        }

        private void ShowTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetOutputtext();
            if (outputtext!.Count > 0)
            {
                msgbar.ShowTimer.Interval = 92.0;
                char str = outputtext![0];
                outputtext!.RemoveAt(0);
                ((DispatcherObject)(object)msgbar).Dispatcher.Invoke(delegate
                {
                    TextBox tText = msgbar.TText;
                    ReadOnlySpan<char> readOnlySpan = tText.Text;
                    char reference = str;
                    tText.Text = string.Concat(readOnlySpan, new ReadOnlySpan<char>(ref reference));
                });
                char[] array = chars;
                foreach (char c in array)
                {
                    if (str == c)
                    {
                        msgbar.ShowTimer.Interval = 165.0;
                    }
                }
                return;
            }
            if (base.MW.Main.PlayingVoice)
            {
                if (base.MW.Main.windowMediaPlayerAvailable)
                {
                    if (((DispatcherObject)(object)msgbar).Dispatcher.Invoke(delegate
                    {
                        MediaElement voicePlayer = base.MW.Main.VoicePlayer;
                        return (voicePlayer != null && (voicePlayer.Clock?.NaturalDuration.HasTimeSpan).GetValueOrDefault()) ? (base.MW.Main.VoicePlayer.Clock.NaturalDuration.TimeSpan - base.MW.Main.VoicePlayer.Clock.CurrentTime.Value) : TimeSpan.Zero;
                    }).TotalSeconds > 2.0)
                    {
                        return;
                    }
                    Console.WriteLine(1);
                }
                else if (base.MW.Main.soundPlayer.IsLoadCompleted)
                {
                    base.MW.Main.PlayingVoice = false;
                    base.MW.Main.soundPlayer.PlaySync();
                }
            }
            msgbar.ShowTimer.Stop();
            msgbar.EndTimer.Start();
        }

        public void CheckIsSleeping(string message)
        {
            if (MW.Main.State != Main.WorkingState.Sleep)
            {
                MW.Main.SayRnd(message);
            }
        }
    }
}
