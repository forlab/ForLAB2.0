using ForLab.Core.Interfaces;

namespace ForLab.DTO.Common
{
    public class ResponseDTO : IResponseDTO
    {
        public ResponseDTO()
        {
            IsPassed = false;
            Message = "";
        }
        public bool IsPassed { get; set; }

        public string Message { get; set; }

        public dynamic Data { get; set; }

        public void Copy(ResponseDTO x)
        {
            IsPassed = x.IsPassed;
            Message = x.Message;
            Data = x.Data;
        }
    }
}
