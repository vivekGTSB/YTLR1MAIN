<%@ Page Language="VB" AutoEventWireup="false" Debug="true" EnableEventValidation="false" Inherits="YTLWebApplication.LafargePrivateGeofenceAlertManagement" Codebehind="LafargePrivateGeofenceAlertManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>YTL Private Geofence Alert</title>
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
    <style type="text/css">
        .dataTables_filter
        {
            width: 50%;
            float: right;
            text-align: right;
            margin-top: 0px;
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
            if (document.getElementById("email1").value == "") {
                alertbox("Please enter email id");
                return false;
            }
            else if (document.getElementById("mobile1").value == "") {
                alertbox("Please enter mobile nos");
                return false;
            }
            else {
                return true;
            }
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

            $.getJSON('GetGeofenceAlertsData.aspx?opr=0&r=' + Math.random(), null, function (json) {
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
            $.getJSON('GetGeofenceAlertsData.aspx?geoid=' + chekitems + '&opr=3&r=' + Math.random(), null, function (json) {
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

            fnFeaturesInit();
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 100,
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

                             ]
            });
            $("div.MyButton").html("");

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
                $.get('GetGeofenceAlertsData.aspx?geoid=' + geoid + extracond + listcond + '&r=' + Math.random(), null, function (json) {
                    if (json == 1) {
                        alertbox("Successfully processed..");
                        refreshTable();
                    }
                });
                $("#dialog-message").dialog("close");
            }
        }



        function AddData() {
            var res = validate();
            var listcond = "&eml=" + $("#email1").val() + "&mob=" + $("#mobile1").val();
            var extracond = "&opr=1";
            var geoid = $("#ddlGeofence").val();
            if (res == true) {
                $.get('GetGeofenceAlertsData.aspx?geoid=' + geoid + extracond + listcond + '&r=' + Math.random(), null, function (json) {
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
    <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
</head>
<body style="font-size: 11px; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">
    <center>
        <br />
        <br />
        <div style="font-family: Verdana; font-size: 22px; color: #38678B; font-weight: bold;">
            &nbsp;YTL Private Geofence Alert</div>
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
                                                    Geofence Name
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
                                                    Geofence Name
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
                    <b style="color: #4E6CA3;">Geofence Name </b>
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
    </form>
</body>
</html>
