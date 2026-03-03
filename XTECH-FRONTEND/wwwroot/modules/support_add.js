// support_add.js

const replyEditor = {
    _files: [],
    _objectUrls: new Map(), // localId -> blob url (for images)

    el() { return document.getElementById('replyEditor'); },
    previewEl() { return document.getElementById('replyAttachments'); },

    focus() { this.el()?.focus(); },

    cmd(command) {
        this.focus();
        document.execCommand(command, false, null);
    },

    insertLink() {
        this.focus();
        const selected = (window.getSelection()?.toString() || '').trim();

        let url = prompt('Nhập link (https://...)');
        if (!url) return;
        if (!/^https?:\/\//i.test(url)) url = 'https://' + url;

        if (selected) {
            document.execCommand('createLink', false, url);
        } else {
            document.execCommand('insertHTML', false,
                `<a href="${url}" target="_blank" rel="noopener noreferrer">${this._escape(url)}</a>`
            );
        }
    },

    addFiles(fileList) {
        if (!fileList || fileList.length === 0) return;

        const arr = Array.from(fileList);
        for (const f of arr) {
            const id = (crypto?.randomUUID)
                ? crypto.randomUUID()
                : (Date.now() + '_' + Math.random().toString(16).slice(2));

            f.__localId = id;
            this._files.push(f);
        }

        this.renderPreviews();
        this._updateAttachStatus();
    },

    removeFile(localId) {
        const u = this._objectUrls.get(localId);
        if (u) {
            URL.revokeObjectURL(u);
            this._objectUrls.delete(localId);
        }

        this._files = this._files.filter(f => f.__localId !== localId);
        this.renderPreviews();
        this._updateAttachStatus();
    },

    renderPreviews() {
        const el = this.previewEl();
        if (!el) return;

        if (!this._files.length) {
            el.innerHTML = '';
            return;
        }

        el.innerHTML = this._files.map(f => this._renderPreviewItem(f)).join('');
    },

    _renderPreviewItem(f) {
        const id = f.__localId;
        const name = this._escape(f.name || 'file');
        const sizeText = this._formatBytes(f.size || 0);

        const isImg = (f.type || '').toLowerCase().startsWith('image/');
        if (isImg) {
            let url = this._objectUrls.get(id);
            if (!url) {
                url = URL.createObjectURL(f);
                this._objectUrls.set(id, url);
            }

            return `
        <div class="preview-card" data-file-id="${id}">
          <div class="preview-thumb">
            <img src="${url}" alt="${name}" />
          </div>
          <div class="preview-meta">
            <div class="preview-name">${name}</div>
            <div class="preview-sub">${sizeText}</div>
          </div>
          <div class="preview-remove" title="Remove" onclick="replyEditor.removeFile('${id}')">&times;</div>
        </div>
      `;
        }

        const iconClass = this._iconForFile(f);
        const ext = this._getExt(f.name);

        return `
      <div class="preview-card" data-file-id="${id}">
        <div class="preview-thumb">
          <i class="bi ${iconClass}" style="font-size:24px;"></i>
        </div>
        <div class="preview-meta">
          <div class="preview-name">${name}</div>
          <div class="preview-sub">${ext} • ${sizeText}</div>
        </div>
        <div class="preview-remove" title="Remove" onclick="replyEditor.removeFile('${id}')">&times;</div>
      </div>
    `;
    },

    // ✅ trước khi submit form
    beforeSendToForm() {
        const html = (this.el().innerHTML || '').trim();
        document.getElementById('content').value = html;

        const dt = new DataTransfer();
        for (const f of this._files) dt.items.add(f);
        document.getElementById('attachFiles').files = dt.files;
    },

    _updateAttachStatus() {
        const el = document.getElementById('attachStatus');
        if (!el) return;
        el.textContent = this._files.length ? `Attached ${this._files.length} file(s)` : '';
    },

    _iconForFile(f) {
        const name = (f.name || '').toLowerCase();
        const type = (f.type || '').toLowerCase();

        if (type === 'application/pdf' || name.endsWith('.pdf')) return 'bi-file-earmark-pdf';
        if (name.endsWith('.doc') || name.endsWith('.docx')) return 'bi-file-earmark-word';
        if (name.endsWith('.xls') || name.endsWith('.xlsx')) return 'bi-file-earmark-excel';
        if (name.endsWith('.zip') || name.endsWith('.rar') || name.endsWith('.7z')) return 'bi-file-earmark-zip';
        if (type.startsWith('image/')) return 'bi-image';
        return 'bi-file-earmark';
    },

    _getExt(name) {
        if (!name) return 'FILE';
        const parts = name.split('.');
        return (parts.length > 1 ? parts.pop() : 'FILE').toUpperCase();
    },

    _escape(s) {
        return (s || '').replace(/[&<>"']/g, m => ({
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#39;'
        }[m]));
    },

    _formatBytes(bytes) {
        if (!bytes) return '0 B';
        const k = 1024;
        const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        const v = bytes / Math.pow(k, i);
        return `${v.toFixed(v >= 10 || i === 0 ? 0 : 1)} ${sizes[i]}`;
    }
};

var supportAdd = {
    init: function () {
        var $form = $('#createTicketForm');
        if (!$form.length) return;

        $form.on('submit', function (e) {
            replyEditor.beforeSendToForm();

            var ok = supportAdd.validate();
            if (!ok) {
                e.preventDefault();
                return false;
            }
            return true;
        });
    },

    validate: function () {
        var serviceId = $('#serviceId').val();
        var subject = ($('#subject').val() || '').trim();
        var content = ($('#content').val() || '').trim();
        var files = document.getElementById('attachFiles')?.files || [];

        $('#errService,#errSubject,#errContent').hide();

        var ok = true;
        if (!serviceId) { $('#errService').show(); ok = false; }
        if (!subject) { $('#errSubject').show(); ok = false; }
        if (!content) { $('#errContent').show(); ok = false; }

        // total 25MB
        var total = 0;
        for (var i = 0; i < files.length; i++) total += files[i].size || 0;
        if (total > 25 * 1024 * 1024) {
            alert('Tổng dung lượng file vượt quá 25MB.');
            ok = false;
        }

        return ok;
    }
};

$(document).ready(function () {
    supportAdd.init();
});