// support_detail.js - WebUser
// Thay SignalR bằng SSE (Server-Sent Events)
// Không còn phụ thuộc vào hubUrl, không bị Mixed Content

var userTicket = {
    eventSource: null,
    ticketId: null,

    init: function () {
        this.ticketId = ($('#TicketId').val() || '').trim();
        this.initSSE();

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
    // SSE: thay thế hoàn toàn SignalR
    // Kết nối tới /support/ticket-stream?ticketId=xxx (cùng domain, cùng HTTPS)
    // =========================================================================
    initSSE: function () {
        var self = this;

        $('#rtStatus').text('Connecting...');

        // SSE chỉ dùng HTTP GET thuần, không cần WebSocket
        // Cùng domain x-tech.vn => không bao giờ bị Mixed Content
        this.eventSource = new EventSource(
            '/support/ticket-stream?ticketId=' + this.ticketId
        );

        this.eventSource.onopen = function () {
            $('#rtStatus').text('Realtime: Online');
        };

        this.eventSource.onmessage = function (event) {
            try {
                var data = JSON.parse(event.data);

                // Phân biệt message thường và attachments
                if (data.type === 'attachments') {
                    // Cập nhật attachments cho message đã có
                    if (data.messageId && data.attachFiles && data.attachFiles.length) {
                        userTicket.updateMessageAttachments(data.messageId, data.attachFiles);
                    }
                    return;
                }

                // Message thường
                var msg = userTicket.normalizeMessage(data);
                if (!msg) return;

                // Chỉ xử lý message thuộc ticket này
                if ((msg.ticketId + '').toLowerCase() !== (userTicket.ticketId + '').toLowerCase()) return;

                userTicket.appendMessage(msg);

            } catch (e) {
                console.error('SSE parse error:', e);
            }
        };

        this.eventSource.onerror = function () {
            $('#rtStatus').text('Realtime: Offline - Reconnecting...');
            // EventSource tự reconnect sau vài giây (built-in browser behavior)
        };
    },

    // =========================================================================
    // Send Reply (AJAX - không đổi)
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

        if (files && files.length) {
            for (var i = 0; i < files.length; i++) {
                fd.append('attachFiles', files[i]);
            }
        }

        $.ajax({
            url: window.userTicketEndpoints.replyUrl,
            type: 'POST',
            data: fd,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res && res.success) {
                    // Append ngay từ HTTP response (không chờ SSE để tránh duplicate)
                    if (res.data) {
                        var msg = userTicket.normalizeMessage(res.data);
                        if (msg) userTicket.appendMessage(msg);
                    }

                    if (window.replyEditor && replyEditor.reset) replyEditor.reset();
                    window.__replyFiles = [];
                    $('#txtReplyContent').val('');
                } else {
                    alert(res?.message || 'Reply failed');
                }
            },
            error: function (xhr) {
                var msg = xhr?.responseJSON?.message || ('HTTP ' + xhr.status + ': ' + (xhr.responseText || 'Request failed'));
                alert(msg);
            },
            complete: function () {
                $btn.prop('disabled', false);
            }
        });
    },

    // =========================================================================
    // Append Message
    // =========================================================================
    appendMessage: function (m) {
        if (!m) return;

        // Dedup: nếu message đã có trong DOM thì bỏ qua
        if (m.id && $('#msg-' + m.id).length > 0) return;

        var senderType = (m.senderType || '').toLowerCase();
        var isStaff = senderType === 'agent' || senderType === 'staff';
        var who = isStaff ? 'Support' : 'You';
        var createdAt = m.createdAt || '';
        var contentHtml = m.contentHtml ? m.contentHtml : this.escapeHtml(m.content || '');
        var bubbleClass = isStaff ? 'bubble-staff' : 'bubble-cus';
        var staffBadge = isStaff
            ? `<span class="badge badge-staff rounded px-2 py-1">STAFF</span>`
            : '';

        var attHtml = this.renderAttachments(m.attachFiles);

        var html = `
        <article class="card-soft p-4 mb-4" ${m.id ? `id="msg-${m.id}"` : ''}>
            <div class="d-flex align-items-start justify-content-between gap-3 mb-2">
                <div>
                    <div class="fw-bold">${who}</div>
                    <div class="text-secondary" style="font-size:12px;">${createdAt}</div>
                </div>
                ${staffBadge}
            </div>
            <div class="bubble ${bubbleClass}" style="line-height:1.65;">
                ${contentHtml}
            </div>
            ${attHtml}
        </article>`;

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
            lower.endsWith('.png') || lower.endsWith('.gif') ||
            lower.endsWith('.webp');

        if (isImg) {
            return `
              <a class="att-img" href="${url}" target="_blank" rel="noopener">
                <img src="${url}" alt="${this.escapeText(name)}"/>
              </a>`;
        }

        var ext = '';
        if (name) { var parts = name.split('.'); ext = (parts.length > 1 ? parts.pop() : '').toUpperCase(); }

        return `
            <a class="att-file" href="${url}" download>
              <div class="meta">
                <span class="ext">${ext || 'FILE'}</span>
                <span class="name">${this.escapeText(name || url)}</span>
              </div>
              <i class="bi bi-download"></i>
            </a>`;
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
        return (s || '')
            .replaceAll('&', '&amp;').replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;').replaceAll('"', '&quot;')
            .replaceAll("'", '&#39;');
    },

    escapeHtml: function (s) {
        return (s || '')
            .replaceAll('&', '&amp;').replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;').replaceAll('"', '&quot;')
            .replaceAll("'", '&#39;').replaceAll('\n', '<br/>');
    }
};

$(document).ready(function () {
    userTicket.init();
});