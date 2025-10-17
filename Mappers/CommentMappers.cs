using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMappers
    {

        public static CommentDto ToCommentDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Title = comment.Title,
                Content = comment.Content,
                CreatedOn = comment.CreatedOn,
                Createdby = comment.appUser.UserName,
                StockId = comment.StockId
                
            };

        }

        public static Comment ToCommentFromCreate(this CreateCommentRequestDto createCommentRequest, int stockId, string appUserId)
        {
            return new Comment
            {
                Title = createCommentRequest.Title,
                Content = createCommentRequest.Content,
                StockId = stockId,
                AppUserId = appUserId
            };
        }

        public static Comment ToCommentFromUpdate(this UpdateCommentDto updateComment)
        {
            return new Comment
            {
                Title = updateComment.Title,
                Content = updateComment.Content,
            };
        }
    }
}