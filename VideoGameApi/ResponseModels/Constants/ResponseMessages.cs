namespace VideoGameApi.Constants
{
    public static class ResponseMessages
    {
        public const string VideoGamesRetrieved = "Video games retrieved successfully.";
        public const string VideoGameRetrieved = "Video game retrieved successfully.";
        public const string VideoGameNotFound = "Video game not found.";
        public const string VideoGameInvalidData = "Invalid video game data.";
        public const string VideoGameValidationFailed = "Validation failed.";
        public const string VideoGameAdded = "Video game added successfully.";
        public const string VideoGameUpdated = "Video game updated successfully.";
        public const string VideoGameDeleted = "Video game deleted successfully.";
        public const string VideoGameNoChanges = "No changes were made to the video game.";
        public const string VideoGameConflict = "Concurrency conflict occurred while updating the video game.";
        public const string InternalServerError = "An unexpected error occurred. Please try again later.";
    }
}