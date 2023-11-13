namespace shushi_shop_api.Models
{
    public class User : BaseModel
    {
        public string Password { get; set; }
        public string role { get; set; }
    }
}
