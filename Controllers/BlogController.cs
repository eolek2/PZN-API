using API.Data;
using API.DTO;
using API.Filters;
using API.Types;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BlogController : BaseController
    {
        IDataRepository _repository;
        UserManager<User> _userManager;
        IMapper _autoMapper;
        public BlogController(IDataRepository repository, UserManager<User> userManager, IMapper autoMapper)
        {
            _repository = repository;
            _userManager = userManager;
            _autoMapper = autoMapper;
        }

        [HttpGet]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> GetPosts([FromQuery]PostFilters filters, CancellationToken cancellationToken = default(CancellationToken))
        {
            var posts = await _repository.GetPosts(filters, cancellationToken);
            var postsToReturn = _autoMapper.Map<ICollection<PostForListDto>>(posts);
            return Ok(postsToReturn);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "RequireUserRole")]
        public async Task<IActionResult> GetPost(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var post = await _repository.GetPost(id, cancellationToken);
            var postToReturn = _autoMapper.Map<PostForDetailDto>(post);
            var nextPostId = await _repository.GetNextPostId(post.Id);
            var previousId = await _repository.GetPreviousPostId(post.Id);
            postToReturn.NextId = nextPostId;
            postToReturn.HasNext = nextPostId.HasValue;
            postToReturn.PreviousId = previousId;
            postToReturn.HasPrevious = previousId.HasValue;
            return Ok(postToReturn);
        }

        [HttpPost]
        [Authorize(Policy = "RequireModeratorRole")]
        public async Task<IActionResult> AddPost(PostForCreateDto dto, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);

            var post = new BlogPost()
            {
                CreationDate = DateTime.Now,
                CreatedBy = userMakingReq,
                Content = dto.Content,
                Title = dto.Title,
                ShortContent = dto.ShortContent
            };

            _repository.Add<BlogPost>(post);
            
            if(await _repository.SaveAll())
            {
                var postToReturn = _autoMapper.Map<PostForDetailDto>(post);
                var previousId = await _repository.GetPreviousPostId(post.Id);
                postToReturn.NextId = null;
                postToReturn.HasNext = false;
                postToReturn.PreviousId = previousId;
                postToReturn.HasPrevious = previousId.HasValue;
                return Ok(postToReturn);
            }

            return BadRequest("Nie udało się dodać wpisu.");
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, PostForCreateDto dto, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);

            var existingPost = await _repository.GetPost(id, cancellationToken);

            if(existingPost.UserId != userMakingReq.Id)
            {
                return Unauthorized("Brak uprawnień do edycji posta.");
            }

            existingPost.Title = dto.Title;
            existingPost.Content = dto.Content;
            existingPost.ShortContent = dto.ShortContent;
            existingPost.UpdateDate = DateTime.Now;

            if(await _repository.SaveAll())
            {
                var postToReturn = _autoMapper.Map<PostForDetailDto>(existingPost);
                var nextPostId = await _repository.GetNextPostId(existingPost.Id);
                var previousId = await _repository.GetPreviousPostId(existingPost.Id);
                postToReturn.NextId = nextPostId;
                postToReturn.HasNext = nextPostId.HasValue;
                postToReturn.PreviousId = previousId;
                postToReturn.HasPrevious = previousId.HasValue;
                return Ok(postToReturn);
            }

            return BadRequest("Nie udało się zaktualizować wpisu.");
        }

        [Authorize(Policy = "RequireModeratorRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMakingReq = await _userManager.GetUserAsync(User);
            var post = await _repository.GetPost(id);

            if(post != null)
            {
                if(post.UserId != userMakingReq.Id)
                    return Unauthorized();

                _repository.Delete(post);
                if(await _repository.SaveAll())
                    return Ok();
                else
                    return BadRequest("Nie udało się usunąć wpisu");
            }

            return Unauthorized();
        }
    }
}