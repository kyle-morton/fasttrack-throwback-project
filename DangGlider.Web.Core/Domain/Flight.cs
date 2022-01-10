namespace DangGlider.Web.Core.Domain
{
    public class Flight : EntityBase
    {
        public int FlightNumber { get; set; }
        public int OriginId { get; set; }
        public virtual GeoCode Origin { get; set; }
        public int DestinationId { get; set; }
        public virtual GeoCode Destination { get; set; }
        public DateTime ScheduledDeparture { get; set; }
        public DateTime ScheduledArrival { get; set; }
        public bool HasDeparted { get; set; }
        public bool HasArrived { get; set; }
    }
}
