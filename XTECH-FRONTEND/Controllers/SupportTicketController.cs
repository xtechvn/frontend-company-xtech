using B2B.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WEB.CMS.Customize;
using XTECH_FRONTEND.Models.Tickets;
using XTECH_FRONTEND.Services;
using XTECH_FRONTEND.Utilities;

namespace XTECH_FRONTEND.Controllers
{
    [CustomAuthorize]
    public class SupportTicketController : Controller
    {
        private readonly IConfiguration _configuration;

        public SupportTicketController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private string GetUserId()
        {
            // ✅ userId = idAccount
            return User?.FindFirst("AccountId")?.Value
                   ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        [HttpGet("support")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrWhiteSpace(userId))
                {
                    ViewBag.error = "Bạn chưa đăng nhập.";
                    ViewBag.data = new List<TicketListItemVm>();
                    return View();
                }

                var apiService = new ApiService(_configuration);
                var result = await apiService.GetMyTickets(userId);

                if (result != null && result.status == 0 && result.data != null)
                {
                    var vms = (result.data.items ?? new List<TicketListItemDto>())
                        .Select(x => new TicketListItemVm
                        {
                            id = x.id,
                            code = x.code,
                            serviceId = x.serviceId,
                            serviceName = x.serviceName,
                            departmentName=x.departmentName,
                            subject = x.subject,
                            status = x.status,

                            assignedAgent = string.IsNullOrWhiteSpace(x.assignedAgentId) ? "Unassigned" : x.assignedAgentId,
                            lastUpdate = ToTimeAgo(x.lastMessageAt)
                        })
                        .ToList();

                    ViewBag.data = vms;
                    ViewBag.page = result.data.page;
                    ViewBag.total = result.data.total;
                    ViewBag.size = result.data.size;
                }
                else
                {
                    ViewBag.error = result?.msg;
                    ViewBag.data = new List<TicketListItemVm>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Index - SupportTicketController: " + ex);
                ViewBag.error = "Load tickets failed";
                ViewBag.data = new List<TicketListItemVm>();
            }

            return View();
        }

        private static string ToTimeAgo(DateTime dt)
        {
            var span = DateTime.UtcNow - dt.ToUniversalTime();
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} mins ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
            if (span.TotalDays < 2) return "Yesterday";
            return $"{(int)span.TotalDays} days ago";
        }

        [Route("support/add")]
        public IActionResult AddSupport()
        {
            return View();
        }

        public class CreateSupportTicketRequest
        {
            public int serviceId { get; set; }
            public int departmentId { get; set; }
            public string subject { get; set; }
            public string content { get; set; }
            public int priority { get; set; } = 0;
        }

        [HttpPost("support/add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupport(
     [FromForm] CreateSupportTicketRequest model,
     [FromForm] List<IFormFile> attachFiles
 )
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrWhiteSpace(userId))
                {
                    ViewBag.error = "Bạn chưa đăng nhập hoặc không lấy được user_id.";
                    return View();
                }

                if (model == null ||
                    model.serviceId <= 0 ||
                    model.departmentId <= 0 ||
                    string.IsNullOrWhiteSpace(model.subject) ||
                    string.IsNullOrWhiteSpace(model.content))
                {
                    ViewBag.error = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                    return View();
                }

                // validate tổng size 25MB
                if (attachFiles != null && attachFiles.Any())
                {
                    long totalSize = attachFiles.Sum(f => f.Length);
                    if (totalSize > 25 * 1024 * 1024)
                    {
                        ViewBag.error = "Tổng dung lượng file vượt quá 25MB.";
                        return View();
                    }
                }

                var apiService = new ApiService(_configuration);

                // ✅ Create ticket => cần trả về first_message_id
                var result = await apiService.CreateTicket(userId, model.serviceId, model.departmentId, model.subject, model.content);

                if (result == null || result.status != 0 || result.data == null)
                {
                    ViewBag.error = result?.msg ?? "Create ticket failed";
                    return View();
                }

                var ticketId = result.data.ticket_id;                 // Guid
                long firstMessageId = result.data.first_message_id;   // long

                // ✅ upload + save attachments nếu có
                if (attachFiles != null && attachFiles.Any())
                {
                    var attList = new List<AttachFileViewModel>();

                    foreach (var f in attachFiles)
                    {
                        var url = await UpLoadHelper.UploadFileOrImage(f, firstMessageId, 200);
                        if (!string.IsNullOrEmpty(url))
                        {
                            attList.Add(new AttachFileViewModel
                            {
                                Url = url,
                                Name = f.FileName
                            });
                        }
                    }

                    if (attList.Any())
                    {
                        var urls = attList.Select(x => x.Url).ToList();
                        var names = attList.Select(x => x.Name).ToList();

                        var saveAtt = await apiService.AddMessageAttachments(firstMessageId, urls, names);
                        if (saveAtt == null || saveAtt.status != 0)
                        {
                            ViewBag.error = saveAtt?.msg ?? "Save attachments failed";
                            return View();
                        }
                    }
                }

