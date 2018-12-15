jQuery.support.cors = true;
$(document).ready(function () {
    
	$('body').on('click', '.btn-login', function () {
	    Authenticateuser();
	});
	

});

function Authenticateuser() {
    if ($.trim($('#txtusername').val()) != "" && $.trim($('#txtpassword').val()) != "") {
        $.ajax({

            url: 'api/umsuser/authenticateuser?username=' + $('#txtusername').val() + '&password=' + $('#txtpassword').val(),
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, xhr) {
                if ($.parseJSON(data).status == "succeed") {
                    location.href = "listview.html";
                }
                else {
                    $(".lblmsg").text("Login Failed");
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                alert(textStatus + "test1");
            }
        });
    }
    else {
        $(".lblmsg").text("Enter Credentials");
    }

}