using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Models
{
    public class HomeViewModel
    {
        public int TotalCompanies { get; set; } 
        public int TotalUsers { get; set; } 
        public int TotalCondominiums { get; set; } 
        public int TotalActivities { get; set; } 
        public int NewUsersCount { get; set; } 

      
        public List<string> ActivityMonths { get; set; } = new List<string>(); 
        public List<int> ActivityCounts { get; set; } = new List<int>(); 

      
        public List<(string Title, DateTime Start, DateTime? End, string BackgroundColor, string BorderColor, bool AllDay)> CalendarEvents { get; set; } = new List<(string, DateTime, DateTime?, string, string, bool)>(); 


        public List<(string Name, double Latitude, double Longitude)> CondominiumLocations { get; set; } = new List<(string, double, double)>(); 

     
        public int WeatherTemperature { get; set; } 
        public string WeatherLocation { get; set; } = string.Empty; 
        public string WeatherCondition { get; set; } = string.Empty;

        public string? WelcomeMessage { get; set; }
        public bool RegistrationEnabled { get; set; }
        public bool LoginEnabled { get; set; }


        public List<(string FullName, string ProfileImageUrl, DateTime? LastActivity)> RecentUsers { get; set; } = new List<(string, string, DateTime?)>();

       
        public List<(string Title, DateTime Date)> RecentNews { get; set; } = new List<(string, DateTime)>(); 

     
        public List<(string AuthorName, string Content, DateTime CreatedAt)> ForumPosts { get; set; } = new List<(string, string, DateTime)>();
    }
}
