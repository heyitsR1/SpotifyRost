using System.Net;

namespace SpotifyRoast.Dtos
{
    public class ResponseDto<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<T> Datas { get; set; } = new List<T>();

        public ResponseDto()
        {
            StatusCode = HttpStatusCode.OK;
            Message = "Success";
        }
    }
}
