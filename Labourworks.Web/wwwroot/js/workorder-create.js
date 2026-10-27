$(document).ready(function () {

    setDefaultWorkOrderDate();

    loadNextNumbers();

    loadCompanies();

    loadCustomers();

    loadModels();

    $('#WorkOrderDate').on('change', function () {
        loadNextNumbers();
    });

    $('#CompanyId').on('change', function () {

        let companyId = $(this).val();

        $('#PlantId')
            .empty()
            .append('<option value="">-- Select Plant --</option>')
            .prop('disabled', true);

        $('#PlantIdError').text('');

        if (companyId) {
            loadPlants(companyId);
        }
    });

    $('#btnSave').click(function () {

        clearErrors();

        if (!validateForm()) {
            return;
        }

        let button = $('#btnSave');

        button.prop('disabled', true);

        button.html(
            '<span class="spinner-border spinner-border-sm"></span> Saving...'
        );

        let token =
            $('#workOrderForm input[name="__RequestVerificationToken"]')
                .val();

        let data = {

            companyId:
                parseInt($('#CompanyId').val()),

            plantId:
                parseInt($('#PlantId').val()),

            poNumber:
                $('#PONumber').val().trim(),

            customerId:
                parseInt($('#CustomerId').val()),

            modelId:
                parseInt($('#ModelId').val()),

            quantity:
                parseInt($('#Quantity').val()),

            workOrderDate:
                $('#WorkOrderDate').val(),

            status:
                $('#Status').val()
        };

        $.ajax({

            url: '/WorkOrder/Create',

            type: 'POST',

            contentType:
                'application/json; charset=utf-8',

            headers: {
                'RequestVerificationToken': token
            },

            data:
                JSON.stringify(data),

            success: function (response) {

                if (response.success) {

                    $('#WorkOrderNo')
                        .val(response.data.workOrderNo);

                    $('#LotNo')
                        .val(response.data.lotNo);

                    alert(
                        response.message +
                        '\n\nWork Order No: ' +
                        response.data.workOrderNo +
                        '\nLot No: ' +
                        response.data.lotNo
                    );

                    window.location.href =
                        '/WorkOrder/Index';
                }
                else {

                    alert(response.message);
                }
            },

            error: function (xhr) {

                let message =
                    xhr.responseJSON?.message ||
                    'Something went wrong.';

                alert(message);
            },

            complete: function () {

                button.prop(
                    'disabled',
                    false
                );

                button.html(
                    '<i class="bi bi-check-circle"></i> Save'
                );
            }
        });
    });
});


function setDefaultWorkOrderDate() {

    let today = new Date();

    let year =
        today.getFullYear();

    let month =
        String(today.getMonth() + 1)
            .padStart(2, '0');

    let day =
        String(today.getDate())
            .padStart(2, '0');

    let todayDate =
        year + '-' +
        month + '-' +
        day;

    $('#WorkOrderDate').val(todayDate);
}


function loadNextNumbers() {

    let workOrderDate =
        $('#WorkOrderDate').val();

    if (!workOrderDate) {
        return;
    }

    $.ajax({

        url: '/WorkOrder/GetNextNumbers',

        type: 'GET',

        data: {
            workOrderDate: workOrderDate
        },

        success: function (response) {

            if (response.success) {

                $('#WorkOrderNo')
                    .val(response.workOrderNo);

                $('#LotNo')
                    .val(response.lotNo);
            }
            else {

                $('#WorkOrderNo').val('');

                $('#LotNo').val('');

                alert(response.message);
            }
        },

        error: function () {

            $('#WorkOrderNo').val('');

            $('#LotNo').val('');

            alert(
                'Unable to generate Work Order Number.'
            );
        }
    });
}


function loadCompanies() {

    $.ajax({

        url: '/WorkOrder/GetCompanies',

        type: 'GET',

        success: function (response) {

            if (!response.success) {

                alert(response.message);

                return;
            }

            let ddl =
                $('#CompanyId');

            ddl.empty();

            ddl.append(
                '<option value="">-- Select Company --</option>'
            );

            $.each(
                response.data,
                function (index, item) {

                    ddl.append(
                        $('<option>')
                            .val(item.companyId)
                            .text(item.companyName)
                    );
                }
            );
        },

        error: function (xhr) {

            console.log(xhr.responseText);

            alert(
                'Unable to load companies.'
            );
        }
    });
}


