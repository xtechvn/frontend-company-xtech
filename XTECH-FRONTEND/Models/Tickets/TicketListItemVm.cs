using ENTITIES.ViewModels.AttachFiles;

namespace XTECH_FRONTEND.Models.Tickets
{
    public class TicketListItemVm
    {
        public Guid id { get; set; }
        public string code { get; set; }
        public int serviceId { get; set; }
        public string serviceName { get; set; }
        public string subject { get; set; }
        public int status { get; set; }
        public string assignedAgent { get; set; }
        public string lastUpdate { get; set; }
    }


    public class TicketMessageVm
    {
        public long id { get; set; }
        public List<FileViewModel> AttachFiles { get; set; } = new List<FileViewModel>();
        public Guid ticketId { get; set; }
        public string senderType { get; set; } // "Customer"|"Agent"
        public string senderId { get; set; }
        public string content { get; set; }
        public string contentHtml { get; set; }
        public string createdAt { get; set; }
    }
    public class FileViewModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class TicketDetailVm
    {
        public Guid id { get; set; }
        public string code { get; set; }
        public string subject { get; set; } 
        public int status { get; set; }
        public int? priority { get; set; }
        public string assignedAgent { get; set; }

        public List<TicketMessageVm> messages { get; set; } = new();
    }
    public class CreateTicketResultVm
    {
        public Guid ticket_id { get; set; }
        public string code { get; set; }
        public long first_message_id { get; set; }
    }

    public class TicketListResponseDto
    {
        public int page { get; set; }
        public int size { get; set; }
        public int total { get; set; }
        public List<TicketListItemDto> items { get; set; } = new();
    }

    public class TicketListItemDto
    {
        public Guid id { get; set; }
        public string code { get; set; }
        public int serviceId { get; set; }
        public string serviceName { get; set; }
        public string subject { get; set; }
        public int status { get; set; }
        public string assignedAgentId { get; set; }
        public DateTime lastMessageAt { get; set; }
    }
}
