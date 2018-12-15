jQuery.support.cors = true;
validateuser();
console.log("validated")
var temp = { "1": "CUSTOMER ONE", "2": "CUSTOMER TWO" };
var drpworkoptions = "";
var frminfo = [];
var dataSet = "";
$(function () {
    var d = new Date();
    $('#entry_date').datetimepicker({
        format: 'DD/MM/YYYY'
    });
    $('#entry_date').val(d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear());
    $('#dc_date').datetimepicker({
        format: 'DD/MM/YYYY'
    });
    $('.job_starttime').datetimepicker({
        format: 'DD/MM/YYYY hh:mm A'
    });
    $('.job_endtime').datetimepicker({
        format: 'DD/MM/YYYY hh:mm A'
    });
});
var machinecodearr = [];

$(document).ready(function () {
    
    $(".entry_no").attr("disabled", "disabled");
    $(".lblmsg").text("");    
    $(".customer_code").attr("disabled", "disabled");
    $(".customer_code").val("");
    $(".customer_address").attr("disabled", "disabled");
    $(".customer_address").val("");
    $(".gst_no").attr("disabled", "disabled");
    var rdo = $("input[name=work_progress][value='Office']");
    rdo.prop("checked", true);
    rdo.parent().addClass("selected");
    rdo.parent().next().addClass("selected");
    
    
	$.ajax({
        url: 'api/ums/getContactDetails',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, xhr) {
            // alert(data);
            var jsonData = $.parseJSON(data);
            var $select = $('.customerName');
            // console.log($select);
            $select.find('option').remove();
            $select.append('<option value=>Please Select</option>');
            
            $.each(jsonData, function (key, value) {
                if (value == "jobid") 
				{
                    if($.trim($(".entry_no").val()) == "")
                    $(".entry_no").val(key);
                }
                else {
                    $select.append("<option value='" + key + "'>" + value + "</option>");
                }
                
                //alert(key);
            });
            
           // $(".customer_code").val($('.customerName option:selected').val());
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error in Operation', xhr);
        }
    });
    $.ajax({
        url: 'api/umsMachine/getMachineDetails',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, xhr) {
            // alert(data);
            var jsonData = $.parseJSON(data);
            var $select = $('.machine_detail');
            // console.log($select);
            $select.find('option').remove();
            $.each(jsonData, function (key, value) {
                $select.append('<option value=' + key + '>' + value + '</option>');
                drpworkoptions += '<option value=' + key + '>' + value + '</option>';
            });
            if ($.trim(geturlparam()) != "")
                getformdatareload(geturlparam());

        },
        error: function (xhr, textStatus, errorThrown) {
            console.log('Error in Operation', xhr);
        }
    });

	$('body').on('change keyup focusin','input.form-input',function(){
		var thisval = $(this).val();
		$(this).closest('.row-field').addClass('focused');
	});
	//var $select = $('.customerName');
	//// console.log($select);
	//$select.find('option').remove(); 
	//$.each(temp, function(key, value) {              
	//	$select.append('<option value=' + key + '>' + value + '</option>');
	//});
	
	$('body').on('change keyup blur','input.form-input',function(){
		var thisval = $(this).val();
		if(thisval == ""){
			$(this).closest('.row-field').removeClass('focused');
		}
	});
	

	$('.radio-btn').click(function(){
		$("input[name='"+$(this).attr('name')+"']").each(function(){
			$(this).parent().removeClass("selected");
			$(this).parent().next().removeClass("selected");
		}); 
		if($(this).prop('checked')){
			$(this).parent().addClass("selected");
			$(this).parent().next().addClass("selected");
		}
	});
	/* Add new customer */
	$('.add-new-customer').click(function(){
		$('.add-customers').removeClass('hide');
		$('.add-new-customer').hide();
	});
	$('.showcontacttbl').click(function () {
	    if ($('.showcontacttbl').text() == "Show") {
	        $('.contacttbl').removeClass('hide');
	        $('.showcontacttbl').text('Hide');
	    }
	    else {
	        $('.contacttbl').addClass('hide');
	        $('.showcontacttbl').text('Show');
	    }
	    
	});
	$("#customerinfo").autocomplete({
	    source: function (request, response) {
	        $.ajax({
	            url: 'api/ums/getcustomerautocomplete?prefix=' + $("#customerinfo").val() + '%',	           
	            dataType: "json",
	            type: "GET",
	            contentType: "application/json; charset=utf-8",
	            success: function (data) {
	                var dataArray = [];
	                var obj = jQuery.parseJSON(data);
	                for (key in obj) {
                        dataArray.push({ "label": obj[key], "value": key.toString() });
	                }
	                response(dataArray);

	            },
	            error: function (response) {
	                alert(response.responseText);
	            },
	            failure: function (response) {
	                alert(response.responseText);
	            }
	        });
	    },
	    select: function (event, ui) {
	        event.preventDefault();
	        console.log(ui.item.value);
	        $("#customerinfo").val(ui.item.label);
	        $("#customer_name1 option").each(function (i) {
	            //console.log($(this).val());
	            if ($(this).val().toString().indexOf(ui.item.value + "~") > -1) {
	                $("#customer_name1").val($(this).val());	                
	            }
	        });
	        customerNameChange();

	    },
	    focus: function(event, ui) {
	        event.preventDefault();
	        $("#customerinfo").val(ui.item.label);
	    },
	    minLength: 1
	});
	/* Display contact Person details for the customer*/
	$('input[name="customer_code"]').blur(function(){
		if($(this).val() != ''){
			// console.log('te')
			$.ajax({
				url: "	customer_details.json", 
				success: function(data){
					// console.log(data);
				}
			});
		}
	})
	
	/* Adding machine details in the grid view*/
	
	$('body').on('click','.add-machine-details',function(e){
		e.preventDefault();
		var $this = $(this);		

		var $parentEle = $(this).closest('.job-machine-section');
		var machincode = $parentEle.find('.machine_detail').val();
		//machinecodearr.push($this.attr("data-id") + "~" + $parentEle.find('.job_no').val() + "~" + machincode);
		//alert($this.attr("data-id") + "~" + machincode);
		//var machinename = $parentEle.find('.machine_detail').text();
		//if($parentEle.find('.job_no').val() !=''){
			if($parentEle.find('.machine_detail').val()!=''){
				var trlength = (parseInt($parentEle.find('.machine_details_view table tr').length ));
				$parentEle.find('.machine_details_view table tr:last').after("<tr><td>" + $parentEle.find('.job_no').val() + "</td><td class=" + machincode + '_' + trlength + ">" + $parentEle.find('.machine_detail').val() + "</td><td class=" + machincode + '1_' + trlength + ">" + $parentEle.find('.machine_detail option:selected').text() + "</td><td><a href='javascript:;' data-id='" + $parentEle.find('.job_no').val() + "~" + machincode + "' class='remove machinedetaildata'>X</a></td></tr>");
				//$('<input />').attr('type', 'hidden').attr('name', 'hdn_' + machincode + '_' + trlength).attr('value', $this.attr("data-id") + "~" + machincode).appendTo('form');
				//$('<input />').attr('type', 'hidden').attr('name', machinename+'_'+trlength+'').attr('value', $parentEle.find('.machinename').val()).appendTo('form');
				if($parentEle.find('.machine_details_view table tr').length > 1){
					if(!$parentEle.find('.machine_details_view').is(":visible"))
						$parentEle.find('.machine_details_view').fadeIn();
				}
			}
		//}else{
			//alert("Please enter job no");
		//}
	});
	
	/* Adding multipe jobs */
	$(".add-job").click(function(){
		var divLength = (parseInt($('.job-machine-section').length) + 1);
		$("<div class='job-machine-section'><div class='row'><div class='col-xs-12'><h2 class='subHead'>Job Details " + divLength + " <a href='javascript:;' class='remove_job_no hearderlink'>remove</a></h2></div></div><div class='job-details'><div class='row'><div class='col-xs-12 col-sm-4'><label>Job No</label><div class='row-field' placeholder='Job No'><input type='text' class='form-input job_no' name='job_" + divLength + "_no'></div></div><div class='col-xs-12 col-sm-4'><label>Job Name</label><div class='row-field' placeholder='Job Name'><input type='text' class='form-input' name='job_" + divLength + "_name'></div></div></div></div><div class='row'><div class='col-xs-12 col-sm-4'><label>Drawing Details</label><div class='row-field' placeholder='Drawing Details'><input type='text' class='form-input' name='drawing_" + divLength + "_details'></div></div><div class='col-xs-12 col-sm-4'><label>Work Details</label><div class='row-field' placeholder='Work Details'><input type='text' class='form-input' name='work_" + divLength + "_details'></div></div></div><div class='row'><div class='col-xs-12 col-sm-4'> <label>Job StartTime</label> <div class='row-field input-group date' placeholder='Job StartTime'><input type='text' class='form-input form-control job_starttime' name='job_" + divLength + "_starttime' id='job_" + divLength + "_starttime'><span class='input-group-addon'> <span class='glyphicon glyphicon-calendar'></span> </span></div> </div> <div class='col-xs-12 col-sm-4'><label>Job EndTime</label><div class='row-field input-group date' placeholder='Job EndTime'><input type='text' class='form-input form-control job_endtime' name='job_" + divLength + "_endtime' id='job_" + divLength + "_endtime'><span class='input-group-addon'> <span class='glyphicon glyphicon-calendar'></span> </span></div></div><div class='row'><div class='col-xs-12'><h2>Machine Details</h2></div></div><div class='row'><div class='col-xs-12 col-sm-4'><label>Machine Details</label><div class='row-field'><select class='form-select machine_detail' name='machine_" + divLength + "_details'>" + drpworkoptions + "</select></div></div></div><div class='row'><div class='col-xs-offset-0 col-sm-4'><a href='javascript:;' class='btn btn-primary btn-md add-machine-details' data-id='machdetjob_" + divLength + "' title='Add Machine'>Add Machine</a></div></div></div><div class='machine_details_view'><div class='table-responsive'><table class='table table-striped'><tbody><tr><th>S.No</th><th>Machine ID</th><th>Machine Details</th><th>&nbsp;</th></tr></tbody></table></div></div></div></div>").insertAfter($(".job-machine-section:last"));
		$('.job_starttime').datetimepicker({
		    format: 'DD/MM/YYYY hh:mm A'
		});
		$('.job_endtime').datetimepicker({
		    format: 'DD/MM/YYYY hh:mm A'
		});
	});
	/*Remove Job Details*/
	$('body').on('click','.remove_job_no',function(){
		$(this).closest('.job-machine-section').remove();
		changeAttr();
	});
	/* Remove Machine Details*/
	$('body').on('click','.remove',function(){
		var $this = $(this);
		var $parentEle = $(this).closest('.machine_details_view');
		//var machinecode = $this.closest('tr').find('td').eq(1).attr('class');
		//var machinename = $this.closest('tr').find('td').eq(2).attr('class');
		//$("input[name=" + $this.attr("data-id") + "]").remove();
		//$("input[name="+machinename+"]").remove();
		$this.closest('tr').remove();
		if($parentEle.find('table tr').length === 1){
			$parentEle.hide();
		}
	});

	$('body').on('click', '.btn-submitfrm', function () {    
	    if ($("#frmentry").valid()) {
	        uploadPhotoAndData();
	    }
	});

});
function changeAttr(){
	var $length = $('.job-machine-section').length;
	// console.log($length);
	if($length > 1){
		for(var i=1;i<$('.job-machine-section').length;i++){
			// console.log($('.job-machine-section').eq(i))
			
		}
	}
}
function overlay(option){
	if(option =='open'){
		$('.overlay-loader').fadeIn('400');
		$('html,body').css({'overflow':'hidden'});
	}else{
		$('.overlay-loader').fadeOut('400');
		$('html,body').css({'overflow':'auto'});
	}
}
function customerNameChange() {
    if (typeof $('.customerName option:selected').val() != 'undefined')
        {   
        
            var customerCode = $('.customerName option:selected').val().split("~")[0];
            var customerAddress = $('.customerName option:selected').val().split("~")[1];
            var GSTNo = $('.customerName option:selected').val().split("~")[2];
            //if ($.trim($("#customerinfo").val()) == "") {
                $("#customerinfo").val($(".customerName option:selected").text());
            //}
            $(".customer_code").val(customerCode);
            $(".customer_address").val(customerAddress);
            $(".gst_no").val(GSTNo);

            $.ajax({
                url: 'api/ums/getcontactdetail?strPartyCode=' + customerCode,
                type: 'GET',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data, textStatus, xhr) {
                    var jsonData = $.parseJSON(data);
                    var strTableRow = "";
                    $.each(jsonData, function (key, value) {
                        strTableRow = strTableRow + "<tr><td>1</td><td>" + key + "</td><td>" + value + "</td></tr>"
                    });
                    $("#customerDetailTable").find("tr:gt(0)").remove();
                    $('#customerDetailTable tr#trHeader').after(strTableRow);
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.log('Error in Operation: Customer Name change request)', xhr);
                }
            });
        }
    }


