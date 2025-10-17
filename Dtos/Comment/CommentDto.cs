using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty; 

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string Createdby { get; set; } = string.Empty;//this was added in order to show to the user who has created the comment 

        public int? StockId { get; set; }
//navigation we erase this because with only the 
        // public Stock? Stock { get; set; }
    }
}