using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace WebService.Pages
{
    public partial class Pesquisa : PhoneApplicationPage
    {
        IsolatedStorageSettings _isoSettings;
        string ISO_KEY_SIMPLE = "Key_simpleIS";

        public Pesquisa()
        {
            InitializeComponent();
            title.DataContext = "CONSULTA CARTÕES VISA VALE";
            title.FontSize = 26;

            _isoSettings = IsolatedStorageSettings.ApplicationSettings;

            ConsultaCache();
        }

        public void ConsultaCache()
        {
            if (_isoSettings[ISO_KEY_SIMPLE].ToString() != "")
            {
                tbox1.Text = _isoSettings[ISO_KEY_SIMPLE].ToString();
                cbx1.IsChecked = true;
            }
            else
            {
                cbx1.IsChecked = false;
            }
        }

        private void ConsultarServer(object sender, EventArgs e)
        {

            if (cbx1.IsChecked == true)
            {
                _isoSettings[ISO_KEY_SIMPLE] = tbox1.Text;

            }
            else
            {
                _isoSettings[ISO_KEY_SIMPLE] = "";
            }

            this.NavigationService.Navigate(new Uri("/MainPage.xaml?numero=" + tbox1.Text, UriKind.Relative));
        }
    }
}