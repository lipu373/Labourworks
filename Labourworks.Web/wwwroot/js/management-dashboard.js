let productionChart = null;
let qualityChart = null;
let repairChart = null;
let dispatchChart = null;


$(document).ready(function () {

    const fromDatePicker = flatpickr("#FromDate", {
        dateFormat: "d-m-Y",
        allowInput: false,
        clickOpens: true
    });

    const toDatePicker = flatpickr("#ToDate", {
        dateFormat: "d-m-Y",
        allowInput: false,
        clickOpens: true
    });

    $("#FromDateBtn").on("click", function () {
        fromDatePicker.open();
    });

    $("#ToDateBtn").on("click", function () {
        toDatePicker.open();
    });

    initializeCharts();

    loadPlants();
    loadCustomers();
    loadProductModels();
    loadWorkOrders();

    loadDashboard();

    $("#btnFilter").off("click").on("click", function (e) {
        e.preventDefault();
        loadDashboard();
    });

    $("#btnReset").off("click").on("click", function (e) {
        e.preventDefault();

        fromDatePicker.clear();
        toDatePicker.clear();

        $("#PlantId").val("").trigger("change");
        $("#CustomerId").val("").trigger("change");
        $("#ProductModelId").val("").trigger("change");
        $("#WorkOrderId").val("").trigger("change");

        loadDashboard();
    });

});


function loadPlants() {

    $.ajax({
        url: "/ManagementDashboard/GetPlants",
        type: "GET",

        success: function (data) {

            $("#PlantId")
                .empty()
                .append('<option value="">All Plants</option>');

            $.each(data, function (i, x) {

                $("#PlantId").append(
                    `<option value="${x.plantId}">${x.plantName}</option>`
                );

            });

            $("#PlantId").select2({
                placeholder: "All Plants",
                allowClear: true,
                width: "100%"
            });

        },

        error: function (xhr) {
            console.error("Error loading plants:", xhr.responseText);
        }
    });

}


function loadCustomers() {

    $.ajax({
        url: "/ManagementDashboard/GetCustomers",
        type: "GET",

        success: function (data) {

            $("#CustomerId")
                .empty()
                .append('<option value="">All Customers</option>');

            $.each(data, function (i, x) {

                $("#CustomerId").append(
                    `<option value="${x.customerId}">${x.customerName}</option>`
                );

            });

            $("#CustomerId").select2({
                placeholder: "All Customers",
                allowClear: true,
                width: "100%"
            });

        },

        error: function (xhr) {
            console.error("Error loading customers:", xhr.responseText);
        }
    });

}


function loadProductModels() {

    $.ajax({
        url: "/ManagementDashboard/GetProductModels",
        type: "GET",

        success: function (data) {

            $("#ProductModelId")
                .empty()
                .append('<option value="">All Models</option>');

            $.each(data, function (i, x) {

                $("#ProductModelId").append(
                    `<option value="${x.productModelId}">${x.modelName}</option>`
                );

            });

            $("#ProductModelId").select2({
                placeholder: "All Models",
                allowClear: true,
                width: "100%"
            });

        },

        error: function (xhr) {
            console.error("Error loading product models:", xhr.responseText);
        }
    });

}


function loadWorkOrders() {

    $.ajax({
        url: "/ManagementDashboard/GetWorkOrders",
        type: "GET",

        success: function (data) {

            $("#WorkOrderId")
                .empty()
                .append('<option value="">All Work Orders</option>');

            $.each(data, function (i, x) {

                $("#WorkOrderId").append(
                    `<option value="${x.workOrderId}">${x.workOrderNo}</option>`
                );

            });

            $("#WorkOrderId").select2({
                placeholder: "All Work Orders",
                allowClear: true,
                width: "100%"
            });

        },

        error: function (xhr) {
            console.error("Error loading work orders:", xhr.responseText);
        }
    });

}


function convertDateToIso(dateValue) {

    if (!dateValue) {
        return null;
    }

    const parts = dateValue.split("-");

    if (parts.length !== 3) {
        return null;
    }

    const day = parts[0];
    const month = parts[1];
    const year = parts[2];

    return `${year}-${month}-${day}`;
}


