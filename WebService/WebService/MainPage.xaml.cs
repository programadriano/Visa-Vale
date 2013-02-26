using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Runtime.Serialization;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using System.Threading;
using Microsoft.Phone.Scheduler;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Resources;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;

namespace WebService
{

    //http://json.codeplex.com/releases/view/89222

    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageSettings _isoSettings;
        private const string conn = @"isostore:/visaVale.sdf";

        private Informacoes info = new Informacoes();
        private XDocument xDoc;

        public MainPage()
        {
            InitializeComponent();
            title.DataContext = "CONSULTA CARTÕES VISA VALE";
            title.FontSize = 26;
            _isoSettings = IsolatedStorageSettings.ApplicationSettings;
           // RegistrarAgente();
            ConsultaCache();
        }

        public void ConsultaCache()
        {
            using (var ctx = new visaValeDataContext(conn))
            {
                if (ctx.DatabaseExists())
                {
                    foreach (var item in ctx.VisaVales)
                    {
                        if (item.Id == 1)
                        {
                            if (item.SalvarDados == true)
                            {
                                tbox1.Text = item.NumeroCartao.ToString();
                                cbx1.IsChecked = true;
                            }
                        }
                    }
                }

            }

        }

        public void DownloadStringCompleted(object sender, HtmlDocumentLoadCompleted e)
        {

            if (e.Document != null)
            {
                HtmlNodeCollection table = e.Document.DocumentNode.SelectNodes("//*[@class='consulta ']");

                var appBar = table[2].InnerHtml.Replace(" class=\"rowTable\"", "").Replace(" style=\"width:50px\"", "").Replace(" style=\"width:400px\"", "").Replace(" class=\"corUm\" align=\"right\"", "").Replace("&nbsp;", "");
                appBar = appBar.Insert(0, "<xml>").Insert(appBar.Length - 1, "</xml>");

                var tableSaldo = table[3].InnerHtml.Replace(" class=\"rowTable\"", "")
                    .Replace(" class=\"corUm fontWeightDois\"", "")
                    .Replace(" align=\"right\"", "")
                    .Replace(" class=\"corUm fontWeightDois\"", "")
                    .Replace(" style=\"width:80px\"", "")
                    .Replace("dispon&iacute;vel:", "");
                // dispon&iacute;vel:
                xDoc = XDocument.Parse(appBar);
                XDocument xDocSaldo = XDocument.Parse(tableSaldo);



                foreach (var item in xDocSaldo.Root.Nodes())
                {
                    if (((System.Xml.Linq.XElement)(item)).Value.ToString().Contains("$"))
                    {
                        info.getSaldo = ((System.Xml.Linq.XElement)(item)).Value.ToString();
                        //MessageBox.Show(((System.Xml.Linq.XElement)(item)).Value.ToString());
                    }

                }

                foreach (var item in table.Descendants("td"))
                {

                    var teste = item.InnerHtml;
                    //data ultima disponibilizacao
                    if (item.XPath == "/html[1]/body[1]/table[1]/tr[3]/td[2]")
                    {
                        info.DataUltimoBeneficioDataBeneficio = item.InnerHtml;
                        //MessageBox.Show(item.InnerHtml);
                    }//valor disponibilizado
                    else if (item.XPath == "/html[1]/body[1]/table[1]/tr[3]/td[3]")
                    {
                        info.ValorUltimoBeneficio = item.InnerHtml;
                        //MessageBox.Show(item.InnerHtml);
                    }///data proximo        
                    else if (item.XPath == "/html[1]/body[1]/table[1]/tr[4]/td[2]")
                    {
                        // MessageBox.Show(item.InnerHtml);
                        info.DataProximoBeneficio = item.InnerHtml;
                        // MessageBox.Show(item.InnerHtml);
                    }
                    /// valor proximo
                    else if (item.XPath == "/html[1]/body[1]/table[1]/tr[4]/td[3]")
                    {
                        info.ValorProximoBeneficio = item.InnerHtml;
                        // MessageBox.Show(item.InnerHtml);
                    }
                    else if (item.XPath == "/html[1]/body[1]/table[1]/tr[1]/td[2]")
                    {
                        info.DataConsulta = item.InnerHtml.Substring(0, 10);
                    }
                    else if (item.XPath == "/html[1]/body[1]/table[1]/tr[2]/td[2]")
                    {
                        info.NumeroCartao = item.InnerHtml;
                    }

                }

                List<VisaValeGastos> c = new List<VisaValeGastos>();

                foreach (var item in xDoc.Root.Nodes())
                {
                    c.Add(new VisaValeGastos() { Local = ((System.Xml.Linq.XElement)(item)).Value.ToString().Remove(0, 5).Remove(((System.Xml.Linq.XElement)(item)).Value.IndexOf("$") - 6), Data = ((System.Xml.Linq.XElement)(item)).Value.ToString().Substring(0, 5), Valor = ((System.Xml.Linq.XElement)(item)).Value.Substring(((System.Xml.Linq.XElement)(item)).Value.IndexOf("R$")) });
                }

                c.Reverse();

                listaGastos.ItemsSource = c;
                listaGastos.Foreground = new SolidColorBrush(Colors.Black);
                var coldata = listaGastos.Columns["Data"];
                coldata.Width = new GridLength(80);
                var colLocal = listaGastos.Columns["Local"];
                colLocal.Width = new GridLength(260);
                var colValor = listaGastos.Columns["Valor"];
                colValor.Width = new GridLength(140);
                tbox1.Visibility = Visibility.Collapsed;
                tbloc1.Visibility = Visibility.Collapsed;
                cbx1.Visibility = Visibility.Collapsed;

                tblDate.DataContext = info.DataUltimoBeneficioDataBeneficio;
                tblText.DataContext = "Último benefício";
                tblLast.DataContext = info.ValorUltimoBeneficio.Substring(6);

                tblDateNext.DataContext = info.DataProximoBeneficio != "" ? info.DataProximoBeneficio : "N/D";
                tblTextNext.DataContext = "Próximo benefício";
                tblLastNext.DataContext = info.ValorProximoBeneficio.Substring(6) != "" ? info.ValorProximoBeneficio.Substring(6) : "N/D";

                tblTextSaldo.DataContext = "Saldo atual";
                tblSaldo.DataContext = info.getSaldo;

                tblTextSaldo.Visibility = Visibility.Visible;
                tblSaldo.Visibility = Visibility.Visible;

                tblDate.Visibility = Visibility.Visible;
                tblText.Visibility = Visibility.Visible;
                tblLast.Visibility = Visibility.Visible;

                tblDateNext.Visibility = Visibility.Visible;
                tblTextNext.Visibility = Visibility.Visible;
                tblLastNext.Visibility = Visibility.Visible;
                tblDataPesquisa.Visibility = Visibility.Visible;
                tblDetalhe.Visibility = Visibility.Visible;
                tblDataPesquisa.DataContext = "Data da pesquisa: " + info.DataConsulta;
                title.DataContext = "Extrato";
                title.FontSize = 30;


                ApplicationBar.Buttons.RemoveAt(0);


                this.ApplicationBar = new ApplicationBar();

                var newButton = new ApplicationBarIconButton();
                newButton.IconUri = new Uri("/Resourses/appbar.refresh.png", UriKind.Relative);
                newButton.Text = "update";
                newButton.Click += ConsultarServer;

                this.ApplicationBar.Buttons.Add(newButton);
                stackPanel.Visibility = Visibility.Collapsed;
                this.progressBar.IsIndeterminate = false;

                Dispatcher.BeginInvoke(() =>
                {
                    Save();
                });
            }
            else
            {
                if (this.progressBar.IsIndeterminate)
                {
                    using (var ctx = new visaValeDataContext(conn))
                    {
                        if (ctx.DatabaseExists())
                        {
                            var confirm = MessageBox.Show("Não foi possível obter conexão de dados! \n Deseja continuar com as informações da sua última pesquisa ?", "", MessageBoxButton.OKCancel);

                            if (confirm.ToString() == "OK")
                            {
                                foreach (var item in ctx.VisaVales)
                                {
                                    if (item.Id == 1)
                                    {
                                        tblDate.DataContext = item.DataUltimoBeneficioDataBeneficio;
                                        tblText.DataContext = "Último benefício";
                                        tblLast.DataContext = item.ValorUltimoBeneficio.Substring(6);

                                        tblDateNext.DataContext = item.DataProximoBeneficio != "" ? item.DataProximoBeneficio : "N/D";
                                        tblTextNext.DataContext = "Próximo benefício";
                                        tblLastNext.DataContext = item.ValorProximoBeneficio != "" ? item.ValorProximoBeneficio.Substring(6) : "N/D";

                                        tblTextSaldo.DataContext = "Saldo atual";
                                        tblSaldo.DataContext = item.GetSaldo;


                                        tblTextSaldo.Visibility = Visibility.Visible;
                                        tblSaldo.Visibility = Visibility.Visible;

                                        tblDate.Visibility = Visibility.Visible;
                                        tblText.Visibility = Visibility.Visible;
                                        tblLast.Visibility = Visibility.Visible;

                                        tblDateNext.Visibility = Visibility.Visible;
                                        tblTextNext.Visibility = Visibility.Visible;
                                        tblLastNext.Visibility = Visibility.Visible;
                                        tblDataPesquisa.Visibility = Visibility.Visible;
                                        tblDetalhe.Visibility = Visibility.Visible;
                                        tblDataPesquisa.DataContext = "Data da pesquisa: " + item.DataConsulta;
                                        title.DataContext = "Extrato";
                                        title.FontSize = 30;


                                        ApplicationBar.Buttons.RemoveAt(0);


                                        this.ApplicationBar = new ApplicationBar();

                                        var newButton = new ApplicationBarIconButton();
                                        newButton.IconUri = new Uri("/Resourses/appbar.refresh.png", UriKind.Relative);
                                        newButton.Text = "update";
                                        newButton.Click += ConsultarServer;

                                        this.ApplicationBar.Buttons.Add(newButton);
                                        stackPanel.Visibility = Visibility.Collapsed;
                                        this.progressBar.IsIndeterminate = false;

                                        List<VisaValeGastos> listaGastosVisa = new List<VisaValeGastos>();



                                        foreach (var item2 in item.VisaValeGastos)
                                        {
                                            listaGastosVisa.Add(new VisaValeGastos { Data = item2.Data, Local = item2.Local, Valor = item2.Valor });
                                        }

                                        listaGastosVisa.Reverse();

                                        listaGastos.ItemsSource = listaGastosVisa;
                                        listaGastos.Foreground = new SolidColorBrush(Colors.Black);
                                        var coldata = listaGastos.Columns["Data"];
                                        coldata.Width = new GridLength(80);
                                        var colLocal = listaGastos.Columns["Local"];
                                        colLocal.Width = new GridLength(260);
                                        var colValor = listaGastos.Columns["Valor"];
                                        colValor.Width = new GridLength(140);
                                        tbox1.Visibility = Visibility.Collapsed;
                                        tbloc1.Visibility = Visibility.Collapsed;
                                        cbx1.Visibility = Visibility.Collapsed;


                                        break;
                                    }
                                }
                            }
                        }
                        else if (!ctx.DatabaseExists())
                        {
                            MessageBox.Show("Não foi possível estabelecer conexão com o servidor de dados!");
                        }
                    }
                }
            }

            this.progressBar.IsIndeterminate = false;
            stackPanel.Visibility = Visibility.Collapsed;
        }

