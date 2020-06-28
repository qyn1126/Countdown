using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 倒计时wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime DayDate;
        private DateTime WeekDate;
        private Dictionary<string, int> Holliday = new Dictionary<string, int>();
        public MainWindow()
        {
            InitializeComponent();
            DayDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd " + "17:30:00"));
            WeekDate = DateTime.Now.AddDays(-1);
            GetHoliday(DateTime.Now.ToString("yyyy-MM-dd"));

            while (true){
                WeekDate = WeekDate.AddDays(1);
                if (Holliday.ContainsKey(WeekDate.ToString("yyyy-M-d"))&&Holliday[WeekDate.ToString("yyyy-M-d")] == 1)
                {
                    break;
                }
                if (Holliday.ContainsKey(WeekDate.ToString("yyyy-M-d")) && Holliday[WeekDate.ToString("yyyy-M-d")] == 2)
                {

                    continue;
                }
                if ((((int)WeekDate.DayOfWeek) % 6) == 0)
                {
                    break;
                }
               
            }
            WeekDate = WeekDate.AddDays(-1);
            WeekDate = Convert.ToDateTime(WeekDate.ToString("yyyy-MM-dd " + "17:30:00"));
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 1000;//执行间隔时间,单位为毫秒    
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(settime);
        }
        private void settime(object sender, System.Timers.ElapsedEventArgs e)
        {


            this.BlockText.Dispatcher.Invoke(() =>
            {
                var time = DateTime.Now;

                if (time < DayDate)
                {
                    TimeSpan ts = DayDate.Subtract(time).Duration();
                    this.BlockText.Text = $"距离下班还有{Math.Ceiling(ts.TotalSeconds).ToString()}秒";
                }
                else
                {
                    this.BlockText.Text = "下班!!!!!";
                }
                if (time < WeekDate)
                {
                    TimeSpan ts = WeekDate.Subtract(time).Duration();
                    this.BlockText1.Text = $"距离放假还有{Math.Ceiling(ts.TotalMinutes).ToString()}分钟";
                }
                else
                {
                    this.BlockText1.Text = "放假!!!!!";
                }

            });
          


        }
        private  void GetHoliday(string date)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var url = $"https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?query={date}&resource_id=6018";
            var jsondata = client.DownloadString(url);
            try
            {
                var model = JsonConvert.DeserializeObject<Calendar>(jsondata);
                foreach (var item in model.data)
                {
                    foreach (var holiday in item.holiday)
                    {
                        foreach (var day in holiday.list)
                        {
                            Holliday[day.date] = day.status;
                        }
                    }
                }
            }
            catch (Exception)
            {
                var model = JsonConvert.DeserializeObject<Calendar2>(jsondata);
                foreach (var item in model.data)
                {
                    if (item.holiday != null && item.holiday.list != null)
                    {
                        foreach (var day in item.holiday.list)
                        {
                            Console.WriteLine($"日期：{day.date}，{day.remark}");
                        }
                    }
                }
            }

        }
        public class Calendar
        {
            public int status { get; set; }
            public List<CalendarData> data { get; set; }
        }
        public class CalendarData
        {
            //public Holiday holiday { get; set; }
            public List<Holiday> holiday { get; set; }
        }
        public class Holiday
        {
            public string desc { get; set; }
            public string festival { get; set; }
            public List<HolidayList> list { get; set; }
            public string name { get; set; }
            public string rest { get; set; }
        }
        public class HolidayList
        {
            public string date { get; set; }
            /// <summary>
                    /// 1休息2上班
                    /// </summary>
            public int status { get; set; }
            public string remark
            {
                get
                {
                    return status == 1 ? "休假" : "上班";
                }
            }
        }
        public class Calendar2
        {
            public int status { get; set; }
            public List<CalendarData2> data { get; set; }
        }
        public class CalendarData2
        {
            public Holiday holiday { get; set; }
        }

    }
}
