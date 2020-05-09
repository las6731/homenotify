namespace HomeNotify.API.Database
{
    public enum RepositoryResult
    {
        Failure = 0,
        PartialFailure = 1,
        Success = 2
    }

    public static class Extensions
    {
        public static bool IsSuccess(this RepositoryResult result)
        {
            return result == RepositoryResult.Success;
        }
    }
}