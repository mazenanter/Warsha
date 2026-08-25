using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Domain.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; private set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public List<string>? Errors { get; private set; } = null;

        public static Result<T> Success(T data, string message) => new() { IsSuccess = true, Data = data, Message = message };
        public static Result<T> Failure(string message) => new() { IsSuccess = false, Message = message, Errors = null };
        public static Result<T> Failure(List<string> errors, string message) => new() { IsSuccess = false, Errors = errors, Message = message };
    }

    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; private set; } = null;

        public static Result Success(string message) => new() { IsSuccess = true, Message = message };
        public static Result Failure(string message) => new() { IsSuccess = false, Message = message, Errors = null };
        public static Result Failure(List<string> errors, string message) => new() { IsSuccess = false, Errors = errors, Message = message };
    }
}
