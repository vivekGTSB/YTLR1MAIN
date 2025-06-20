<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GetKPIDetailsAPK.aspx.vb" Inherits="YTLWebApplication.GetKPIDetailsAPK" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>KPI Details</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/ColVis.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #fw_container
        {
            width: 100%;
        }
        table.display td
        {
            padding: 2px 2px;
        }
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/ColVis.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">

        function getData(plateno) {
            document.getElementById("StatusFrame").style.visibility = "visible";
            document.getElementById("StatusFrame").src = "GetStatusPopup.aspx?p=" + plateno ;
            $("#dialog-form").dialog("open");
            console.log(plateno);
        }
        
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
            $("#dialog-form").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 1050,
                minHeight: 400,
                height: 400,
                buttons: {
                    Update: function () {
                        UpdateData();
                        $(this).dialog("close");
                    },
                    Close: function () {
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
           // oTable.fnSetColumnVis(4, false);
        });

    </script>
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="fuelform1" runat="server">
        <input type="hidden" id="txtna" value="<%=loggedinUID %>" />
    <div id="fw_container">
        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px;
            font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <%=sb1.ToString()%>
        </table>
    </div>
    </form>
     <div id="dialog-form" title="Update Vehicle's Status" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
      <iframe id="StatusFrame" name="StatusFrame" src="" frameborder="0" scrolling="auto" height="300"
            width="1040" style="visibility: hidden; border: solid 1px #aac6ff;"></iframe>
    </div>
    <div class="demo">
       <div id="Div1" title="Alert">
            <p id="displayp">
                <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
                </span>
            </p>
        </div>
    </div>
</body>
</html>


