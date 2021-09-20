// use datatable library
// https://datatables.net/

$(document).ready(function () {
    loadDataTable()
});

// Make sure all field names match totally !   capital sensitive
function loadDataTable() {
    $('#tblData').DataTable(
        {
            "ajax": { "url": "/inquiry/GetInquiryList" },
            "columns": [
                { "data": "id", "width": "10%" },
                { "data": "fullName", "width": "10%" },
                { "data": "phoneNumber", "width": "10%" },
                { "data": "email", "width": "10%" },
                {
                    "data": "id",
                    "render": function (data) {
                        // the "data" is the value of "id"
                        return `
                        <div class="text-center">
                        <a href="/Inquiry/Details/${data}" class="btn btn-success text-white" style="cursor:pointer">
                        <i class="fas fa-edit"></i>
                        </a>
                        </div>
                        ` ;
                    },
                    "width":"5%"

                }
            ]

        }
    );
}
