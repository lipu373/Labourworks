// Master Pages Scripts

// DataTable Configuration
$(document).ready(function () {
    if ($.fn.DataTable) {
        $('.datatable').DataTable({
            pageLength: 10,
            responsive: true,
            language: {
                search: 'Search:',
                lengthMenu: 'Show _MENU_ entries',
                info: 'Showing _START_ to _END_ of _TOTAL_ entries',
                infoEmpty: 'No entries found',
                infoFiltered: '(filtered from _MAX_ total entries)',
                zeroRecords: 'No matching records found'
            }
        });
    }
});

// Form Validation Helper
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return true;

    const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;

    inputs.forEach(input => {
        input.classList.remove('is-invalid');
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        }
    });

    return isValid;
}

// Generate Code
function generateCode(prefix, callback) {
    if (typeof callback === 'function') {
        callback(`${prefix}${String(Math.floor(Math.random() * 9000) + 1000)}`);
    }
    return `${prefix}${String(Math.floor(Math.random() * 9000) + 1000)}`;
}

// Export Table Data
function exportTableToCSV(tableId, fileName = 'export') {
    const table = document.getElementById(tableId);
    if (!table) return;

    let csv = [];
    const rows = table.querySelectorAll('tr');

    for (let row of rows) {
        const cols = row.querySelectorAll('td, th');
        const rowData = [];
        for (let col of cols) {
            let text = col.textContent.trim();
            // Remove icons, buttons, and badges
            text = text.replace(/[^\w\s,.-]/g, '');
            rowData.push(text);
        }
        csv.push(rowData.join(','));
    }

    const blob = new Blob([csv.join('\n')], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${fileName}_${new Date().toISOString().slice(0, 10)}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
}

// Refresh Page with Loading
function refreshPage() {
    const overlay = document.createElement('div');
    overlay.className = 'spinner-overlay';
    overlay.innerHTML = `<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>`;
    document.body.appendChild(overlay);

    setTimeout(() => {
        location.reload();
    }, 500);
}

// Toggle Status
function toggleStatus(element, id, url) {
    const badge = element.closest('tr').querySelector('.badge');
    const isActive = badge.classList.contains('bg-success');

    if (confirm(`Are you sure you want to ${isActive ? 'deactivate' : 'activate'} this record?`)) {
        $.ajax({
            url: `${url}/${id}`,
            type: 'POST',
            data: { active: !isActive },
            success: function (result) {
                if (result.success) {
                    if (isActive) {
                        badge.className = 'badge bg-danger';
                        badge.textContent = 'Inactive';
                    } else {
                        badge.className = 'badge bg-success';
                        badge.textContent = 'Active';
                    }
                    showToast('Status updated successfully!');
                } else {
                    alert(result.message || 'Error updating status');
                }
            },
            error: function () {
                alert('Error updating status');
            }
        });
    }
}

// Auto-generate fields
document.addEventListener('DOMContentLoaded', function () {
    // Auto-generate code fields
    const codeFields = document.querySelectorAll('[data-auto-generate]');
    codeFields.forEach(field => {
        const prefix = field.dataset.prefix || 'CODE';
        if (!field.value) {
            field.value = generateCode(prefix);
        }
    });

    // Apply status colors
    document.querySelectorAll('.badge').forEach(badge => {
        const text = badge.textContent.toLowerCase().trim();
        if (text.includes('active') || text.includes('yes')) {
            badge.className = 'badge bg-success';
        } else if (text.includes('inactive') || text.includes('no')) {
            badge.className = 'badge bg-danger';
        } else if (text.includes('pending')) {
            badge.className = 'badge bg-warning text-dark';
        } else if (text.includes('closed')) {
            badge.className = 'badge bg-secondary';
        } else if (text.includes('critical') || text.includes('high')) {
            badge.className = 'badge bg-danger';
        } else if (text.includes('medium')) {
            badge.className = 'badge bg-warning text-dark';
        } else if (text.includes('low')) {
            badge.className = 'badge bg-info';
        }
    });
});

// Export functions to global scope
window.showToast = showToast;
window.confirmDelete = confirmDelete;
window.formatDate = formatDate;
window.formatCurrency = formatCurrency;
window.validateForm = validateForm;
window.generateCode = generateCode;
window.exportTableToCSV = exportTableToCSV;
window.refreshPage = refreshPage;
window.toggleStatus = toggleStatus;