function validateuser() {    
    $.ajax({
        url: 'api/UserManage/getuser',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, xhr) {
            if (data != "true") {
                location.href = "login.html";
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert("Error in validateuser: " + textStatus);
        }
    });

}

function geturlparam() {    
    if(window.location.search.substring(1) != "")
        return window.location.search.substring(1).split('=')[1];
    return "";
}

function getformdatareload(entryno) {
    
    $(".entry_no").val(entryno);   
    $.ajax({
        url: 'api/umsentry/getentryinfo?entryid=' + entryno,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, xhr) {
            var jsonData = $.parseJSON(data);            
            $.each(jsonData, function (key, value) {
                //alert(key);
                console.log(key);
                console.log(value);
                if (key.indexOf(".") >= 0)
				{
					if(key == ".radio-btn")
					{
					    if (value == "On Site") {
					        var rdo = $("input[name=work_progress][value='On Site']");
					        rdo.prop("checked", true);
					        rdo.parent().addClass("selected");
					        rdo.parent().next().addClass("selected");
					        var rdo = $("input[name=work_progress][value='Office']");
					        rdo.parent().removeClass("selected");
					        rdo.parent().next().removeClass("selected");
					    }
					    				
					}
					else if (key == ".customerName")
					{
					    //console.log("SSET");
					    $("#customer_name1 option").each(function (i) {
					        //console.log($(this).val());
					        if ($(this).val().toString().indexOf(value + "~") > -1) {
					            $("#customer_name1").val($(this).val());
					        }
					    });
					    customerNameChange();
					}
					else	
					{
						$(key).val(value);
					}
				}
                else
                    frminfo[key] = value;
                      
            });
            populatecontrols();            
            customerNameChange();
        },
        error: function (xhr, textStatus, errorThrown) {
            alert(textStatus + "test1");
        }
    });
}

