var supportIndex = {
    init: function () {
        supportIndex.bindRowClick();
        supportIndex.bindTabs();
        supportIndex.updateCount();
    },

    bindRowClick: function () {
        // click cả row -> vào detail
        $('#ticketTable').on('click', 'tbody tr.row-link', function (e) {
            // nếu click vào dropdown/button thì không redirect
            if ($(e.target).closest('.dropdown').length > 0) return;

            var id = $(this).data('ticket-id');
            if (!id) return;
            window.location.href = '/support/' + id;
        });
    },

    bindTabs: function () {
        $('#ticketTabs').on('click', 'button[data-status]', function () {
            // active style
            $('#ticketTabs button').removeClass('active text-primary-custom').addClass('text-secondary');
            $(this).addClass('active text-primary-custom').removeClass('text-secondary');

            var status = ($(this).data('status') || 'all').toString().toLowerCase();
            supportIndex.filter(status);
        });
    },

    filter: function (status) {
        var $rows = $('#ticketTable tbody tr.row-link');

        $rows.each(function () {
            var rowStatus = ($(this).data('status') || '').toString().toLowerCase();

            if (status === 'all') {
                $(this).show();
            } else {
                // normalize open/inprogress/resolved
                var ok = rowStatus.indexOf(status) >= 0;
                // special case inprogress có thể là "inprogress" / "in_progress" / "processing"
                if (!ok && status === 'inprogress') {
                    ok = rowStatus.indexOf('progress') >= 0 || rowStatus.indexOf('processing') >= 0;
                }
                $(this).toggle(ok);
            }
        });

        supportIndex.updateCount();
    },

    updateCount: function () {
        var visible = $('#ticketTable tbody tr.row-link:visible').length;
        $('#ticketCountText').html('Showing <span class="fw-semibold">' + visible + '</span> ticket(s)');
    }
};

$(document).ready(function () {
    supportIndex.init();
});