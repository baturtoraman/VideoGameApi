using Microsoft.EntityFrameworkCore;
using VideoGameApi.Constants;
using VideoGameApi.Controllers;
using VideoGameApi.Data;
using VideoGameApi.Entities;
using VideoGameApi.Models.Converters;
using VideoGameApi.Models;
using VideoGameApi.Responses;
using VideoGameApi.Validators;
using Minio;
using Minio.DataModel.Args;


namespace VideoGameApi.Services
{
    public class VideoGameService : IVideoGameService
    {
        private readonly VideoGameDbContext context;
        private readonly ILogger<VideoGameController> logger;

        public VideoGameService(ILogger<VideoGameController> logger, VideoGameDbContext context)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<ResponseModel<string?>> UploadImageAsync(int id, IFormFile image)
        {
           logger.LogInformation("Begin: UploadImageAsync");

           try
           {
               var game = await context.VideoGames.FindAsync(id);
               if (game == null)
               {
                   return new ResponseModel<string?>(false, ResponseMessages.VideoGameNotFound, null);
               }

               if (image == null || image.Length == 0)
               {
                   return new ResponseModel<string?>(false, "No file uploaded", null);
               }

               var minioClient = new MinioClient()
                   .WithEndpoint("your-minio-endpoint")
                   .WithCredentials("your-access-key", "your-secret-key")
                   .Build();

               var bucketName = "your-bucket-name";
               var objectName = $"images/{id}/{image.FileName}";

                using (var stream = image.OpenReadStream())
                {
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithStreamData(stream)
                        .WithObjectSize(image.Length)
                        .WithContentType(image.ContentType);

                    await minioClient.PutObjectAsync(putObjectArgs);
                }

               game.ImageUrl = $"https://{bucketName}.your-minio-domain/{objectName}";

               await context.SaveChangesAsync();

               return new ResponseModel<string?>(true, "Image uploaded successfully", game.ImageUrl);
           }
           catch (Exception ex)
           {
               logger.LogError($"Error uploading image: {ex.Message}");
               return new ResponseModel<string?>(false, "An error occurred while uploading the image.", null);
           }
        }


        public async Task<ResponseModel<List<VideoGameDto>>> GetVideoGamesAsync()
        {
            logger.LogInformation("Begin: GetVideoGamesAsync");
            try
            {
                var games = await context.VideoGames
                    .Include(g => g.VideoGameDetails)
                    .Include(g => g.Publisher)
                    .Include(g => g.Genres)
                    .ToListAsync();

                var gameDtos = games.Select(VideoGameConverter.ToDto).ToList();

                return new ResponseModel<List<VideoGameDto>>(true, ResponseMessages.VideoGamesRetrieved, gameDtos);
            }
            catch
            {
                return new ResponseModel<List<VideoGameDto>>(false, ResponseMessages.InternalServerError, null);
            }
        }

        public async Task<ResponseModel<VideoGameDto>> GetVideoGameByIdAsync(int id)
        {
            logger.LogInformation("Begin: GetVideoGameByIdAsync");
            try
            {
                var game = await context.VideoGames.FindAsync(id);
                if (game is null)
                {
                    return new ResponseModel<VideoGameDto>(false, ResponseMessages.VideoGameNotFound, null);
                }

                return new ResponseModel<VideoGameDto>(true, ResponseMessages.VideoGameRetrieved, VideoGameConverter.ToDto(game));
            }
            catch
            {
                return new ResponseModel<VideoGameDto>(false, ResponseMessages.InternalServerError, null);
            }
        }

        public async Task<ResponseModel<VideoGame?>> AddVideoGameAsync(VideoGameDto newGameDto)
        {
            logger.LogInformation("Begin: AddVideoGameAsync");
            if (newGameDto == null)
            {
                return new ResponseModel<VideoGame?>(false, ResponseMessages.VideoGameInvalidData, null);
            }

            var validator = new VideoGameValidator();
            var validationResult = await validator.ValidateAsync(newGameDto);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return new ResponseModel<VideoGame?>(false, errorMessages, null);
            }

