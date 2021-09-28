namespace ForLab.Core.Common
{
    public class ErrorDetail
    {
        public int StatusCode { get; set; }
        public string Message{ get; set; }
        public override string ToString()
        {
            return $"Status Code:{StatusCode} \n\r Message:{Message}";
        }
    }
}
