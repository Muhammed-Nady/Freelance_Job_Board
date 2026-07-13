namespace Freelify.Models.Entities
{
    public class ClientProfile : ApplicationUser
    {
        public string CompanyName { get; set; }

        public string CompanyLogoUrl { get; set; }

        public string CompanyDescription { get; set; }

        // TODO
        //reviews
        //avgrating
        //reviewCount
    }
}
