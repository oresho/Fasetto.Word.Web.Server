using System;
using System.ComponentModel.DataAnnotations;

namespace Fasetto.Word.Web.Server.Data
{
    public class SettingsDataModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        [MaxLength(2048)]
        public string Value { get; set; }
    }
}