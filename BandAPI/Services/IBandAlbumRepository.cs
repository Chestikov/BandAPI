using BandAPI.Entities;
using BandAPI.Helpers;
using System;
using System.Collections.Generic;

namespace BandAPI.Services
{
    public interface IBandAlbumRepository
    {
        IEnumerable<Album> GetAlbums(Guid bandId);
        Album GetAlbum(Guid bandId, Guid albumId);
        void AddAlbum(Guid bandId, Album album);
        void UpdateAlbum(Album album);
        void DeleteAlbum(Album album);

        IEnumerable<Band> GetBands();
        Band GetBand(Guid bandId);
        IEnumerable<Band> GetBands(IEnumerable<Guid> bandIds);
        PagedList<Band> GetBands(BandResourceParameters bandResourceParameters);
        void AddBand(Band band);
        void UpdateBand(Band band);
        void DeleteBand(Band band);
        
        bool BandExists(Guid bandId);
        bool AlbumExists(Guid AlbumId);
        bool Save();




    }
}
