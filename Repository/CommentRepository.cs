using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.Include( x => x.appUser).ToListAsync();//Include allow us to bring the data from the other relationed table in this case AppUser
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {

            // var commentModel = await _context.Comments.FindAsync(id);
            var commentModel = await _context.Comments.Include(x => x.appUser).FirstOrDefaultAsync( x => x.Id == id);//Include allow us to bring the data from the other relationed table in this case Appuser table data
            if (commentModel == null)
            {
                return null;
            }

            return commentModel;

        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {

            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();

            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (commentModel == null)
            {
                return null;
            }
            _context.Comments.Remove(commentModel);

            await _context.SaveChangesAsync();

            return commentModel;
        }

        public async Task<Comment?> UpdateAsync(int id, Comment commentModel)
        {
            // var commentx = await _context.Comments.FindAsync(id);
            var commentx = await _context.Comments.Include(x => x.appUser).FirstOrDefaultAsync(x => x.Id == id);//was required to add the includesince we have the CommentDTO modified and access to a nest value in AppUser
            if (commentx == null)
            {
                return null;
            }

            commentx.Content = commentModel.Content;
            commentx.Title = commentModel.Title;

            await _context.SaveChangesAsync();

            return commentx;
        }

    }
}