(function () {
    var tokenInput = document.querySelector('#userStatusAntiForgeryForm input[name="__RequestVerificationToken"]');
    var notification = document.getElementById('userStatusNotification');
    var toggles = document.querySelectorAll('.js-user-status-toggle');

    if (!toggles.length || !tokenInput) {
        return;
    }

    toggles.forEach(function (toggle) {
        toggle.addEventListener('change', function () {
            var checkbox = toggle;
            var previousValue = !checkbox.checked;
            var userId = Number(checkbox.getAttribute('data-user-id'));
            var url = checkbox.getAttribute('data-toggle-url');
            var kind = checkbox.getAttribute('data-toggle-kind');
            var payload = { userId: userId };

            if (kind === 'active') {
                payload.isActive = checkbox.checked;
            } else {
                payload.isLeft = checkbox.checked;
            }

            checkbox.disabled = true;

            fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': tokenInput.value
                },
                body: JSON.stringify(payload)
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Status update failed.');
                    }

                    return response.json();
                })
                .then(function (result) {
                    if (!result.success) {
                        checkbox.checked = previousValue;
                        showNotification(result.message || 'Status update failed.', false);
                        return;
                    }

                    checkbox.checked = !!result.value;
                    showNotification(result.message || 'Status updated.', true);
                })
                .catch(function (error) {
                    checkbox.checked = previousValue;
                    showNotification(error.message || 'Status update failed.', false);
                })
                .finally(function () {
                    checkbox.disabled = false;
                });
        });
    });

    function showNotification(message, isSuccess) {
        if (!notification) {
            return;
        }

        notification.textContent = message;
        notification.className = isSuccess ? 'alert alert-success' : 'alert alert-danger';
    }
})();
