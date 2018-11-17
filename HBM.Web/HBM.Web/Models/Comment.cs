using System.ComponentModel.DataAnnotations;

namespace HBM.Web.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
    }
}