function loadDashboard() {

    let filter = {
        fromDate: convertDateToIso($("#FromDate").val()),
        toDate: convertDateToIso($("#ToDate").val()),
        plantId: getNullableInt("#PlantId"),
        customerId: getNullableInt("#CustomerId"),
        productModelId: getNullableInt("#ProductModelId"),
        workOrderId: getNullableLong("#WorkOrderId")
    };

    console.log("Dashboard Filter:", filter);

    let filterButton = $("#btnFilter");

    filterButton
        .prop("disabled", true)
        .html('<span class="spinner-border spinner-border-sm"></span> Loading...');

    $.ajax({

        url: "/ManagementDashboard/GetDashboard",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(filter),

        success: function (response) {

            console.log("Dashboard API Response:", response);

            if (response && response.success === true) {

                if (response.data) {
                    updateDashboard(response.data);
                }
                else {
                    resetDashboard();
                }

            }
            else {

                console.error("Dashboard API failed:", response);

                resetDashboard();

                alert(
                    response && response.message
                        ? response.message
                        : "Unable to load dashboard."
                );
            }
        },

        error: function (xhr, status, error) {

            console.error("Dashboard API Error:", xhr.responseText);

            resetDashboard();

            let message = "Unable to load dashboard.";

            if (
                xhr.responseJSON &&
                xhr.responseJSON.message
            ) {
                message = xhr.responseJSON.message;
            }

            alert(message);
        },

        complete: function () {

            filterButton
                .prop("disabled", false)
                .html('<i class="bi bi-search"></i> Apply Filter');

        }

    });

}


function getNullableInt(selector) {

    let value = $(selector).val();

    if (
        value === null ||
        value === undefined ||
        value === ""
    ) {
        return null;
    }

    let number = parseInt(value, 10);

    return isNaN(number) ? null : number;
}


function getNullableLong(selector) {

    let value = $(selector).val();

    if (
        value === null ||
        value === undefined ||
        value === ""
    ) {
        return null;
    }

    let number = parseInt(value, 10);

    return isNaN(number) ? null : number;
}


