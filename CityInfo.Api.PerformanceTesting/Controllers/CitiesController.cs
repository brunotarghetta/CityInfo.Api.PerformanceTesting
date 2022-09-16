using AutoMapper;
using CityInfo.Api.Models;
using CityInfo.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.Api.Controllers
{
    [ApiController]
    //[Authorize]
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    //[Route("api/v{version:apiVersion}/cities")]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        const int MAXPAGESIZE = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }

        public ICityInfoRepository _cityInfoRepository { get; }

        private IMapper _mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities([FromQuery]string? name, string? searchQuery, 
            int pageNumber = 1, int pageSize = 10)
        {
            if(pageSize> MAXPAGESIZE)
                pageSize= MAXPAGESIZE;  

            var (citiesEntities, paginationMetadata) =await _cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(citiesEntities));
        }

        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="id">The id of the city to get</param>
        /// <param name="includePointOfInterest">wheather or not to include the points of interes</param>
        /// <returns>an IActionResult</returns>
        /// <response code="200">Return the requested city</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCity(int id, bool includePointOfInterest = false)
        {
            var cityToReturn =await  _cityInfoRepository.GetCityAsync(id, includePointOfInterest);

            if(cityToReturn == null) { return NotFound(); }

            if (includePointOfInterest) { 
                return Ok(_mapper.Map<CityDto>(cityToReturn));    
            }

            return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(cityToReturn));
        }
    }


    //[ApiController]
    ////[Authorize]
    //[ApiVersion("2.0")]
    
    //[Route("api/v{version:apiVersion}/cities")]
    //public class Cities2Controller : ControllerBase
    //{

    //    public Cities2Controller()
    //    {
    //    }


    //    [HttpGet]
    //    public ActionResult<IEnumerable<CityWithoutPointOfInterestDto>> GetCities([FromQuery] string? name, string? searchQuery,
    //        int pageNumber = 1, int pageSize = 10)
    //    {

    //        CityWithoutPointOfInterestDto c = new CityWithoutPointOfInterestDto { Id = 99, Name = "Bruno", Description = "Targhetta" };
    //        List<CityWithoutPointOfInterestDto> citiesEntities = new List<CityWithoutPointOfInterestDto>();

    //        citiesEntities.Add(c);

    //        return Ok(citiesEntities);
    //    }

    //}
   }
