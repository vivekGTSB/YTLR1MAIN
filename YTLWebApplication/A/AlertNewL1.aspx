<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.AlertNewL" Codebehind="AlertNewL1.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        @import "css/demo_page.css";
        @import "css/demo_table_jui.css";
        @import "css/common1.css";
        @import "css/jquery-ui.css";
        .dataTables_info
        {
            width: 35%;
            float: left;
        }
    </style>
    <style>
        .hideen
        {
            visibility: hidden; /* margin-top:-80px;*/
        }
        .opacity
        {
            /* IE 8 */
            -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=30)"; /* IE 5-7 */
            filter: alpha(opacity=30); /* Netscape */
            -moz-opacity: 0.3; /* Safari 1.x */
            -khtml-opacity: 0.3;
            opacity: 0.3;
            pointer-events: none;
        }
        #floatingBarsG
        {
            position: fixed;
            width: 62px;
            height: 77px;
            top: 50%;
            left: 50%;
        }
    </style>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script src="js/jquery_ui.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.dataTables.js"></script>
    <script src="js/DatePickerConv.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
        function resize() {

            table = oTable.dataTable();
            table.fnSettings().oScroll.sY = getWindowHeight() - 110 + "px";
            table.fnDraw();

        }
        var reultdata, resultdata1;

        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            if (chkvalue) {
                for (i = 0; i < document.forms[0].elements.length; i++) {
                    elm = document.forms[0].elements[i]
                    if (elm.type == 'checkbox') {
                        document.forms[0].elements[i].checked = chkvalue;
                        if ($(elm).val() == "on") {

                        }
                        else {
                            chekitems.push($(elm).val());
                        }

                    }
                }
            }
            else {
                for (i = 0; i < document.forms[0].elements.length; i++) {
                    elm = document.forms[0].elements[i]
                    if (elm.type == 'checkbox') {
                        document.forms[0].elements[i].checked = chkvalue;
                    }
                }
                chekitems.length = 0;
            }

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
        function refreshTable1() {
            $("#floatingBarsG").removeClass("hideen");
            $("#form1").addClass("opacity");
            $.ajax({
                type: "POST",
                url: "GetAlertInbox.aspx?b="+ $('#txtBeginDate').val() + "&e=" + $('#txtEndDate').val(),              
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    resultdata1 = response.length;
                    if (response[0][0] == "--") {
                        $('#lblTotal').text(0);

                    }
                    else {
                        $('#lbltotal2').text(resultdata1);
                    }
                    var json = response.d;
                    table = oTable1.dataTable();
                    table._fnProcessingDisplay(true);
                    oSettings = table.fnSettings();
                    table.fnClearTable(this);
                    for (var i = 0; i < response.length; i++) {
                        table.oApi._fnAddData(oSettings, response[i]);
                    }
                    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                    table._fnProcessingDisplay(false);
                    table.fnDraw();
                    $(".dataTables_scrollBody").css('height', getWindowHeight() - 300);
                    $("#floatingBarsG").addClass("hideen").delay();
                    $("#form1").removeClass("opacity");
                    return false;
                },
                failure: function (response) {
                    alert("Failed");
                }
            });
        }
        var oTable;
        var oTable1;

        $(document).ready(function () {
            var h = getWindowHeight() - 110 + "px";

            fnFeaturesInit();

            oTable1 = $('#examples1').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 50,
                "aLengthMenu": [50, 100, 500, 1000, 5000, 10000],
                "bLengthChange": true,
                "bProcessing": true,
                "bScrollCollapse": true,
                "sScrollY": h,


                "bAutoWidth": false,
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "oLanguage": {
                    "oPaginate": {
                        "sNext": "Next",
                        "sFirst": "First",
                        "sLast": "Last",
                        "sPrevious": "Previous"
                    },
                    "sSearch": "Search",
                    "sEmptyTable": "No data available in table",
                    "sInfo": "Got a total of _TOTAL_ entries to show (_START_ to _END_)",
                    "sInfoFiltered": "#NAME?",
                    "sZeroRecords": "No records to display",
                    "sInfoEmpty": "No entries to show",
                    "sLoadingRecords": "Please wait - loading...",
                    "sProcessing": "DataTables is currently busy"
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
                "aoColumnDefs": [
                           { "sClass": "left", "bVisible": true, "bSortable": false, "sWidth": "20px", "aTargets": [0] },
                             { "sClass": "left", "bVisible": false, "bSortable": true, "sWidth": "100px", "aTargets": [1] },
                                { "sClass": "left", "bVisible": true, "bSortable": true, "sWidth": "140px", "aTargets": [2] },
                             { "sClass": "left", "bVisible": true, "bSortable": true, "aTargets": [3] },
                                { "sClass": "left", "bVisible": true, "bSortable": true, "aTargets": [4] },
                             { "sClass": "left", "bVisible": false, "bSortable": true, "sWidth": "100px", "aTargets": [5] },
                                { "sClass": "left", "bVisible": false, "bSortable": true, "sWidth": "100px", "aTargets": [6] },
                             { "sClass": "left", "bVisible": false, "bSortable": true, "sWidth": "100px", "aTargets": [7] },
                                { "sClass": "left", "bVisible": true, "bSortable": true, "aTargets": [8] },
                                { "sClass": "right", "bVisible": false, "bSortable": true, "aTargets": [9] }


                              ]
            });


            $("#txtSource").prop('disabled', true);
            $('#accl').show();
            $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: new Date(2015, 08, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2, gotoCurrent: true });
            $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: new Date(2015, 08, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2, gotoCurrent: true });

        });
        function check(chkobj) {

            var chkvalue = chkobj.checked;
            if (chkvalue == false) {
                chekitems.splice($.inArray($(chkobj).val(), chekitems), 1);
            }
            else {
                chekitems.push($(chkobj).val());
            }
        }
        
       




    </script>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <div>
            <div class="c1">
                Alert History</div>
            <br />
        </div>
        <table>
            <tr>
                <td align="center">
                    <table style="font-family: Verdana; font-size: 11px;">
                        <tr>
                            <td colspan="2" class="t1">
                                <div class="h1">
                                    &nbsp; Search Criteria&nbsp;:</div>
                            </td>
                        </tr>
                        <tr>
                            <td class="t2" style="width: 280px;">
                                <table style="width: 280px; margin-left: 80px">
                                    <tbody>
                                        <tr>
                                            <td align="left" style="width: 133px;">
                                                <b style="color: #465AE8;">Start Date </b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input readonly="readonly" style="width: 70px;" type="text" value="<%=bd%>" id="txtBeginDate"
                                                    runat="server" name="txtBeginDate" enableviewstate="false" /><b style="color: #465AE8;">&nbsp;</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">End Date </b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input style="width: 70px;" readonly="readonly" type="text" value="<%=ed%>" id="txtEndDate"
                                                    runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #465AE8;">&nbsp;</b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                            </td>
                                            <td colspan="2" align="center">
                                                <input id="btnsubmit" class="btn" onclick="refreshTable1(); " type="button" value="Submit"
                                                    style="cursor: pointer; background-color: #0000FF; color: #FFFFFF; font-weight: bold;
                                                    float: left;" />
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table style="font-family: Verdana; font-size: 11px;">
            
            <tr>
                <td>
                    <table>
                        <tr>
                            <td colspan="3" align="left">
                                <div id="accl">
                                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples1"
                                        style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
                                        <thead>
                                            <tr align="left">
                                                <th>
                                                    Sno
                                                </th>
                                                <th>
                                                    Source
                                                </th>
                                                <th>
                                                    Time Stamp
                                                </th>
                                                <th>
                                                    Data
                                                </th>
                                                <th>
                                                    Username
                                                </th>
                                                <th>
                                                    Mobile No
                                                </th>
                                                <th>
                                                    Phone No
                                                </th>
                                                <th>
                                                    Server
                                                </th>
                                                <th>
                                                    Action
                                                </th>
                                                <th>
                                                    Response Time
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                        <tfoot>
                                            <tr align="left">
                                                <th>
                                                    Sno
                                                </th>
                                                <th>
                                                    Source
                                                </th>
                                                <th>
                                                    Time Stamp
                                                </th>
                                                <th>
                                                    Data
                                                </th>
                                                <th>
                                                    Username
                                                </th>
                                                <th>
                                                    Mobile No
                                                </th>
                                                <th>
                                                    Phone No
                                                </th>
                                                <th>
                                                    Server
                                                </th>
                                                <th>
                                                    Action
                                                </th>
                                                <th>
                                                    Response Time
                                                </th>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </center>
    <div>
    </div>
    </form>
    <div id="floatingBarsG" class="hideen" style="z-index: 999; opacity: 2;">
        <center>
            <div>
                <img src="images/allplatesloder.gif" />
            </div>
        </center>
    </div>
</body>
</html>
