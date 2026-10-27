
$(document).ready(function () {

    const baseUrl = '/PlantMaster';

    // Open Add Plant form
    $('#btnAddPlant').on('click', function () {
        clearForm();
        $('#formTitle').text('Add Plant');
        $('#plantFormCard').slideDown();
        $('#PlantName').trigger('focus');
    });

    // Cancel form
    $('#btnCancel').on('click', function () {
        clearForm();
        $('#plantFormCard').slideUp();
    });

    // Load plant list
    loadPlants();

    // Save Plant
    $('#btnSave').on('click', function () {

        const plantName = $('#PlantName').val().trim();
        const plantId = parseInt($('#PlantId').val()) || 0;

        $('#PlantNameError').text('');
        $('#msg').empty();

        if (plantName === '') {
            $('#PlantNameError').text('Plant Name is required.');
            $('#PlantName').trigger('focus');
            return;
        }

        const token = $('input[name="__RequestVerificationToken"]').val();

        const data = {
            PlantId: plantId,
            PlantName: plantName
        };

        $('#btnSave').prop('disabled', true);

        $.ajax({
            url: baseUrl + '/Save',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {

                if (response.success === false) {
                    showMessage('danger', response.message || 'Unable to save plant.');
                    return;
                }

                showMessage('success', response.message || 'Plant saved successfully.');

                clearForm();
                $('#plantFormCard').slideUp();
                loadPlants();
            },
            error: function (xhr) {
                const message = xhr.responseJSON?.message
                    || 'Error while saving plant. Please check the controller.';
                showMessage('danger', message);
            },
            complete: function () {
                $('#btnSave').prop('disabled', false);
            }
        });
    });

    // Load Plant Details
    function loadPlants() {

        $.ajax({
            url: baseUrl + '/GetAll',
            type: 'GET',
            dataType: 'json',
            success: function (response) {

                const plants = Array.isArray(response)
                    ? response
                    : (response.data || []);

                const tbody = $('#plantTableBody');
                tbody.empty();

                if (plants.length === 0) {
                    tbody.append(`
                        <tr>
                            <td colspan="4" class="text-center">
                                No plant records found.
                            </td>
                        </tr>
                    `);
                    return;
                }

                $.each(plants, function (index, plant) {

                    const id = plant.plantId ?? plant.PlantId;
                    const name = plant.plantName ?? plant.PlantName ?? '';
                    const active = plant.isActive ?? plant.IsActive ?? true;

                    const status = active
                        ? '<span class="badge bg-success">Active</span>'
                        : '<span class="badge bg-secondary">Inactive</span>';

                    const row = $('<tr>');

                    row.append($('<td>').text(index + 1));
                    row.append($('<td>').text(name));
                    row.append($('<td>').html(status));

                    const actionCell = $('<td>');

                    const editButton = $('<button>')
                        .attr('type', 'button')
                        .addClass('btn btn-sm btn-warning me-2')
                        .html('<i class="bi bi-pencil-square"></i> Edit')
                        .on('click', function () {
                            editPlant(id);
                        });

                    const deleteButton = $('<button>')
                        .attr('type', 'button')
                        .addClass('btn btn-sm btn-danger')
                        .html('<i class="bi bi-trash"></i> Delete')
                        .on('click', function () {
                            deletePlant(id);
                        });

                    actionCell.append(editButton, deleteButton);
                    row.append(actionCell);
                    tbody.append(row);
                });
            },
            error: function () {
                $('#plantTableBody').html(`
                    <tr>
                        <td colspan="4" class="text-center text-danger">
                            Unable to load plant details.
                        </td>
                    </tr>
                `);
            }
        });
    }

    // Edit Plant
    function editPlant(id) {

        $.ajax({
            url: baseUrl + '/GetById',
            type: 'GET',
            data: { id: id },
            success: function (response) {

                const plant = response.data || response;

                $('#PlantId').val(plant.plantId ?? plant.PlantId ?? id);
                $('#PlantName').val(plant.plantName ?? plant.PlantName ?? '');
                $('#PlantNameError').text('');
                $('#msg').empty();

                $('#formTitle').text('Edit Plant');
                $('#plantFormCard').slideDown();

                $('html, body').animate({
                    scrollTop: $('#plantFormCard').offset().top - 80
                }, 300);
            },
            error: function () {
                alert('Unable to retrieve plant details.');
            }
        });
    }

    // Delete Plant
    function deletePlant(id) {

        if (!confirm('Are you sure you want to delete this plant?')) {
            return;
        }

        $.ajax({
            url: baseUrl + '/Delete',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id: id }),
            headers: {
                'RequestVerificationToken':
                    $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {

                if (response.success === false) {
                    alert(response.message || 'Unable to delete plant.');
                    return;
                }

                alert(response.message || 'Plant deleted successfully.');
                loadPlants();
            },
            error: function () {
                alert('Error while deleting plant.');
            }
        });
    }

    // Clear form
    function clearForm() {
        $('#PlantId').val(0);
        $('#PlantName').val('');
        $('#PlantNameError').text('');
        $('#msg').empty();
    }

    // Display message
    function showMessage(type, message) {
        $('#msg').html(
            $('<div>')
                .addClass('alert alert-' + type + ' alert-dismissible fade show')
                .attr('role', 'alert')
                .text(message)
        );

        // Add a close button safely
        $('#msg .alert').append(`
            <button type="button" class="btn-close"
                    data-bs-dismiss="alert"
                    aria-label="Close"></button>
        `);
    }
});
