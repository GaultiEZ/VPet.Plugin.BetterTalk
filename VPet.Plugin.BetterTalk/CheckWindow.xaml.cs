using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// CheckWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckWindow : Window
    {
        public CheckWindow()
        {
            InitializeComponent();  
        }

        private void AgreementCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
            BetterTalk.CreatFlagFile();
        }

    }
}
