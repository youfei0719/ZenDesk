using System;
using System.Threading;
using System.Windows;

namespace ZenDesk
{
    public partial class App : Application
    {
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Change shutdown mode so app doesn't unexpectedly close during 
            // the transition between windows or when all containers are closed.
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _mutex = new Mutex(true, "ZenDesk_SingleInstance_Mutex", out bool isNewInstance);

            if (!isNewInstance)
            {
                // Optionally bring the existing instance to front
                MessageBox.Show("ZenDesk is already running.", "ZenDesk", MessageBoxButton.OK, MessageBoxImage.Information);
                Current.Shutdown();
                return;
            }

            // Launch Onboarding or Main Window depending on config
            // For this MVP, we show onboarding to demonstrate the PRD requirements
            var onboarding = new Views.OnboardingWindow();
            onboarding.Show();
        }
    }
}
