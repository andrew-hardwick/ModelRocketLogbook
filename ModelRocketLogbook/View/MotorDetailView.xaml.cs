using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModelRocketLogbook.View
{
    /// <summary>
    /// Interaction logic for MotorsDetailView.xaml
    /// </summary>
    public partial class MotorDetailView : UserControl
    {
        public MotorDetailView()
        {
            InitializeComponent();
        }

        private void IntegerValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void FloatingPointValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            var isDigit = !regex.IsMatch(e.Text);
            var isPeriod = e.Text.Equals(".");

            var valid = isDigit || (isPeriod && !((TextBox)sender).Text.Contains("."));

            e.Handled = !valid;
        }
    }
}