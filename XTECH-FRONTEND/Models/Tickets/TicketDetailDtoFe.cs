namespace XTECH_FRONTEND.Models.Tickets
{
    public class TicketDetailDtoFe
    {
        public TicketDtoFe ticket { get; set; }
        public List<TicketMessageDtoFe> messages { get; set; } = new();
        //public List<TicketAttachmentDtoFe> attachments { get; set; } = new();
    }

    public class TicketDtoFe
    {
        public Guid id { get; set; }
        public string code { get; set; }
        public int serviceId { get; set; }
        public int departmentId { get; set; }
        public string subject { get; set; }
        public int status { get; set; }
        public int? priority { get; set; }
        public string createdByUserId { get; set; }
        public string assignedAgentId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime lastMessageAt { get; set; }
    }

    public class TicketMessageDtoFe
    {


        public long id { get; set; }

        public List<FileViewModel> AttachFiles { get; set; } = new List<FileViewModel>();
        public Guid ticketId { get; set; }
        public string senderType { get; set; }
        public string senderId { get; set; }
        public string content { get; set; }
        public string contentHtml { get; set; }
        public DateTime createdAt { get; set; }
    }



   
}