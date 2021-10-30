using System;
using System.Collections.Generic;

namespace Domain
{
    public enum ActivityDateSort
    {
        Asc,
        Desc,
    }
    
    public class ActivityFiltersDto
    {
        public ActivityDateSort DateSort { get; set; }
        public string Title { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Cities { get; set; } = new List<string>();
        public bool? IsMy { get; set; }
        public bool? Attending { get; set; }
    }
}