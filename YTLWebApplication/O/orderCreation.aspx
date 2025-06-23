<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.orderCreation" Codebehind="orderCreation.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Delivery Order Creation</title>
<%--    <style type="text/css" media="screen">
        @import "css/demo_table_jui.css";
        @import "css/jquery-ui.css";
        @import "css/TableTools.css";
        @import "css/ColVis.css";
        @import "css/common1.css";
        @import "css/chosen.css";
    </style>

    <script type="text/javascript" src="js/googana.js"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/jquery_ui.js"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="js/FixedColumns.js"></script>--%>

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
            width:100%;
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
        .singleclass{
            white-space:nowrap;
        }
        .multiline{
            overflow-wrap:anywhere;
            max-width:300px;
        }
        
    </style>
    <script type="text/javascript" language="javascript">




        function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();

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


      
        var oTable;

        $(document).ready(function () {
            $("#txtBeginDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: -30, maxDate: +60, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txtEndDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: -30, maxDate: +60, changeMonth: true, changeYear: true, numberOfMonths: 2

            });
            $(".inreal").keypress(function (event) {
                var keyCodeEntered = (event.which) ? event.which : (window.event.charCode) ? window.event.charCode : -1;
                if (event.keyCode == 8) {
                    $(this.parentNode.parentElement.childNodes[2]).hide();
                    return true;
                }
                else if ((keyCodeEntered >= 48) && (keyCodeEntered <= 57)) {
                    $(this.parentNode.parentElement.childNodes[2]).hide();
                    //$(".validator").hide();
                    return true;
                }
                else {
                    $(this.parentNode.parentElement.childNodes[2]).show();
                }
                return false;
            });
            $("#dialog-message").hide();
            $("#dialog:ui-dialog").dialog("destroy");
            $("#dialog-msg").dialog({
                autoOpen: false,
                resizable: false,
                height: 140,
                modal: true,
                buttons: {
                    "Close": function () {
                        $(this).dialog("close");
                    }

                }
            });
            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 500,
                minHeight: 500,
                height: 500,
                buttons: {

                    Add: function () {
                        AddOrder();
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
                "iDisplayLength": 5000,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "bLengthChange": false,
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
                    { "bVisible": true, "bSortable": false, "sWidth": "20px", "sClass": "right", "aTargets": [0] },
                    { "bVisible": true, "sWidth": "150px","bSortable": true, "aTargets": [1] },
                    { "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [2] },
                    { "bVisible": true, "sWidth": "130px", "sClass": "singleclass", "bSortable": true, "aTargets": [3] },
                    { "bVisible": true, "sWidth": "130px", "sClass": "singleclass", "bSortable": true, "aTargets": [4] },
                    { "bVisible": true, "bSortable": true, "aTargets": [5] },
                    { "bVisible": true, "bSortable": true, "sWidth": "300px", "aTargets": [6] },
                    { "bVisible": true, "bSortable": true, "aTargets": [7] },
                    { "bVisible": true, "bSortable": true, "aTargets": [8] },
                    { "bVisible": true, "sWidth": "300px", "sClass": "multiline", "bSortable": true, "aTargets": [9] },
                    { "bVisible": true, "sWidth": "300px", "sClass": "multiline", "bSortable": true, "aTargets": [10] },
                ]
            });

            refreshData();
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
            $("#oid").val();
            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 500,
                minHeight: 500,
                height: 500,
                buttons: {
                    Add: function () {
                        AddOrder();
                    },
                    Close: function () {
                        $(this).dialog("close");
                    }
                }
            });
            $("#oid").val(0);
            $("#dialog-message").dialog("open");
        }
        function openUpdatePopup(pid, pname, uid) {
            $("#opr").val(1);
            $("#txtcustomername").val(unescape(pname));
            $("#txtcustomerid").val(unescape(pid));

            $("#dialog-message").dialog({
                title: "Update Customer",
                buttons: {
                    "Update Customer Info": function () {
                        UpdateData(uid);
                    },
                    "Close": function () {
                        $(this).dialog("close");

                    }
                }
            });
           
            $("#dialog-message").dialog("open");
        }




        function AddOrder() {
            if (Validate()) {
                var bdt = $("#txtBeginDate").val() + " " + $("#ddlbh").val() + ":" + $("#ddlbm").val() + ":00"
                var edt = $("#txtEndDate").val() + " " + $("#ddleh").val() + ":" + $("#ddlem").val() + ":59"
                var data = $("#ddlplant").val();
                var source = "";
                for (let i = 0; i < data.length; i++) {
                    if (source == "") {
                        source = data[i];
                    }
                    else {
                        source = "," + data[i];
                    }

                }
                $.ajax({
                    type: "POST",
                    url: "orderCreation.aspx/InsertOrder",
                    data: '{plant: \"' + $("#ddlplant").val() + '\",cid:\"' + escape($("#txtcustomerid").val()) + '\",cname:\"' + escape($("#txtcustomername").val()) + '\",bdt: \"' + bdt + '\",edt:\"' + edt + '\",trips:\"' + $("#txtordertrip").val() + '\",tonnage:\"' + $("#txttonnage").val() + '\",internaltruck:\"' + escape($("#txtinternal").val()) + '\",externaltruck:\"' + escape($("#txtexternal").val()) + '\",destination:\"' + $("#ddlcustomer").val() + '\",id:\"' + $("#oid").val() + '\"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {

                        var json = response;
                        if (json == 1) {
                            alertbox("Order details added successfully");
                            refreshData();
                            $("#dialog-message").dialog("close");
                        }
                        else {
                            alertbox("Something went wrong, Please try again");
                        }

                    },
                    failure: function (response) {
                        alert("Error");
                    }
                });
            }

        }

        function Validate() {
            if ($("#ddlplant").val() == null) {
                alertbox("Please select source");
                return false;
            }
            if ($("#ddlcustomer").val() == 0) {
                alertbox("Please select customer");
                return false;
            }
            if ($("#txtcustomerid").val() == "") {
                alertbox("Please enter order no");
                return false;
            }
            if ($("#txtcustomername").val() == "") {
                alertbox("Please enter customer name");
                return false;
            }
            if ($("#txtordertrip").val() == "") {
                alertbox("Please enter trips");
                return false;
            }
            if ($("#txttonnage").val() == "") {
                alertbox("Please enter tonnage");
                return false;
            }
            if ($("#txtinternal").val() == "") {
                alertbox("Please enter internal trucks");
                return false;
            }
            //if ($("#txtexternal").val() == "") {
            //    alertbox("Please select source");
            //    return false;
            //}

            return true;
        }

        function refreshData() {
            $.ajax({
                type: "POST",
                url: "orderCreation.aspx/GetData",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response.aaData;
                    table = oTable.dataTable();
                    table._fnProcessingDisplay(true);
                    oSettings = table.fnSettings();
                    table.fnClearTable(this);
                    for (var i = 0; i < json.length; i++) {
                        json[i][1] = "<span style=\"color:blue;cursor:pointer;\" onclick=\"javascript:openPopup('" + json[i][0] + "','" + json[i][1] + "','" + json[i][2] + "','" + json[i][3] + "','" + json[i][4] + "','" + json[i][5] + "','" + json[i][11] + "','" + json[i][7] + "','" + json[i][8] + "','" + json[i][9] + "','" + json[i][10] + "');\">" + json[i][1] + "</span>";
                        table.oApi._fnAddData(oSettings, json[i]);
                    }
                    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                    table._fnProcessingDisplay(false);
                    table.fnDraw();
                    ec = true;
                },
                failure: function (response) {
                    alert("Error");
                }
            });
        }


        function openPopup(id, orderid, customername,bdt,edt,source,dest, trips, tonnage, internal,external) {
            $("#ddlplant").val(source);
            $("#txtcustomerid").val(orderid);
            $("#txtcustomername").val(customername);
            $("#txtordertrip").val(trips);
            $("#txttonnage").val(tonnage);
            $("#txtinternal").val(internal);
            $("#txtexternal").val(external);
            $("#ddlcustomer").val(dest);
            $("#oid").val(id);
            var dates = bdt.toString().split(" ");
            if (dates.length > 0) {
                $("#txtBeginDate").val(dates[0]);
                let times = dates[1].split(":");
                $("ddlbh").val(times[0]);
                $("ddlbm").val(times[1]);

            }
            dates = edt.toString().split(" ");
            if (dates.length > 0) {
                $("#txtEndDate").val(dates[0]);
                let times = dates[1].split(":");
                $("ddleh").val(times[0]);
                $("ddlem").val(times[1]);

            }
            $("#dialog-message").dialog("open");
        }





        function Clearfields() {
            $("#ddlplant").val("");
            $("#txtcustomerid").val("");
            $("#txtcustomername").val("");
            $("#txtordertrip").val("");
            $("#txttonnage").val("");
            $("#txtinternal").val("");
            $("#txtexternal").val("");
            $("#ddlcustomer").val(0);
        }
        function alertbox(message) {
            if (message == "") {
                document.getElementById("displayp").innerHTML = message;

            }
            else {
                document.getElementById("displayp").innerHTML = message;
                $("#dialog-msg").dialog("open");
            }
        }
        function confirm(message) {

            document.getElementById("P1").innerHTML = message;
            $("#div1").dialog("open");

        }



    </script>
    <link href="css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="js/chosen.jquery.js"></script>
