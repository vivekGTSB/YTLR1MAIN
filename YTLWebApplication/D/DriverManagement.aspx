<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.DriverManagement" Codebehind="DriverManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Driver Management</title>
    <link rel="shortcut icon" type="image/ico" href="images/car.ico">
    <style type="text/css" media="screen">
        @import "css/demo_table_jui.css";
        @import "css/jquery-ui.css";
        @import "css/TableTools.css";
        @import "css/ColVis.css";
        @import "css/common1.css";
        @import "css/chosen.css";
    </style>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/jquery_ui.js"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="js/FixedColumns.js"></script>
    <script type="text/javascript" language="javascript" src="js/ColReorder.js"></script>
    <style type="text/css">
        .dataTables_info
        {
            width: 32%;
            float: left;
        }
        
        
        .ui-widget-header
        {
            border: 0px solid #4297d7;
        }
        
        
        table.display thead th div.DataTables_sort_wrapper
        {
            position: relative;
            padding-right: 6px;
        }
        table.display tfoot th
        {
            padding: 0px 0px 0px 0px;
            padding-left: 3px;
        }
        table.display thead th
        {
            padding: 0px 0px 0px 0px;
            padding-left: 3px;
            height: 31px;
        }
        table.display td
        {
            padding: 2px 2px;
        }
        .dataTables_filter
        {
            width: 300px;
        }
        
        .MyButton
        {
            text-align: left;
            float: left;
            width: 350px;
        }
        
        
        .chzn-container .chzn-results
        {
            color: #4E6CA3;
        }
        .chzn-container .chzn-results
        {
            max-height: 300px;
        }
        
        .textbox1
        {
            height: 18px;
            width: 180px;
            border-right: #cbd6e4 1px solid;
            border-top: #cbd6e4 1px solid;
            border-left: #cbd6e4 1px solid;
            color: #0b3d62;
            border-bottom: #cbd6e4 1px solid;
        }
        .button
        {
            text-decoration: none;
            text-shadow: 0 1px 0 #fff;
            text-align: center;
            font: bold 11px Helvetica, Arial, sans-serif;
            color: #444;
            height: 24px;
            display: inline-block;
            width: 60px;
            margin: 5px;
            padding: 0px 3px 2px 2px;
            background: #F3F3F3;
            border: solid 1px #D9D9D9;
            border-radius: 2px;
            -webkit-border-radius: 2px;
            -moz-border-radius: 2px;
            -webkit-transition: border-color .20s;
            -moz-transition: border-color .20s;
            -o-transition: border-color .20s;
            transition: border-color .20s;
        }
        button
        {
            cursor: pointer;
        }
        a.button:active
        {
            border-color: #4B8DF8;
        }
        a.button:hover
        {
            color: White;
            text-shadow: 0 1px 0 #fff;
            border: 1px solid #2F5BB7 !important;
            background: #3F83F1;
            background: -webkit-linear-gradient(top, #4D90FE, #357AE8);
            background: -moz-linear-gradient(top, #4D90FE, #357AE8);
            background: -ms-linear-gradient(top, #4D90FE, #357AE8);
            background: -o-linear-gradient(top, #4D90FE, #357AE8);
        }
        a.button
        {
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
        button, .button
        {
            width: inherit;
            height: inherit;
        }
    </style>
    <script type="text/javascript" language="javascript">


        function validate() {
            if (document.getElementById("ddluserid").value == "SELECT USERNAME") {
                alertbox("<%=Literal19.Text%>");
                return false;
            }
            else if (document.getElementById("txtpoiname").value == "") {
                alertbox("Please enter Driver Name.");
                return false;
            }
//            else if (document.getElementById("txtDOB").value == "") {
//                alertbox("Please select Date of Birth.");
//                return false;
//            }
//            else if (document.getElementById("txtrfid").value == "") {
//                alertbox("Please enter RFID.");
//                return false;
//            }            
//            else if (document.getElementById("txtPhone").value == "") {
//                 alertbox("Please enter Mobile Number.");
//                return false;
//            }
//            else if (document.getElementById("txtAddress").value == "") {
//                alertbox("Please enter Address.");
//                return false;
//            }
//            else if (document.getElementById("txtLicenceno").value == "") {
//                 alertbox("Please enter Licence No.");
//                return false;
//            }

//            else if (document.getElementById("txtIssuingdate").value == "") {
//                alertbox("Please select Issuing Date.");
//                return false;
//            }
//            else if (document.getElementById("txtExpiryDate").value == "") {
//                 alertbox("Please select Expiry Date.");
//                return false;
//            }
//             else if (document.getElementById("txtFuelCardNo").value == "") {
//                 alertbox("Please enter Fuel Card Number.");
//                return false;
//            }

            else if (document.getElementById("txtdriveric").value == "") {
                alertbox("Please enter driver IC");
                return false;
            }
            else if (document.getElementById("txtpwd").value == "") {
               document.getElementById("txtpwd").value = document.getElementById("txtdriveric").value;
               return true;
            }

            else {
                return true;
            }
        }

         function confirmInActive(message) {

            document.getElementById("P2").innerHTML = message;
            $("#div2").dialog("open");

        }

        function confirmActive(message) {

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
            $.get("GetDriverManagement.aspx?ugData="+$('#ddluser1').val()+"&role="+$('#rle').val()+"&userslist="+$('#ulist').val()+"&op=1", function (response) {

                var json = JSON.parse( response);
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


           <%-- $.ajax({
                type: "POST",
                url: "DriverManagement.aspx/FillGrid",
                data: '{ugData: \"' + $('#ddluser1').val() + '\",role: \"' + $('#rle').val() + '\",userslist: \"' + $('#ulist').val() + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response.d;
                    table = oTable.dataTable();
                    table._fnProcessingDisplay(true);
                    oSettings = table.fnSettings();
                    table.fnClearTable(this);
                    for (var i = 0; i < response.length; i++) {
                        table.oApi._fnAddData(oSettings, response[i]);
                    }
                    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                    table._fnProcessingDisplay(false);
                    table.fnDraw();
                    return false;
                },
                failure: function (response) {
                    alert("<%=Literal15.Text%>");
                }
            });--%>
        }

        function ConfirmActivate() {

         var checked = false;
            var matches = [];
            $(".group1:checked").each(function () {
                matches.push(this.value);
            });
            if (matches.length == 0) {
                alertbox("Please select at least one Driver to activate.");
            }
            else {
                confirmActive("Do you really want to activate the selected drivers ?");

            }
        }

        function ConfirmInActivate() {

         var checked = false;
            var matches = [];
            $(".group1:checked").each(function () {
                matches.push(this.value);
            });
            if (matches.length == 0) {
                alertbox("Please select at least one Driver to In-activate.");
            }
            else {
                confirmInActive("Do you really want to In-activate the selected drivers ?");

            }
        }



        function Activate() {
          var checked = false;
            var matches = [];
            $(".group1:checked").each(function () {
                matches.push(this.value);
            });

            $.get("GetDriverManagement.aspx?chekitems="+matches+"&op=3", function (response) {
                Ondeletesuccess(response);
                refreshTable();

            });

            //$.ajax({
            //    type: "POST",
            //    url: "DriverManagement.aspx/Activate",
            //    data: "{'chekitems':" + JSON.stringify(matches) + "}",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: Ondeletesuccess,
            //    failure: function (response) {
            //        alert(response.d);
            //    }
            //});

        }

         function InActivate() {
          var checked = false;
            var matches = [];
            $(".group1:checked").each(function () {
                matches.push(this.value);
            });

            $.get("GetDriverManagement.aspx?chekitems=" + matches + "&op=4", function (response) {
                Ondeletesuccess(response);
                refreshTable();

            });

            //$.ajax({
            //    type: "POST",
            //    url: "DriverManagement.aspx/InActivate",
            //    data: "{'chekitems':" + JSON.stringify(matches) + "}",
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: Ondeletesuccess,
            //    failure: function (response) {
            //        alert(response.d);
            //    }
            //});

        }

        function Ondeletesuccess(response) {

            if (response.d > 0) {
                refreshTable();
            }
            else if (response.d <= 0) {
                alertbox("Sorry, operation failed.");
            }

        }

        var oTable;
        var chekitems = new Array();
        var confirmresult = false;
        var checkids = "";
        $(document).ready(function () {

            $("#txtExpiryDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: 0, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txtIssuingdate").datepicker({ dateFormat: 'yy/mm/dd',  maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

            });

            $("#txtDOB").datepicker({ dateFormat: 'yy/mm/dd', yearRange: '-100:+0',maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

           

            $("#dialog-message").hide();
            $("#dialog:ui-dialog").dialog("destroy");
            $("#dialog-msg").dialog({
                autoOpen: false,
                resizable: false,
                modal: true,
                buttons: {
                    "<%=Literal38.Text%>": function () {
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
                    "<%=Literal36.Text%>": function () {
                        confirmresult = true;
                        Activate();
                        $(this).dialog("close");
                    },
                    "<%=Literal37.Text%>": function () {
                        confirmresult = false;
                        $(this).dialog("close");

                    }
                }
            });

            $("#div2").dialog({
                resizable: false,
                draggable: false,
                height: 160,
                modal: true,
                autoOpen: false,
                buttons: {
                    Yes: function () {
                        confirmresult = true;
                        InActivate();
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
                width: 480,
                minHeight: 480,
                height: 480

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
                "oLanguage": {
                    "oPaginate": {
                        "sNext": "<%=Literal25.text %>",
                        "sFirst": "<%=Literal23.text %>",
                        "sLast": "<%=Literal26.text %>",
                        "sPrevious": "<%=Literal24.text %>"
                    },
                    "sSearch": "<%=Literal9.text %>",
                    "sEmptyTable": "<%=Literal21.text %>",
                    "sInfo": "<%=Literal22.text %>",
                    "sInfoFiltered": "<%=Literal43.Text%>",
                    "sZeroRecords":"<%=Literal44.Text%>",
                    "sInfoEmpty": "<%=Literal45.Text%>",
                    "sLoadingRecords":"<%=Literal46.Text%>",
                    "sProcessing": "<%=Literal47.Text%>",
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
                "aoColumnDefs": [
                    {
                        "sClass": "center", "sWidth": "10px", "aTargets": [0], "bSortable": false, "bVisible": true,
                        "fnRender": function (oData, sVal) {
                            if (sVal != "--") {
                                return "<input type='checkbox' name='chk' id='chk' value='" + sVal + "' class=\"group1\"  />";
                            }
                            else {
                                return "--";
                            }

                        }

                    },
                    {
                        "bVisible": true, "bSortable": false, "sWidth": "60px", "aTargets": [1]
                    },
                    {
                        "sClass": "left", "aTargets": [2], "bSortable": true, "sWidth": "450px", "fnRender": function (oData, sVal) {
                            if (oData.aData[0] != "--") {
                                var myvar = "";
                                myvar = oData.aData[2];
                                if (oData.aData[13] == "0") {
                                    myvar = "<span style=\"color:Red;\">" + oData.aData[2] + "</span>"
                                }
                                return " <span style='cursor:pointer;text-decoration:none;'  onclick='javascript: openUpdatePopup(\"" + oData.aData[12] + "\",\"" + escape(oData.aData[2]) + "\",\"" + oData.aData[3] + "\",\"" + oData.aData[4] + "\",\"" + oData.aData[5] + "\",\"" + oData.aData[6] + "\",\"" + oData.aData[7] + "\",\"" + oData.aData[8] + "\",\"" + oData.aData[9] + "\",\"" + oData.aData[10] + "\",\"" + oData.aData[11] + "\",\"" + oData.aData[19] + "\",\"" + oData.aData[20] + "\",\"" + oData.aData[21] + "\")'>" + myvar + "</span>";
                            }
                            else {
                                return "--"
                            }
                        }
                    },
                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [3],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[21];
                        }
                    },
                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [4],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[18];
                        }
                    },
                    {
                        "bVisible": true, "sWidth": "80px", "bSortable": true, "aTargets": [5],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[5];
                        }
                    },
                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [6],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[14];
                        }
                    },

                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [7],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[10];
                        }
                    },
                    {
                        "bVisible": false, "sWidth": "150px", "bSortable": true, "aTargets": [8],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[15];
                        }
                    },
                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [9],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[16];
                        }
                    },
                    {
                        "bVisible": false, "sWidth": "150px", "bSortable": true, "aTargets": [10],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[17];
                        }
                    },
                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [11],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[19];
                        }
                    },
                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [12],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[20];
                        }
                    }
                ]
            });
            $("div.MyButton").html('<div><table><tr><td><%=Literal2.Text%> : </td><td><%=opt %></td></tr> </table></div>');
            jQuery(".chosen").data("placeholder", "<%=Literal39.Text%>...").chosen();
         //    settings = oTable.dataTable().fnSettings();
           // oTable = jQuery("#examples").dataTable(settings);