            var gameEntity = VideoGameConverter.ToEntity(newGameDto);

            try
            {
                context.VideoGames.Add(gameEntity);
                await context.SaveChangesAsync();

                return new ResponseModel<VideoGame?>(true, ResponseMessages.VideoGameAdded, gameEntity);
            }
            catch
            {
                return new ResponseModel<VideoGame?>(false, ResponseMessages.InternalServerError, null);
            }
        }

        public async Task<ResponseModel<string?>> UpdateVideoGameAsync(int id, VideoGameDto updatedGameDto)
        {
            logger.LogInformation("Begin: UpdateVideoGameAsync");
            if (updatedGameDto == null)
            {
                return new ResponseModel<string?>(false, ResponseMessages.VideoGameInvalidData, null);
            }

            var validator = new VideoGameValidator();
            var validationResult = await validator.ValidateAsync(updatedGameDto);

            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return new ResponseModel<string?>(false, ResponseMessages.VideoGameValidationFailed, errorMessages);
            }

            var game = await context.VideoGames.FindAsync(id);
            if (game is null)
            {
                return new ResponseModel<string?>(false, ResponseMessages.VideoGameNotFound, null);
            }
            var updatedGame = VideoGameConverter.ToEntity(updatedGameDto);

            game.Title = updatedGame.Title;
            game.Platform = updatedGame.Platform;
            game.Publisher = updatedGame.Publisher;

            try
            {
                var affectedRows = await context.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    return new ResponseModel<string?>(false, ResponseMessages.VideoGameNoChanges, null);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ResponseModel<string?>(false, ResponseMessages.VideoGameConflict, null);
            }
            catch
            {
                return new ResponseModel<string?>(false, ResponseMessages.InternalServerError, null);
            }

            return new ResponseModel<string?>(true, ResponseMessages.VideoGameUpdated, null);
        }

        public async Task<ResponseModel<string?>> DeleteVideoGameAsync(int id)
        {
            logger.LogInformation("Begin: DeleteVideoGameAsync");
            try
            {
                var game = await context.VideoGames.FindAsync(id);
                if (game == null)
                {
                    return new ResponseModel<string?>(false, ResponseMessages.VideoGameNotFound, null);
                }

                context.VideoGames.Remove(game);
                await context.SaveChangesAsync();

                return new ResponseModel<string?>(true, ResponseMessages.VideoGameDeleted, null);
            }
            catch
            {
                return new ResponseModel<string?>(false, ResponseMessages.InternalServerError, null);
            }
        }

        public async Task<ResponseModel<VideoGameDto>> PurchaseVideoGameAsync(PurchaseRequest request)
        {
            var user = await context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new ResponseModel<VideoGameDto>(false, "User not found.", null);
            }

            var videoGame = await context.VideoGames.FindAsync(request.VideoGameId);
            if (videoGame == null)
            {
                return new ResponseModel<VideoGameDto>(false, "Video game not found.", null);
            }

            if (videoGame.Stock < request.Quantity)
            {
                return new ResponseModel<VideoGameDto>(false, $"Insufficient stock for '{videoGame.Title}'. Available stock: {videoGame.Stock}.", null);
            }

            var totalCost = videoGame.Price * request.Quantity;
            if (user.Balance < totalCost)
            {
                return new ResponseModel<VideoGameDto>(false, "Insufficient balance.", null);
            }

            user.Balance -= totalCost;
            videoGame.Stock -= request.Quantity;

            await context.SaveChangesAsync();

            var videoGameDto = new VideoGameDto
            {
                Id = videoGame.Id,
                Title = videoGame.Title,
                Platform = videoGame.Platform,
                Price = videoGame.Price,
                Stock = videoGame.Stock
            };

            return new ResponseModel<VideoGameDto>(true, "Purchase successful.", videoGameDto);
        }

    }
}
