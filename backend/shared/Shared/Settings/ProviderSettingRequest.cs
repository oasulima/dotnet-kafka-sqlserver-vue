using System.ComponentModel.DataAnnotations;

namespace Shared.Settings
{
    public class ProviderSettingRequest
    {
        [Required]
        [MinLength(1), MaxLength(150)]
        public string ProviderId { get; set; }

        [Required]
        [MinLength(1), MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0, 0.9999, ErrorMessage = "Should be more or equal to 0 and less than 1")]
        public decimal? Discount { get; set; }

        [Required] public bool? Active { get; set; }
    }
}