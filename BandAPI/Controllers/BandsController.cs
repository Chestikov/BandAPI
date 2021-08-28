using AutoMapper;
using BandAPI.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using BandAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace BandAPI.Controllers
{
    [ApiController]
    [Route("api/bands")]
    public class BandsController : ControllerBase
    {
        private readonly IBandAlbumRepository _bandAlbumRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyValidationService _propertyValidationService;

        public BandsController(IBandAlbumRepository bandAlbumRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
                                IPropertyValidationService propertyValidationService)
        {
            _bandAlbumRepository = bandAlbumRepository ?? throw new ArgumentNullException(nameof(bandAlbumRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyValidationService = propertyValidationService ?? throw new ArgumentNullException(nameof(propertyValidationService));
        }

        [HttpHead]
        [HttpGet(Name = "GetBands")]
        public IActionResult GetBands([FromQuery] BandResourceParameters bandResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExists<BandDto, Band>(bandResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyValidationService.HasValidProperties<BandDto>(bandResourceParameters.Fields))
            {
                return BadRequest();
            }

            var bandsFromRepo = _bandAlbumRepository.GetBands(bandResourceParameters);

            var previuosPageLink = bandsFromRepo.HasPrevious ? CreateBandsUri(bandResourceParameters, UriType.PreviousPage) : null;
            var nextPageLink = bandsFromRepo.HasNext ? CreateBandsUri(bandResourceParameters, UriType.NextPage) : null;

            var metaData = new
            {
                totalCount = bandsFromRepo.TotalCount,
                pageSize = bandsFromRepo.PageSize,
                currenPage = bandsFromRepo.CurrentPage,
                totalPages = bandsFromRepo.TotalPages,
                previuosPageLink = previuosPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            return Ok(_mapper.Map<IEnumerable<BandDto>>(bandsFromRepo).ShapeData(bandResourceParameters.Fields));

            // Заполнение без автомапера.

            // var bandsDto = new List<BandDto>();

            //foreach (var band in bandsFromRepo)
            //{
            //    bandsDto.Add(new BandDto()
            //    {
            //        Id = band.Id,
            //        Name = band.Name,
            //        MainGenre = band.MainGenre,
            //        FoundedYearsAgo = $"{band.Founded.ToString("yyyy")} ({band.Founded.GetYearsAgo()} years ago)"
            //    });
            //}
        }

        [HttpGet("{bandId}", Name = "GetBand")] // Name, чтобы можно было вызвать из CreateBand в CreatedAtRoute()
        public IActionResult GetBand(Guid bandId, string fields)
        {
            if (!_propertyValidationService.HasValidProperties<BandDto>(fields))
            {
                return BadRequest();
            }

            var bandFronRepo = _bandAlbumRepository.GetBand(bandId);

            if (bandFronRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BandDto>(bandFronRepo).ShapeData(fields));
        }

        [HttpPost]
        public ActionResult<BandDto> CreateBand([FromBody] BandForCreatingDto band)
        {
            // Проверка, что на входе не null будет автоматом, поэтому не делаем
            // А почему не проверяем, что json корректный?!

            var bandEntity = _mapper.Map<Entities.Band>(band);
            _bandAlbumRepository.AddBand(bandEntity);
            _bandAlbumRepository.Save();

            var bandToReturn = _mapper.Map<BandDto>(bandEntity);

            return CreatedAtRoute("GetBand", new { bandId = bandToReturn.Id }, bandToReturn);
        }

        [HttpOptions]
        public IActionResult GetBandsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, DELETE, HEAD, OPTIONS");
            return Ok();
        }

        [HttpDelete("{bandId}")]
        public ActionResult DeleteBand(Guid bandId)
        {
            var bandFromRepo = _bandAlbumRepository.GetBand(bandId);
            if (bandFromRepo == null)
            {
                return NotFound();
            }

            _bandAlbumRepository.DeleteBand(bandFromRepo);
            _bandAlbumRepository.Save();

            return NoContent();
        }

        private string CreateBandsUri(BandResourceParameters bandResourceParameters, UriType uriType)
        {
            switch (uriType)
            {
                case UriType.PreviousPage:
                    return Url.Link("GetBands", new
                    {
                        fields = bandResourceParameters.Fields,
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber - 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
                case UriType.NextPage:
                    return Url.Link("GetBands", new
                    {
                        fields = bandResourceParameters.Fields,
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber + 1,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
                default:
                    return Url.Link("GetBands", new
                    {
                        fields = bandResourceParameters.Fields,
                        orderBy = bandResourceParameters.OrderBy,
                        pageNumber = bandResourceParameters.PageNumber,
                        pageSize = bandResourceParameters.PageSize,
                        mainGenre = bandResourceParameters.MainGenre,
                        searchQuery = bandResourceParameters.SearchQuery
                    });
            }
        }
    }
}
