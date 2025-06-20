<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GetStatusPopup.aspx.vb" Inherits="YTLWebApplication.GetStatusPopup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Status Details</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/ColVis.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #fw_container {
            width: 100%;
        }

        table.display td {
            padding: 2px 2px;
        }
        .hidenow {
        display:none;
        }
        .btnedit, .btnupdate,.btncancel {
        cursor:pointer;
        text-decoration:underline;
        color:blue;
        }
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/ColVis.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">        
        function UpdateData() {
            var str = "updatevehiclestatus.aspx?s=" + $('#ddlstatus').val() + "&pno=" + $('#plate_no').text() + "&u=" + $("#txtna").val() + "&re=" + $("#txtRemarks").val();
            $.get(str, function (data) {
                alertbox("Updated Successfully");
                $("#dialog-message").dialog("close");
            });
            $('#plate_no').val() == "";
            $('#ddlstatus').val() == "";
        }

        function alertbox(message) {
            document.getElementById("displayp").innerHTML = message;
            $("#Div1").dialog("open");
        }
        function EditIt(plateno, statusdate) {
           // alert(plateno + " and " + statusdate);
        }
        function fnFeaturesInit() {

            $('ul.limit_length>li').each(function (i) {
                if (i > 10) {
                    this.style.display = 'none';
                }
            });

            $('ul.limit_length').append('<li class="css_link">Show more<\/li>');
            $('ul.limit_length li.css_link').click(function () {
                $('ul.limit_length li').each(function (i) {
                    if (i > 5) {
                        this.style.display = 'list-item';
                    }
                });
                $('ul.limit_length li.css_link').css('display', 'none');
            });
        }
        $(document).ready(function () {
            fnFeaturesInit();
            var oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 25,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "aaSorting": [[7, "asc"]],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "aoColumnDefs": [
                    { "bVisible": true, "bSortable": false, "aTargets": [0] }
                ]
            });
            $("#Div1").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                buttons: {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
            });          

            $('.modal-content').resizable({
                //alsoResize: ".modal-dialog",
                //minHeight: 150
            });
            $('.modal-dialog').draggable();

            $('#dialog-form').on('show.bs.modal', function () {
                $(this).find('.modal-body').css({
                    'max-height': '100%'
                });
            });
            var oldValue = "";
            $(".btnedit").click(function (e) {
                oldValue = $(this).closest('tr').find('.clsremarks').text();
                $(this).closest('tr').find('.clsremarks').html("<input type='text' value='" + oldValue + "' id='apk_id' />");
                $(this).closest('tr').find(".btnupdate").removeClass("hidenow");
                $(this).closest('tr').find(".btncancel").removeClass("hidenow");                
                $(this).addClass("hidenow");
            });
            $(".btnupdate").click(function (e) {
                var myVal = $(this).closest('tr').find('.clsremarks').find("#apk_id").val();
               // alert(myVal);
                var plateno = $(this).attr("data-plateno");
                var servicingDate = $(this).attr("data-statusdate");
                $(this).closest('tr').find(".btnedit").removeClass("hidenow");
                $(this).addClass("hidenow");
                $(this).closest('tr').find(".btncancel").addClass("hidenow");
                $(this).closest('tr').find('.clsremarks').html(myVal);


                try {
                    $.ajax({
                        type: "POST",
                        url: "GetStatusPopup.aspx/saveData",
                        data: '{plateno: \"' + plateno + '\",sd:\"' + servicingDate + '\",remarks:\"' + myVal + '\"}',
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            alert("Updated");
                        },
                        failure: function (response) {
                            alert("Error");
                        }
                    });
                } catch (e) {

                }


            });

            $(".btncancel").click(function (e) {               
                $(this).closest('tr').find(".btnedit").removeClass("hidenow");
                $(this).addClass("hidenow");
                $(this).closest('tr').find(".btnupdate").addClass("hidenow");
                $(this).closest('tr').find('.clsremarks').html(oldValue);
            });

            
            // oTable.fnSetColumnVis(4, false);
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <table border="0" cellpadding="1" cellspacing="1" style="width: 1025px; font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">Plate No</b> &nbsp;:&nbsp;&nbsp;
                </td>
                <td>
                    <span id="plate_no"><%=plateno %></span>
                </td>
            </tr>
            <tr>
                <br />
                <td align="left">
                    <b style="color: #4E6CA3;">Status</b> &nbsp;&nbsp;&nbsp; :&nbsp;&nbsp;
                </td>
                <td>
                    <select name="ddlstatus" id="ddlstatus" style="height: 21px; width: 156px;">
                        <option value="Select Status">Select Status</option>
                        <option value="Workshop">WORKSHOP</option>
                        <option value="Battery Taken Out">BATTERY TAKEN OUT</option>
                        <option value="Power Cut">POWER CUT</option>
                        <option value="Spare Truck">SPARE TRUCK</option>
                        <option value="Not in Operation">NOT IN OPERATION</option>
                        <option value="Accident">ACCIDENT</option>
                        <option value="Service Scheduled">SERVICE SCHEDULED</option>
                        <option value="Pending Service Schedule">PENDING SERVICE SCHEDULE</option>

                    </select>
                </td>
            </tr>
            <tr>

                <td align="left">
                    <b style="color: #4E6CA3;">Latest Remarks</b>
                </td>
                <td>
                    <textarea name="txtRemarks" rows="2" cols="20" id="txtRemarks" style="height: 30px; width: 150px;">
</textarea>
                </td>
            </tr>
            <tr>
                <td align="left"></td>
                <td></td>
            </tr>
        </table>
        <div id="fw_container">
        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px;
            font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <%=sb1.ToString()%>
        </table>
    </div>
       

    </form>
     <div class="demo">
            <div id="Div1" title="Alert">
                <p id="displayp">
                    <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;"></span>
                </p>
            </div>
        </div>
</body>
</html>
