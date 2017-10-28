using System;

namespace Web.Models
{
    public class CounterState
    {
        public int? Count { get; set; } 
        
        public DateTimeOffset CreatedAt { get; set; }
    }
}
