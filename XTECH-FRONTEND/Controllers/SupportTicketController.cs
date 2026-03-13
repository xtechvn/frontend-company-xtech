using B2B.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
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
        private readonly ISubscriber _subscriber;

        public SupportTicketController(IConfiguration configuration)
        {
            _configuration = configuration;
            var redisConn = ConnectionMultiplexer.Connect(
                _configuration["Redis:Host"] + ":" + _configuration["Redis:Port"]);
            _subscriber = redisConn.GetSubscriber();
        }

        private string GetUserId() =>
            User?.FindFirst("AccountId")?.Value
            ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // =====================================================================
        // INDEX
        // =====================================================================
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
                    ViewBag.data = (result.data.items ?? new List<TicketListItemDto>())
                        .Select(x => new TicketListItemVm
                        {
                            id = x.id,
                            code = x.code,
                            serviceId = x.serviceId,
                            serviceName = x.serviceName,
                            departmentName = x.departmentName,
                            subject = x.subject,
                            status = x.status,
                            assignedAgent = string.IsNullOrWhiteSpace(x.assignedAgentId) ? "Chưa phân công" : x.assignedAgentId,
                            lastUpdate = ToTimeAgo(x.lastMessageAt)
                        }).ToList();
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
                ViewBag.error = "Tải danh sách ticket thất bại";
                ViewBag.data = new List<TicketListItemVm>();
            }
            return View();
        }

        // =====================================================================
        // ADD SUPPORT
        // =====================================================================
        [Route("support/add")]
        public IActionResult AddSupport() => View();

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
            [FromForm] List<IFormFile> attachFiles)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrWhiteSpace(userId)) { ViewBag.error = "Bạn chưa đăng nhập."; return View(); }
                if (model == null || model.serviceId <= 0 || model.departmentId <= 0 ||
                    string.IsNullOrWhiteSpace(model.subject) || string.IsNullOrWhiteSpace(model.content))
                { ViewBag.error = "Vui lòng nhập đầy đủ thông tin."; return View(); }

                if (attachFiles != null && attachFiles.Sum(f => f.Length) > 25 * 1024 * 1024)
                { ViewBag.error = "Tổng dung lượng file vượt quá 25MB."; return View(); }

                var apiService = new ApiService(_configuration);
                // ✅ Lấy tên hiển thị thay vì userId số
                var creatorName = User?.FindFirst(ClaimTypes.Name)?.Value
                               ?? User?.FindFirst("Name")?.Value
                               ?? userId;
                var result = await apiService.CreateTicket(creatorName, model.serviceId, model.departmentId, model.subject, model.content);

                if (result == null || result.status != 0 || result.data == null)
                { ViewBag.error = result?.msg ?? "Create ticket failed"; return View(); }

                var ticketId = result.data.ticket_id;
                long firstMessageId = result.data.first_message_id;

                if (attachFiles != null && attachFiles.Any())
                {
                    var attList = new List<AttachFileViewModel>();
                    foreach (var f in attachFiles)
                    {
                        var url = await UpLoadHelper.UploadFileOrImage(f, firstMessageId, 200);
                        if (!string.IsNullOrEmpty(url)) attList.Add(new AttachFileViewModel { Url = url, Name = f.FileName });
                    }
                    if (attList.Any())
                        await apiService.AddMessageAttachments(firstMessageId, attList.Select(x => x.Url).ToList(), attList.Select(x => x.Name).ToList());
                }

                return Redirect($"/support/{ticketId}");
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("AddSupport - SupportTicketController: " + ex);
                ViewBag.error = "Create ticket failed";
                return View();
            }
        }

        // =====================================================================
        // DETAIL
        // =====================================================================
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
                    ViewBag.data = new TicketDetailVm
                    {
                        id = dto.ticket.id,
                        code = dto.ticket.code,
                        subject = dto.ticket.subject,
                        status = dto.ticket.status,
                        priority = dto.ticket.priority,
                        assignedAgent = string.IsNullOrWhiteSpace(dto.ticket.assignedAgentId) ? "Chưa phân công" : dto.ticket.assignedAgentId,
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
                                AttachFiles = m.AttachFiles
                            }).ToList()
                    };
                    ViewBag.ticketId = id;
                    // Tên hiển thị lấy từ Claim Name (session login)
                    ViewBag.currentUserId = GetUserId();
                    ViewBag.currentUserName = User?.FindFirst(ClaimTypes.Name)?.Value
                                           ?? User?.FindFirst("Name")?.Value
                                           ?? User?.FindFirst("AccountId")?.Value
                                           ?? "Bạn";
                }
                else
                {
                    ViewBag.error = result?.msg ?? "Ticket not found";
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Detail - SupportTicketController: " + ex);
                ViewBag.error = "Tải chi tiết ticket thất bại";
            }
            return View();
        }

        // =====================================================================
        // REPLY (AJAX)
        // =====================================================================
        [HttpPost]
        [Route("support/reply")]
        public async Task<IActionResult> Reply(
            [FromForm] Guid ticketId,
            [FromForm] string content,
            [FromForm] List<IFormFile> attachFiles)
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
                if (attachFiles != null && attachFiles.Sum(f => f.Length) > 25 * 1024 * 1024)
                    return Json(new { success = false, message = "Tổng dung lượng file vượt quá 25MB." });

                var apiService = new ApiService(_configuration);

                // BE TicketAPIController sẽ tự Publish Redis sau khi lưu DB
                // ✅ Lưu thẳng tên (Claim Name) vào SenderId thay vì AccountId
                // Thử lần lượt các claim phổ biến để lấy tên/email hiển thị
                var senderName = User?.FindFirst(ClaimTypes.Name)?.Value
                              ?? User?.FindFirst(ClaimTypes.Email)?.Value
                              ?? User?.FindFirst("Name")?.Value
                              ?? User?.FindFirst("Email")?.Value
                              ?? User?.FindFirst("FullName")?.Value
                              ?? userId;

                var createMsg = await apiService.ReplyTicket(
                    ticketId: ticketId,
                    senderType: "Customer",
                    senderId: senderName,   // ✅ lưu tên thay vì số Id
                    content: content,
                    contentHtml: content);

                if (createMsg == null || createMsg.status != 0 || createMsg.data == null)
                    return Json(new { success = false, message = createMsg?.msg ?? "Reply failed" });

                long messageId = createMsg.data.id;
                var fileUrls = new List<string>();
                var fileNames = new List<string>();

                if (attachFiles != null && attachFiles.Any())
                {
                    foreach (var f in attachFiles)
                    {
                        var url = await UpLoadHelper.UploadFileOrImage(f, messageId, 200);
                        if (!string.IsNullOrEmpty(url)) { fileUrls.Add(url); fileNames.Add(f.FileName); }
                    }
                    var saveAtt = await apiService.AddMessageAttachments(messageId, fileUrls, fileNames);
                    if (saveAtt == null || saveAtt.status != 0)
                        return Json(new { success = false, message = saveAtt?.msg ?? "Save attachments failed" });
                }

                createMsg.data.AttachFiles = fileUrls.Zip(fileNames, (u, n) => new FileViewModel { Url = u, Name = n }).ToList();
                return Json(new { success = true, data = createMsg.data });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("Reply - SupportTicketController: " + ex);
                return Json(new { success = false, message = "Reply failed" });
            }
        }

        // =====================================================================
        // REOPEN TICKET (AJAX) - WebUser mở lại ticket đã bị đóng bởi CMS
        // POST /support/reopen
        // =====================================================================
        [HttpPost]
        [Route("support/reopen")]
        public async Task<IActionResult> ReopenTicket([FromForm] Guid ticketId)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrWhiteSpace(userId))
                    return Json(new { success = false, message = "Bạn chưa đăng nhập." });
                if (ticketId == Guid.Empty)
                    return Json(new { success = false, message = "TicketId không hợp lệ." });

                var apiService = new ApiService(_configuration);

                // Gọi BE API đổi status => Open (0)
                var result = await apiService.ChangeTicketStatus(ticketId, 0);
                if (result == null || result.status != 0)
                    return Json(new { success = false, message = result?.msg ?? "Reopen failed" });

                // Publish Redis => CMS SSE nhận => cập nhật UI staff
                var payload = new
                {
                    type = "status_changed",
                    ticketId = ticketId,
                    status = 0,      // TicketStatus.Open
                    statusText = "Open"
                };

                await _subscriber.PublishAsync(
                    $"TICKET_{ticketId}",
                    JsonSerializer.Serialize(payload));

                return Json(new { success = true, message = "Ticket đã được mở lại." });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ReopenTicket - SupportTicketController: " + ex);
                return Json(new { success = false, message = "Reopen failed" });
            }
        }

        // =====================================================================
        // SSE ENDPOINT - nhận realtime từ Redis Pub/Sub
        // GET /support/ticket-stream?ticketId=xxx
        // =====================================================================
        [HttpGet]
        [Route("support/ticket-stream")]
        public async Task TicketStream(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId)) { Response.StatusCode = 400; return; }

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("X-Accel-Buffering", "no");

            var dataQueue = new ConcurrentQueue<string>();

            await _subscriber.SubscribeAsync($"TICKET_{ticketId}", (channel, message) =>
            {
                dataQueue.Enqueue(message!);
            });

            try
            {
                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    while (dataQueue.TryDequeue(out var message))
                    {
                        var buf = System.Text.Encoding.UTF8.GetBytes($"data: {message}\n\n");
                        await Response.Body.WriteAsync(buf, 0, buf.Length);
                        await Response.Body.FlushAsync();
                    }
                    await Task.Delay(20, HttpContext.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { LogHelper.InsertLogTelegram("TicketStream - SupportTicketController: " + ex); }
            finally { await _subscriber.UnsubscribeAsync($"TICKET_{ticketId}"); }
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
    }
}