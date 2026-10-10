// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Reusable modal: Details / Add / Edit / Delete are partial views loaded into #crudModal (no new page).
(function () {
    const modalEl = document.getElementById('crudModal');
    const content = document.getElementById('crudModalContent');
    if (!modalEl || !content || typeof bootstrap === 'undefined') return;

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    const ajaxHeaders = { 'X-Requested-With': 'XMLHttpRequest' };

    async function openModal(url) {
        try {
            const response = await fetch(url, { headers: ajaxHeaders });
            if (!response.ok) throw new Error('HTTP ' + response.status);
            content.innerHTML = await response.text();
            modal.show();
        } catch (error) {
            alert('The requested information could not be loaded.');
        }
    }

    // Links marked with data-modal open their partial view inside the modal.
    document.addEventListener('click', function (e) {
        const link = e.target.closest('a[data-modal]');
        if (!link) return;
        e.preventDefault();
        openModal(link.href);
    });

    // Forms marked with data-modal-form are posted with fetch.
    // Success -> JSON { success: true } and the list is reloaded; otherwise the partial is re-rendered with its errors.
    document.addEventListener('submit', async function (e) {
        const form = e.target.closest('form[data-modal-form]');
        if (!form) return;
        e.preventDefault();

        const submit = form.querySelector('[type="submit"]');
        if (submit) submit.disabled = true;

        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: ajaxHeaders
            });
            const type = response.headers.get('content-type') || '';

            if (type.includes('application/json')) {
                const result = await response.json();
                if (result.success) {
                    modal.hide();
                    window.location.reload();
                    return;
                }
            } else if (response.ok) {
                content.innerHTML = await response.text();
                return;
            }
            throw new Error('HTTP ' + response.status);
        } catch (error) {
            alert('The operation could not be completed.');
            if (submit) submit.disabled = false;
        }
    });
})();
