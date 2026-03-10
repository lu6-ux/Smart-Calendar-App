using System;

namespace Smart_Calendar_App.Models
{
    public class RecurringEvent
    {
        public int RecurringID { get; set; }         
        public int EventID { get; set; }             
        public string RecurrenceType { get; set; }   
        public DateTime? RecurrenceEndDate { get; set; }  
        public int? RecurrenceCount { get; set; }         
        public DateTime CreatedAt { get; set; }      
    }
}