</head>
<body style="font-size: 11px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">
   
    <center>
        <br />
        <br />
        <div style="font-family: Verdana; font-size: 22px; color: #38678B; font-weight: bold;">
            &nbsp;Delivery Order Creation </div>
        <br />
        <table style="font-family: Verdana; font-size: 11px; width: 100%">
            <tr>
                <td align="left">
                     <a  class="button" title="Add New Customer" style="width: 79px; " onclick="openAddPopup()">
                        <span class="ui-button-text">Add </span></a>
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right: 0px;">
                 
                        <a href="javascript:ExcelReport();" class="button" style="margin-right: 3px; width: 75px;">
                       Save Excel</a>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="width: 100%">
                    <div id="fw_container" style="max-width: 100%">
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples"  style="font-family: Verdana; font-size: 11px; width: 100%">
                                        <thead align="left">
                                            <tr >                                               
                                              <th style="width: 50px;">
                                                 S No
                                                </th>
                                                <th>
                                                    Sales Order NO
                                                </th>
                                                <th>
                                                   Customer Name
                                                </th>
                                                 <th>
                                                   From
                                                </th>
                                                 <th>
                                                   To
                                                </th>
                                                 <th>
                                                   Source
                                                </th>
                                                 <th>
                                                   Destination
                                                </th>
                                                <th>
                                                    Total Order Trip
                                                </th>
                                                <th>
                                                    Total Order Tonnage
                                                </th>
                                                <th>
                                                    Internal Trucks
                                                </th>
                                                <th>
                                                    External Trucks
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                        <tfoot align="left">
                                            <tr >                                               
                                             <th style="width: 50px;">
                                                 S No
                                                </th>
                                                <th>
                                                    Sales Order NO
                                                </th>
                                                <th>
                                                   Customer Name
                                                </th>
                                                 <th>
                                                   From
                                                </th>
                                                 <th>
                                                   To
                                                </th>
                                                <th>
                                                   Source
                                                </th>
                                                 <th>
                                                   Destination
                                                </th>
                                                <th>
                                                    Total Order Trip
                                                </th>
                                                <th>
                                                    Total Order Tonnage
                                                </th>
                                                <th>
                                                    Internal Trucks
                                                </th>
                                                <th>
                                                    External Trucks
                                                </th>
                                            </tr>
                                        </tfoot>
                                    </table>
                    </div>
                </td>
            </tr>
           
        </table>
        
        <input type="hidden" id="opr" runat="server" />
        <input type="hidden" id="oid" runat="server" />
        <input type="hidden" name="uid" value="" runat="server" id="uid" />
        <input type="hidden" name="rle" value="" runat="server" id="rle" />
        <input type="hidden" name="ulist" value="" runat="server" id="ulist" />
        <input type="hidden" id="gid" value="" runat="server" />
        <input type="hidden" value="" id="ss" runat="server" />
    </center>
 
    <div id="dialog-msg" title="Alert!">
        <p id="displayp">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-message" title="Add Order" align="center" style="padding-top: 1px;
        padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 5px;
        font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <table border="0" cellpadding="1" cellspacing="1" style="width: 320px; font-size: 11px;
           font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;
            vertical-align: middle;">
            <br />      
           <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                       Plant
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                     <asp:DropDownList runat="server" ID="ddlplant" Width="355px"  EnableViewState="False" multiple>
                        </asp:DropDownList>
                </td>
            </tr>

              <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                       Customer
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                     <asp:DropDownList runat="server" ID="ddlcustomer" EnableViewState="False">
                        </asp:DropDownList>
                </td>
            </tr>
                 <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                       Sales Order NO
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txtcustomerid" runat="Server" CssClass="textbox1" TabIndex="11" />
                </td>
            </tr>
             <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                      Customer Name
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txtcustomername" runat="Server" CssClass="textbox1" TabIndex="11" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                      Total Order Trip
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txtordertrip" runat="Server" CssClass="textbox1 inreal" TabIndex="11" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                      Total Order Tonnage
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txttonnage" runat="Server" CssClass="textbox1 inreal" TabIndex="11" />
                </td>
            </tr>
             <tr>
                                            <td align="left">
                                                <b style="color: #4E6CA3;">Begin Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #4E6CA3;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                    id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" /><b
                                                        style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlbh" runat="server" Width="40px" EnableViewState="False">
                                                            <asp:ListItem Value="00">00</asp:ListItem>
                                                            <asp:ListItem Value="01">01</asp:ListItem>
                                                            <asp:ListItem Value="02">02</asp:ListItem>
                                                            <asp:ListItem Value="03">03</asp:ListItem>
                                                            <asp:ListItem Value="04">04</asp:ListItem>
                                                            <asp:ListItem Value="05">05</asp:ListItem>
                                                            <asp:ListItem Value="06">06</asp:ListItem>
                                                            <asp:ListItem Value="07" Selected ="True" >07</asp:ListItem>
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
                                                    </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlbm" runat="server" Width="40px" EnableViewState="False">
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
                                                    </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #4E6CA3;">End Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #4E6CA3;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                    id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: 

