using AutoMapper;

namespace BandAPI.Profiles
{
    public class AlbumsProfile : Profile
    {
        public AlbumsProfile()
        {
            CreateMap<Entities.Album, Models.AlbumsDto>().ReverseMap();
            CreateMap<Models.AlbumForCreatingDto, Entities.Album>();
            CreateMap<Models.AlbumForUpdatingDto, Entities.Album>().ReverseMap();
        }
    }
}
