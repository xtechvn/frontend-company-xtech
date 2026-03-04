using B2B.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Collections.Concurrent;
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
        private readonly ISubscriber _subscriber;

        public SupportTicketController(IConfiguration configuration)
        {
            _configuration = configuration;

            // Kết nối Redis (cùng server với BE và CMS)
            var redisConn = ConnectionMultiplexer.Connect(
                _configuration["Redis:Host"] + ":" + _configuration["Redis:Port"]);
            _subscriber = redisConn.GetSubscriber();
        }

        private string GetUserId()
        {
            return User?.FindFirst("AccountId")?.Value
                   ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

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
                    var vms = (result.data.items ?? new List<TicketListItemDto>())
                        .Select(x => new TicketListItemVm
                        {
                            id = x.id,
                            code = x.code,
                            serviceId = x.serviceId,
                            serviceName = x.serviceName,
                            departmentName = x.departmentName,
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
                var result = await apiService.CreateTicket(
                    userId, model.serviceId, model.departmentId, model.subject, model.content);

                if (result == null || result.status != 0 || result.data == null)
                {
                    ViewBag.error = result?.msg ?? "Create ticket failed";
                    return View();
                }

                var ticketId = result.data.ticket_id;
                long firstMessageId = result.data.first_message_id;

                if (attachFiles != null && attachFiles.Any())
                {
                    var attList = new List<AttachFileViewModel>();

                    foreach (var f in attachFiles)
                    {
                        var url = await UpLoadHelper.UploadFileOrImage(f, firstMessageId, 200);
                        if (!string.IsNullOrEmpty(url))
                            attList.Add(new AttachFileViewModel { Url = url, Name = f.FileName });
                    }

                    if (attList.Any())
                    {
                        var saveAtt = await apiService.AddMessageAttachments(
                            firstMessageId,
                            attList.Select(x => x.Url).ToList(),
                            attList.Select(x => x.Name).ToList());

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

                if (attachFiles != null && attachFiles.Any())
                {
                    long totalSize = attachFiles.Sum(f => f.Length);
                    if (totalSize > 25 * 1024 * 1024)
                        return Json(new { success = false, message = "Tổng dung lượng file vượt quá 25MB." });
                }

                var apiService = new ApiService(_configuration);

                // 1) Tạo message => lấy messageId
                //    BE TicketAPIController sẽ tự Redis PUBLISH sau khi lưu DB
                var createMsg = await apiService.ReplyTicket(
                    ticketId: ticketId,
                    senderType: "Customer",
                    senderId: userId,
                    content: content,
                    contentHtml: content
                );

                if (createMsg == null || createMsg.status != 0 || createMsg.data == null)
                    return Json(new { success = false, message = createMsg?.msg ?? "Reply failed" });

                long messageId = createMsg.data.id;

                // 2) Upload file
                var fileUrls = new List<string>();
                var fileNames = new List<string>();

                if (attachFiles != null && attachFiles.Any())
                {
                    foreach (var f in attachFiles)
                    {
                        var url = await UpLoadHelper.UploadFileOrImage(f, messageId, 200);
                        if (!string.IsNullOrEmpty(url))
                        {
                            fileUrls.Add(url);
                            fileNames.Add(f.FileName);
                        }
                    }

                    // 3) Lưu attachments vào DB
                    var saveAtt = await apiService.AddMessageAttachments(messageId, fileUrls, fileNames);
                    if (saveAtt == null || saveAtt.status != 0)
                        return Json(new { success = false, message = saveAtt?.msg ?? "Save attachments failed" });
                }

                // Gắn attachFiles vào response để FE append ngay
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

        // =====================================================================
        // SSE ENDPOINT
        // Browser WebUser kết nối vào đây để nhận tin nhắn realtime
        // GET /support/ticket-stream?ticketId=xxx
        // =====================================================================
        [HttpGet]
        [Route("support/ticket-stream")]
        public async Task TicketStream(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(ticketId))
            {
                Response.StatusCode = 400;
                return;
            }

            // SSE headers - dùng HTTP thông thường, không cần WebSocket
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("X-Accel-Buffering", "no"); // tắt buffer Nginx

            var dataQueue = new ConcurrentQueue<string>();

            // Subscribe Redis channel TICKET_{ticketId}
            // BE Publish → Redis → đây nhận → SSE stream xuống browser
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
                        var sseData = $"data: {message}\n\n";
                        var buffer = System.Text.Encoding.UTF8.GetBytes(sseData);
                        await Response.Body.WriteAsync(buffer, 0, buffer.Length);
                        await Response.Body.FlushAsync();
                    }

                    await Task.Delay(20, HttpContext.RequestAborted);
                }
            }
            catch (OperationCanceledException)
            {
                // Client ngắt kết nối - bình thường
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("TicketStream - SupportTicketController: " + ex);
            }
            finally
            {
                // Unsubscribe khi client disconnect để giải phóng resource
                await _subscriber.UnsubscribeAsync($"TICKET_{ticketId}");
            }
        }

        // =====================================================================
        // HELPER
        // =====================================================================
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