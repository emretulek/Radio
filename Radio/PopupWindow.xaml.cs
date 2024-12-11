using System.Windows;

namespace Radio
{
    public partial class PopupWindow : Window
    {
        public Radio? NewRadio;

        public PopupWindow(Radio? radio = null)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (radio != null)
            {
                NewRadio = radio;
                Url.Text = radio.Url;
                RadioName.Text = radio.Name;
                Description.Text = radio.Description;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Uri.TryCreate(Url.Text, UriKind.Absolute, out Uri? url))
            {
                MessageBox.Show("Invalid Stream Url", "Error");
                return;
            }

            if (RadioName.Text.Length == 0)
            {
                MessageBox.Show("Radio Name is Required", "Error");
                return;
            }

            if (NewRadio != null)
            {
                NewRadio.Url = url.ToString();
                NewRadio.Name = RadioName.Text;
                NewRadio.Description = Description.Text;
            }
            else
            {
                NewRadio = new Radio
                {
                    Url = url.ToString(),
                    Name = RadioName.Text,
                    Description = Description.Text,
                };
            }

            DialogResult = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
