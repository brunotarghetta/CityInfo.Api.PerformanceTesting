using AutoMapper;
using CityInfo.Api.Models;
using CityInfo.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.Api.Controllers
{
    //[Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    //[ApiVersion("2.0")]
    //[Authorize(Policy = "MustBeFromAntwerp")]
    public class PointsOfInterestController : ControllerBase
    {
        public ILogger<PointsOfInterestController> _logger { get; }
        public IMailService _localMailService { get; }

        private ICityInfoRepository _cityInfoRepository;       
        private IMapper _mapper;
        

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService localMailService,ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger;
            _localMailService = localMailService;
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterest(int cityId)
        {

            try
            {

                //var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

                //if (!await _cityInfoRepository.CityNameMatchCityId(cityName, cityId))
                //{
                //    return Forbid();
                //}
                

                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"City with id: {cityId} wasn't found");
                    return NotFound();
                }

                var pointOfInterestForCity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId);

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity));

            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting point of interes with city id {cityId}", ex);

                return StatusCode(StatusCodes.Status500InternalServerError, "A problem happened while handling the request.");
            }
        }


        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound();
            }



            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if(pointOfInterest == null) return NotFound();

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInteres(int cityId, [FromBody] PointOfInterestForCrationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound();
            }


            var finalPointOfIneterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfIneterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfIneterest);  

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, pointOfInterestId = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
        }

        [HttpPut("{pointOfInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfIneterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) return NotFound();

            //source to destination .. override values on destination
            _mapper.Map(pointOfIneterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{pointOfInterestId}")]
        public async Task<ActionResult> PartialUpdatePointOfInerest(
            int cityId, int pointOfInterestId, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument
            )
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound();
            }


            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) return NotFound();


            var pointOfIneterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfIneterestToPatch, ModelState);

            if (!ModelState.IsValid) return BadRequest();

            _mapper.Map(pointOfIneterestToPatch, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{pointOfInterestId}")]
        public async Task<ActionResult> DeletePointOfInerest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound();
            }
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) return NotFound();

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            _localMailService.Send("Point of interest deleted", $"Point of interes: {pointOfInterestEntity.Name} with id: {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
