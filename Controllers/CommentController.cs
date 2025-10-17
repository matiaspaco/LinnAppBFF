using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        //private readonly CommentRepository _commentRepository;

        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;

        private readonly UserManager<AppUser> _userManager;

        public CommentController(ApplicationDBContext context, ICommentRepository commentRepository, IStockRepository stockRepository, UserManager<AppUser> appUser)
        {
            _context = context;
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _userManager = appUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            if (!ModelState.IsValid)//Controller Base provide the modelState and it has the porpuse of verify if all the Data Anotations Validations are ok or not 
            {
                return BadRequest(ModelState);
            }
            var commentModel = await _commentRepository.GetAllAsync();
            if (commentModel == null)
            {
                //return null;
                return NotFound();
            }
            var commentDto = commentModel.Select(x => x.ToCommentDto());
            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]

        public async Task<IActionResult> GetById(int id)
        {
            var commentModel = await _commentRepository.GetByIdAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }
            return Ok(commentModel.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]

        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentRequestDto createComment)
        {

            if (!ModelState.IsValid)//Controller Base provide the modelState and it has the porpuse of verify if all the Data Anotations Validations are ok or not 
            {
                return BadRequest(ModelState);
            }
            bool stockExists = await _stockRepository.StockExits(stockId);
            if (!stockExists)
            {
                return BadRequest("Stock doesn't exist. ");
            }

            var appUserz = User.GetUserName();
            var userManager = await _userManager.FindByNameAsync(appUserz);
            if (userManager == null)
            {
                return BadRequest("User doesn't exists.");
            }

            var createCommentmodel = createComment.ToCommentFromCreate(stockId, userManager.Id);
            await _commentRepository.CreateAsync(createCommentmodel);

            return CreatedAtAction(nameof(GetById), new { id = createCommentmodel.Id }, createCommentmodel.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]//we applied URL constraints to only allows a particular type in the endpoint this is a kind of validation

        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var commentModel = await _commentRepository.DeleteAsync(id);
            if (commentModel == null)
            {
                return NotFound("Comment not found");
            }
            return Ok(commentModel);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto commentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var commentObj = await _commentRepository.UpdateAsync(id, commentModel.ToCommentFromUpdate());
            if (commentObj == null)
            {
                return NotFound("Comment not found");
            }

            return Ok(commentObj?.ToCommentDto());
        }

    }
}