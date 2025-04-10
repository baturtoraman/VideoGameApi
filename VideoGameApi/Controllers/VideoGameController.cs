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
           logger.LogInformation("Begin: UploadImage");

           try
           {
               if (image == null || image.Length == 0)
               {
                   return BadRequest(new { message = "No file uploaded" });
               }

               var uploadResponse = await videoGameService.UploadImageAsync(id, image);

               if (!uploadResponse.Success)
               {
                   return BadRequest(new { message = uploadResponse.Message });
               }

               return Ok(new { message = "Image uploaded successfully" });
           }
           catch (Exception ex)
           {
               return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
           }
        }

        [HttpGet]
        public async Task<ActionResult<List<VideoGameDto>>> GetVideoGames()
        {
            logger.LogInformation("Begin: GetVideoGames");

            try {
                var videoGames = cache.GetData<IEnumerable<VideoGameDto>>("videoGames");
                if (videoGames is not null)
                {
                    var redisVideoGames = videoGames.Select(vg =>
                    {
                        vg.Title = $"{vg.Title} (From Redis)";
                        return vg;
                    }).ToList();

                    return Ok(redisVideoGames);
                }

                var videoGamesDb = await context.VideoGames
                    .Include(g => g.VideoGameDetails)
                    .Include(g => g.Publisher)
                    .Include(g => g.Genres)
                    .ToListAsync();


                if (videoGamesDb != null)
                {
                    var redisVideoGames = videoGamesDb.Select(vg =>
                    {
                        vg.Title = $"{vg.Title}";
                        return vg;
                    }).ToList();

                    cache.SetData("videoGames", redisVideoGames);
                    return Ok(redisVideoGames);
                }
                return Ok(videoGames);
            }
            catch (Exception e) {
                logger.LogError($"Mumtaz Error = {e}");
                return BadRequest(e);
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

        [HttpPost("{id}/purchase")]
        public async Task<IActionResult> PurchaseVideoGame(int id, [FromBody] Guid userId)
        {
            logger.LogInformation("Begin: PurchaseVideoGame");

            try
            {
                var response = await videoGameService.PurchaseVideoGameAsync(id, userId);
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
