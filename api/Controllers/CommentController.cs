using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;

        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepository.GetAllAsync();
            var commentDtos = comments.Select(comment => comment.ToCommentDto()).ToList();
            return Ok(commentDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute] int stockId,[FromBody] CreateCommentDto commentDto)
        {
            if(!await _stockRepository.StockExists(stockId)){
                return BadRequest("Stock does not exist");
            }
            var commentModel = commentDto.ToCommentFromCreate(stockId);
            var createdComment = await _commentRepository.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new { id = createdComment.Id }, createdComment.ToCommentDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            var comment = await _commentRepository.UpdateAsync(id, updateDto.ToCommentFromUpdate(id));

            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var comment = await _commentRepository.DeleteAsync(id);

            if (comment == null)
            {
                return NotFound("Comment does not exist");
            }

            return Ok(comment.ToCommentDto());
        }
    }
}