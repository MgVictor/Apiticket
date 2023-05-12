using System.Collections.Generic;

namespace apiTicket.Models
{
    public class TicketUpdateJiraPutModel
    {
        public string system { get; set; }
        public List<CustomField> fields { get; set; }
        public string code { get; set; }

    }
    public class CustomField
    {
        public string id { get; set; }
        //DEV CY -- INI
        //public string value { get; set; }
        public object value { get; set; }
        //DEV CY -- FIN
    }

}
