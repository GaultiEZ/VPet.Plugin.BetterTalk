﻿using LinePutScript.Localization.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
 
namespace VPet.Plugin.BetterTalk
{
    /// <summary>
    /// IntervalSet.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class IntervalSet : Window
    {
        public double num;

        private string pastNum;

        public Action UpdateTimerInterval;

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                double num2 = double.Parse(TalkTimeSetnum.Text);
                if (num2 <= 0.0)
                {
                    MessageBox.Show("等。。。等一下啦! 数字不可以小于0呢。。笨蛋。。".Translate());
                    TalkTimeSetnum.Text = pastNum.ToString();
                    return;
                }
                try
                {
                    num = double.Parse(TalkTimeSetnum.Text);
                }
                catch
                {
                    MessageBox.Show("Please input numble!");
                }
                UpdateTimerInterval();
            }
            catch
            {
                MessageBox.Show("确保你输入的是数字！".Translate());  
            }
        }
        public IntervalSet()
        {  
            InitializeComponent();
            TranslateText();
        }
        private void TranslateText()
        {
            Dispatcher.Invoke(() =>
            {
                textBlock.Text = textBlock.Text.Translate();
                textBlock2.Text = textBlock2.Text.Translate();
                textBlock3.Text = textBlock3.Text.Translate();
                textBlock4.Text = textBlock4.Text.Translate();
                textBlockAddPlay.Text = textBlockAddPlay.Text.Translate();
                textBlockAddJump.Text = textBlockAddJump.Text.Translate();
                textBlockAddLive.Text = textBlockAddLive.Text.Translate();
                textBlockAddNWork.Text = textBlockAddNWork.Text.Translate();
                textBlockAddSleep.Text = textBlockAddSleep.Text.Translate();
                AddTextBox1.Text = AddTextBox1.Text.Translate();
                AddTextBox2.Text = AddTextBox2.Text.Translate();
                AddTextBox3.Text = AddTextBox3.Text.Translate();
                AddTextBoxJump.Text = AddTextBoxJump.Text.Translate();
                AddTextBoxLive.Text = AddTextBoxLive.Text.Translate();
                AddTextBoxNWork.Text = AddTextBoxNWork.Text.Translate();
                AddTextBoxPlay.Text = AddTextBoxPlay.Text.Translate();
                AddTextBoxSleep.Text = AddTextBoxSleep.Text.Translate();
                Button1.Content = Button1.Content.ToString().Translate();
                Button2.Content = Button2.Content.ToString().Translate();
                Button3.Content = Button3.Content.ToString().Translate();
                Button4.Content = Button4.Content.ToString().Translate();
                Button5.Content = Button5.Content.ToString().Translate();
                ButtonJUMP.Content = ButtonJUMP.Content.ToString().Translate();
                ButtonLiveF.Content = ButtonLiveF.Content.ToString().Translate();
                ButtonNWorkF.Content = ButtonNWorkF.Content.ToString().Translate();
                ButtonPlay.Content = ButtonPlay.Content.ToString().Translate();
                ButtonJUMP.Content = ButtonJUMP.Content.ToString().Translate();
                ButtonPlayLive.Content = ButtonPlayLive.Content.ToString().Translate();
                ButtonSleepOF.Content = ButtonSleepOF.Content.ToString().Translate();
                ButtonOPPlayF.Content = ButtonOPPlayF.Content.ToString().Translate();

                ButtonPlayNWork.Content = ButtonPlayNWork.Content.ToString().Translate();
                ButtonPlayJump.Content = ButtonPlayJump.Content.ToString().Translate();
                ButtonPlayLiveSleep.Content = ButtonPlayLiveSleep.Content.ToString().Translate();
            });
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (AddTextBox1.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            {
                string path = Environment.CurrentDirectory + @"\TouchHeadText.txt";
                try
                {
                    File.AppendAllText(path,Environment.NewLine + AddTextBox1.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR1".Translate());

                }

                MessageBox.Show("添加成功".Translate());
                
                    AddTextBox1.Text = "";
                
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (AddTextBox2.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            { 
                string path = Environment.CurrentDirectory + @"\TouchBodyText.txt";
                try
                {
                File.AppendAllText(path, Environment.NewLine + AddTextBox2.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR2".Translate());

                }

                MessageBox.Show("添加成功".Translate());
                
                    AddTextBox2.Text = "";
                
            }
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            if (AddTextBox3.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            {

                string path = Environment.CurrentDirectory + @"\TimerText.txt";
                try
                {
                    File.AppendAllText(path, Environment.NewLine + AddTextBox3.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR3".Translate());

                }

                MessageBox.Show("添加成功".Translate());
                
                
                AddTextBox3.Text = "";
                
            }
        }

        private void OpenTHFile(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\TouchHeadText.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void OpenTBFile(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\TouchBodyText.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void OpenTTFile(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\TimerText.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void ButtonOPPlayF_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\PlayText.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void ButtonJUMP_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\JumpText.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void ButtonNWorkF_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\WorkText.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void ButtonLiveF_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\LIVEtext.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void ButtonSleepOF_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + @"\SleepText.txt";
            try
            {
                // 使用记事本打开文件
                Process.Start("notepad.exe", path);

            }
            catch
            {
                MessageBox.Show("打开失败".Translate());
            }
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {

            if (AddTextBoxPlay.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            {

                string path = Environment.CurrentDirectory + @"\PlayText.txt";
                try
                {
                    File.AppendAllText(path, Environment.NewLine + AddTextBoxPlay.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR3".Translate());

                }

                MessageBox.Show("添加成功".Translate());


                AddTextBoxPlay.Text = "";

            }
        }

        private void ButtonPlayJump_Click(object sender, RoutedEventArgs e)
        {
            if (AddTextBoxJump.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            {

                string path = Environment.CurrentDirectory + @"\JumpText.txt";
                try
                {
                    File.AppendAllText(path, Environment.NewLine + AddTextBoxJump.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR3".Translate());

                }

                MessageBox.Show("添加成功".Translate());


                AddTextBoxJump.Text = "";

            }
        }

        private void ButtonPlayNWork_Click(object sender, RoutedEventArgs e)
        {
            if (AddTextBoxNWork.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            {

                string path = Environment.CurrentDirectory + @"\WorkText.txt";
                try
                {
                    File.AppendAllText(path, Environment.NewLine + AddTextBoxNWork.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR3".Translate());

                }

                MessageBox.Show("添加成功".Translate());


                AddTextBoxNWork.Text = "";

            }
        }

        private void ButtonPlayLive_Click(object sender, RoutedEventArgs e)
        {
            if (AddTextBoxLive.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            {

                string path = Environment.CurrentDirectory + @"\LIVEtext.txt";
                try
                {
                    File.AppendAllText(path, Environment.NewLine + AddTextBoxLive.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR3".Translate());

                }

                MessageBox.Show("添加成功".Translate());


                AddTextBoxLive.Text = "";

            }
        }

        private void ButtonPlayLiveSleep_Click(object sender, RoutedEventArgs e)
        {
            if (AddTextBoxSleep.Text == "")
            {
                MessageBox.Show("怎么不输入字符呢？".Translate());
            }
            else
            {

                string path = Environment.CurrentDirectory + @"\SleepText.txt";
                try
                {
                    File.AppendAllText(path, Environment.NewLine + AddTextBoxSleep.Text);
                }
                catch
                {
                    MessageBox.Show("发生错误 错误代码：FR3".Translate());

                }

                MessageBox.Show("添加成功".Translate());


                AddTextBoxSleep.Text = "";

            }
        }
    }
    
    
    
}
