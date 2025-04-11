using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameApi.Data;
using VideoGameApi.Models;
using VideoGameApi.Models.Converters;
using VideoGameApi.Responses;
using VideoGameApi.Services;
using VideoGameApi.Services.Caching;

namespace VideoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController : ControllerBase
    {
        private readonly ILogger<VideoGameController> logger;

        private readonly VideoGameDbContext context;
        private readonly IVideoGameService videoGameService;
        private readonly IRedisCacheService cache;
        public VideoGameController(ILogger<VideoGameController> logger, VideoGameDbContext context, IVideoGameService videoGameService, IRedisCacheService cache)
        {
            this.context = context;
            this.videoGameService = videoGameService;
            this.cache = cache;
            this.logger = logger;
        }


        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile image)
        {
            logger.LogInformation($"Begin: UploadImage for VideoGame ID {id}");

            try
            {
                if (image == null || image.Length == 0)
                {
                    logger.LogWarning("No file uploaded.");
                    return BadRequest(new { message = "No file uploaded" });
                }

                var uploadResponse = await videoGameService.UploadImageAsync(id, image);

                if (!uploadResponse.Success)
                {
                    logger.LogWarning($"Image upload failed for VideoGame ID {id}: {uploadResponse.Message}");
                    return BadRequest(new { message = uploadResponse.Message });
                }

                // Redis cache'i temizle
                cache.RemoveData($"videoGame_{id}");
                logger.LogInformation($"Cache cleared for VideoGame ID {id}");

                return Ok(new { message = "Image uploaded successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in UploadImage for VideoGame ID {id}: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<VideoGameDto>>> GetVideoGames()
        {
            logger.LogInformation("Begin: GetVideoGames");

            try
            {
                var cachedVideoGames = cache.GetData<IEnumerable<VideoGameDto>>("videoGames");
                if (cachedVideoGames != null)
                {
                    logger.LogInformation("Data retrieved from Redis cache.");
                    return Ok(cachedVideoGames);
                }

                var videoGamesDb = await context.VideoGames
                    .Include(g => g.VideoGameDetails)
                    .Include(g => g.Publisher)
                    .Include(g => g.Genres)
                    .Include(g=> g.Stock)
                    .Include(g=> g.Price)
                    .ToListAsync();

                if (videoGamesDb == null || !videoGamesDb.Any())
                {
                    logger.LogWarning("No video games found in the database.");
                    return NotFound("No video games found.");
                }

                var videoGamesDto = videoGamesDb.Select(vg => new VideoGameDto
                {
                    Id = vg.Id,
                    Title = vg.Title,
                    Platform = vg.Platform,
                    PublisherId = vg.PublisherId,
                    Publisher = vg.Publisher != null ? new PublisherDto { Id = vg.Publisher.Id, Name = vg.Publisher.Name } : null,
                    VideoGameDetails = vg.VideoGameDetails != null ? new VideoGameDetailsDto { Description = vg.VideoGameDetails.Description } : null,
                    Genres = vg.Genres?.Select(genre => new GenreDto { Id = genre.Id, Name = genre.Name }).ToList(),
                    ImageUrl = vg.ImageUrl,
                    Stock = vg.Stock,
                    Price = vg.Price
                }).ToList();

                cache.SetData("videoGames", videoGamesDto);
                logger.LogInformation("Data cached in Redis.");

                return Ok(videoGamesDto);
            }
            catch (Exception e)
            {
                logger.LogError($"Error in GetVideoGames: {e.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = e.Message });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoGameById(int id)
        {
            logger.LogInformation("Begin: GetVideoGameById");
            try
            {
                var response = await videoGameService.GetVideoGameByIdAsync(id);

                if (!response.Success)
                {
                    logger.LogInformation(response.Message);
                    return BadRequest(new { message = response.Message, errors = response.Message });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddVideoGame(VideoGameDto newGame)
        {
            logger.LogInformation("Begin: AddVideoGame");
            try
            {
                var response = await videoGameService.AddVideoGameAsync(newGame);

                if (!response.Success)
                {
                    logger.LogInformation(response.Message);
                    return BadRequest(new { message = response.Message, errors = response.Message });
                }

                return CreatedAtAction(nameof(GetVideoGameById), new { id = response.Data?.Id }, response.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateVideoGame(int id, VideoGameDto updatedGame)
        {
            logger.LogInformation("Begin: UpdateVideoGame");
            try
            {
                var response = await videoGameService.UpdateVideoGameAsync(id, updatedGame);

                if (!response.Success)
                {
                    logger.LogInformation(response.Message);
                    return BadRequest(new { message = response.Message, errors = response.Message });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideoGame(int id)
        {
            logger.LogInformation("Begin: DeleteVideoGame");
            try
            {
                var response = await videoGameService.DeleteVideoGameAsync(id);

                if (!response.Success)
                {
                    logger.LogInformation(response.Message);
                    return BadRequest(new { message = response.Message, errors = response.Message });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseVideoGame([FromBody] PurchaseRequest request)
        {
            logger.LogInformation("Begin: PurchaseVideoGame");

            try
            {
                var response = await videoGameService.PurchaseVideoGameAsync(request);
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(new { message = "Purchase successful", game = response.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("/Batur1")]
        public IActionResult GetBatur()
        {
            logger.LogInformation("Begin: GetBatur");
            return Ok("Batur1");
        }
    }
}
