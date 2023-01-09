using Makement.Service;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace Makement.Views
{
    public partial class MainWindow : Window
    {
        private bool NavbarShow = false;
        public MainWindow()
        {
            InitializeComponent();
            App.IsRunning = false;
            //Start.Click += OnClick;
            LocalSaverService.save += AppUsageService.SaveLocal;
            LocalSaverService.save += ActiveCheckService.SaveLocal;

            ServerSaverService.save += SaveFilesService.Send;

            LoginBtn.Click += Login;
            ExitBtn.Click += Exit;
            Item.Click += ToggleNavbar;
            NavbarBackgroundBtn.Click += ToggleNavbar;
            this.Show();

            var token = FileService.Get("token");

            // case when we have token
            if (token != null)
            {
                Email.Text = token.Split(' ')[0];
                Password.Password = token.Split(' ')[1];
                Login(null, null);
            }

            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //string latestVersion = ApplicationService.GetVersion();

            //if (latestVersion != null && !Equals(version, latestVersion))
            //{
            //    UpdateWindow updateWindow = new UpdateWindow();
            //    updateWindow.Show();
            //    this.Close();
            //}
        }

        private void ToggleNavbar(object sender, RoutedEventArgs e)
        {
            if (NavbarShow)
            {
                NavbarBlock.Visibility = Visibility.Visible;
                NavbarBackground.Visibility = Visibility.Visible;
            }
            else
            {
                NavbarBlock.Visibility = Visibility.Collapsed;
                NavbarBackground.Visibility = Visibility.Collapsed;
            }

            NavbarShow ^= true;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Login(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Email.Text == "" || Password.Password == "")
            {
                return;
            }

            if (AuthorizationService.Authorize(Email.Text, Password.Password))
            {
                App.User = AuthorizationService.GetCurrent();
                AuthorizationService.InitOption();
                TaskBlock.Init(lblTime);
                Name.Text = $"{App.User.FirstName} {App.User.SecondName}";
                LoginForm.Visibility = Visibility.Collapsed;
                TrackPanel.Visibility = Visibility.Visible;
                Item.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorText.Visibility = Visibility.Visible;
            }
        }
        private void Exit(object sender, RoutedEventArgs routedEventArgs)
        {
            FileService.Remove("token");
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
