namespace CityInfo.Api.Models
{
    public class CityWithoutPointOfInterestDto
    {
        /// <summary>
        /// The id of the city
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// the name of the city
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// the description of the city
        /// </summary>
        public string? Description { get; set; }
    }
}