function populatecontrols() {
   
    var cnt = 1;   
    for (i = 1; i <= parseInt(frminfo["jobcnt"]) ; i++) {
        if (i == 1) {
            $("#job_1_no").val(frminfo["prcode1"]);
            $("#job_1_name").val(frminfo["prname1"]);
            $(".draw_det1").val(frminfo["prdrawing1"]);
            $(".work_det1").val(frminfo["narration1"]);
            $(".inspection_status").val(frminfo["insstatus1"]);
            $("#job_1_starttime").val(frminfo["jobstarttime1"]);
            $("#job_1_endtime").val(frminfo["jobendtime1"]);

            var str = frminfo["machinecode1"].split(',');
            for (j = 0; j < str.length; j++) {
                if (str[j] != "") {
                    $(".machine_detail").val(str[j]);
                    $(".add-machine-details").trigger("click");
                }
            }
        }
        else if (i == 2) {
            $(".add-job").trigger("click");
            $('input[name = "job_2_no"]').val(frminfo["prcode2"]);
            $('input[name = "job_2_name"]').val(frminfo["prname2"]);
            $('input[name = "drawing_2_details"]').val(frminfo["prdrawing2"]);
            $('input[name = "work_2_details"]').val(frminfo["narration2"]);
            $("#job_2_starttime").val(frminfo["jobstarttime2"]);
            $("#job_2_endtime").val(frminfo["jobendtime2"]);
            var str = frminfo["machinecode2"].split(',');
        }
        
    }
        
}


