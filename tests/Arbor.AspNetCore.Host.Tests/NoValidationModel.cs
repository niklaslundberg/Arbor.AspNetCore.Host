using System.ComponentModel.DataAnnotations;

namespace Arbor.AspNetCore.Host.Tests
{
    public class NoValidationModel
    {
        [Required]
        public string? Name { get; set; }
    }
}