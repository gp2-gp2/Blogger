using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Filters;
using WebAPI.Helpers;
using WebAPI.Wrappers;

namespace WebAPI.Controllers.V1
{
    [ApiExplorerSettings(IgnoreApi = false)]
    //[Route("api/{v:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [SwaggerOperation(Summary = "Retrieves all posts")]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]PaginationFilter paginationFilter)
        {
            var validPaginationFilter = new PaginationFilter(paginationFilter.PageNumber, paginationFilter.PageSize);

            var posts = await _postService.GetAllPostsAsync(validPaginationFilter.PageNumber, validPaginationFilter.PageSize);
            var totalRecords = await _postService.GetAllPostsCountAsync();

            return Ok(PaginationHelper.CreatePagedResponse(posts, validPaginationFilter, totalRecords));
        }

        [SwaggerOperation(Summary = "Retrieves a specific post by unique id")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(new Response<PostDto>(post));
        }

        [SwaggerOperation(Summary = "Retrieves posts by title")]
        [HttpGet("Search/{title}")]
        public async Task<IActionResult> GetAsync(string title)
        {
            var posts = await _postService.GetByTitleAsync(title);
            return Ok(new Response<IEnumerable<PostDto>>(posts));
        }

        [SwaggerOperation(Summary = "Create a new post")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreatePostDto newPost)
        {
            var post = await _postService.AddNewPostAsync(newPost);
            return Created($"api/posts/{post.Id}", new Response<PostDto>(post));
        }

        [SwaggerOperation(Summary = "Update an existing post")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdatePostDto updatePost)
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
        public async Task<IActionResult> DeleteAsync(int id)
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
