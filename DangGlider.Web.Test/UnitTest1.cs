using System;
using System.Collections.Generic;
using System.Linq;
using DangGlider.Web.Core.Domain;
using NUnit.Framework;

namespace DangGlider.Web.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Test1()
    {
        var flights = GetFlights();

        var reqOrigin = "Little Rock, AR";
        var reqDestination = "Memphis, TN";
        var reqDeparture = DateTime.Parse("1/1/22 9:30");
        var reqArrival = DateTime.Parse("1/1/22 10:00");


    }

    public List<FlightOption> GetFlightOptions(string origin, string destination, DateTime departureTime, DateTime arrivalTime)
    {
        var flights = GetFlights();
        var flightOptions = new List<FlightOption>();

        var flightsPickingUpInTime = flights.Where(f => f.Origin == origin && f.DepartureTime <= departureTime).ToList();
        var followingFlights = flights.Where(f => f.Origin != origin && f.DepartureTime > departureTime).ToList();
        foreach(var pickingUpFlight in flightsPickingUpInTime)
        {
            var flightOption = GetOption(followingFlights, pickingUpFlight, destination, arrivalTime);
            if (flightOption != null)
            {
                flightOptions.Add(flightOption);
            }
        }

        return flightOptions;
    }

    public FlightOption GetOption(List<TestableFlight> flights, TestableFlight flightPickingUp, string destination, DateTime arrivalTime)
    {
        // bubble up ON -> 1) found a flight that hits our destination 2) no more options
        // keep digging ON -> 1) found a flight that leaves our pick up
    }

    public List<TestableFlight> GetFlights()
    {
        return new List<TestableFlight>
        {
            new TestableFlight
            {
                Id = 1,
                DepartureTime = DateTime.Parse("1/1/22 08:00"),
                ArrivalTime = DateTime.Parse("1/1/22 10:00"),
                Origin = "Little Rock, AR",
                Destination = "Memphis, TN"
            },
            new TestableFlight
            {
                Id = 2,
                DepartureTime = DateTime.Parse("1/1/22 11:00"),
                ArrivalTime = DateTime.Parse("1/1/22 13:30"),
                Origin = "Memphis, TN",
                Destination = "Chicago, IL"
            },
            new TestableFlight
            {
                Id = 3,
                DepartureTime = DateTime.Parse("1/1/22 10:30"),
                ArrivalTime = DateTime.Parse("1/1/22 12:00"),
                Origin = "Memphis, TN",
                Destination = "Dallas, TX"
            },
            new TestableFlight
            {
                Id = 4,
                DepartureTime = DateTime.Parse("1/1/22 10:30"),
                ArrivalTime = DateTime.Parse("1/1/22 12:00"),
                Origin = "Memphis, TN",
                Destination = "Atlanta, GA"
            },
            new TestableFlight
            {
                Id = 5,
                DepartureTime = DateTime.Parse("1/1/22 12:30"),
                ArrivalTime = DateTime.Parse("1/1/22 14:00"),
                Origin = "Atlanta, GA",
                Destination = "Chicago, IL"
            },
        };
    }
    
}
