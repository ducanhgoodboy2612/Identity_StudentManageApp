namespace TokenCheck.Entities
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int ClassID { get; set; }
        public string DayOfWeek { get; set; } // Monday, Tuesday, ...
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Room { get; set; }

        public Class Class { get; set; }
    }

}
