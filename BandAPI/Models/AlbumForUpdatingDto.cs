using System.ComponentModel.DataAnnotations;

namespace BandAPI.Models
{
    public class AlbumForUpdatingDto : AlbumManipulationDto
    {
        [Required(ErrorMessage = "You should fill description")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
