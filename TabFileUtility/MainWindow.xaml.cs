using System;
using System.Collections.Generic;
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

namespace TabFileUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBrowseClick(object sender, RoutedEventArgs e)
        {
            var VM = (ViewModels.MainWindowViewModel)this.DataContext;
            VM.Browse();
        }

        private void ButtonProcessClick(object sender, RoutedEventArgs e)
        {
            var VM = (ViewModels.MainWindowViewModel)this.DataContext;
            VM.Process();
        }
    }
}
