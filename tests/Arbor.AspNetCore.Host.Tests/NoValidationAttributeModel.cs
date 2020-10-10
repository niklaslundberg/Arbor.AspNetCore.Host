using System.ComponentModel.DataAnnotations;
using Arbor.AspNetCore.Host.Validation;

namespace Arbor.AspNetCore.Host.Tests
{
    [NoValidation]
    public class NoValidationAttributeModel
    {
        [Required]
        public string? Name { get; set; }
    }
}