        private void ConsultarServer(object sender, EventArgs e)
        {
            if (tbox1.Text.Length < 16)
            {
                MessageBox.Show("Número de catão inválido!");
            }
            else
            {
                stackPanel.Visibility = Visibility.Visible;
                this.progressBar.IsIndeterminate = true;
                var wc = new HtmlWeb();
                wc.LoadAsync("http://www.cartoesbeneficio.com.br/inst/convivencia/SaldoExtrato.jsp?numeroCartao=" + tbox1.Text + "", Encoding.UTF8);
                wc.LoadCompleted += DownloadStringCompleted;
            }

        }

        public void Save()
        {
            using (var ctx = new visaValeDataContext(conn))
            {
                //if (ctx.DatabaseExists())
                //{
                ctx.DeleteDatabase();
                ctx.CreateDatabase();

                var infoVisa = new VisaVale1
                    {
                        DataConsulta = info.DataConsulta,
                        DataProximoBeneficio = info.DataProximoBeneficio,
                        DataUltimoBeneficioDataBeneficio = info.DataUltimoBeneficioDataBeneficio,
                        GetSaldo = info.getSaldo,
                        ValorProximoBeneficio = info.ValorProximoBeneficio,
                        ValorUltimoBeneficio = info.ValorUltimoBeneficio,
                        NumeroCartao = info.NumeroCartao

                    };
                foreach (var item2 in xDoc.Root.Nodes())
                {
                    infoVisa.VisaValeGastos.Add(new VisaValeGasto { Local = ((System.Xml.Linq.XElement)(item2)).Value.ToString().Remove(0, 5).Remove(((System.Xml.Linq.XElement)(item2)).Value.IndexOf("$") - 6), Data = ((System.Xml.Linq.XElement)(item2)).Value.ToString().Substring(0, 5), Valor = ((System.Xml.Linq.XElement)(item2)).Value.Substring(((System.Xml.Linq.XElement)(item2)).Value.IndexOf("R$")) });
                }

                infoVisa.SalvarDados = cbx1.IsChecked;
                ctx.VisaVales.InsertOnSubmit(infoVisa);
                ctx.SubmitChanges();
            }

        }

