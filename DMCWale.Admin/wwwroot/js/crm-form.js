(function () {
    var forms = document.querySelectorAll('.crm-validated-form');
    if (!forms.length) {
        return;
    }

    forms.forEach(function (form) {
        form.addEventListener('submit', function (event) {
            var isValid = true;
            var fields = form.querySelectorAll('.crm-field');

            fields.forEach(function (field) {
                clearFieldError(field);

                var label = field.getAttribute('data-crm-label') || 'Field';
                var type = field.getAttribute('data-crm-type') || field.getAttribute('type') || 'text';
                var isRequired = field.getAttribute('data-crm-required') === 'true';
                var value = (field.value || '').trim();

                if (isRequired && !value) {
                    setFieldError(field, label + ' is required.');
                    isValid = false;
                    return;
                }

                if (!value) {
                    return;
                }

                if (type === 'number' && isNaN(Number(value))) {
                    setFieldError(field, label + ' must be a valid number.');
                    isValid = false;
                    return;
                }

                if (type === 'date' && isNaN(Date.parse(value))) {
                    setFieldError(field, label + ' must be a valid date.');
                    isValid = false;
                    return;
                }

                if (field.name && field.name.toLowerCase().indexOf('email') >= 0) {
                    var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                    if (!emailPattern.test(value)) {
                        setFieldError(field, label + ' must be a valid email address.');
                        isValid = false;
                    }
                }
            });

            if (!isValid) {
                event.preventDefault();
                event.stopPropagation();
                var firstInvalid = form.querySelector('.has-error .crm-field');
                if (firstInvalid) {
                    firstInvalid.focus();
                }
            }
        });

        form.querySelectorAll('.crm-field').forEach(function (field) {
            field.addEventListener('input', function () { clearFieldError(field); });
            field.addEventListener('change', function () { clearFieldError(field); });
        });
    });

    function setFieldError(field, message) {
        var group = field.closest('.crm-field-group');
        if (!group) {
            return;
        }

        group.classList.add('has-error');
        var error = group.querySelector('.crm-field-error');
        if (error) {
            error.textContent = message;
        }
    }

    function clearFieldError(field) {
        var group = field.closest('.crm-field-group');
        if (!group) {
            return;
        }

        group.classList.remove('has-error');
        var error = group.querySelector('.crm-field-error');
        if (error) {
            error.textContent = '';
        }
    }
})();
