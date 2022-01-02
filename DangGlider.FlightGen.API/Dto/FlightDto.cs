namespace DangGlider.FlightGen.API.Dto
{
    public class FlightDto
    {
        public int Id { get; set; }
        public GeoCodeDto Origin { get; set; }
        public GeoCodeDto Destination { get; set; }
        public DateTime ScheduledDeparture { get; set; }
        public DateTime ScheduledArrival { get; set; }
        public bool HasDeparted { get; set; }
        public bool HasArrived { get; set; }
    }
}
