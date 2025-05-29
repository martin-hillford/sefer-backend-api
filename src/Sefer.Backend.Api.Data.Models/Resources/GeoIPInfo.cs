// This file acts as a model and may not be initialized in code.
//
// ReSharper disable InconsistentNaming
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Sefer.Backend.Api.Data.Models.Resources;

public class GeoIPInfo
{
    /// <summary>
    /// The unique id for this lookup
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The date the information was retrieved
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// The ip address found
    /// </summary>
    [MaxLength(52)]
    public string IpAddress { get; set; }

    /// <summary>
    /// The name of the country where the IP address is located.
    /// </summary>
    [MaxLength(255)]
    public string CountryName { get; set; }

    /// <summary>
    /// The ISO 3166-1 alpha-2 country code of the IP address location.
    /// </summary>
    [MaxLength(10)]
    public string CountryCode { get; set; }

    /// <summary>
    /// The name of the city where the IP address is located.
    /// </summary>
    [MaxLength(255)]
    public string City { get; set; }

    /// <summary>
    /// The name of the region or state where the IP address is located.
    /// </summary>
    [MaxLength(255)]
    public string Region { get; set; }

    /// <summary>
    /// The code of the region or state where the IP address is located.
    /// </summary>
    [MaxLength(255)]
    public string RegionCode { get; set; }

    /// <summary>
    /// The name of the continent where the IP address is located.
    /// </summary>
    [MaxLength(50)]
    public string Continent { get; set; }

    /// <summary>
    /// The latitude coordinate of the IP address location.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// The longitude coordinate of the IP address location.
    /// </summary>
    public double? Longitude { get; set; }
}