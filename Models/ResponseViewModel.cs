namespace apiTicket.Models
{
    public class ResponseViewModel
    {
        public string message { get; set; }
        public int code { get; set; }
        public dynamic customMessage { get; set; }
    }
}
