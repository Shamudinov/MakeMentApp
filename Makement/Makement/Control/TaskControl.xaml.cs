using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;
using Makement.Model;
using Makement.Enum;
using Makement.Tracker;
using Makement.Service;

namespace Makement.Control
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
        private int currentActiveId = -1;// -1 mean no have active task
        private Label lblTime;

        public TaskControl()
        {
            InitializeComponent();
        }

        private void InitPanel(IEnumerable<TaskModel> list, bool withAppToggle)
        {
            foreach (var x in list)
            {
                var panel = new WrapPanel();
                panel.Margin = new Thickness(0, 0, 0, 20);

                var txt = new TextBlock();
                txt.Text = x.Text;
                txt.FontFamily = new FontFamily("Arial");
                txt.Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100));
                txt.TextWrapping = TextWrapping.WrapWithOverflow;
                txt.Margin = new Thickness(0, 0, 10, 0);
                txt.Width = 200;

                panel.Children.Add(txt);

                var btn = new Button();
                btn.Width = 50;
                btn.Height = 20;
                btn.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                btn.Margin = new Thickness(0, 0, 5, 0);

                var btnFinish = new Button();
                btnFinish.Width = 50;
                btnFinish.Height = 20;
                btnFinish.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                btnFinish.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));

                if (x.Status == TaskStatusEnum.NonActive)
                {
                    btn.Content = "Start";
                    btn.Background = new SolidColorBrush(Color.FromRgb(30, 144, 255));

                    btnFinish.Content = "Finish";
                    btnFinish.Visibility = Visibility.Collapsed;

                    btn.Click += (s, e) => { HandleBtn(btn, btnFinish, x.Id); };
                    btnFinish.Click += (s, e) => { Finish(btnFinish, x.Id); };

                    panel.Children.Add(btn);
                    panel.Children.Add(btnFinish);
                }
                else if (x.Status == TaskStatusEnum.Active)
                {
                    btn.Content = "Stop";
                    btn.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    btnFinish.Content = "Finish";

                    btn.Click += (s, e) => { HandleBtn(btn, btnFinish, x.Id); };
                    panel.Children.Add(btn);

                    btnFinish.Click += (s, e) => { Finish(btnFinish, x.Id); };
                    panel.Children.Add(btnFinish);

                    HandleBtn(btn, btnFinish, x.Id, withAppToggle);
                }

                Tasks.Children.Add(panel);
            }
        }

        public void Init(Label lblTime)
        {
            var list = GetTasks();
            this.lblTime = lblTime;
            Tasks.Children.Clear();

            InitPanel(list, true);

            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += (s, e) => { Load(lblTime); };

            timer.Start();
        }

        private void Load(Label lblTime)
        {
            var list = GetTasks();

            this.lblTime = lblTime;
            Tasks.Children.Clear();

            //Update data has active task
            if (list.Any(x => x.Status == TaskStatusEnum.Active))
            {
                // current data has not active task
                if (currentActiveId == -1)
                {
                    InitPanel(list, true);
                }
                else
                {
                    currentActiveId = -1;
                    InitPanel(list, false);
                }
            }
            else
            {
                // current data has not active task
                if (currentActiveId == -1)
                {
                    InitPanel(list, true);
                }
                else
                {
                    currentActiveId = -1;
                    AppToggle();
                    InitPanel(list, true);
                }
            }
        }

        private void HandleBtn(Button btn, Button btnFinish, int id, bool withAppToggle = true)
        {
            Debug.WriteLine(id);

            if (!InternetTrack.IsConnectedToInternet())
                return;

            // Activate
            if (currentActiveId == -1)
            {
                currentActiveId = id;
                ChangeStatus(id, TaskStatusEnum.Active);
                StartInit(id);
                btn.Content = "Stop";
                btn.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                btnFinish.Visibility = Visibility.Visible;

                if (withAppToggle)
                    AppToggle();
            }
            // Stop
            else if (currentActiveId == id)
            {
                currentActiveId = -1;
                btnFinish.Visibility = Visibility.Collapsed;
                ChangeStatus(id, TaskStatusEnum.NonActive);
                btn.Content = "Start";
                btn.Background = new SolidColorBrush(Color.FromRgb(30, 144, 255));

                if (withAppToggle)
                    AppToggle();
            }
        }

        private void Finish(Button btn, int id)
        {
            currentActiveId = -1;
            ChangeStatus(id, TaskStatusEnum.Done);
            AppToggle();
            Load(lblTime);
        }

        public void AppToggle()
        {
            if (!App.IsRunning)
            {
                App.BeginTime = DateTime.Now;
                TimerService.Begin(lblTime);

                AppUsageService.Begin();
                ActiveCheckService.Begin();
                ScreenCaptureService.Begin();
                LocalSaverService.Begin();
                ServerSaverService.Begin();
            }
            else
            {
                AppUsageService.Stop();
                TimerService.Stop();
                ActiveCheckService.Stop();
                ScreenCaptureService.Stop();
            }

            App.IsRunning ^= true;
        }

        public void StartInit(int id)
        {
            if (!InternetTrack.IsConnectedToInternet())
                return;

            var response = App.HttpClient.GetAsync(App.BaseUrl + "Task/GetPeriodsByTaskId?taskId=" + id.ToString()).Result;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                JObject obj = JObject.Parse(json);
                var BeginTime = Convert.ToDateTime(obj["beginTime"]);
                var totalSecond = Int64.Parse(obj["totalTime"].ToString()) / 1000;
                DateTime now = DateTime.Now.ToUniversalTime().AddHours(6);
                DateTime begin = BeginTime;
                App.WorkSecond = Convert.ToInt64((now - begin).TotalSeconds) + totalSecond;

                if (App.WorkSecond < 0)
                    App.WorkSecond = 0;

                Debug.WriteLine($"NOW {now.Hour}:{now.Minute}:{now.Second} BEGIN {begin.Hour}:{begin.Minute}:{begin.Second} Total {totalSecond}");
            }
        }

        public void ChangeStatus(int id, TaskStatusEnum status)
        {
            var data = new
            {
                id = id,
                status = status
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            App.HttpClient.PostAsync(App.BaseUrl + "Task/ChangeStatus", content).Wait();
        }

        public IEnumerable<TaskModel> GetTasks()
        {
            var response = App.HttpClient.GetAsync(App.BaseUrl + "Task/GetTasksByToken").Result;
            var list = new List<TaskModel>();

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                JArray array = JArray.Parse(json);

                foreach (var s in array)
                {
                    list.Add(new TaskModel
                    {
                        Id = (int)(s["id"]),
                        Text = (string)(s["text"]),
                        Status = (TaskStatusEnum)int.Parse(s["status"].ToString())
                    });
                }
            }

            return list;
        }
    }
}
