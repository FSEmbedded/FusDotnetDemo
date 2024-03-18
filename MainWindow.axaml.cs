using Avalonia.Controls;
using ReactiveUI;
using System.Windows.Input;

namespace IoTLib_Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            btnGpio.Command = ReactiveCommand.Create(GpioTest);
        }

        public void GpioTest()
        {
            tbInfo.Text = "Run GPIO Test";
        }
    }
}