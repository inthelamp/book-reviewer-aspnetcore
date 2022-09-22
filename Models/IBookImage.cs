namespace BookReviewer.Models
{
    /// <summary>
    /// Properties of interface IBookImage
    /// </summary>    
    public interface IBookImage
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
        public string FileExtension { get; set; }
        public decimal Size { get; set; }
        public DateTime CreateDate { get; set; } 
        public Guid ReviewId { get; set; }
        public Review Review { get; set; }  
    }
}