#465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddleh" runat="server" Width="40px" EnableViewState="False">
                                                            <asp:ListItem Value="00">00</asp:ListItem>
                                                            <asp:ListItem Value="01">01</asp:ListItem>
                                                            <asp:ListItem Value="02">02</asp:ListItem>
                                                            <asp:ListItem Value="03">03</asp:ListItem>
                                                            <asp:ListItem Value="04">04</asp:ListItem>
                                                            <asp:ListItem Value="05">05</asp:ListItem>
                                                            <asp:ListItem Value="06">06</asp:ListItem>
                                                            <asp:ListItem Value="07" Selected="True">07</asp:ListItem>
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
                                                            <asp:ListItem Value="23" >23</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlem" runat="server" Width="40px" EnableViewState="False">
                                                            <asp:ListItem Value="00" Selected="True">00</asp:ListItem>
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
                                                            <asp:ListItem Value="59" >59</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </b>
                                            </td>
                                        </tr>
             <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Internal Trucks</b>
                    </td>
                  <td style="color: #4E6CA3;">
                    :
                </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtinternal" TextMode="MultiLine" Width="300px" Height="40px" />
                    </td>
                </tr>
           <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">External Trucks</b>
                    </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtexternal" TextMode="MultiLine" Width="300px" Height="40px" />
                    </td>
                </tr>
        </table>
    </div>
    </form>
      <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Delivery Orders" />
    <input type="hidden" id="rd" name="rd" value="Report Date" />
  
   
    </form>
</body>
</html>
