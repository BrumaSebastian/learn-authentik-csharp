namespace Server.Models.Responses
{
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public ErrorMessage ErrorMessage { get; init; }
        public T Content { get; init; }

        private Result(bool isSuccess, ErrorMessage errorMessage = default, T content = default)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Content = content;
        }

        public static Result<T> Success(T content) => new Result<T>(true, content: content);
        public static Result<T> Failure(ErrorMessage errorMessage) => new(false, errorMessage: errorMessage);
    }
}
