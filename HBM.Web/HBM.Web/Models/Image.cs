using System.ComponentModel.DataAnnotations;

namespace HBM.Web.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public string Path { get; set; }
    }
}