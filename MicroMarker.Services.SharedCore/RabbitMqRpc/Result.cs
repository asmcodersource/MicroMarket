namespace MicroMarket.Services.SharedCore.RabbitMqRpc
{
    public class Result<ValueType>
    {
        public ValueType Value { get; set; } = default(ValueType)!;
        public string? Error { get; set; }
        public bool IsSuccesful { get; set; }
        public bool IsFailure { get => !IsSuccesful; }

        public static Result<ValueType> Success(ValueType value)
        {
            return new Result<ValueType>()
            {
                Value = value,
                IsSuccesful = true
            };
        }

        public static Result<ValueType> Failure(string? error = null)
        {
            return new Result<ValueType>()
            {
                Error = error,
                IsSuccesful = false
            };
        }

        public static CSharpFunctionalExtensions.Result<ValueType> ConvertToCSharpFunctionalExtensionsResult(Result<ValueType> result)
        {
            if (result.IsSuccesful)
                return CSharpFunctionalExtensions.Result.Success<ValueType>(result.Value);
            else
                return CSharpFunctionalExtensions.Result.Failure<ValueType>(result.Error);
        }

        public static Result<ValueType> ConvertToResult(CSharpFunctionalExtensions.Result<ValueType> result)
        {
            if (result.IsSuccess)
                return Result<ValueType>.Success(result.Value);
            else
                return Result<ValueType>.Failure(result.Error);
        }
    }
}
