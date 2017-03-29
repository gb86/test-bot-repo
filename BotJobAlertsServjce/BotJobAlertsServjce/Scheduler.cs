using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BotJobAlertsServjce
{
    public class Scheduler
    {
        private static bool turnOn = bool.Parse(ConfigurationManager.AppSettings["oneMinuteAlerts"]);

        public static void Start()
        {
            IJobDetail dailyJob = JobBuilder.Create<IDailyAlertsJob>().Build();
            IJobDetail weeklyJob = JobBuilder.Create<IWeeklyAlertsJob>().Build();
            IJobDetail hourlyJob = JobBuilder.Create<IHourlyAlertsJob>().Build();
            IJobDetail twoMinuteJob = JobBuilder.Create<ITwoMinuteAlertsJob>().Build();

            ITrigger dailyTrigger = TriggerBuilder.Create()
               .WithCronSchedule("0 0 10 ? * MON-FRI *")
               .StartAt(DateTime.UtcNow)
               .WithPriority(1)
               .Build();

            ITrigger weeklyTrigger = TriggerBuilder.Create()
               .WithCronSchedule("0 0 12 ? * THU *")
               .StartAt(DateTime.UtcNow)
               .WithPriority(1)
               .Build();

            ITrigger hourlyTrigger = TriggerBuilder.Create()
               .WithCronSchedule("0 0 0/1 1/1 * ? *")
               .StartAt(DateTime.UtcNow)
               .WithPriority(1)
               .Build();

            ITrigger twoMinuteTrigger = TriggerBuilder.Create()
               .WithCronSchedule("0 0/1 * 1/1 * ? *")
               .StartAt(DateTime.UtcNow)
               .WithPriority(1)
               .Build();

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            scheduler.ScheduleJob(dailyJob, dailyTrigger);
            scheduler.ScheduleJob(weeklyJob, weeklyTrigger);
            scheduler.ScheduleJob(hourlyJob, hourlyTrigger);
            
            if(turnOn)
            {
                scheduler.ScheduleJob(twoMinuteJob, twoMinuteTrigger);
            }
        }
    }

    public class IDailyAlertsJob : IJob
    {
        private Client client = new Client();

        public void Execute(IJobExecutionContext context)
        {
            var alertsToSend = DocumentDBHelper<SavedSearch>.GetItemsAsync(x => x.Frequency == 1).Result;

            if(alertsToSend != null)
            {
                foreach(var alert in alertsToSend)
                {
                    client.MakeRequest(alert.ResumptionCookie, alert.Criteria);
                }
            }
        }
    }

    public class IWeeklyAlertsJob : IJob
    {
        private Client client = new Client();

        public void Execute(IJobExecutionContext context)
        {
            var alertsToSend = DocumentDBHelper<SavedSearch>.GetItemsAsync(x => x.Frequency == 2).Result;

            if (alertsToSend != null)
            {
                foreach (var alert in alertsToSend)
                {
                    client.MakeRequest(alert.ResumptionCookie, alert.Criteria);
                }
            }
        }
    }

    public class IHourlyAlertsJob : IJob
    {
        private Client client = new Client();

        public void Execute(IJobExecutionContext context)
        {
            var alertsToSend = DocumentDBHelper<SavedSearch>.GetItemsAsync(x => x.Frequency == 3).Result;

            if (alertsToSend != null)
            {
                foreach (var alert in alertsToSend)
                {
                    client.MakeRequest(alert.ResumptionCookie, alert.Criteria);
                }
            }
        }
    }

    public class ITwoMinuteAlertsJob : IJob
    {
        private Client client = new Client();

        public void Execute(IJobExecutionContext context)
        {
            var alertsToSend = DocumentDBHelper<SavedSearch>.GetItemsAsync(x => x.Frequency == 4).Result;

            if (alertsToSend != null)
            {
                foreach (var alert in alertsToSend)
                {
                    client.MakeRequest(alert.ResumptionCookie, alert.Criteria);
                }
            }
        }
    }

    public class Client
    {
        private HttpClient _client;
        private static readonly string _host = ConfigurationManager.AppSettings["botHost"];

        public Client()
        {
            _client = new HttpClient();
        }

        public bool MakeRequest(string resumationCookie, string criteria)
        {
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Criteria", criteria),
                new KeyValuePair<string, string>("ResumptionCookie", resumationCookie)
            };

            var encodedData = new FormUrlEncodedContent(formData);

            var resp = _client.PostAsync($"http://{_host}/api/savesearch", encodedData).Result;

            return resp.IsSuccessStatusCode;
        }
    }
}
