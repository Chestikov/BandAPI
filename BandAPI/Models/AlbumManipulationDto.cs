using BandAPI.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace BandAPI.Models
{
    [TitleAndDescription(ErrorMessage = "Title must be different from description")] // переопределяется сообщением из TitleAndDescriptionAttribute
    public abstract class AlbumManipulationDto
    {
        [Required(ErrorMessage = "Title needs to be filled in")] // переопределяет стандартное сообщение
        [MaxLength(200, ErrorMessage = "Title needs to be up to 200 characters")]
        public string Title { get; set; }

        [MaxLength(400)]
        public virtual string Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult("The title and description need to be different", new[] { "AlbumForCreatingDto" });
        //    }
        //}
    }
}
