<%@ Page Language="VB" AutoEventWireup="false" Debug="true" EnableEventValidation="false" Inherits="YTLWebApplication.LafargeVDM" Codebehind="LafargeVDM.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>YTL Signal Loss(>30 Mins) & V Data (>30 Mins) Alert Settings</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
    <style type="text/css">
        .dataTables_filter
        {
            width: 50%;
            float: right;
            text-align: right;
            margin-top: -29px;
        }
        .chzn-container .chzn-results li
        {
            line-height: 80%;
            padding-bottom: 8px;
            padding-top: 8px;
            margin: 0;
            list-style: none;
            z-index: 1;
            color: #4E6CA3;
        }
        
        
        div.dataTables_wrapper .ui-widget-header
        {
            font-weight: normal;
            float: left;
            text-align: left;
        }
        .dataTables_wrapper .ui-toolbar
        {
            padding: 5px;
            width: 988px;
        }
        .chzn-container .chzn-results
        {
            font-family: verdana;
        }
        thead input
        {
            margin-left: 0.95em;
            color: #444;
        }
        
        tfoot input
        {
            margin: 0.2em 0;
            width: 100%;
            margin-left: -0.35em;
            color: #444;
        }
    </style>
    <script type="text/javascript" language="javascript">


        function validate() {
            var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if (document.getElementById("email1").value == "") {
                alertbox("Please enter email id");
                return false;
            }

            if ($('#email1').val() != "") {
                var arr;
                var emls = document.getElementById("email1").value;
                if (emls.indexOf(",") > 0) {
                    var mar = new Array();
                    mar = emls.split(",");
                    arr = new Array();
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else if (emls.indexOf(";") > 0) {
                    arr = new Array();
                    var mar = new Array();
                    mar = emls.split(";");
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else {
                    arr = new Array();
                    arr.push(emls);
                }
                for (var i = 0; i < arr.length; i++) {
                    if (!emailRegEx.test(arr[i])) {
                        alertbox("Please enter a valid EMail Id.");
                        return false;
                    }
                }

            }

            return true;
        }

        function validateP() {
            var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if (document.getElementById("emlMul").value == "") {
                alertbox("Please enter email id");
                return false;
            }

            if ($('#emlMul').val() != "") {
                var arr;
                var emls = document.getElementById("emlMul").value;
                if (emls.indexOf(",") > 0) {
                    var mar = new Array();
                    mar = emls.split(",");
                    arr = new Array();
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else if (emls.indexOf(";") > 0) {
                    arr = new Array();
                    var mar = new Array();
                    mar = emls.split(";");
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else {
                    arr = new Array();
                    arr.push(emls);
                }
                for (var i = 0; i < arr.length; i++) {
                    if (!emailRegEx.test(arr[i])) {
                        alertbox("Please enter a valid EMail Id.");
                        return false;
                    }
                }

            }

            return true;
        }

        function confirm(message) {
            document.getElementById("P1").innerHTML = message;
            $("#div1").dialog("open");
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


        function refreshTable() {

            $.getJSON('GetLVAData.aspx?opr=0&suid=' + $("#ddluser1").val() + '&r=' + Math.random(), null, function (json) {
                table = oTable.dataTable();
                table._fnProcessingDisplay(true);
                oSettings = table.fnSettings();
                table.fnClearTable(this);
                for (var i = 0; i < json.length; i++) {
                    table.oApi._fnAddData(oSettings, json[i]);
                }
                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                table._fnProcessingDisplay(false);
                table.fnDraw();
                return false;
            });

        }

        function Conformdelete() {
            if (chekitems.length == 0) {
                alertbox("Please select at least one alert to delete.");
            }
            else {
                confirm("Are you sure you want to delete selected alert(s) ?");

            }
        }

        function DeletePOI() {
            $.getJSON('GetLVAData.aspx?geoid=' + chekitems + '&opr=3&r=' + Math.random(), null, function (json) {
                if (json == "1") {
                    refreshTable();
                    checkall(false);
                }
                else {
                    refreshTable();
                }

            });

        }

        function Ondeletesuccess(response) {

            if (response.d > 0) {
                refreshTable();
            }
            else if (response.d <= 0) {
                alertbox("Sorry, alert cannot be Deleted.");
            }

        }

        var oTable;
        var chekitems = new Array();
        var confirmresult = false;
        var checkids = "";
        $(document).ready(function () {
            $("#dialog-message").hide();
            $("#dialog:ui-dialog").dialog("destroy");
            $("#dialog-msg").dialog({
                autoOpen: false,
                resizable: false,
                height: 140,
                modal: true,
                buttons: {
                    "OK": function () {
                        $(this).dialog("close");
                    }

                }
            });


            $("#div1").dialog({
                resizable: false,
                draggable: false,
                height: 160,
                modal: true,
                autoOpen: false,
                buttons: {
                    "Yes": function () {
                        confirmresult = true;
                        DeletePOI();
                        $(this).dialog("close");
                    },
                    No: function () {
                        confirmresult = false;
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 350,
                minHeight: 220,
                height: 220

            });

            $("#div_all").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 1000,
                minHeight: 510,
                height: 510,
                buttons: {
                    Add: function () {
                        AddMData();
                    },
                    Close: function () {
                        $(this).dialog("close");

                    }
                }

            });
            fnFeaturesInit();
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 100000,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "bLengthChange": false,
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(1)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
                "aoColumnDefs": [

                          { "sClass": "center", "sWidth": "10px", "aTargets": [0], "bSortable": false, "bVisible": true,
                              "fnRender": function (oData, sVal) {
                                  if (sVal != "0") {
                                      return "<input type='checkbox' name='chk' id='chk' value='" + sVal + "' onclick='javascript:check(this);'/>";
                                  }
                                  else {
                                      return "";
                                  }

                              }
                          },
                             { "bVisible": true, "bSortable": false, "sWidth": "30px", "aTargets": [1] },

                               { "sClass": "left", "sWidth": "180px", "aTargets": [2], "bSortable": true, "bVisible": true,
                                   "fnRender": function (oData, sVal) {
                                       return "<span style='cursor:pointer;color:blue;' onclick=\"javascript:openUpdatePopup('" + oData.aData[6] + "','" + oData.aData[5] + "','" + oData.aData[3] + "','" + oData.aData[4] + "')\">" + oData.aData[2] + "</span>";

                                   }
                               },


                            { "bVisible": true, "sWidth": "180px", "bSortable": true, "aTargets": [3] },
                             { "bVisible": true, "sWidth": "180px", "bSortable": true, "aTargets": [4] }

                             ],
                "sDom": '<"H"Cl<"MyButton">f>tr<"F"iT>'
            });
            $("div.MyButton").html('<div><%=opt %>  </div>');
            jQuery(".chosen").data("placeholder", "Select Frameworks...").chosen();

            refreshTable();
            $("#examples_wrapper .fg-toolbar").height("23");
        });
        function alertbox(message) {
            if (message == "") {
                document.getElementById("displayp").innerHTML = message;

            }
            else {
                document.getElementById("displayp").innerHTML = message;
                $("#dialog-msg").dialog("open");
            }
        }

        function openAddPopup() {
            Clearfields();
            $("#opr").val(0);
            $("#dialog-message").dialog({
                title: "Add New Alert",
                buttons: {
                    Add: function () {
                        AddData();
                    },
                    Close: function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }
        function openUpdatePopup(id, geoid, emaillist, mobilelist) {
            $("#ddlGeofence").removeAttr("disabled");
            $("#ddlGeofence").val(geoid);
            $("#ddlGeofence").attr("disabled", "true");
            $("#email1").val(emaillist);
            $("#mobile1").val(mobilelist);
            $("#dialog-message").dialog({
                title: "Update Alert",
                buttons: {
                    Update: function () {
                        if (id == "0") {
                            AddData();
                        }
                        else {
                            UpdateData(id);
                        }

                    },
                    Close: function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }



        function UpdateData(geoid) {
            var res = validate();
            var listcond = "&eml=" + $("#email1").val() + "&mob=" + $("#mobile1").val();
            var extracond = "&opr=2";
            if (geoid == 0) {
                extracond = "&opr=1";
            }
            if (res == true) {
                $.get('GetLVAData.aspx?geoid=' + geoid + extracond + listcond + '&r=' + Math.random(), null, function (json) {
                    if (json == 1) {
                        alertbox("Successfully processed..");
                        refreshTable();
                    }
                });
                $("#dialog-message").dialog("close");
            }
        }

        function AddMData() {
            if ($("#ddluser").val() == "0") {
                alertbox("Please select a User Name");
                return false;
            }

            else if ($("#ddluser").val() == "ALL USERS") {
                var res = validateP();
                var listcond = "&eml=" + $("#emlMul").val() + "&mob=" + $("#mobMul").val();
                var extracond = "&opr=5";
                if (res == true) {
                    $.get('GetLVAData.aspx?geoid=ALLUSERS' + extracond + listcond + '&r=' + Math.random(), null, function (json) {
                        if (json == 1) {
                            alertbox("Successfully processed..");
                            // $("#ddluser1").val($("#ddluser").val());
                            refreshTable();
                        }
                    });
                    $("#div_all").dialog("close");
                }
                return false;
            }


            var checked = false;
            var matches = [];
            $(".p1:checked").each(function () {
                if (this.value != "on") {
                    matches.push(this.value);
                }
            });

            if (matches.length > 0) {
                var res = validateP();
                var listcond = "&eml=" + $("#emlMul").val() + "&mob=" + $("#mobMul").val();
                var extracond = "&opr=4";
                if (res == true) {
                    $.get('GetLVAData.aspx?geoid=' + matches + extracond + listcond + '&r=' + Math.random(), null, function (json) {
                        if (json == 1) {
                            alertbox("Successfully processed..");
                           // $("#ddluser1").val($("#ddluser").val());
                            refreshTable();
                        }
                    });
                    $("#div_all").dialog("close");
                }
            }
            else {
                alertbox("Please select a plateno");
                return false;
            }

        }


        function AddData() {
            var res = validate();
            var listcond = "&eml=" + $("#email1").val() + "&mob=" + $("#mobile1").val();
            var extracond = "&opr=1";
            var geoid = $("#ddlGeofence").val();
            if (res == true) {
                $.get('GetLVAData.aspx?geoid=' + geoid + extracond + listcond + '&r=' + Math.random(), null, function (json) {
                    if (json == 1) {
                        alertbox("Successfully processed..");
                        refreshTable();
                    }
                });
                $("#dialog-message").dialog("close");
            }
        }



        function Clearfields() {
            $("#email1").val("");
            $("#mobile1").val("");
        }

        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];
                if (elm.type == 'checkbox') {

                    document.forms[0].elements[i].checked = chkvalue;
                    if (chkvalue) {
                        if (document.forms[0].elements[i].value != "on") {
                            chekitems.push(document.forms[0].elements[i].value);
                        }
                    }
                    else {
                        if (document.forms[0].elements[i].value != "on") {
                            chekitems.pop(document.forms[0].elements[i].value);
                        }
                    }
                }
            }
        }

        function checkallp(chkobj) {
            var chkvalue = chkobj.checked;
            $(".p1").attr("checked", chkvalue);
        }


        function check(chkobj) {
            var chkvalue = chkobj.checked;
            if (chkvalue == false) {
                chekitems.splice($.inArray($(chkobj).val(), chekitems), 1);
            }
            else {
                chekitems.push($(chkobj).val());
            }
        }
        function openAll() {
            $("#emlMul").val("");
            $("#mobMul").val("");
            document.getElementById("ddluser").value = "0";
            $("#mulplate").html("");
            $("#mplate").hide();
            $("#div_all").dialog("open");
        }

        function getPlates() {
            if (document.getElementById("ddluser").value == "0" || document.getElementById("ddluser").value  == "ALL USERS") {
                $("#mulplate").html("");
                $("#mplate").hide();
                return false;
            }
            var lastXhr = $.getJSON("GetPlates.aspx?r=" + Math.random() + "&groupid=ALLGROUPS&userid=" + document.getElementById("ddluser").value, "", function (data) {
                if (data.length > 0) {
                    $("#mplate").show();
                    $("#mulplate").html("");
                    var json = data;
                    var spancontent = "";
                    var j = 0;
                    var st = 5;

                    var tdcount = 1;
                    var maxColCount = 11;

                 
                    spancontent = spancontent + "<table border='0' cellpadding='0' cellspacing='6'><tr><td colspan='" + maxColCount + "'>";
                    spancontent = spancontent + "<input type='checkbox' onclick='checkallp(this)' class='p1' value='on'/><span style='color:blue;font-weight:bolder;'>Check All</span></td><tr>";
                    for (var i = 0; i < data.length; i++) {
                        spancontent = spancontent + "<td>";
                        spancontent = spancontent + "<input  type='checkbox' class='p1'  value='" + data[i][1] + "' /> " + data[i][1];
                        spancontent = spancontent + "<td>";
                        tdcount += 1;
                        if (tdcount == maxColCount) {
                            spancontent = spancontent + "</tr><tr>";
                            tdcount = 1;
                        }
                    }
                    spancontent = spancontent + "</tr></table>";
                    $("#mulplate").html(spancontent);
                    return false;
                }
            });
        }

    </script>
    <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
</head>
<body style="font-size: 11px; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">
    <center>
        <br />
        <br />
        <div style="font-family: Verdana; font-size: 22px; color: #38678B; font-weight: bold;">
            &nbsp;YTL Signal Loss(>30 Mins) & V Data (>30 Mins) Alert Settings</div>
        <br />
        <table style="font-family: Verdana; font-size: 11px; width: 1000px">
            <tr>
                <td align="left">
                    <a href="#" class="button" title="Delete Alert" style="width: 59px;" onclick="javascript:Conformdelete()">
                        <span class="ui-button-text">Delete </span></a>
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right: 0px;">
                    <a href="#" class="button" title="Add" style="width: 59px;" onclick="javascript:openAll()">
                        <span class="ui-button-text">Add </span></a>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="width: 1000px;">
                    <div id="fw_container">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td colspan="3" align="center">
                                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-family: Verdana;
                                        font-size: 11px; width: 1000px">
                                        <thead align="left">
                                            <tr>
                                                <th align="center" style="width: 20px;">
                                                    <input type="checkbox" id="chkh" onclick=" checkall(this)" />
                                                </th>
                                                <th style="width: 50px;">
                                                    S No
                                                </th>
                                                <th>
                                                    Plate No
                                                </th>
                                                <th>
                                                    Email Id
                                                </th>
                                                <th>
                                                    Mobile No
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                        <tfoot align="left">
                                            <tr>
                                                <th>
                                                    <input type="checkbox" id="Checkbox1" onclick=" checkall(this)" />
                                                </th>
                                                <th>
                                                    S No
                                                </th>
                                                <th>
                                                    Plate No&nbsp;
                                                </th>
                                                <th>
                                                    Email Id
                                                </th>
                                                <th>
                                                    Mobile No
                                                </th>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <a href="#" class="button" title="Delete Alert" style="width: 59px;" onclick="javascript:Conformdelete()">
                        <span class="ui-button-text">Delete </span></a>
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right: 0px;">
                </td>
            </tr>
        </table>
    </center>
    <div id="div1" title="confirm">
        <p id="P1">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-msg" title="Alert">
        <p id="displayp">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-message" title="Add Alert" align="center" style="padding-top: 1px;
        padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 5px;
        font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <br />
        <table border="0" cellpadding="1" cellspacing="1" style="width: 320px; font-size: 11px;
            font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;
            vertical-align: middle;">
            <tr id="txttr" align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Plate No </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:DropDownList ID="ddlGeofence" runat="server" Width="182px" Height="20px" TabIndex="10">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">E Mail </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <input type="text" name="t1" value="" id="email1" style="width: 182px;" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Mobile No </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <input type="text" name="t1" value="" id="mobile1" style="width: 182px;" />
                </td>
            </tr>
        </table>
    </div>
    
    <div id="div_all" title="Add Multiple Alert" align="center" style="padding-top: 1px;
        padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 5px;
        font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <br />
        <table border="0" cellpadding="1" cellspacing="1" style="width: 100%; font-size: 11px;
            font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;
            vertical-align: middle;">
            <tr id="Tr1" align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Username</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:DropDownList ID="ddluser" runat="server" Width="182px" Height="20px" TabIndex="10"
                        onchange="javascript:getPlates()">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr align="left" id="mplate" style="display: none;">
                <td align="left" colspan="3">
                <div >
                 <b style="color: #4E6CA3;">Plate No :</b>
                    <br />
                    <span id="mulplate"></span>
                </div>
                   
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">E Mail </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <input type="text" name="t1" value="" id="emlMul" style="width: 182px;" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Mobile No </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <input type="text" name="t1" value="" id="mobMul" style="width: 182px;" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