function uploadPhotoAndData() {
    //var formData = new FormData();
    //var files = $("#fileUpload").get(0).files;
    //// Add the uploaded image content to the form data collection
    //if (files.length > 0) {
    //    debugger;
    //    var acceptedFiles = ["jpg", "jpeg", "png", "gif"];
    //    var isAcceptedImageFormat = ($.inArray(files[0].name.split('.').pop(), acceptedFiles)) != -1;

    //    if (!isAcceptedImageFormat) {
    //        alert("Not a valid image format")
    //    }
    //    else {
    //        formData.append("UploadedImage", files[0]);
    //        formData.append("ImageName", $("#yourdcno").val());
    //    }

    //    $.ajax({
    //        url: 'api/umsMachine/SaveImage',
    //        type: 'POST',
    //        dataType: 'json',
    //        contentType: false,
    //        processData: false,
    //        data: formData,
    //        success: function (uploadedImagePath) {
    //            if (uploadedImagePath != '') {
    //                uploadFormData(uploadedImagePath);
    //            }
    //            else {
    //                alert('Image Upload Failed.Try again!');
    //            }
    //        },
    //        error: function (x, y, z) {
    //            return false;
    //        }

    //    });

    //}
    //else {
    //    uploadFormData("");
    //}

    uploadFormData("");
    
}

function uploadFormData(uploadedImagePath) {
    $(".gst_no").removeAttr('disabled');
    var frmdata = $('form').serialize().toString();

    var macdata = "";
    $(".machinedetaildata").each(function () {
        macdata += $(this).attr("data-id") + "^";
    });

    var finaldata = frmdata.replace(/&/g, '$') + '$macdata=' + macdata + '$entryno=' + geturlparam() + '$imgpath=' + uploadedImagePath;


    $.ajax({
        url: 'api/umsMachine/getJobDetails?frmdata=' + finaldata,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, xhr) {
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
            alert("error" + textStatus + ":" + xhr);
            console.log('Error in Operation: Customer Name change request)', xhr);
        }
    });
}