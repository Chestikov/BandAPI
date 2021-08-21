using AutoMapper;
using BandAPI.Models;

namespace BandAPI.Profiles
{
    public class AlbumsProfile : Profile
    {
        public AlbumsProfile()
        {
            CreateMap<Entities.Album, Models.AlbumsDto>().ReverseMap();

            CreateMap<AlbumForCreatingDto, Entities.Album>();
        }
    }
}
