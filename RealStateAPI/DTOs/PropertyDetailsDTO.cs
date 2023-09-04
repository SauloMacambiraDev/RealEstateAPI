namespace RealStateAPI.DTOs
{
    public class PropertyDetailsDTO
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyDetail { get; set; }
        public string PropertyAddress { get; set; }
        public string PropertyImageUrl { get; set; }
        public decimal PropertyPrice { get; set; }
        public bool PropertyIsTrending { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
    }
}
