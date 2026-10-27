
(function () {
    window.initCommonDataTable = function (selector) {
        const table = $(selector);

        if (!table.length || !$.fn.DataTable) {
            console.error('DataTables is not loaded.');
            return null;
        }

        if ($.fn.DataTable.isDataTable(table[0])) {
            table.DataTable().destroy();
        }

        return table.DataTable({
            pageLength: 10,
            lengthMenu: [5, 10, 25, 50],
            searching: true,
            ordering: true,
            info: true,
            autoWidth: false,
            order: [],
            language: {
                emptyTable: 'No records found'
            }
        });
    };
})();