        void RegistrarAgente()
        {
            //start background agent 
            PeriodicTask periodicTask = new PeriodicTask("PeriodicAgent");

            periodicTask.Description = "My live tile periodic task";
            periodicTask.ExpirationTime = System.DateTime.Now.AddDays(1);
         
            if (ScheduledActionService.Find(periodicTask.Name) != null)
            {
                ScheduledActionService.Remove("PeriodicAgent");
            }
         
            ScheduledActionService.Add(periodicTask);


            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("TileID=2"));
                       

            if (TileToFind == null)
            {
                StandardTileData NewTileData = new StandardTileData();   
                NewTileData.Title = "First";
                NewTileData.Count = 1;
                NewTileData.BackTitle = "Second";
                NewTileData.BackContent = "Second content";
                NewTileData.BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/testtile.jpg", UriKind.Absolute);
                

                //TileToFind.Update(NewTileData);
                ShellTile.Create(new Uri("/MainPage.xaml?TileID=2", UriKind.Relative), NewTileData);
                // ShellTile.ActiveTiles(new Uri("/MainPage.xaml?TileID=2", UriKind.Relative), NewTileData);
            }

            ScheduledActionService.LaunchForTest("PeriodicAgent", TimeSpan.FromMilliseconds(1500));

        }

        public class Informacoes
        {
            public string DataConsulta { get; set; }
            public string NumeroCartao { get; set; }
            public string DataUltimoBeneficioDataBeneficio { get; set; }
            public string ValorUltimoBeneficio { get; set; }
            public string DataProximoBeneficio { get; set; }
            public string ValorProximoBeneficio { get; set; }
            public string getSaldo { get; set; }
            public bool Salvar { get; set; }
        }

        public class VisaValeGastos
        {
            public string Data { get; set; }
            public string Local { get; set; }
            public string Valor { get; set; }

        }

    }
}