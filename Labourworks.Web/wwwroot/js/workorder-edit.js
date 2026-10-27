$(document).ready(function () {

    loadCompanies();
    loadCustomers();
    loadModels();

    $('#CompanyId').on('change', function () {

        let companyId = $(this).val();

        if (!companyId) {

            $('#PlantId')
                .empty()
                .append('<option value="">-- Select Plant --</option>')
                .prop('disabled', true);

            return;
        }

        loadPlants(companyId);
    });


    function loadCompanies() {

        $.ajax({

            url: '/WorkOrder/GetCompanies',

            type: 'GET',

            success: function (response) {

                if (!response.success) {

                    alert(response.message);
                    return;
                }

                let ddl = $('#CompanyId');

                let selectedId =
                    ddl.attr('data-selected');

                ddl.empty();

                ddl.append(
                    '<option value="">-- Select Company --</option>'
                );

                $.each(
                    response.data,
                    function (index, item) {

                        let option =
                            $('<option></option>')
                                .val(item.companyId)
                                .text(item.companyName);

                        if (
                            item.companyId.toString() ===
                            selectedId
                        ) {

                            option.prop(
                                'selected',
                                true
                            );
                        }

                        ddl.append(option);
                    }
                );

                if (selectedId) {

                    loadPlants(selectedId);
                }
            },

            error: function () {

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

                let ddl = $('#PlantId');

                let selectedId =
                    ddl.attr('data-selected');

                ddl.empty();

                ddl.append(
                    '<option value="">-- Select Plant --</option>'
                );

                $.each(
                    response.data,
                    function (index, item) {

                        let option =
                            $('<option></option>')
                                .val(item.plantId)
                                .text(item.plantName);

                        if (
                            item.plantId.toString() ===
                            selectedId
                        ) {

                            option.prop(
                                'selected',
                                true
                            );
                        }

                        ddl.append(option);
                    }
                );

                ddl.prop('disabled', false);
            },

            error: function () {

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

                let ddl = $('#CustomerId');

                let selectedId =
                    ddl.attr('data-selected');

                ddl.empty();

                ddl.append(
                    '<option value="">-- Select Customer --</option>'
                );

                $.each(
                    response.data,
                    function (index, item) {

                        let option =
                            $('<option></option>')
                                .val(item.customerId)
                                .text(item.customerName);

                        if (
                            item.customerId.toString() ===
                            selectedId
                        ) {

                            option.prop(
                                'selected',
                                true
                            );
                        }

                        ddl.append(option);
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

                let ddl = $('#ModelId');

                let selectedId =
                    ddl.attr('data-selected');

                ddl.empty();

                ddl.append(
                    '<option value="">-- Select Model --</option>'
                );

                $.each(
                    response.data,
                    function (index, item) {

                        let option =
                            $('<option></option>')
                                .val(item.modelId)
                                .text(item.modelName);

                        if (
                            item.modelId.toString() ===
                            selectedId
                        ) {

                            option.prop(
                                'selected',
                                true
                            );
                        }

                        ddl.append(option);
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


    $('#btnUpdate').click(function () {

        clearErrors();

        if (!validateForm()) {

            return;
        }

        let button = $('#btnUpdate');

        button.prop(
            'disabled',
            true
        );

        button.html(
            '<span class="spinner-border spinner-border-sm"></span> Updating...'
        );

        let token =
            $('#editWorkOrderForm input[name="__RequestVerificationToken"]')
                .val();

        let data = {

            workOrderId:
                parseInt(
                    $('#WorkOrderId').val()
                ),

            companyId:
                parseInt(
                    $('#CompanyId').val()
                ),

            plantId:
                parseInt(
                    $('#PlantId').val()
                ),

            poNumber:
                $('#PONumber').val().trim(),

            customerId:
                parseInt(
                    $('#CustomerId').val()
                ),

            modelId:
                parseInt(
                    $('#ModelId').val()
                ),

            quantity:
                parseInt(
                    $('#Quantity').val()
                ),

            workOrderDate:
                $('#WorkOrderDate').val(),

            status:
                $('#Status').val()
        };


        $.ajax({

            url: '/WorkOrder/Update',

            type: 'POST',

            contentType:
                'application/json; charset=utf-8',

            headers: {

                'RequestVerificationToken':
                    token
            },

            data:
                JSON.stringify(data),

            success: function (response) {

                if (response.success) {

                    alert(
                        response.message
                    );

                    window.location.href =
                        '/WorkOrder/Index';
                }
                else {

                    alert(
                        response.message
                    );
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
                    '<i class="bi bi-check-circle"></i> Update Work Order'
                );
            }
        });
    });


    function validateForm() {

        let valid = true;


        if (!$('#CompanyId').val()) {

            $('#CompanyIdError')
                .text(
                    'Please select company.'
                );

            valid = false;
        }


        if (!$('#PlantId').val()) {

            $('#PlantIdError')
                .text(
                    'Please select plant.'
                );

            valid = false;
        }


        if (!$('#PONumber').val().trim()) {

            $('#PONumberError')
                .text(
                    'Please enter P.O. Number.'
                );

            valid = false;
        }


        if (!$('#CustomerId').val()) {

            $('#CustomerIdError')
                .text(
                    'Please select customer.'
                );

            valid = false;
        }


        if (!$('#ModelId').val()) {

            $('#ModelIdError')
                .text(
                    'Please select model.'
                );

            valid = false;
        }


        let quantity =
            parseInt(
                $('#Quantity').val()
            );

        if (!quantity || quantity <= 0) {

            $('#QuantityError')
                .text(
                    'Please enter quantity greater than 0.'
                );

            valid = false;
        }


        if (!$('#WorkOrderDate').val()) {

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
    }

});