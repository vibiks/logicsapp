jQuery.support.cors = true;
$(document).ready(function () {
    validateuser();

    var dateTable;
    //var changedRows = [];

    $("#min").datepicker({ onSelect: function () { dateTable.draw(); }, changeMonth: true, changeYear: true, minDate: -360, dateFormat: 'dd/mm/yy', maxDate: 0 });
    $("#max").datepicker({ onSelect: function () { dateTable.draw(); }, changeMonth: true, changeYear: true, dateFormat: 'dd/mm/yy', maxDate: 0, minDate: -360 });


    var minDate = new Date();
    minDate.setDate(minDate.getDate() - 7);//any date you want

    $('#min').datepicker('setDate', minDate);
    $('#max').datepicker('setDate', '0');

    $.ajax({
        url: 'api/umslist/getentries',
        type: 'GET',
        dataType: 'json',
        order: [[1, "desc"]],
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, xhr) {
            dateTable = $('#dtentrylist').DataTable({
                data: jQuery.parseJSON(data),
                dataSrc: "",
                responsive: true,
                order: [[ 0, "desc" ]],
                columns: [
                    {
                        data: "entryno",
                        render: function (data, type, row, meta) {
                            return '<a href="Index.html?entryno=' + row.entryno + '">' + data + '</a><br/><input type="text" readonly="true" style="display:none;" value="' + data + '"/>';
                        }
                    },
                    { data: "entrydate" },
                    //{ data: "customcode" },
                    { data: "customername" },
                    { data: "JobType" },
                    //{ data: "PaDcNo" },
                    //{ data: "PaDcDate" },
                    {
                        data: "PaStatus",
                        "render": function (data, type, row) {
                            return createStatusDropDown(data, type, row);
                        }

                    },
                    //{
                    //    data: "Delete",
                    //    "render": function (data, type, row) {
                    //        return createTrashLink(data, type, row);
                    //    }

                    //},
                ],
                columnDefs: [{ orderable: false, "targets": -1 }]
            });
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error in Operation', xhr);
        }
    });

    $('#min, #max').change(function () {
        dateTable.draw();
    });

    $.fn.dataTable.ext.search.push(
        function (settings, data, dataIndex) {
            var dateSuccess = false;
            var statusSuccess = false;

            var min = $('#min').datepicker("getDate");
            var max = $('#max').datepicker("getDate");
            var currDateString = data[1];
            var currDate = (currDateString).split('/');
            var startDate = new Date(currDate[2], (currDate[1] - 1), currDate[0]);
            if (min == null && max == null) { dateSuccess = true; }
            if (min == null && startDate <= max) { dateSuccess = true; }
            if (max == null && startDate >= min) { dateSuccess = true; }
            if (startDate <= max && startDate >= min) { dateSuccess = true; }

            var dropdownValue = $(".insStatusFilter").val();
            statusSuccess = dropdownValue == "" ? true : ( (($('#dtentrylist').DataTable().data()[dataIndex]['PaStatus'] != "") && ($('#dtentrylist').DataTable().data()[dataIndex]['PaStatus'] == dropdownValue)) ? true : false )
            
            return (dateSuccess && statusSuccess) ? true : false;
        }
    );

    $('body').on('click', '.btnaddentry', function () {
        location.href = "Index.html";
    });

    // on listview submit....
    $('.btn-listViewsubmitfrm').click(function () {
        var data = dateTable.$('input:hidden, select').serialize();
        var jsonObj = [];

        // creating json value..
        if (($('input[type=text][readonly]').length) > 0) {
            var values = data.split('&');
            $.each($('input[type=text][readonly]'), function (index, value) {
                var item = {};
                item["JobNo"] = $(this)[0].value;
                item["Status"] = values[index].split('=')[1];
                jsonObj.push(item);
            });
        }

        $.ajax({
            url: 'api/umslist/updatejobdetails',
            contentType: 'application/json',
            processData: false,
            data: JSON.stringify(jsonObj),
            type: 'POST',
            dataType: 'json',
            success: function (data, textStatus, xhr) {
                // alert(data);
                var jsonData = $.parseJSON(data);
                var strkey = "";
                var strval = "";
                $.each(jsonData, function (key, value) {
                    strkey = key;
                    strval = value;
                });
                if (strkey == 'Error') {
                    alert("Error:" + strval);
                }
                else
                    location.href = "listview.html";
            },
            error: function (xhr, textStatus, errorThrown) {
                //alert("error" + textStatus + ":" + xhr);
            }
        });

    });

    $(document).on('click', '.delete-job', function (e) {
        e.preventDefault();
        var jobNumber = $(this).data('jobno');
        deleteJob(jobNumber);
    });

    // inspection status filter change..
    $('.insStatusFilter').on('change', function () {
        // dateTable.columns(6).search(this.value).draw();
        dateTable.draw();
    });   

});


function validateuser() {
    $.ajax({
        url: 'api/umsuser/getuser',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, xhr) {
            if (data != "succeed") {
                location.href = "login.html";
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert(textStatus + "test1");
        }
    });

}

function createStatusDropDown(data, type, row) {
    // console.log(row);
    var select = '<select class="inspection_status" name="inspection_status" data-entryno="' + row.entryno + '">';
    var ddValues = ["Started","Pending", "Completed", "Cancel"];
    select = select + '<option value="">Select</option>';

    for (var i = 0; i < ddValues.length; i++) {
        var isSelected = ddValues[i] == data ? 'selected' : "";        
        select = select + '<option ' + isSelected + ' value="' + ddValues[i] + '">' + ddValues[i] + '</option>'
    }
    return select + "</select>";
}

function createTrashLink(data, type, row) {
    var dataAttribs = 'data-jobno="' + row.entryno + '"';
    return "<a class='delete-job dt-button buttons-html5' " + dataAttribs + "href='#' title='Delete'><span><i class='fa fa-trash-o' aria-hidden='true' style='padding-left: 25%;'></i></span></a>";

}

function deleteJob(jobNumber) {
    var job = { jobNo: jobNumber };
    $.ajax({
        url: 'api/UMSlist/deletejobdetail?JobNo=' + jobNumber,
        type: 'GET',
        //data: jobNumber,
        dataType: 'json',
        cache: false,
        contentType: 'application/json; charset=utf-8',
        beforeSend: function () {
            $('#myModal').modal('toggle');
        },
        success: function (data, textStatus, xhr) {
            $('#myModal').modal('toggle');
            location.href = "listview.html";
        },
        error: function (data) {
            alert('Ajax Error' + data);
            $('#myModal').modal('toggle');
        }

    })
}