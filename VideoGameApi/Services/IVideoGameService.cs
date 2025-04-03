using VideoGameApi.Entities;
using VideoGameApi.Models;
using VideoGameApi.Responses;

namespace VideoGameApi.Services
{
    public interface IVideoGameService
    {
        //Task<ResponseModel<string?>> UploadImageAsync(int id, IFormFile image);
        Task<ResponseModel<List<VideoGameDto>>> GetVideoGamesAsync();
        Task<ResponseModel<VideoGameDto>> GetVideoGameByIdAsync(int id);
        Task<ResponseModel<VideoGame?>> AddVideoGameAsync(VideoGameDto newGameDto);
        Task<ResponseModel<string?>> UpdateVideoGameAsync(int id, VideoGameDto updatedGameDto);
        Task<ResponseModel<string?>> DeleteVideoGameAsync(int id);
    }
}
