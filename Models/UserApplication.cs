namespace OktaClone.Web.Models
{
    public class UserApplication
    {
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int ApplicationId { get; set; }
        public Application? Application { get; set; }

        public string? Status { get; set; } // "Assigned", "Requested"
    }
}
