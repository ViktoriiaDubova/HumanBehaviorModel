using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBM.Web.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        [Index("IX_TagKey", IsUnique = true)]
        [Required, StringLength(16, MinimumLength = 2)]
        public string Key { get; set; }
    }
}