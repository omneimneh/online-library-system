(function () {
    $(document).ready(function () {
        var url = document.location.toString();
        if (url.match('#')) {
            $('.nav-tabs a[href="#' + url.split('#')[1] + '"]').tab('show');
        }
        $('#editSubmit').click(function () {
            $.ajax({
                type: "post",
                url: "api/AccountApi/UpdateProfile",
                data: {
                    Token: $('#Token').val(),
                    Email: $('#email').val(),
                    Phone: $('#phone').val()
                },
                success: function (ret) {
                    if (ret) {
                        if (window.location.href === '/Account/Profile') {
                            window.location.reload(true);
                        }
                        window.location.href = '/Account/Profile';
                    } else {
                        Alert(_errorMsg, 'Make sure your input is valid!');
                    }
                },
                error: function () {
                    console.log('Something went wrong!');
                }
            });
        });
    });
})();

function edit() {
    $('input.editable').each(function () {
        var $this = $(this);
        $this.removeClass('form-control-plaintext');
        $this.addClass('form-control');
        $this.attr('readonly', null);
    });
    $('#editCancel').removeClass('hidden');
    $('#editSubmit').removeClass('hidden');
}

function cancelEdit() {
    $('input.editable').each(function () {
        var $this = $(this);
        $this.addClass('form-control-plaintext');
        $this.removeClass('form-control');
        $this.attr('readonly', true);
    });
    $('#editCancel').addClass('hidden');
    $('#editSubmit').addClass('hidden');
}