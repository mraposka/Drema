using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Drema.Resources
{
    public class Singleton
    { 
        public bool tokenUpdated=false;
        public string CalculateSHA256Hash(string input)
        { 
            using (SHA256 sha256 = SHA256.Create())
            { 
                byte[] bytes = Encoding.UTF8.GetBytes(input); 
                byte[] hash = sha256.ComputeHash(bytes); 
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                } 
                return sb.ToString().ToLower();
            }
        }
        public string GetCallForExceptionThisMethod(MethodBase methodBase, Exception e)
        {
            StackTrace trace = new StackTrace(e);
            StackFrame previousFrame = null;

            foreach (StackFrame frame in trace.GetFrames())
            {
                if (frame.GetMethod() == methodBase) 
                    break; 
                previousFrame = frame;
            } 
            return previousFrame != null ? previousFrame.GetMethod().Name : null;
        }
        public string GetIstanbulTimeFormatted()
        {
            try
            { 
                string timeServerUrl = "http://worldtimeapi.org/api/timezone/Europe/Istanbul.txt"; 
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    string response = client.DownloadString(timeServerUrl);
                    string[] lines = response.Split('\n'); 
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("datetime"))
                        {
                            string[] parts = line.Split(' ');
                            if (parts.Length >= 2)
                            {
                                string dateTimeString = parts[1].Trim();
                                DateTime istanbulTime = DateTime.Parse(dateTimeString); 
                                string formattedDateTime = istanbulTime.ToString("dd.MM.yyyy HH:mm:ss");
                                return formattedDateTime;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
            } 
            return "01.01.1900 00:00:00";
        }
    }
}
