<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.ClientServiceManagementLog" CodeBehind="ClientServiceManagementLog.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Service Log Management</title>
    <style type="text/css" title="currentStyle">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/ColVis.css";
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui.js"></script>
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <link href="cssfiles/style.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/demos22.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/jquery.ui.dialog.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {

            $("#txttodate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2023, 01, 01), maxDate: +180, changeMonth: true, changeYear: true, numberOfMonths: 2
            });
            $("#txtserdate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2023, 01, 01), maxDate: +180, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txtsearchdate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2023, 01, 01), maxDate: +180, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

        });
    </script>
    <script language="javascript" type="text/javascript">

        function refreshTable() {
            table = oTable.dataTable();
            table._fnProcessingDisplay(true);
            oSettings = table.fnSettings();
            $.getJSON('ServiceingLogJson.aspx?u=' + $("#hidloginuser").val() + '&role=' + $("#hidrole").val() + '&opr=0&r=' + Math.random() + '&d=' + $("#txtsearchdate").val() + '&td=' + $("#txttodate").val() + '', null, function (json) {
                table.fnClearTable(this);

                for (var i = 0; i < json.aaData.length; i++) {
                    table.oApi._fnAddData(oSettings, json.aaData[i]);
                }
                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                table._fnProcessingDisplay(false);

                table.fnDraw();
            });
            return false;
        }


        $(document).ready(function () {
            jQuery.extend(jQuery.fn.dataTableExt.oSort, {
                "num-html-pre": function (a) {
                    a = $(a).text().substr(2);
                    var x = parseFloat(a.toString().replace(/<.*?>/g, ""));
                    if (isNaN(x)) {
                        if (a.toString().indexOf("fuel sensor problem") > 0) {
                            return -1;
                        }
                        else if (a.toString().indexOf("no fuel sensor") > 0) {
                            return -0.1;
                        }

                        return 0.0;
                    }
                    else {
                        return x;
                    }
                },

                "num-html-asc": function (a, b) {
                    return ((a < b) ? -1 : ((a > b) ? 1 : 0));
                },

                "num-html-desc": function (a, b) {
                    return ((a < b) ? 1 : ((a > b) ? -1 : 0));
                }
            });


            oTable = $('#example').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "bPaginate": true,
                "iDisplayLength": 25,
                "aaSorting": [[3, "desc"]],
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },

                "aoColumnDefs": [{ "sClass": "left", "bVisible": true, "sWidth": "30px", "bSortable": false, "aTargets": [0] },
                { "sClass": "left", "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [1] }]
            });

            $('#example tbody').on('click', 'tr', function () {
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                }
                else {
                    table.$('tr.selected').removeClass('selected');
                    $(this).addClass('selected');
                }
            });

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 510,
                minHeight: 400,
                height: 400,
                title: "Add Service Log",
                buttons: {
                    Save: function () {
                        var opr = $("#hidopr").val();
                        if (opr == 0)
                            Insertticketinformation();
                        else if (opr == 1)
                            updateticket();


                    },
                    Close: function () {
                        $(this).dialog("close");
                    }
                }
            });

            $("#div-ticket").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 570,
                minHeight: 600,
                height: 600,
                title: "Service Infromation",
                buttons: {
                    Close: function () {
                        refreshTable();
                        $(this).dialog("close");
                    }
                }
            });
            $("#dialog-confirm").dialog({
                resizable: false,
                draggable: false,
                height: 140,
                modal: true,
                autoOpen: false,
                buttons: {
                    "Yes": function () {
                        confirmresult = true;
                        $(this).dialog("close");
                    },
                    No: function () {
                        confirmresult = false;
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-alert").dialog({
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
            refreshTable();
        });

        function openNewticket() {
            $("#hidopr").val(0);
            clearfields();
            $("#dialog-message").dialog("open");
        }

        function clearfields() {
            $("#ddlplatenumbers").val("--Select Plate No--");
            $("#servicetype").val("Data Lost");
            $("#ddlpriority").val("LOW");
            $("#txtserdate").val("");
            $("#ddlbh").val("00");
            $("#ddlbm").val("00");
            $("#txtdescri").val("");
        }

        function openupdateticket(plateno, servicetype, priority, reqdatetime, hrs, mins, remarks, serviceid) {
            $("#hidopr").val(1);
            $("#ddlplatenumbers").val(plateno);
            $("#servicetype").val(servicetype);
            $("#ddlpriority").val(priority);
            $("#txtserdate").val(reqdatetime);
            $("#ddlbh").val(hrs);
            $("#ddlbm").val(mins);
            $("#txtdescri").val(remarks);
            $("#hidserviceid").val(serviceid);
            $("#dialog-message").dialog("open");
        }
        function updateticket() {
            var res = mysubmit();
            if (res == true) {
                $.getJSON('ServiceingLogJson.aspx?opr=10&p=' + $("#ddlplatenumbers").val() + '&stype=' + $("#servicetype").val() + '&expdt=' + $("#txtserdate").val() + ' ' + $("#ddlbh").val() + ':' + $("#ddlbm").val() + '&rem=' + encodeURIComponent($("#txtdescri").val()) + '&r=' + Math.random() + '&status=' + $("#ddlstatus").val() + '&pic=' + $("#txtpic").val(), null, function (json) {
                    if (json.d == 1) {
                        $("#dialog-message").dialog("close");
                    }
                });
            }
            refreshTable();
        }

        function Insertticketinformation() {
            var res = mysubmit();
            if (res == true) {
                $.getJSON('ServiceingLogJson.aspx?opr=1&p=' + $("#ddlplatenumbers").val() + '&stype=' + $("#servicetype").val() + '&expdt=' + $("#txtserdate").val() + ' ' + $("#ddlbh").val() + ':' + $("#ddlbm").val() + '&rem=' + encodeURIComponent($("#txtdescri").val()) + '&r=' + Math.random() + '&status=' + $("#ddlstatus").val() + '&pic=' + $("#txtpic").val(), null, function (json) {
                    if (json.d == 1) {
                        $("#dialog-message").dialog("close");
                        refreshTable();
                    }
                });
            }
            refreshTable();
        }
        function mysubmit() {

            if (document.getElementById("ddlplatenumbers").value == "--Select Plate No--") {
                alertbox("Please Select Plate Number");
                return false;
            }


            else if (document.getElementById("txtdescri").value.trim() == "") {
                alertbox("Please enter Remarks");
                return false;
            }

            else
                return true;
        }

        function Getticketdetails(serviceid) {
            $("#serviceid").val(serviceid);
            $.getJSON('ServiceingLogJson.aspx?serviceid=' + serviceid + '&u=' + $("#hidloginuser").val() + '&opr=2&r=' + Math.random(), null, function (json) {
                if (json.length == 1) {
                    $("#plateno").text(json[0][1]);
                    $("#status").text(json[0][5]);
                    $("#priority").text(json[0][4]);
                    $("#assigned").text(json[0][6]);
                    if (json[0][2] == null)
                        $("#datetime").text("");
                    else
                        $("#datetime").text(json[0][2]);
                    if (json[0][7] == null)
                        $("#tdcloseddt").text("");
                    else
                        $("#tdcloseddt").text(json[0][7]);

                    $("#tdDescription").html("<p>" + json[0][8] + "</P>");

                    $("#tdServiceddate").text(json[0][9]);

                    if (json[0][9] != "--") {
                        $("#trserrem").remove();
                        $("#trserrem1").remove();
                        $("<tr id='trserrem' ><td><u>Serviced Remarks</u></td><td colspan='5'></td> </tr><tr id='trserrem1'><td colspan='6'>" + json[0][9] + "</td></tr>").appendTo("#tblservicedetails");
                    }
                    else {
                        $("#trserrem").remove();
                        $("#trserrem1").remove();
                    }


                    if (json[0][5] == "Closed") {
                        $("#txtcomment").attr("disabled", "disabled");
                        $("#btnsend").attr("disabled", "disabled");
                    }
                    else {
                        $("#txtcomment").removeAttr("disabled");
                        $("#btnsend").removeAttr("disabled");
                    }
                    GetComments(0);
                }
            });

            $("#div-ticket").dialog("open");

        }


        function sendcomment() {
            var comment = document.getElementById("txtcomment").value.trim();
            if (comment != "") {

                $.getJSON('ServiceingLogJson.aspx?serviceid=' + $("#serviceid").val() + '&u=' + $("#hidloginuser").val() + '&Comment=' + encodeURIComponent(comment) + '&opr=3&r=' + Math.random(), null, function (json) {
                    if (json.d == 1) {
                        $("#txtcomment").val("");
                        GetComments(0)
                    }
                });
            }
            else {
                alertbox("Please enter Comment");
            }
        }

        function GetComments(st) {
            $.getJSON('ServiceingLogJson.aspx?serviceid=' + $("#serviceid").val() + '&st=' + st + '&opr=4&r=' + Math.random(), null, function (json) {
                if (json.Comments.length >= 1) {
                    $("#ullist").empty();
                    $("#ullist").append(json.Comments);
                }
                else {
                    $("#ullist").empty();
                }
            });
        }
        function getplateno() {
            $.getJSON('ServiceingLogJson.aspx?u=' + $("#ddluser").val() + '&opr=11&r=' + Math.random(), null, function (json) {
                $("#ddlplatenumbers").empty();
                for (var i = 0; i < json.aaData.length; i++) {
                    $("#ddlplatenumbers").append("<option value=" + json.aaData[i] + ">" + json.aaData[i] + "</option>");
                }
            });
        }

        function confirmbox(confirmMessage) {
            confirmresult = false;
            document.getElementById("displayc").innerHTML = confirmMessage;
            $("#dialog-confirm").dialog("open");
        }
        function alertbox(message) {
            document.getElementById("displayp").innerHTML = message;
            $("#dialog-alert").dialog("open");
        }
        function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();
        }

    </script>
    <style type="text/css">
        ul, ol {
            padding: 0;
            margin: 0 0 0px 0px;
        }

        .comment {
            padding-right: 65px;
        }

        .comment {
            margin: 0 0 3px;
            padding: 3px 15px 8px;
            background-color: #ffff;
            position: relative;
            word-wrap: break-word;
            overflow-wrap: break-word;
            border-radius: 10px;
            border: #030303;
        }

        .comment-meta {
            position: absolute;
            right: 15px;
            top: 1px;
        }

        .commentmetadata {
            font-size: 10px;
        }

        .comment p {
            line-height: 10px;
            clear: both;
            font-size: 11px;
            margin-bottom: 0.5em;
        }

        .hor-minimalist-b {
            font-family: "Verdana";
            font-size: 11px;
            background: #fff;
            margin: 0px;
            width: 520px;
            border-collapse: collapse;
            text-align: left;
        }

            .hor-minimalist-b th {
                font-size: 14px;
                font-weight: normal;
                color: #039;
                padding: 3px 2px;
                border-bottom: 1px solid #6678b1;
            }

            .hor-minimalist-b td {
                border-bottom: 1px solid #ccc;
                color: #669;
                padding: 4px 6px;
            }

            .hor-minimalist-b tbody tr:hover td {
                color: #009;
            }

        button {
            cursor: pointer;
        }

        a.button:active {
            border-color: #4B8DF8;
        }

        a.button:hover {
            color: White;
            text-shadow: 0 1px 0 #fff;
            border: 1px solid #2F5BB7 !important;
            background: #3F83F1;
            background: -webkit-linear-gradient(top, #4D90FE, #357AE8);
            background: -moz-linear-gradient(top, #4D90FE, #357AE8);
            background: -ms-linear-gradient(top, #4D90FE, #357AE8);
            background: -o-linear-gradient(top, #4D90FE, #357AE8);
        }

        a.button {
            text-align: center;
            font: bold 11px Helvetica, Arial, sans-serif;
            cursor: pointer;
            text-shadow: 0 1px 0 #fff;
            display: inline-block;
            width: 74px;
            border: 1px solid #3079ED !important;
            color: White;
            height: 14px;
            background: #4B8DF8;
            background: -webkit-linear-gradient(top, #4C8FFD, #4787ED);
            background: -moz-linear-gradient(top, #4C8FFD, #4787ED);
            background: -ms-linear-gradient(top, #4C8FFD, #4787ED);
            background: #4B8DF8;
            -webkit-transition: border .20s;
            -moz-transition: border .20s;
            -o-transition: border .20s;
            transition: border .20s;
            margin: 5px;
            padding: 3px 5px 5px 3px;
        }

        .textbox1 {
            height: 20px;
            width: 180px;
            border-right: #cbd6e4 1px solid;
            border-top: #cbd6e4 1px solid;
            border-left: #cbd6e4 1px solid;
            color: #0b3d62;
            border-bottom: #cbd6e4 1px solid;
        }

        .dropdown1 {
            height: 25px;
            width: 182px;
            border-right: #cbd6e4 1px solid;
            border-top: #cbd6e4 1px solid;
            border-left: #cbd6e4 1px solid;
            color: #0b3d62;
            border-bottom: #cbd6e4 1px solid;
        }

        .badge {
            background: radial-gradient( 5px -9px, circle, white 8%, red 26px );
            background-color: red;
            border: 2px solid white;
            border-radius: 12px;
            box-shadow: 1px 1px 1px black;
            color: white;
            font: bold 10px/7px Helvetica, Verdana, Tahoma;
            height: 10px;
            min-width: 8px;
            padding: 4px 3px 0 3px;
            text-align: center;
        }

        .badge {
            float: left;
            left: 60px;
            margin: -10px;
            position: relative;
            top: 5px;
        }

        tr.even.selected {
            background-color: #00cccc;
        }

        tr.odd.selected {
            background-color: #00cccc;
        }

        tr.even.selected td.sorting_1 {
            background-color: #00cccc;
        }

        tr.odd.selected td.sorting_1 {
            background-color: #00cccc;
        }

        .auto-style1 {
            height: 18px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <center>
            <input type="hidden" id="hidopr" />
            <div style="width: 1340px; margin: auto;">
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Service Request Log Management</b>



                <table>
                    <tr>
                        <td style="text-align: left; padding-left: 0px;">From Date :
                            <input type="text" id="txtsearchdate" runat="server" style="width: 100px;" onchange="refreshTable()" />
                            To Date :
                            <input type="text" id="txttodate" runat="server" style="width: 100px;" onchange="refreshTable()" />
                        </td>
                        <td style="text-align: right; padding-right: 0px;">
                           
                            <a style="cursor: pointer;display :none " class="button" id="addbtn" onclick="javascript:openNewticket()">Add New</a>
                            <a style="cursor: pointer;" class="button" onclick="javascript:ExcelReport()">Excel</a>
                             <%  If company = "GUSSMANN TECHNOLOGIES SDN BHD" Then%>
                            <script type="text/javascript" language="javascript">
                                $("#addbtn").show();
                            </script>
                            <%  End If%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <table width="1300" cellpadding="0" cellspacing="0" border="0" align="center" class="display"
                                id="example" style="font-size: 11px; font-family: verdana">
                                <thead style="text-align: left; font-weight: bold;">
                                    <tr>
                                        <th class="auto-style1">SNo.
                                        </th>
                                        <th class="auto-style1">User Name
                                        </th>
                                        <th class="auto-style1">Raised Date
                                        </th>
                                        <th class="auto-style1">Plateno
                                        </th>
                                        <th class="auto-style1">Service Type
                                        </th>
                                        <th class="auto-style1">Status
                                        </th>
                                        <th class="auto-style1">Reported By
                                        </th>
                                        <th class="auto-style1">PIC
                                        </th>
                                        <th class="auto-style1">Remarks
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                                <tfoot style="text-align: left;">
                                    <tr>
                                        <th>SNo.
                                        </th>
                                        <th>User Name
                                        </th>
                                        <th>Raised Date
                                        </th>
                                        <th>Plateno
                                        </th>
                                        <th>Service Type
                                        </th>
                                        <th>Status
                                        </th>
                                        <th>Reported By
                                        </th>
                                        <th>PIC
                                        </th>
                                        <th>Remarks
                                        </th>
                                    </tr>
                                </tfoot>
                            </table>
                        </td>
                    </tr>
                </table>
                <input type="hidden" id="hidrole" runat="server" />
                <input type="hidden" id="hidloginuser" runat="server" />
                <input type="hidden" id="serviceid" runat="server" />
            </div>
        </center>
        <div id="dialog-message" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
            <br />
            <div class="table info_box" style="width: 500px;">
                <table width="width:500px;" style="padding-left: 10px; font-size: 11px; font-family: verdana; font-weight: bold; color: #4E6CA3;">
                    <tr align="left">
                        <td>Username
                        </td>
                        <td>:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="dropdown1" ID="ddluser" onchange="getplateno()">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr align="left">
                        <td>Plate No
                        </td>
                        <td>:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" CssClass="dropdown1" ID="ddlplatenumbers">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr align="left">
                        <td>Service Type
                        </td>
                        <td>:
                        </td>
                        <td>
                            <select id="servicetype" class="dropdown1 ">
                                <option>GPS Fault</option>
                                <option>PTO Fault</option>
                                <option>Panic Fault</option>
                                <option>Appt Aborted</option>
                                <option>Breakdown</option>
                                <option>Call Fail</option>
                                <option>Heavy Rain</option>
                                <option>Inside Plant</option>
                                <option>New Trip</option>
                                <option>Truck Delay</option>
                                <option>Truck Busy</option>
                                <option>Under Repair</option>
                                <option>Under Puspakom</option>
                                <option>Other</option>
                            </select>
                        </td>
                    </tr>
                    <tr align="left">
                        <td>Date & Time
                        </td>
                        <td>:
                        </td>
                        <td>
                            <input type="text" id="txtserdate" style="width: 100px;"></input>&nbsp;HH&nbsp;
                        <asp:DropDownList ID="ddlbh" runat="server" Width="42px" EnableViewState="False"
                            Font-Size="12px" Font-Names="verdana">
                            <asp:ListItem Value="00">00</asp:ListItem>
                            <asp:ListItem Value="01">01</asp:ListItem>
                            <asp:ListItem Value="02">02</asp:ListItem>
                            <asp:ListItem Value="03">03</asp:ListItem>
                            <asp:ListItem Value="04">04</asp:ListItem>
                            <asp:ListItem Value="05">05</asp:ListItem>
                            <asp:ListItem Value="06">06</asp:ListItem>
                            <asp:ListItem Value="07">07</asp:ListItem>
                            <asp:ListItem Value="08">08</asp:ListItem>
                            <asp:ListItem Value="09">09</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                            <asp:ListItem Value="12">12</asp:ListItem>
                            <asp:ListItem Value="13">13</asp:ListItem>
                            <asp:ListItem Value="14">14</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                            <asp:ListItem Value="16">16</asp:ListItem>
                            <asp:ListItem Value="17">17</asp:ListItem>
                            <asp:ListItem Value="18">18</asp:ListItem>
                            <asp:ListItem Value="19">19</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="21">21</asp:ListItem>
                            <asp:ListItem Value="22">22</asp:ListItem>
                            <asp:ListItem Value="23">23</asp:ListItem>
                        </asp:DropDownList>
                            &nbsp; MM &nbsp;
                                                    <asp:DropDownList ID="ddlbm" runat="server" Width="42px" EnableViewState="False"
                                                        Font-Size="12px" Font-Names="verdana">
                                                        <asp:ListItem Value="00">00</asp:ListItem>
                                                        <asp:ListItem Value="01">01</asp:ListItem>
                                                        <asp:ListItem Value="02">02</asp:ListItem>
                                                        <asp:ListItem Value="03">03</asp:ListItem>
                                                        <asp:ListItem Value="04">04</asp:ListItem>
                                                        <asp:ListItem Value="05">05</asp:ListItem>
                                                        <asp:ListItem Value="06">06</asp:ListItem>
                                                        <asp:ListItem Value="07">07</asp:ListItem>
                                                        <asp:ListItem Value="08">08</asp:ListItem>
                                                        <asp:ListItem Value="09">09</asp:ListItem>
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="11">11</asp:ListItem>
                                                        <asp:ListItem Value="12">12</asp:ListItem>
                                                        <asp:ListItem Value="13">13</asp:ListItem>
                                                        <asp:ListItem Value="14">14</asp:ListItem>
                                                        <asp:ListItem Value="15">15</asp:ListItem>
                                                        <asp:ListItem Value="16">16</asp:ListItem>
                                                        <asp:ListItem Value="17">17</asp:ListItem>
                                                        <asp:ListItem Value="18">18</asp:ListItem>
                                                        <asp:ListItem Value="19">19</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                        <asp:ListItem Value="21">21</asp:ListItem>
                                                        <asp:ListItem Value="22">22</asp:ListItem>
                                                        <asp:ListItem Value="23">23</asp:ListItem>
                                                        <asp:ListItem Value="24">24</asp:ListItem>
                                                        <asp:ListItem Value="25">25</asp:ListItem>
                                                        <asp:ListItem Value="26">26</asp:ListItem>
                                                        <asp:ListItem Value="27">27</asp:ListItem>
                                                        <asp:ListItem Value="28">28</asp:ListItem>
                                                        <asp:ListItem Value="29">29</asp:ListItem>
                                                        <asp:ListItem Value="30">30</asp:ListItem>
                                                        <asp:ListItem Value="31">31</asp:ListItem>
                                                        <asp:ListItem Value="32">32</asp:ListItem>
                                                        <asp:ListItem Value="33">33</asp:ListItem>
                                                        <asp:ListItem Value="34">34</asp:ListItem>
                                                        <asp:ListItem Value="35">35</asp:ListItem>
                                                        <asp:ListItem Value="36">36</asp:ListItem>
                                                        <asp:ListItem Value="37">37</asp:ListItem>
                                                        <asp:ListItem Value="38">38</asp:ListItem>
                                                        <asp:ListItem Value="39">39</asp:ListItem>
                                                        <asp:ListItem Value="40">40</asp:ListItem>
                                                        <asp:ListItem Value="41">41</asp:ListItem>
                                                        <asp:ListItem Value="42">42</asp:ListItem>
                                                        <asp:ListItem Value="43">43</asp:ListItem>
                                                        <asp:ListItem Value="44">44</asp:ListItem>
                                                        <asp:ListItem Value="45">45</asp:ListItem>
                                                        <asp:ListItem Value="46">46</asp:ListItem>
                                                        <asp:ListItem Value="47">47</asp:ListItem>
                                                        <asp:ListItem Value="48">48</asp:ListItem>
                                                        <asp:ListItem Value="49">49</asp:ListItem>
                                                        <asp:ListItem Value="50">50</asp:ListItem>
                                                        <asp:ListItem Value="51">51</asp:ListItem>
                                                        <asp:ListItem Value="52">52</asp:ListItem>
                                                        <asp:ListItem Value="53">53</asp:ListItem>
                                                        <asp:ListItem Value="54">54</asp:ListItem>
                                                        <asp:ListItem Value="55">55</asp:ListItem>
                                                        <asp:ListItem Value="56">56</asp:ListItem>
                                                        <asp:ListItem Value="57">57</asp:ListItem>
                                                        <asp:ListItem Value="58">58</asp:ListItem>
                                                        <asp:ListItem Value="59">59</asp:ListItem>
                                                    </asp:DropDownList>
                        </td>
                    </tr>
                    <tr align="left">
                        <td>Status
                        </td>
                        <td>:
                        </td>
                        <td>
                            <select id="ddlstatus" class="dropdown1 ">
                                <option>Job Created</option>
                                <option>Appointment initiated</option>
                                <option>Follow-up</option>
                                <option>Completed</option>
                                <option>Sabotage</option>
                            </select>
                        </td>
                    </tr>
                    <tr align="left">
                        <td>PIC
                        </td>
                        <td>:
                        </td>
                        <td>
                            <input type="text" id="txtpic" class="dropdown1" />
                        </td>
                    </tr>
                    <tr align="left">
                        <td colspan="3">Remarks
                        </td>
                    </tr>
                    <tr align="left">
                        <td colspan="3">
                            <textarea style="width: 480px; height: 100px;" id="txtdescri" rows="15" cols="50"></textarea>
                        </td>
                    </tr>
                </table>
            </div>
            <input type="hidden" id="hidserviceid" />
        </div>
        <div id="div-ticket" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 0px; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
            <br />
            <table width="500px" style="padding-left: 10px; font-size: 12px; font-family: verdana;">
                <tbody>
                    <tr>
                        <td colspan="6">
                            <table class="hor-minimalist-b" id="tblservicedetails">
                                <tr>
                                    <td style="width: 120px;">Plateno
                                    </td>
                                    <td style="width: 10px;">:
                                    </td>
                                    <td id="plateno"></td>
                                    <td style="width: 80px;">Status
                                    </td>
                                    <td style="width: 10px;">:
                                    </td>
                                    <td id="status"></td>
                                </tr>
                                <tr>
                                    <td style="width: 120px;">Priority
                                    </td>
                                    <td style="width: 10px;">:
                                    </td>
                                    <td id="priority"></td>
                                    <td style="width: 80px;">Assigned To
                                    </td>
                                    <td style="width: 10px;">:
                                    </td>
                                    <td id="assigned"></td>
                                </tr>
                                <tr>
                                    <td style="width: 120px;">Assigned Date Time
                                    </td>
                                    <td style="width: 10px;">:
                                    </td>
                                    <td id="datetime" colspan="4"></td>
                                </tr>
                                <tr>
                                    <td style="width: 120px;">Closed Date Time
                                    </td>
                                    <td style="width: 10px;">:
                                    </td>
                                    <td id="tdcloseddt" colspan="4"></td>
                                </tr>
                                <tr>
                                    <td style="width: 120px;">Serviced Date Time
                                    </td>
                                    <td style="width: 10px;">:
                                    </td>
                                    <td id="tdServiceddate" colspan="4"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <u>Remarks</u>
                                    </td>
                                    <td colspan="5"></td>
                                </tr>
                                <tr>
                                    <td colspan="6" id="tdDescription"></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">Comments
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div>
                                <ul id="ullist">
                                </ul>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">Comment:
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <textarea style="width: 510px; height: 100px;" id="txtcomment" rows="15" cols="50"></textarea>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="6" style="text-align: right;">
                            <button id="btnsend" type="button" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only ui-state-hover"
                                onclick="javascript:sendcomment()" role="button" aria-disabled="false">
                                <span style="cursor: pointer;" class="ui-button-text">send</span></button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="demo">
            <div id="dialog-confirm" title="Confirmation ">
                <p id="displayc">
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
                </p>
            </div>
            <div id="dialog-alert" title="Information">
                <p id="displayp">
                    <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;"></span>
                </p>
            </div>
        </div>
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Service Request Log Report" />
    </form>
</body>
</html>
