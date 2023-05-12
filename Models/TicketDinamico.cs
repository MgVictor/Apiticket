using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class TicketDinamico
    {
        public string system { get; set; }
        public IList<Field> fields { get; set; }
        public IList<Attachment> attachments { get; set; }
    }
}