function loadPlants(companyId) {

    $.ajax({

        url: '/WorkOrder/GetPlants',

        type: 'GET',

        data: {
            companyId: companyId
        },

        success: function (response) {

            if (!response.success) {

                alert(response.message);

                return;
            }

            let ddl =
                $('#PlantId');

            ddl.empty();

            ddl.append(
                '<option value="">-- Select Plant --</option>'
            );

            $.each(
                response.data,
                function (index, item) {

                    ddl.append(
                        $('<option>')
                            .val(item.plantId)
                            .text(item.plantName)
                    );
                }
            );

            ddl.prop(
                'disabled',
                false
            );
        },

        error: function (xhr) {

            console.log(xhr.responseText);

            alert(
                'Unable to load plants.'
            );
        }
    });
}


function loadCustomers() {

    $.ajax({

        url: '/WorkOrder/GetCustomers',

        type: 'GET',

        success: function (response) {

            if (!response.success) {

                alert(response.message);

                return;
            }

            let ddl =
                $('#CustomerId');

            ddl.empty();

            ddl.append(
                '<option value="">-- Select Customer --</option>'
            );

            $.each(
                response.data,
                function (index, item) {

                    ddl.append(
                        $('<option>')
                            .val(item.customerId)
                            .text(item.customerName)
                    );
                }
            );
        },

        error: function () {

            alert(
                'Unable to load customers.'
            );
        }
    });
}


function loadModels() {

    $.ajax({

        url: '/WorkOrder/GetModels',

        type: 'GET',

        success: function (response) {

            if (!response.success) {

                alert(response.message);

                return;
            }

            let ddl =
                $('#ModelId');

            ddl.empty();

            ddl.append(
                '<option value="">-- Select Model --</option>'
            );

            $.each(
                response.data,
                function (index, item) {

                    ddl.append(
                        $('<option>')
                            .val(item.modelId)
                            .text(item.modelName)
                    );
                }
            );
        },

        error: function () {

            alert(
                'Unable to load models.'
            );
        }
    });
}


function validateForm() {

    let valid = true;

    let companyId =
        $('#CompanyId').val();

    let plantId =
        $('#PlantId').val();

    let poNumber =
        $('#PONumber').val().trim();

    let customerId =
        $('#CustomerId').val();

    let modelId =
        $('#ModelId').val();

    let quantity =
        parseInt($('#Quantity').val());

    let workOrderDate =
        $('#WorkOrderDate').val();


    if (!companyId) {

        $('#CompanyIdError')
            .text(
                'Please select company.'
            );

        valid = false;
    }


    if (!plantId) {

        $('#PlantIdError')
            .text(
                'Please select plant.'
            );

        valid = false;
    }


    if (!poNumber) {

        $('#PONumberError')
            .text(
                'Please enter P.O. Number.'
            );

        valid = false;
    }


    if (!customerId) {

        $('#CustomerIdError')
            .text(
                'Please select customer.'
            );

        valid = false;
    }


    if (!modelId) {

        $('#ModelIdError')
            .text(
                'Please select model.'
            );

        valid = false;
    }


    if (!quantity || quantity <= 0) {

        $('#QuantityError')
            .text(
                'Please enter quantity greater than 0.'
            );

        valid = false;
    }


    if (!workOrderDate) {

        $('#WorkOrderDateError')
            .text(
                'Please select work order date.'
            );

        valid = false;
    }


    return valid;
}


function clearErrors() {

    $('#CompanyIdError').text('');

    $('#PlantIdError').text('');

    $('#PONumberError').text('');

    $('#CustomerIdError').text('');

    $('#ModelIdError').text('');

    $('#QuantityError').text('');

    $('#WorkOrderDateError').text('');

    $('#StatusError').text('');
}