namespace apiTicket.Models
{
    public class BaseTable
    {
        public int NIDCM { get; set; }
        public string SDESCRIPTION { get; set; }
        public string SSHORT_DES { get; set; }
        public char SSTATREGT { get; set; }
        public int NUSERCODE { get; set; }
        public string DNULLDATE { get; set; }
        public string DCOMPDATE { get; set; }
        public string DREPLICATION { get; set; }

    }
}