//            oTable.fnColReorder(5, 3);//move the 4th column on the 10th position
//           oTable.fnAdjustColumnSizing();//a good idea to make sure there will be no displaying issues

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

        function openAddPopup() {
            Clearfields();
            $("#opr").val(0);
            $("#dialog-message").dialog({
                title: "Add Driver",
                buttons: {
                    "<%=Literal14.Text%>": function () {
                        UpdateData("0");
                    },
                    "<%=Literal12.Text%>": function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }
        function openUpdatePopup(driverid, drivername,Licence,Expipydate,Phone,Address,FuelCard,rfid ,dob,issuedate,userid,ic,pwd,driverrole) {
            $("#opr").val(1);
            $("#ddluserid").val(userid);
            $("#txtpoiname").val(unescape(drivername));
                      
            $("#txtDOB").val(dob);
             $("#txtPhone").val(Phone);
            $("#txtAddress").val(Address);
            $("#txtLicenceno").val(Licence);
             $("#txtIssuingdate").val(issuedate);
            $("#txtExpiryDate").val(Expipydate);
            $("#txtFuelCardNo").val(FuelCard);
            $("#txtrfid").val(rfid);

            $("#txtdriveric").val(ic);
            $("#txtpwd").val(pwd);
            if (driverrole == "OWNER") {
                $("#drroleddl").val(1);
            }
            else {
                $("#drroleddl").val(0);
            }



            $("#dialog-message").dialog({
                title: "Update Driver",
                buttons: {
                    "<%=Literal11.Text%>": function () {
                        UpdateData(driverid);
                    },
                    "<%=Literal12.Text%>": function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }

        function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();
        }

        function UpdateData(poiid) {
            var res = validate();
            var opr = $("#opr").val();
            if (res == true) {


                $.get("GetDriverManagement.aspx?op=2&userId=" + $("#ddluserid").val() + "&poiname=" + $("#txtpoiname").val() + "&txtDOB=" + $("#txtDOB").val() + "&txtrfid=" + $("#txtrfid").val() + "&txtPhone=" + $("#txtPhone").val() + "&txtAddress=" + $("#txtAddress").val() + "&txtLicenceno=" + $("#txtLicenceno").val() + "&txtIssuingdate=" + $("#txtIssuingdate").val() + "&txtExpiryDate=" + $("#txtExpiryDate").val() + "&txtFuelCardNo=" + $("#txtFuelCardNo").val() + "&poiid=" + poiid + "&opr=" + $("#opr").val() + "&ic=" + $("#txtdriveric").val() + "&pwd=" + $("#txtpwd").val() + "&driverrole=" + $("#drroleddl").val() +"", function (response) {
                    if ($("#opr").val() == 0 && response == 1) {
                        alertbox("<%=Literal32.Text%>");
                       $("#dialog-message").dialog("close");
                   }
                    else if ($("#opr").val() == 1 && response == 1) {
                       alertbox("<%=Literal31.Text%>");
                       $("#dialog-message").dialog("close");
                   }
                   else if (response == 99)
                       alertbox("The Phone Number you enter is Already Registered.");
                   else {
                       $("#dialog-message").dialog("close");
                       alertbox("<%=Literal30.Text%>");
                    }
                    refreshTable();


                });


             <%--   $.ajax({
                    type: "POST",
                    url: "DriverManagement.aspx/InsertupdateDriver",
                    data: '{userId: \"' + $('#ddluserid').val() + '\",poiname: \"' + $('#txtpoiname').val() + '\",txtDOB: \"' + $('#txtDOB').val() + '\",txtrfid: \"' + $('#txtrfid').val() + '\",txtPhone: \"' + $('#txtPhone').val() + '\",txtAddress: \"' + $('#txtAddress').val() + '\",txtLicenceno: \"' + $('#txtLicenceno').val() + '\",txtIssuingdate: \"' + $('#txtIssuingdate').val() + '\",txtExpiryDate: \"' + $('#txtExpiryDate').val() + '\", txtFuelCardNo: \"' + $('#txtFuelCardNo').val() + '\",   poiid: \"' + poiid + '\",opr: \"' + $('#opr').val() + '\",ic: \"' + $('#txtdriveric').val() + '\",pwd: \"' + $('#txtpwd').val() + '\" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (opr == 0 && response.d == 1){
                            alertbox("<%=Literal32.Text%>");
                            $("#dialog-message").dialog("close");
                            }
                        else if (opr == 1 && response.d == 1){
                            alertbox("<%=Literal31.Text%>");
                            $("#dialog-message").dialog("close");
                            }
                        else if(response.d==99)
                            alertbox("The Phone Number you enter is Already Registered.");
                        else{
                        $("#dialog-message").dialog("close");
                            alertbox("<%=Literal30.Text%>");
                            }
                        refreshTable();

                    },
                    failure: function (response) {
                        alertbox(response.d);
                    }
                });--%>
                


            }


        }
        function Clearfields() {
            if ($("#rle").val() != "User")
                $('#ddluserid').val("--Select User Name--");
            $("#txtpoiname").val("");
            $("#txtDOB").val("");
             $("#txtPhone").val("");
            $("#txtAddress").val("");
            $("#txtLicenceno").val("");
             $("#txtIssuingdate").val("");
            $("#txtExpiryDate").val("");
            $("#txtFuelCardNo").val("");
            $("#txtrfid").val("");

        }


        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
        }




    </script>
    <link href="css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="js/chosen.jquery.js"></script>
</head>
<body style="font-size: 11px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">
    <asp:Literal ID="Literal2" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, u1%>" />
    <asp:Literal ID="Literal3" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sNo%>" />
    <asp:Literal ID="Literal4" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tit3%>" />
    <asp:Literal ID="Literal5" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tit1%>" />
    <asp:Literal ID="Literal6" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sLat%>" />
    <asp:Literal ID="Literal7" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sLon%>" />
    <asp:Literal ID="Literal8" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g4%>" />
    <asp:Literal ID="Literal9" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sFil%>" />
    <asp:Literal ID="Literal22" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sall%>" />
    <asp:Literal ID="Literal21" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g6%>" />
    <asp:Literal ID="Literal23" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g10%>" />
    <asp:Literal ID="Literal24" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g8%>" />
    <asp:Literal ID="Literal25" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g7%>" />
    <asp:Literal ID="Literal26" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g9%>" />
    <asp:Literal ID="Literal10" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, poi%>" />
    <asp:Literal ID="Literal11" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn6%>" />
    <asp:Literal ID="Literal12" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn1%>" />
    <asp:Literal ID="Literal13" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, aPoi%>" />
    <asp:Literal ID="Literal14" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, atp4%>" />
    <asp:Literal ID="Literal16" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi1%>" />
    <asp:Literal ID="Literal17" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn5%>" />
    <asp:Literal ID="Literal18" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi2%>" />
    <asp:Literal ID="Literal19" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, poiu%>" />
    <asp:Literal ID="Literal20" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, ent%>" />
    <asp:Literal ID="Literal27" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, msm2%>" />
    <asp:Literal ID="Literal28" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, msm3%>" />
    <asp:Literal ID="Literal29" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst27%>" />
    <asp:Literal ID="Literal30" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi4%>" />
    <asp:Literal ID="Literal31" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi5%>" />
    <asp:Literal ID="Literal32" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi6%>" />
    <asp:Literal ID="Literal33" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi7%>" />
    <asp:Literal ID="Literal34" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi8%>" />
    <asp:Literal ID="Literal35" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi9%>" />
    <asp:Literal ID="Literal36" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi10%>" />
    <asp:Literal ID="Literal37" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi11%>" />
    <asp:Literal ID="Literal38" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vm28%>" />
    <asp:Literal ID="Literal39" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst99%>" />
    <asp:Literal ID="Literal40" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst100%>" />
    <asp:Literal ID="Literal43" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt19%>" />
    <asp:Literal ID="Literal44" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt20%>" />
    <asp:Literal ID="Literal45" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt21%>" />
    <asp:Literal ID="Literal46" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt22%>" />
    <asp:Literal ID="Literal47" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt23%>" />
    <asp:Literal ID="Literal15" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tm14%>" />
    <center>
        <br />
        <br />
        <div class="c1">
            &nbsp;Driver Management</div>
        <br />
        <div >
        </div>
        <table style="font-family: Verdana; font-size: 11px; width: 1200px">
            <tr>
                <%--<td colspan="3" align="left" style="color: navy; font-family: Verdana; font-size: 14px;
            font-weight: normal;">
                    <span>Step1: Driver Management Registration</span><br />
                    <span>Step2: <a href="DriverAssignmentManagement.aspx">Driver Assginment for Vehicle</a></span><br />
                    <br />
                </td>--%>
            </tr>
            <tr>
                <td align="left">
                    <a class="button" title="Activate Driver" style="width: 59px;"><span class="ui-button-text"
                        onclick="ConfirmActivate()">Active </span></a><a class="button" title="In-Activate Driver"
                            style="width: 59px;"><span class="ui-button-text" onclick="ConfirmInActivate()">InActive
                            </span></a>
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right: 0px;">
                    <a class="button" title="Add Driver" style="width: 79px;" onclick="openAddPopup()"><span
                        class="ui-button-text">
                        <%=Literal14.Text%>
                    </span></a>
                    <a class="button" title="Download Excel" style="width: 59px;"><span class="ui-button-text"
                        onclick="ExcelReport()">Excel </span></a>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="width: 1200px;">
                    <div id="fw_container">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td colspan="3" align="center">
                                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-family: Verdana;
                                        font-size: 11px; width: 1200px">
                                        <thead align="left">
                                            <tr>
                                                <th align="center" style="width: 20px;">
                                                    <input type="checkbox" id="chkh" onclick=" checkall(this)" />
                                                </th>
                                                <th style="width: 50px;">
                                                    No
                                                </th>
                                                <th>
                                                    Driver Name
                                                </th>
                                                <th>
                                                    Driver Role
                                                </th>
                                                <th>
                                                    RFID
                                                </th>
                                                <th>
                                                    Mobile No
                                                </th>
                                                <th style="width: 120px;">
                                                    Licence No
                                                </th>
                                                <th>
                                                    Issuing Date
                                                </th>
                                                <th>
                                                    Exp Date
                                                </th>
                                                <th>
                                                    Address
                                                </th>
                                                <th>
                                                    Fuel Card No
                                                </th>
                                                  <th>
                                                    Driver IC
                                                </th>
                                                <th>
                                                    Password
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
                                                <th style="width: 50px;">
                                                    No
                                                </th>
                                                <th>
                                                    Driver Name
                                                </th>
                                                <th>
                                                    Driver Role
                                                </th>
                                                <th>
                                                    RFID
                                                </th>
                                                <th>
                                                    Mobile No
                                                </th>
                                                <th style="width: 120px;">
                                                    Licence No
                                                </th>
                                                <th>
                                                    Issuing Date
                                                </th>
                                                <th>
                                                    Exp Date
                                                </th>
                                                <th>
                                                    Address
                                                </th>
                                                <th>
                                                    Fuel Card No
                                                </th>
                                                  <th>
                                                    Driver IC
                                                </th>
                                                <th>
                                                    Password
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
                    <a class="button" title="Activate Driver" style="width: 59px;"><span class="ui-button-text"
                        onclick="ConfirmActivate()">Active </span></a><a class="button" title="In-Activate Driver"
                            style="width: 59px;"><span class="ui-button-text" onclick="ConfirmInActivate()">InActive
                            </span></a>
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right: 0px;">
                    <a class="button" title="Add Driver" style="width: 79px;" onclick="openAddPopup()"><span
                        class="ui-button-text">
                        <%=Literal14.Text%>
                    </span></a>
                </td>
            </tr>
        </table>
        <input type="hidden" id="opr" runat="server" />
        <input type="hidden" name="uid" value="" runat="server" id="uid" />
        <input type="hidden" name="rle" value="" runat="server" id="rle" />
        <input type="hidden" name="ulist" value="" runat="server" id="ulist" />
        <input type="hidden" id="gid" value="" runat="server" />
        <input type="hidden" value="" id="ss" runat="server" />
    </center>
    <div id="div1" title="<%=Literal35.Text%>">
        <p id="P1">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="div2" title="<%=Literal35.Text%>">
        <p id="P2">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-msg" title="<%=Literal29.Text%>">
        <p id="displayp">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-message" title="Add Driver" align="center" style="padding-top: 1px;
        padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 5px;
        font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <table border="0" cellpadding="1" cellspacing="1" style="width: 320px; font-size: 11px;
            font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;
            vertical-align: middle;">
            <br />
            <tr id="txttr" align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                        <%=Literal5.Text%>
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:DropDownList ID="ddluserid" runat="server" Width="182px" Height="20px" TabIndex="10">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Driver Name </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txtpoiname" runat="Server" CssClass="textbox1" TabIndex="11" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                        Driver Role
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <select id="drroleddl" style="width:182px;height:20px;">
                        <option value="0">DRIVER</option>
                        <option value="1">OWNER</option>
                    </select>
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">RFID </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txtrfid" runat="Server" CssClass="textbox1" TabIndex="11" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">DOB </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtDOB" CssClass="textbox1" TabIndex="12" />
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
                    <asp:TextBox runat="server" ID="txtPhone" CssClass="textbox1" TabIndex="12" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Address </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtAddress" CssClass="textbox1" TabIndex="12" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Licence No </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtLicenceno" CssClass="textbox1" TabIndex="12" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Issuing Date </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtIssuingdate" CssClass="textbox1" TabIndex="12" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Expiry Date </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtExpiryDate" CssClass="textbox1" TabIndex="12" />
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Fuel Card No </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtFuelCardNo" CssClass="textbox1" TabIndex="12" />
                </td>
            </tr>

            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Driver IC </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtdriveric" CssClass="textbox1" TabIndex="13" MaxLength ="20" />
                </td>
            </tr>

            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Password</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtpwd" CssClass="textbox1" TabIndex="14" MaxLength ="20" />
                </td>
            </tr>
        </table>
    </div>
    </form>
     <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Drivers List" />
    </form>
</body>
</html>
