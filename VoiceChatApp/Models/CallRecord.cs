using System;
using System.ComponentModel.DataAnnotations;

namespace VoiceChatApp.Models
{
    public class CallRecord
    {
        [Key]
        public int Id { get; set; }

        public string User { get; set; }
        public DateTime CallDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public TimeSpan? Duration { get; set; }

    }
}