                return Redirect($"/support/{ticketId}");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddSupport(Post) - SupportTicketController: " + ex);
                ViewBag.error = "Create ticket failed";
                return View();
            }
        }

        [HttpGet]
        [Route("support/{id}")]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var apiService = new ApiService(_configuration);
                var result = await apiService.GetTicketDetail(id);

                if (result != null && result.status == 0 && result.data?.ticket != null)
                {
                    var dto = result.data;

                    var vm = new TicketDetailVm
                    {
                        id = dto.ticket.id,
                        code = dto.ticket.code,
                        subject = dto.ticket.subject,
                        status = dto.ticket.status,
                        priority = dto.ticket.priority,

                        assignedAgent = string.IsNullOrWhiteSpace(dto.ticket.assignedAgentId)
                            ? "Unassigned"
                            : dto.ticket.assignedAgentId,

                        messages = (dto.messages ?? new List<TicketMessageDtoFe>())
                            .Select(m => new TicketMessageVm
                            {
                                id = m.id,
                                ticketId = m.ticketId,
                                senderType = m.senderType,
                                senderId = m.senderId,
                                content = m.content,
                                contentHtml = m.contentHtml,
                                createdAt = m.createdAt.ToString("dd/MM/yyyy HH:mm"),
                                // ✅ nếu BE trả attachments theo message thì map thêm (nếu chưa có thì để null)
                                AttachFiles = m.AttachFiles
                            })
                            .ToList()
                    };

                    ViewBag.data = vm;
                    ViewBag.ticketId = id;
                   
                }
                else
                {
                    ViewBag.error = result?.msg ?? "Ticket not found";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - SupportTicketController: " + ex);
                ViewBag.error = "Load ticket failed";
            }

            return View();
        }

        // ✅ AJAX reply (User gửi chat + upload)
        [HttpPost]
        [Route("support/reply")]
        public async Task<IActionResult> Reply(
    [FromForm] Guid ticketId,
    [FromForm] string content,
    [FromForm] List<IFormFile> attachFiles
)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrWhiteSpace(userId))
                    return Json(new { success = false, message = "Bạn chưa đăng nhập." });

                if (ticketId == Guid.Empty)
                    return Json(new { success = false, message = "TicketId không hợp lệ." });

                if (string.IsNullOrWhiteSpace(content) && (attachFiles == null || !attachFiles.Any()))
                    return Json(new { success = false, message = "Vui lòng nhập nội dung hoặc đính kèm file." });

                // validate tổng size 25MB
                if (attachFiles != null && attachFiles.Any())
                {
                    long totalSize = attachFiles.Sum(f => f.Length);
                    if (totalSize > 25 * 1024 * 1024)
                        return Json(new { success = false, message = "Tổng dung lượng file vượt quá 25MB." });
                }

                var apiService = new ApiService(_configuration);

                // ✅ 1) tạo message trước để lấy messageId (BIGINT)
                var createMsg = await apiService.ReplyTicket(
                    ticketId: ticketId,
                    senderType: "Customer",
                    senderId: userId,
                    content: content,
                    contentHtml: content
                );

                if (createMsg == null || createMsg.status != 0 || createMsg.data == null)
                    return Json(new { success = false, message = createMsg?.msg ?? "Reply failed" });

                // ⚠️ messageId phải là long (Id tự tăng)
                long messageId = createMsg.data.id; // đảm bảo DTO id là long

                // ✅ 2) upload file lên static server bằng data_id = messageId (long)
                var fileUrls = new List<string>();
                var fileNames = new List<string>();

                if (attachFiles != null && attachFiles.Any())
                {
                    foreach (var f in attachFiles)
                    {
                        var url = await UpLoadHelper.UploadFileOrImage(f, messageId, 200 /*type upload*/);
                        if (!string.IsNullOrEmpty(url))
                        {
                            fileUrls.Add(url);
                            fileNames.Add(f.FileName);
                        }
                    }

                    // ✅ 3) lưu attachments vào DB (AttachFile.DataId = messageId)
                    var saveAtt = await apiService.AddMessageAttachments(messageId, fileUrls, fileNames);

                    if (saveAtt == null || saveAtt.status != 0)
                        return Json(new { success = false, message = saveAtt?.msg ?? "Save attachments failed" });
                }

                // trả về message (SignalR phía API broadcast sẽ bắn realtime)
                // Nếu bạn muốn FE append ngay thì gắn attachFiles vào data trả về:
                createMsg.data.AttachFiles = fileUrls.Zip(fileNames, (u, n) => new FileViewModel
                {
                    Url = u,
                    Name = n
                }).ToList();

                return Json(new { success = true, data = createMsg.data });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Reply - SupportTicketController: " + ex);
                return Json(new { success = false, message = "Reply failed" });
            }
        }
    }
}
