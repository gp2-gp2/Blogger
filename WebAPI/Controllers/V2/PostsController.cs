using Application.Dto;
using Application.Dto.Cosmos;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers.V2
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("2.0")]
    //[Route("api/{v:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ICosmosPostService _postService;

        public PostsController(ICosmosPostService postService)
        {
            _postService = postService;
        }

        [SwaggerOperation(Summary = "Retrieves all posts")]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        [SwaggerOperation(Summary = "Retrieves a specific post by unique id")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [SwaggerOperation(Summary = "Create a new post")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCosmosPostDto newPost)
        {
            var post = await _postService.AddNewPostAsync(newPost);
            return Created($"api/posts/{post.Id}", post);
        }

        [SwaggerOperation(Summary = "Update an existing post")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateCosmosPostDto updatePost)
        {
            var post = await _postService.GetPostByIdAsync(updatePost.Id);

            if (post == null)
            {
                return NotFound();
            }

            await _postService.UpdatePostAsync(updatePost);
            return NoContent();
        }

        [SwaggerOperation(Summary = "Delete a specific post")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            await _postService.DeletePostAsync(id);
            return NoContent();
        }
    }
}
