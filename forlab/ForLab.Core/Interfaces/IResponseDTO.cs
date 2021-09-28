namespace ForLab.Core.Interfaces
{
    public interface IResponseDTO
    {
        bool IsPassed { get; set; }
        string Message { get; set; }
        dynamic Data { get; set; }
    }
}
