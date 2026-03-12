// support_detail.js - WebUser
// SSE thay SignalR + ReopenTicket AJAX + xử lý status_changed

var userTicket = {
    eventSource: null,
    ticketId: null,
    currentStatus: null,

    init: function () {
        this.ticketId = ($('#TicketId').val() || '').trim();
        this.currentStatus = parseInt($('#CurrentStatus').val() || '0');
        this.initSSE();
        this.renderStatusUI();

        // Ctrl+Enter gửi
        $('#replyEditor').on('keydown', function (e) {
            if (e.ctrlKey && e.key === 'Enter') {
                replyEditor.beforeSend();
                userTicket.sendReply();
                e.preventDefault();
            }
        });
    },

    // =========================================================================
    // SSE
    // =========================================================================
    initSSE: function () {
        $('#rtStatus').text('Đang kết nối...');

        this.eventSource = new EventSource(
            '/support/ticket-stream?ticketId=' + this.ticketId
        );

        this.eventSource.onopen = function () {
            $('#rtStatus').text('Realtime: Trực tuyến');
        };

        this.eventSource.onmessage = function (event) {
            try {
                var data = JSON.parse(event.data);

                // ✅ Xử lý status_changed (CMS đóng ticket)
                if (data.type === 'status_changed') {
                    userTicket.handleStatusChanged(data);
                    return;
                }

                // Attachments
                if (data.type === 'attachments') {
                    if (data.messageId && data.attachFiles && data.attachFiles.length)
                        userTicket.updateMessageAttachments(data.messageId, data.attachFiles);
                    return;
                }

                // Message thường
                var msg = userTicket.normalizeMessage(data);
                if (!msg) return;
                if ((msg.ticketId + '').toLowerCase() !== (userTicket.ticketId + '').toLowerCase()) return;
                userTicket.appendMessage(msg);

            } catch (e) {
                console.error('[SSE] parse error:', e);
            }
        };

        this.eventSource.onerror = function () {
            $('#rtStatus').text('Realtime: Đang kết nối lại...');
        };
    },

    // =========================================================================
    // Xử lý status thay đổi từ SSE (CMS gửi)
    // =========================================================================
    handleStatusChanged: function (data) {
        userTicket.currentStatus = data.status;
        userTicket.renderStatusUI();
    },

    // =========================================================================
    // Render UI theo status
    // =========================================================================
    renderStatusUI: function () {
        var isClosed = userTicket.currentStatus === 3; // TicketStatus.Closed

        // Badge status
        var badgeHtml = isClosed
            ? '<span class="badge badge-closed">Closed</span>'
            : '<span class="badge badge-open">Open</span>';
        $('#statusBadge').html(badgeHtml);

        if (isClosed) {
            // Ẩn reply editor, hiện nút Reopen
            $('#replySection').hide();
            $('#btnReopenTicket').show();
            $('#closedNotice').show();
        } else {
            $('#replySection').show();
            $('#btnReopenTicket').hide();
            $('#closedNotice').hide();
        }
    },

    // =========================================================================
    // Reopen Ticket (AJAX) - WebUser mở lại ticket
    // =========================================================================
    reopenTicket: function () {
        if (!confirm('Bạn muốn mở lại ticket này để tiếp tục hỗ trợ?')) return;

        $.ajax({
            url: '/support/reopen',
            type: 'POST',
            data: { ticketId: userTicket.ticketId },
            success: function (res) {
                if (res && res.success) {
                    userTicket.currentStatus = 0; // Open
                    userTicket.renderStatusUI();
                } else {
                    alert(res?.message || 'Mở lại ticket thất bại');
                }
            },
            error: function (xhr) {
                alert('HTTP ' + xhr.status + ': Reopen failed');
            }
        });
    },

    // =========================================================================
    // Send Reply (AJAX)
    // =========================================================================
    sendReply: function () {
        var content = ($('#txtReply').val() || '').trim();
        var files = window.__replyFiles || [];

        if (!content && (!files || files.length === 0)) return;

        var $btn = $('#btnSendReply');
        $btn.prop('disabled', true);

        var fd = new FormData();
        fd.append('ticketId', userTicket.ticketId);
        fd.append('content', content);
        if (files && files.length)
            for (var i = 0; i < files.length; i++) fd.append('attachFiles', files[i]);

        $.ajax({
            url: window.userTicketEndpoints.replyUrl,
            type: 'POST',
            data: fd,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res && res.success) {
                    if (res.data) {
                        var msg = userTicket.normalizeMessage(res.data);
                        if (msg) userTicket.appendMessage(msg);
                    }
                    if (window.replyEditor && replyEditor.reset) replyEditor.reset();
                    window.__replyFiles = [];
                } else {
                    alert(res?.message || 'Gửi trả lời thất bại');
                }
            },
            error: function (xhr) {
                alert('HTTP ' + xhr.status + ': ' + (xhr.responseText || 'Yêu cầu thất bại'));
            },
            complete: function () { $btn.prop('disabled', false); }
        });
    },

    // =========================================================================
    // Append Message
    // =========================================================================
    appendMessage: function (m) {
        if (!m) return;
        if (m.id && $('#msg-' + m.id).length > 0) return;

        var senderType = (m.senderType || '').toLowerCase();
        var isStaff = senderType === 'agent' || senderType === 'staff';
        var currentUserId = window.userTicketEndpoints?.currentUserId || '';
        var currentUserName = window.userTicketEndpoints?.currentUserName || 'Bạn';
        // Nếu senderId == AccountId của user đang login → hiện tên thật, ngược lại hiện senderId
        var who = isStaff
            ? (m.senderId || 'Nhân viên')
            : (m.senderId === currentUserId ? currentUserName : (m.senderId || currentUserName));
        var contentHtml = m.contentHtml ? m.contentHtml : this.escapeHtml(m.content || '');
        var bubbleClass = isStaff ? 'bubble-staff' : 'bubble-cus';
        var staffBadge = isStaff ? `<span class="badge badge-staff rounded px-2 py-1">STAFF</span>` : '';

        var html = `
        <article class="card-soft p-4 mb-4" ${m.id ? `id="msg-${m.id}"` : ''}>
            <div class="d-flex align-items-start justify-content-between gap-3 mb-2">
                <div>
                    <div class="fw-bold">${who}</div>
                    <div class="text-secondary" style="font-size:12px;">${m.createdAt || ''}</div>
                </div>
                ${staffBadge}
            </div>
            <div class="bubble ${bubbleClass}" style="line-height:1.65;">${contentHtml}</div>
            ${this.renderAttachments(m.attachFiles)}
        </article>`;

        $('#chatList').append(html);
        window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
    },

    // Thông báo hệ thống (status changed...)
    appendSystemMessage: function (text) {
        var html = `
        <div class="text-center my-3">
            <span class="badge bg-light text-secondary border px-3 py-2" style="font-size:12px;">
                ${text}
            </span>
        </div>`;
        $('#chatList').append(html);
        window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
    },

    updateMessageAttachments: function (messageId, files) {
        var $msg = $('#msg-' + messageId);
        if ($msg.length === 0) return;
        if ($msg.find('.attachments').length > 0) $msg.find('.attachments').remove();
        $msg.append(this.renderAttachments(files));
    },

    renderAttachments: function (files) {
        if (!files || !files.length) return '';
        return `<div class="attachments">${files.map(f => this.renderAttachmentItem(f)).join('')}</div>`;
    },

    renderAttachmentItem: function (file) {
        var name = file.Name || file.name || '';
        var url = file.Url || file.url || '#';
        var lower = (name || url).toLowerCase();
        var isImg = lower.endsWith('.jpg') || lower.endsWith('.jpeg') ||
            lower.endsWith('.png') || lower.endsWith('.gif') || lower.endsWith('.webp');

        if (isImg) return `<a class="att-img" href="${url}" target="_blank" rel="noopener"><img src="${url}" alt="${this.escapeText(name)}"/></a>`;

        var ext = name ? (name.split('.').pop() || '').toUpperCase() : 'FILE';
        return `<a class="att-file" href="${url}" download><div class="meta"><span class="ext">${ext}</span><span class="name">${this.escapeText(name || url)}</span></div><i class="bi bi-download"></i></a>`;
    },

    normalizeMessage: function (m) {
        if (!m) return null;
        var ticketId = m.ticketId || m.TicketId;
        if (!ticketId) return null;
        return {
            id: m.id || m.Id || null,
            ticketId: ticketId,
            senderType: m.senderType || m.SenderType || '',
            senderId: m.senderId || m.SenderId || '',
            content: m.content || m.Content || '',
            contentHtml: m.contentHtml || m.ContentHtml || '',
            createdAt: m.createdAt || m.CreatedAt || '',
            attachFiles: m.attachFiles || m.AttachFiles || []
        };
    },

    escapeText: function (s) {
        return (s || '').replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&#39;');
    },

    escapeHtml: function (s) {
        return (s || '').replaceAll('&', '&amp;').replaceAll('<', '&lt;').replaceAll('>', '&gt;').replaceAll('"', '&quot;').replaceAll("'", '&#39;').replaceAll('\n', '<br/>');
    }
};

$(document).ready(function () {
    userTicket.init();
});