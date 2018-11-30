using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using HBM.Web.Extensions;

namespace HBM.Web.Models
{
    public class Tag
    {
        public const string Pattern = @"^\w+[\w_]*$";

        [Key]
        public int Id { get; set; }
        [Index("IX_TagKey", IsUnique = true)]
        [Required, StringLength(16, MinimumLength = 2)]
        public string Key { get; set; }

        public virtual ICollection<Article> Articles { get; set; }

        public static string ToString(IEnumerable<Tag> tags) => string.Join(", ", tags.Select(t => t.Key));
        public static IEnumerable<TagCreationResult> CreateTags(string line)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                var splited = line.Split(',');
                splited.ForAll(item => item.Trim());
                for (int i = 0; i < splited.Length; i++)
                {
                    var str = splited[i];
                    if (splited.Count(el => el == str) > 1)
                    {
                        yield return TagCreationResult.Error($"Duplicate tag entry '{str}'");
                        continue;
                    }
                    if (!Regex.IsMatch(str, Pattern))
                    {
                        yield return TagCreationResult.Error($"Invalid tag format for '{str}'");
                        continue;
                    }
                    
                    yield return TagCreationResult.Success(str);
                }
            }
        }
    }
    public struct TagCreationResult
    {
        public readonly string Value;
        public readonly string ErrorMessage;

        private TagCreationResult(string value, string error)
        {
            Value = value;
            ErrorMessage = error;
        }

        public static TagCreationResult Error(string error) => new TagCreationResult(null, error);
        public static TagCreationResult Success(string value) => new TagCreationResult(value, null);
    }
}