using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJobAlertsServjce
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
            Scheduler.Start();
        }

        private static void Init()
        {
            DocumentDBHelper<SavedSearch>.Initialize();
        }
    }

    public class Response
    {
        public string ResumptionCookie { get; set; }
        public string Criteria { get; set; }
    }

    [Serializable]
    public class SavedSearch
    {
        public string Id { get; set; }
        public string Criteria { get; set; }
        public string ResumptionCookie { get; set; }
        [JsonProperty("frequency")]
        public int Frequency { get; set; }
    }
}
