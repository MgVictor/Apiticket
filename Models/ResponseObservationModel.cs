using System.Collections.Generic;

namespace apiTicket.Models
{
    public class ResponseObservationModel
    {
        public string response { get; set; }
        public string code { get; set; }
        public List<Archivo> files { get; set; }
    }
}
