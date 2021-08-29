using AutoMapper;
using BandAPI.Entities;
using BandAPI.Helpers;
using BandAPI.Models;
using BandAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

            // Добавили в HATEOAS
            //var previuosPageLink = bandsFromRepo.HasPrevious ? CreateBandsUri(bandResourceParameters, UriType.PreviousPage) : null;
            //var nextPageLink = bandsFromRepo.HasNext ? CreateBandsUri(bandResourceParameters, UriType.NextPage) : null;

            var metaData = new
            {
                totalCount = bandsFromRepo.TotalCount,
                pageSize = bandsFromRepo.PageSize,
                currenPage = bandsFromRepo.CurrentPage,
                totalPages = bandsFromRepo.TotalPages,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            var links = CreateLinksForBands(bandResourceParameters, bandsFromRepo.HasNext, bandsFromRepo.HasPrevious);
            var shapedBands = _mapper.Map<IEnumerable<BandDto>>(bandsFromRepo).ShapeData(bandResourceParameters.Fields);

            var shapedBandsWithLinks = shapedBands.Select(band =>
            {
                var bandAsDictionary = band as IDictionary<string, object>;
                var bandLinks = CreateLinksForBand((Guid)bandAsDictionary["Id"], null);
                bandAsDictionary.Add("links", bandLinks);
                return bandAsDictionary;
            });

            var linkCollectionResource = new
            {
                value = shapedBandsWithLinks,
                links
            };

            return Ok(linkCollectionResource);

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

            var bandFromRepo = _bandAlbumRepository.GetBand(bandId);

            if (bandFromRepo == null)
            {
                return NotFound();
            }

            var links = CreateLinksForBand(bandId, fields);
            var linkedResourceToReturn = _mapper.Map<BandDto>(bandFromRepo).ShapeData(fields) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateBand")]
        public ActionResult<BandDto> CreateBand([FromBody] BandForCreatingDto band)
        {
            // Проверка, что на входе не null будет автоматом, поэтому не делаем
            // А почему не проверяем, что json корректный?!

            var bandEntity = _mapper.Map<Entities.Band>(band);
            _bandAlbumRepository.AddBand(bandEntity);
            _bandAlbumRepository.Save();

            var bandToReturn = _mapper.Map<BandDto>(bandEntity);

            var links = CreateLinksForBand(bandToReturn.Id, null);
            var linkedResourceToReturn = bandToReturn.ShapeData(null) as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetBand", new { bandId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpOptions]
        public IActionResult GetBandsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, DELETE, HEAD, OPTIONS");
            return Ok();
        }

        [HttpDelete("{bandId}", Name = "DeleteBand")]
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
                case UriType.Current:
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

        private IEnumerable<LinkDto> CreateLinksForBand(Guid bandId, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetBand", new { bandId }),
                    "self",
                    "GET"
                    ));
            }
            else
            {
                links.Add(
                    new LinkDto(Url.Link("GetBand", new { bandId, fields }),
                    "self",
                    "GET"
                    ));
            }

            links.Add(
                new LinkDto(Url.Link("DeleteBand", new { bandId }),
                "delete_band",
                "DELETE"
                ));

            links.Add(
                new LinkDto(Url.Link("CreateAlbumForBand", new { bandId }),
                "create_album_for_band",
                "POST"
                ));
            
            links.Add(
                new LinkDto(Url.Link("GetAlbumsForBand", new { bandId }),
                "albums",
                "GET"
                ));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForBands(BandResourceParameters bandResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(CreateBandsUri(bandResourceParameters, UriType.Current),
                "self",
                "GET"
                ));

            if (hasNext)
            {
                links.Add(
                    new LinkDto(CreateBandsUri(bandResourceParameters, UriType.NextPage),
                    "next_page",
                    "GET"
                    ));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateBandsUri(bandResourceParameters, UriType.PreviousPage),
                    "previous_page",
                    "GET"
                    ));
            }

            return links;
        }
    }
}
