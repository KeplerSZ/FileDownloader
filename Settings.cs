using System;
using System.Collections.Generic;
using System.Text;

namespace FileDownloader
{
    public class Settings
    {
        public ConsultantSettings ConsultantSettings { get; set; }
        public DownloadSettings DownloadSettings { get; set; }
    }

    public class ConsultantSettings
    {
        public string fileurl { get; set; }
        public string downloadBaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TestUrl { get; set; }
    }

    public class DownloadSettings
    {
        public string DownloadFolder { get; set; }
        public string LogFolder { get; set; }
    }
}
