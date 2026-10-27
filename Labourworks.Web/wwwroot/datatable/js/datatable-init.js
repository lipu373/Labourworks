$(document).ready(function () {

    $('.common-datatable').each(function () {

        var table = $(this).DataTable({
            responsive: true,
            pageLength: 10,

            columnDefs: [
                {
                    targets: 0,
                    orderable: false,
                    searchable: false
                }
            ],

            order: [[1, 'asc']]
        });

        table.on('order.dt search.dt draw.dt', function () {

            let i = table.page.info().start + 1;

            table
                .column(0, {
                    search: 'applied',
                    order: 'applied',
                    page: 'current'
                })
                .nodes()
                .each(function (cell) {
                    cell.innerHTML = i++;
                });

        }).draw();

    });

});