function initializeCharts() {

    const productionCtx =
        document.getElementById("productionChart");

    const qualityCtx =
        document.getElementById("qualityChart");

    const repairCtx =
        document.getElementById("repairChart");

    const dispatchCtx =
        document.getElementById("dispatchChart");


    if (!productionCtx ||
        !qualityCtx ||
        !repairCtx ||
        !dispatchCtx) {

        console.error("One or more chart canvas elements not found.");
        return;
    }


    productionChart = new Chart(productionCtx, {

        type: "bar",

        data: {
            labels: [
                "Ordered",
                "Produced",
                "Pending"
            ],

            datasets: [{
                label: "Quantity",
                data: [
                    0,
                    0,
                    0
                ],
                borderWidth: 1
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    display: false
                }
            },

            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });


    qualityChart = new Chart(qualityCtx, {

        type: "doughnut",

        data: {
            labels: [
                "LCM Bonded",
                "LCM Tested",
                "Pass",
                "Fail"
            ],

            datasets: [{
                label: "Quality",
                data: [
                    0,
                    0,
                    0,
                    0
                ],
                borderWidth: 1
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    position: "bottom"
                }
            }
        }
    });


    repairChart = new Chart(repairCtx, {

        type: "bar",

        data: {
            labels: [
                "Repaired",
                "Repair Pending"
            ],

            datasets: [{
                label: "Quantity",
                data: [
                    0,
                    0
                ],
                borderWidth: 1
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    display: false
                }
            },

            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });


    dispatchChart = new Chart(dispatchCtx, {

        type: "bar",

        data: {
            labels: [
                "Dispatched",
                "Dispatch Pending"
            ],

            datasets: [{
                label: "Quantity",
                data: [
                    0,
                    0
                ],
                borderWidth: 1
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    display: false
                }
            },

            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

}


function updateDashboard(data) {

    $("#TotalWorkOrders").text(
        formatNumber(data.totalWorkOrders)
    );

    $("#TotalOrderedQuantity").text(
        formatNumber(data.totalOrderedQuantity)
    );

    $("#TotalProducedQuantity").text(
        formatNumber(data.totalProducedQuantity)
    );

    $("#PendingQuantity").text(
        formatNumber(data.pendingQuantity)
    );

    $("#TotalLCMBonded").text(
        formatNumber(data.totalLCMBonded)
    );

    $("#TotalLCMTested").text(
        formatNumber(data.totalLCMTested)
    );

    $("#PassQuantity").text(
        formatNumber(data.passQuantity)
    );

    $("#FailQuantity").text(
        formatNumber(data.failQuantity)
    );

    $("#RepairedQuantity").text(
        formatNumber(data.repairedQuantity)
    );

    $("#RepairPending").text(
        formatNumber(data.repairPending)
    );

    $("#DispatchQuantity").text(
        formatNumber(data.dispatchQuantity)
    );

    $("#DispatchPending").text(
        formatNumber(data.dispatchPending)
    );

    $("#ProductionAchievementPercent").text(
        Number(data.productionAchievementPercent || 0).toFixed(2) + "%"
    );

    $("#FirstPassYieldPercent").text(
        Number(data.firstPassYieldPercent || 0).toFixed(2) + "%"
    );

    $("#RejectionPercent").text(
        Number(data.rejectionPercent || 0).toFixed(2) + "%"
    );

    $("#TodayProduction").text(
        formatNumber(data.todayProduction)
    );

    $("#TodayDispatch").text(
        formatNumber(data.todayDispatch)
    );


    $("#ProductionProgress").css(
        "width",
        Math.min(
            Math.max(
                Number(data.productionAchievementPercent || 0),
                0
            ),
            100
        ) + "%"
    );


    $("#YieldProgress").css(
        "width",
        Math.min(
            Math.max(
                Number(data.firstPassYieldPercent || 0),
                0
            ),
            100
        ) + "%"
    );


    $("#RejectionProgress").css(
        "width",
        Math.min(
            Math.max(
                Number(data.rejectionPercent || 0),
                0
            ),
            100
        ) + "%"
    );


    updateCharts(data);
}


function updateCharts(data) {

    if (!productionChart ||
        !qualityChart ||
        !repairChart ||
        !dispatchChart) {

        return;
    }


    productionChart.data.datasets[0].data = [

        Number(data.totalOrderedQuantity || 0),

        Number(data.totalProducedQuantity || 0),

        Number(data.pendingQuantity || 0)

    ];

    productionChart.update();


    qualityChart.data.datasets[0].data = [

        Number(data.totalLCMBonded || 0),

        Number(data.totalLCMTested || 0),

        Number(data.passQuantity || 0),

        Number(data.failQuantity || 0)

    ];

    qualityChart.update();


    repairChart.data.datasets[0].data = [

        Number(data.repairedQuantity || 0),

        Number(data.repairPending || 0)

    ];

    repairChart.update();


    dispatchChart.data.datasets[0].data = [

        Number(data.dispatchQuantity || 0),

        Number(data.dispatchPending || 0)

    ];

    dispatchChart.update();
}


function resetDashboard() {

    $("#TotalWorkOrders").text("0");
    $("#TotalOrderedQuantity").text("0");
    $("#TotalProducedQuantity").text("0");
    $("#PendingQuantity").text("0");

    $("#TotalLCMBonded").text("0");
    $("#TotalLCMTested").text("0");
    $("#PassQuantity").text("0");
    $("#FailQuantity").text("0");

    $("#RepairedQuantity").text("0");
    $("#RepairPending").text("0");

    $("#DispatchQuantity").text("0");
    $("#DispatchPending").text("0");

    $("#ProductionAchievementPercent").text("0.00%");
    $("#FirstPassYieldPercent").text("0.00%");
    $("#RejectionPercent").text("0.00%");

    $("#TodayProduction").text("0");
    $("#TodayDispatch").text("0");

    $("#ProductionProgress").css("width", "0%");
    $("#YieldProgress").css("width", "0%");
    $("#RejectionProgress").css("width", "0%");


    resetCharts();
}


function resetCharts() {

    if (productionChart) {

        productionChart.data.datasets[0].data = [
            0,
            0,
            0
        ];

        productionChart.update();
    }


    if (qualityChart) {

        qualityChart.data.datasets[0].data = [
            0,
            0,
            0,
            0
        ];

        qualityChart.update();
    }


    if (repairChart) {

        repairChart.data.datasets[0].data = [
            0,
            0
        ];

        repairChart.update();
    }


    if (dispatchChart) {

        dispatchChart.data.datasets[0].data = [
            0,
            0
        ];

        dispatchChart.update();
    }
}


function formatNumber(value) {

    if (
        value === null ||
        value === undefined ||
        value === ""
    ) {
        return "0";
    }

    let number = Number(value);

    if (isNaN(number)) {
        return "0";
    }

    return number.toLocaleString("en-IN");
}