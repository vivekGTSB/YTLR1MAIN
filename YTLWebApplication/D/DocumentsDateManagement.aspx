<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.DocumentsDateManagement" Codebehind="DocumentsDateManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Documents Date Management</title> 
     <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/ColVis.css";

    </style>
    <style type="text/css">
        .dataTables_filter
        {
            width: 50%;
            float: right;
            text-align: right;
            margin-top: -24px;
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
        .dataTables_filter
        {
            width: 50%;
            float: right;
            text-align: right;
            margin-top: -27px;
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
            width: 1188px;
        }
        
        table.display thead th div.DataTables_sort_wrapper
        {
            font-size: 11px;
            font-family: verdana;
        }
        table.display tfoot th
        {
            font-weight: bold;
            font-family: verdana;
            font-size: 11px;
        }
        table.display td
        {
            padding-left: 12px;
        }
        .table
        {
            font-size: 11px;
            font-weight: normal;
            font-family: verdana;
        }
        
        tfoot input
        {
            margin: 0.2em 0;
            width: 100%;
            margin-left: -0.7em;
            color: #444;
        }
         button, .button
        {
            width: inherit;
            height: inherit;
        }
    </style>
    <style type="text/css">
        .chzn-container-single .chzn-single
        {
            height: 24px;
            line-height: 24px;
        }
        .chzn-search
        {
            width: 203px;
        }
        .chzn-container .chzn-results
        {
            max-height: 300px;
        }
    </style>
    <style type="text/css">
        .ui-button
        {
            font-size: 12px;
        }
         .ui-widget-header{
            height: 20px;
    font-size: 11px;
        }
         .MyButton{
             margin-top :-7px;
         }
    </style>
    <link type="text/css" href="cssfiles/css3-buttons.css" rel="stylesheet" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
    <script src="jsfiles/jquery-ui.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <style type="text/css">
        .ui-button-text
        {
            font-size: 11px;
        }
        .ui-text
        {
            font-size: 10px;
        }
         tfoot input
        {
            margin: 0.2em 0;
            width: 100%;
            margin-left: -0.7em;
            color: #444;
        }
        .leftClass
        {
            text-align: left;
        }
    </style>
    <script type="text/javascript" language="javascript">


        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
        }
        function deleteconfirmation() {
            var checked = false;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];
                if (elm.type == 'checkbox') {
                    if (elm.checked == true) {
                        checked = true;
                        break;
                    }
                }
            }
            if (checked) {
                var result = confirm("Are you sure you want to delete selected documents(s) ?");
                if (result) {
                    return true;
                }
                return false;
            }
            else {
                alertbox("Please select checkboxes");
                return false;
            }
        }
        function AddData() { 

            var lastXHR = $.get("DocMgmtJson.aspx?p=" + $("#ddlplate1").val() + "&opr=2&u=" + $('#ddluser').val() + "&rtax=" + $('#txtrtax').val() + "&pt=" + $('#txtptest').val() + "&insu=" + $('#txtins').val() + "&e1=" + $('#txtpexp').val() + "&oe1=" + $('#txtOtherexp').val() + "&em1=" + $('#txtEmail').val() + "&em2=" + $('#txtEmail2').val() + "&rem=" + $('#txtRemarks').val(), function (data) {
                if (data == "Yes") {
                    //closeDiv();
                    return false;
                }
                else {
                    alertbox("<b style='Color:Red;'>Sorry.Please try again..!!!</b>");
                    // closeDiv();
                    var x;
                    return false;
                }
            });
            $("#dialog-form").dialog("close");
            $('#ddlplate').val() == "";
            $('#rtax1').val() == "";
            $('#rptest1').val() == "";
            $('#rins1').val() == "";
            $('#rexp1').val() == "";
            $('#rpma1').val() == "";
            $('#rptm1').val() == "";
            $('#rbdm1').val() == "";
            refreshTable();
        }

        function UpdateData() {
            var lastXHR = $.get("DocMgmtJson.aspx?p=" + $("#ddlplate1").val() + "&opr=3&u=" + $('#ddluser').val() + "&rtax=" + $('#txtrtax').val() + "&pt=" + $('#txtptest').val() + "&insu=" + $('#txtins').val() + "&e1=" + $('#txtpexp').val() + "&oe1=" + $('#txtOtherexp').val() + "&em1=" + $('#txtEmail').val() + "&em2=" + $('#txtEmail2').val() + "&rem=" + $('#txtRemarks').val(), function (data) {
                if (data == "Yes") {
                    //closeDiv();
                    return false;
                }
                else {
                    alertbox("<b style='Color:Red;'>Sorry.Please try again..!!!</b>");
                    // closeDiv();
                    var x;
                    return false;
                }
            });
            $("#dialog-message").dialog("close");
            $('#ddlplate1').val() == "";
            $('#txtrtax').val() == "";
            $('#txtptest').val() == "";
            $('#txtins').val() == "";
            $('#txtpexp').val() == "";
//            $('#txtpma').val() == "";
            refreshTable();
        }


        function DeleteGroup() {

            var checked = false;
            var matches = [];
            $(".group1:checked").each(function () {
                matches.push(this.value);
            });

            var lastXHR = $.get("DocMgmtJson.aspx?ugData=" + matches + "&opr=2", function (data) {
                if (data == "Yes") { 
                    return false;
                }
                else {
                    alertbox("<b style='Color:Red;'>Sorry.Please try again..!!!</b>"); 
                    var x;
                    return false;
                }
            }); 

            refreshTable();
            //                checked = false;
            $(".group1:checked").each(function () {
                $(this).removeAttr('checked');
            });
        }

        function refreshTable() {
            table = oTable.dataTable();
            table._fnProcessingDisplay(true);
            oSettings = table.fnSettings();
            $.getJSON('DocMgmtJson.aspx?u=' + $("#ddluser").val() + '&opr=1&r=' + $("#rle").val() + '&lst=' + $("#ulist").val() + '&r=' + Math.random(), null, function (json) {
                table.fnClearTable(this);
                for (var i = 0; i < json.length; i++) {
                    table.oApi._fnAddData(oSettings, json[i]);
                }
                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                table._fnProcessingDisplay(false);

                table.fnDraw();
            });
            return false;
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
            $("#dialog:ui-dialog").dialog("destroy");
            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 320,
                buttons: {
                    Update: function () {
                        if ($('#ddlplate1').val() == "") {
                            alertbox("Please Select Plate Number.");
                            return false;
                        } 

                        if ($('#txtEmail').val() == "") {
                            alertbox("Please enter Email.");
                            return false;
                        }
                        var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

                        if (!emailRegEx.test(document.getElementById("txtEmail").value)) {
                            alertbox("Please valid enter Email Id.");
                            return false;
                        }

                        if ($('#txtEmail2').val() != "") {
                            var arr;
                            var emls = document.getElementById("txtEmail2").value;
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
                                    alertbox("Please valid enter EMail Id (CC).");
                                    return false;
                                }
                            }

                        } 

                        UpdateData();

                    },
                    Close: function () {
                        $(this).dialog("close");
                    }
                }
            });

            $("#dialog-form").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 320,

                buttons: {
                    Add: function () {
                        AddData();
                    },
                    Close: function () {
                        $(this).dialog("close");
                    }
                }
            });
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
            $("#div1").dialog(
        {
            title: "Confirm",
            modal: true,
            resizable: false,
            width: 320,
            autoOpen: false,
            buttons: {
                "YES": function () {
                    // This will invoke the form's action - putatively deleting the resources on the server
                    //                    $(form).submit();
                    DeleteGroup();
                    $(this).dialog("close");
                },
                "NO": function () {
                    // Don't invoke the action, just close the dialog
                    $(this).dialog("close");
                }
            }
        });

            fnFeaturesInit();
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 100,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "bLengthChange": false,
                "aaSorting": [[2, "asc"]],
                "fnDrawCallback": function (oSettings) {
//                    if (oSettings.bSorted || oSettings.bFiltered) {
//                         if (oSettings.aoColumns[0].bVisible == true) {
//                        for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
//                            $('td:eq(1)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
//                        }
//                      }
//                    }
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
                "aoColumnDefs": [
                    { "bVisible": true, "bSortable": false, "aTargets": [0] },
                    { "bVisible": false, "bSortable": false, "sWidth": "47px", "aTargets": [1] },
                    { "bVisible": true, "bSortable": true, "sWidth": "150px", "aTargets": [2] },
                    { "bVisible": true, "bSortable": true, "sWidth": "150px", "aTargets": [11] },
                    { "bVisible": true, "bSortable": true, "sWidth": "100px", "aTargets": [3] },
                    { "bVisible": true, "bSortable": true, "sWidth": "150px", "aTargets": [4] },
                    { "bVisible": true, "bSortable": true, "sWidth": "80px", "aTargets": [5] },
                    { "bVisible": true, "bSortable": true, "sWidth": "130px", "aTargets": [6] },
                    { "bVisible": true, "bSortable": true, "sWidth": "180px", "aTargets": [7] },
                    { "bVisible": true, "bSortable": true, "sWidth": "80px", "aTargets": [8] },
                    { "bVisible": true, "bSortable": true, "sWidth": "80px", "aTargets": [9] },
                    { "bVisible": true, "bSortable": true, "aTargets": [10] },
                ]
            });
            $("div.MyButton").html('<div><table><tr><td>Username</td><td><%=opt %></td></tr> </table></div>');
            jQuery(".chosen").data("placeholder", "Select Frameworks...").chosen();
            refreshTable();
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
        function confirm(message) {
            document.getElementById("P1").innerHTML = message;
            $("#div1").dialog("open");
        }

        function openPopup(userid, username, plateno, rtax, ptest, insu, exp,otherexp, pma, btm, bdm, emailid, emailid2, remarks) {
            $('#hdnUserId').val(userid);
            $('#ddlusera').val(username);
            $('#ddlplate1').val(plateno);
            $('#txtrtax').val(rtax);
            $('#txtptest').val(ptest);
            $('#txtins').val(insu);
            $('#txtpexp').val(exp);
            $('#txtOtherexp').val(otherexp);
//            $('#txtpma').val(pma);
//            $('#txtptm').val(btm);
//            $('#txtbdm').val(bdm);
            $('#txtEmail').val(emailid);
            $('#txtEmail2').val(emailid2);
            $('#txtRemarks').val(remarks);
            $("#dialog-message").dialog("open");

        }
        function openPopup1() {
            $('#ddlusername').val("");
            $('#ddlplate').val("");
            $('#ddlplate').val() == "";
            $('#rtax1').val() == "";
            $('#rptest1').val() == "";
            $('#rins1').val() == "";
            $('#rexp1').val() == "";
            $('#rpma1').val() == "";
            $('#rptm1').val() == "";
            $('#rbdm1').val() == "";
            $("#dialog-form").dialog("open");

        }
        function LoadGrouplist(userid, ddlgroup) {
            $("#ddlplate").attr("disabled", "disabled");


            var user = document.getElementById(userid).value;
            if (user == "--Select User Name--") {
                ddlgroup.empty().append('<option selected="selected"  value="0">PLEASE SELECT USERNAME</option>');

            }
            else {
                $('#ddlplate').empty().append('<option selected="selected" value="0">Loading...</option>');

                $.ajax({
                    type: "POST",
                    url: "DocumentsDateManagement.aspx/LoadGrouplist",
                    data: '{userId: \"' + user + '\"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnLoadVehicles1,
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
        }
        function OnLoadVehicles1(response) {
            PopulateControl(response.d, $("#ddlplate"));
        }
        function PopulateControl(list, control) {
            if (list.length > 0) {
                control.removeAttr("disabled");
                control.empty().append('<option selected="selected" value="0">Please select vehicle</option>');
                $.each(list, function () {
                    control.append($("<option></option>").val(this['Value']).html(this['Text']));
                });
            }
            else {
                control.empty().append('<option selected="selected" value="0">No vehicles<option>');
            }
        }
        
    </script>
 
    <script type="text/javascript" language="javascript">
        $(function () {
            $("#rtax1").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0
            });

            $("#rptest1").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0

            });
            $("#rins1").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0
            });

            $("#rexp1").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0

            });
            $("#rpma1").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0

            });

            $("#txtrtax").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0
            });

            $("#txtptest").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0

            });
            $("#txtins").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0
            });

            $("#txtpexp").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0

            });
           
            $("#txtOtherexp").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: -0

            });

        });

        function ExcelReport() {
            refreshTable();
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();
        }
    </script>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 10px; margin-right: 5px;">
    <form id="vehicleform" runat="server" enableviewstate="false">
    <center>
        <br />
        <br />
        <b style="font-family: Verdana; font-size: 20px; color: #38678B;">
            Vehicle Documents Date Management</b>
        <table style="font-family: Verdana; font-size: 11px; width: 1200px">
            <tr>
                <td align="left">
                <input type="button" id="ImageButton2" class="action blue" runat="server" value="Delete"
                        title="Delete Checked Documents" onclick="deleteconfirmation()" style="margin-left: 0px;" />
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle">
                <a href="#" onclick="javascript:ExcelReport()" class="button" title="Download All Settings"
                            style="width: 59px; margin-right: 0px;"><span class="ui-button-text">Download </span>
                        </a>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                  
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples">
                            <thead>
                                <tr style="text-align: left">
                                    <th style="width: 15px;">
                                        <input type="checkbox" name="chk" class="group1" onclick="javascript:checkall(this);" />
                                    </th>
                                    <th style="width: 65px;">S No
                                    </th>
                                    <th style="width: 100px;">Plate No
                                    </th>
                                    <th style="width: 100px;">PM ID
                                    </th>
                                    <th style="width: 100px;">Road Tax
                                    </th>
                                    <th style="width: 100px;">Puspakom Test
                                    </th>
                                    <th style="width: 100px;">Insurance
                                    </th>
                                    <th style="width: 100px;">Permit Expiry
                                    </th>
                                    <th style="width: 120px;">Other Doc Expiry
                                    </th>
                                    <th>Email Id
                                    </th>
                                    <th>Email Id (CC)
                                    </th>
                                    <th>Remarks
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            <tfoot>
                                <tr style="text-align: left">
                                    <th style="width: 15px;">
                                        <input type="checkbox" name="chk" class="group1" onclick="javascript:checkall(this);" />
                                    </th>
                                    <th style="width: 50px;">S No
                                    </th>
                                    <th>Plate No
                                    </th>
                                    <th>PM ID
                                    </th>
                                    <th>Road Tax
                                    </th>
                                    <th>Puspakom Test
                                    </th>
                                    <th>Insurance
                                    </th>
                                    <th>Permit Expiry
                                    </th>
                                    <th>Other Doc Expiry
                                    </th>
                                    <th>Email Id
                                    </th>
                                    <th>Email Id (CC)
                                    </th>
                                    <th>Remarks
                                    </th>
                                </tr>
                            </tfoot>
                        </table>
                    
                </td>
            </tr>
            <tr>
                <td align="left">
                <input type="button" id="ImageButton1" class="action blue" runat="server" value="Delete"
                        title="Delete Checked Documents" onclick="deleteconfirmation()" style="margin-left: 0px;" />
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle">
                </td>
            </tr>
            <tr>
            <td  colspan="3" style="color: Blue;text-align:center; font-family: Verdana; font-size: 12px; font-weight: Bolder;">
            Details and Email Conditions for Document Alerts
            </td>
            </tr>
                <tr>
                <td style="text-align: left;">
                    <span style="color: navy; font-family: Verdana; font-size: 11px; font-weight: normal;">
                        <b>Road Tax</b>
                        <br />
                        RoadTax Expairy Date of Vehicle<br/>
                        <br />
                         <b>Puspakom Test</b>
                        <br />
                        Next Puspakom Test Date of Vehicle<br/>
                        <br />
                         <b>Insurance</b>
                        <br />
                        Insurance Expairy Date of Vehicle<br/>
                        <br />
                         <b>Permit Expairy</b>
                        <br />
                         Expairy Date Permit  of Vehicle<br/>
                        <br />
                         <b>Others</b>
                        <br />
                        Other Documents Expary Date of Vehicle<br/>
                        <br />
                        <b>Email Id</b>
                        <br />
                        Mail Receiver Email Id  (Mendatory)<br/>
                        <br />
                        <b>Email CC</b>
                        <br />
                        Carbon Copy Email Id  (Optional)<br/>
                        <br />
                        <b>Remarks</b>
                        <br />
                        Your Comments or Statements (Optional)   
                        <br />
                        <br />
                                        
                    </span>
                     
                </td>
                <td align="center">
                </td>
                <td style="text-align:left;" valign="middle">
                 <span style="color: navy; font-family: Verdana; font-size: 11px; font-weight: normal;">
                      <b>Email will be send by the below conditions :</b>  
                      <br />
                      For All Expiry Dates<br />
                      <table style ="float :left ;"><tr><td>  1st Alert :</td><td>15 days Before.</td></tr>
                        
                        <tr><td>2nd Alert :</td><td> 7 days Before.</td></tr>
                       <tr><td> 3rd Alert :</td><td> On Expiry Date</td></tr>
                       <tr><td> 4th Alert :</td><td> 1st Day after Expiry Date .</td></tr>
                      <tr><td>  5th Alert : </td><td>2nd Day after Expiry Date .</td></tr>
                       <tr><td> Final Alert :</td><td> 3rd Day after Expiry Date .</td></tr>
                     </table>
                      </span>
                      <br />
                      <br />
                      <%--<b style="color: Red; font-family: Sans-Serif; font-size: 11px;">* Will continue to send One email per day untill new Servicing date is assigned.</b>--%>
                </td>
            </tr>
        </table>
        <div id="dialog-message" title="Update Document" style="padding-top: 1px; padding-right: 0px;
            padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;
            height: 244px; margin-bottom: 0px;">
            <input type="hidden" name="t1" value="" id="hdnUserId" />
            <table border="0" cellpadding="1" cellspacing="1" style="width: 266px; font-size: 11px;
                font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;padding-left :5px;" >
                <tr>
                    <br />
                    <td align="left">
                        <b style="color: #4E6CA3;">User Name</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox ID="ddlusera" runat="server" Width="151px" ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Plate No</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox ID="ddlplate1" runat="server" Width="151px" ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Road Tax</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox ID="txtrtax" runat="server" Width="151px"  ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Puspakom Test</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtptest" Width="151px"   ReadOnly="true"/>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Insurance</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtins" Width="151px"  ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Permit Expiry</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtpexp" Width="151px"  ReadOnly="true" />
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Other Document</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtOtherexp" Width="151px"   />
                    </td>
                </tr>
               
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Email Id</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtEmail" Width="151px" />
                    </td>
                </tr>
                  <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Email Id (CC)</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtEmail2" Width="151px" />
                    </td>
                </tr>
                  <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Remarks</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtRemarks" Width="151px" TextMode="MultiLine" />
                    </td>
                </tr>
            </table>
        </div>
        <input type="hidden" value="" id="ss" runat="server" />
        <input type="hidden" name="uid" value="" runat="server" id="uid" />
        <input type="hidden" name="rle" value="" runat="server" id="rle" />
        <input type="hidden" name="ulist" value="" runat="server" id="ulist" />
        <input type="hidden" id="gid" value="" runat="server" />
    </center>
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Document Alerts Settings" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
    <div id="div1" title="Confirm" style="font-size: 11px; font-weight: normal;">
        <p id="P1">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-msg" title="Alert" style="font-size: 11px; font-weight: normal;">
        <p id="displayp">
            <span class="ui-icon ui-icon-circle-check" style="float: left; font-size: 7px; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    
  
</body>
</html>
