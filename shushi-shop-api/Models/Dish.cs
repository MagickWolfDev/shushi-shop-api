namespace shushi_shop_api.Models
{
    public class Dish : BaseModel
    {
        public string? Description { get; set; }
        public string Price { get; set; }
        public string ImageURL { get; set; }
        public int TypeID { get; set; }
    }
}

