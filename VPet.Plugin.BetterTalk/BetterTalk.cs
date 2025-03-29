using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LinePutScript.Localization.WPF;
using VPet.Plugin.BetterTalk;
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

        private IntervalSet set;

        private CheckWindow CW;

        public System.Timers.Timer SayTimer;

        public override string PluginName => "BetterTalk";

        public BetterTalk(IMainWindow mainwin) : base(mainwin)
        {
            touchHeadtext = new List<string>();
            touchBodytext = new List<string>();
            timertext = new List<string>();
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

        private void SetTimer()
        {
            SayTimer.Elapsed += new ElapsedEventHandler(SayTimer_Elapsed);
            string TargetPath = Environment.CurrentDirectory + "/TimerAndTimeText.txt";
            SayTimer.Interval = double.Parse(File.ReadAllText(TargetPath));
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
            msgbar = (MessageBar)MW.Main.MsgBar;
            SayTimer = new System.Timers.Timer();
            CW = new CheckWindow();
            mww = MW as MainWindow;
            SetTouchTalk();
            SetTimerVPET();
            CreatAndReadTalkingFile();
            SetTimer();
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
        }

        public override void Setting()
        {
            string TargetPath = Environment.CurrentDirectory + @"\TimerAndTimeText.txt";
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
            File.WriteAllText(TargetPath, SayTimer.Interval.ToString());
        }

        private void OffTimer_Click(object sender, RoutedEventArgs e)
        {
            SayTimer.Stop();
        }

        private void OpenTimer_Click(object sender, RoutedEventArgs e)
        {
            SayTimer.Start();
        }

        private void SayTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (MW.Main.State == 0)
            {
                Random r = new Random();
                int index = r.Next(0, timertext.Count());
                MW.Main.SayRnd(timertext[index]);
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

        public void CreatAndReadTalkingFile()
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
            string talkingtxtFile = LoaddllPath("VPet.Plugin.BetterTalk") + @"\TouchHeadText.txt";
            string talkingtxtFileB = LoaddllPath("VPet.Plugin.BetterTalk") + @"\TouchBodyText.txt";
            string talkingtxtFileC = LoaddllPath("VPet.Plugin.BetterTalk") + @"\TimerAndTimeText.txt";
            string talkingtxtFileD = LoaddllPath("VPet.Plugin.BetterTalk") + @"\TimerText.txt";
            string path = Environment.CurrentDirectory + @"\TouchHeadText.txt";
            string path2 = Environment.CurrentDirectory + @"\TouchBodyText.txt";
            string path3 = Environment.CurrentDirectory + @"\TimerAndTimeText.txt";
            string path4 = Environment.CurrentDirectory + @"\TimerText.txt";
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
            string[] array = File.ReadAllLines(path4);
            foreach (string item in array)
            {
                timertext!.Add(item);
            }
            string[] array2 = File.ReadAllLines(path);
            foreach (string item2 in array2)
            {
                touchHeadtext!.Add(item2);
            }
            string[] array3 = File.ReadAllLines(path2);
            foreach (string item3 in array3)
            {
                touchBodytext!.Add(item3);
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
            msgbar.CloseTimer.Interval = 70;
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
    }
}
