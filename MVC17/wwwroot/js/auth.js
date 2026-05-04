$(document).ready(function() {
    // Basic interaction for auth pages
    console.log('Auth module initialized.');

    // Loading state for forms
    $('form').on('submit', function() {
        var form = $(this);
        // Only apply if form is valid (if jquery validation is loaded)
        if (form.valid && !form.valid()) {
            return false;
        }
        var btn = form.find('button[type="submit"]');
        if (!btn.data('original-text')) {
            btn.data('original-text', btn.html());
        }
        btn.prop('disabled', true);
        btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
    });
});
