using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;
using System;
using WebService;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.IO;



namespace VisaValeTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;
        private const string conn = @"isostore:/visaVale.sdf";
        //http://social.technet.microsoft.com/wiki/pt-br/contents/articles/9901.windows-phone-background-agents-atualize-o-tile-de-sua-aplicacao.aspx
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //string value = "";
            //string text = "";                        

            //using (var ctx = new visaValeDataContext(conn))
            //{

            //    if (ctx.DatabaseExists())
            //    {
            //        foreach (var item in ctx.VisaVales)
            //        {
            //            if (item.Id == 1)
            //            {
            //                value = item.GetSaldo;
            //                text = item.DataConsulta;
            //            }
            //        }
            //    }

            //    // Execute periodic task actions here.
            //    ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("TileID=2"));
            //    if (TileToFind != null)
            //    {
            //        TileToFind.Update(new StandardTileData
            //        {
            //            Title = "vvvv",
            //            BackgroundImage = new Uri("isostore:/Shared/ShellContent/testtile.jpg", UriKind.Absolute),
            //            BackBackgroundImage = new Uri("isostore:/Shared/ShellContent/testtile.jpg", UriKind.Absolute),
            //            BackTitle = "yyyy",
            //            BackContent = "iiiiiiiii"
            //        });

            //      //  TileToFind.Update(NewTileData);
            //    }

               
            //}

           

            NotifyComplete();
        }

    }
}
