using System.Runtime.Serialization;

namespace Service.Fees.Grpc.Models
{
    [DataContract]
    public class FeesResponse<T>
    {
        [DataMember(Order = 1)] public T Data { get; set; }
        [DataMember(Order = 2)] public string ErrorMessage { get; set; }
        [DataMember(Order = 3)] public bool IsSuccess { get; set; }

        public static FeesResponse<T> Success(T data)
        {
            return new FeesResponse<T>()
            {
                Data = data,
                IsSuccess = true
            };
        }

        public static FeesResponse<T> Error(string errorMessage)
        {
            return new FeesResponse<T>()
            {
                ErrorMessage = errorMessage,
                IsSuccess = false
            };
        }
    }
}