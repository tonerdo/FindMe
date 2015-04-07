#define DEBUG_AGENT 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System.Threading;
using System.Windows.Threading;
using Microsoft.Phone.Scheduler;

namespace FindMe
{
    public partial class Splashscreen : PhoneApplicationPage
    {
        private DispatcherTimer dispatcherTimer;
        private PeriodicTask periodicTask;
        private bool isEnabled = true;
        public Splashscreen()
        {
            InitializeComponent();
            StartPeriodicAgent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        void StartPeriodicAgent()
        {
            isEnabled = true;
            string periodicTaskName = "PeriodicAgent";
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;
            if (periodicTask != null)
                RemoveAgent(periodicTaskName);

            periodicTask = new PeriodicTask(periodicTaskName);
            periodicTask.Description = "Location Periodic Task";

            try
            {
                ScheduledActionService.Add(periodicTask);
                //MessageBox.Show("Started");
#if DEBUG_AGENT
                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                //MessageBox.Show(exception.Message);
                isEnabled = false;
            }
            catch (SchedulerServiceException) { }
        }

        void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }
    }
}