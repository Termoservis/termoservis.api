namespace termoservis.api.Models
{
    /// <summary>
    /// The concurrent DB model.
    /// </summary>
    /// <remarks>
    /// See link how to handle concurrency exception: https://docs.microsoft.com/en-us/ef/core/saving/concurrency
    /// </remarks>
    public interface IConcurrentModel 
    {
        byte[] Timestamp { get; set; }
    }
}