<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DriverRoleAssignManagement.aspx.vb" Inherits="YTLWebApplication.DriverRoleAssignManagement" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Driver Role Assign Management</title>
    <link rel="shortcut icon" type="image/ico" href="images/car.ico">
    <style type="text/css" media="screen">
        @import "css/demo_table_jui.css";
        @import "css/jquery-ui.css";
        @import "css/TableTools.css";
        @import "css/ColVis.css";
        @import "css/common1.css";
        @import "css/chosen.css";
        @import "css/select2.min.css";
    </style>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/jquery_ui.js"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="js/FixedColumns.js"></script>
    <script type="text/javascript" language="javascript" src="js/ColReorder.js"></script>
    <script type="text/javascript" language="javascript" src="assets/js/plugins/select2/select2.min.js"></script>
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
            if ($("#ddlfdriver").val() == $("#ddlsdriver").val()) {
                if ($("#ddlfdriver").val() == "-1" && $("#ddlsdriver").val() == "-1") {
                    return "0";
                }
                else {
                    return "First Driver and Second Driver cannot be same.";
                }
            }
            else if ($("#ddlfdriver").val() == "-1" && $("#ddlsdriver").val() != "-1") {
                return "Please assign First Driver before you assign Second Driver.";
            }
            else {
                return "0";
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


        function refreshTable() {
            $.ajax({
                type: "POST",
                url: "DriverRoleAssignManagement.aspx/FillGrid",
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
            });
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
            $("#div3").dialog({
                resizable: false,
                draggable: false,
                height: 160,
                modal: true,
                autoOpen: false,
                buttons: {
                    Yes: function () {
                        confirmresult = true;
                        Delete();
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
                minHeight: 400,
                height: 400

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
                            //for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                            //    $('td:eq(1)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            //}
                        }
                    }
                },
                "oLanguage": {
                    "oPaginate": {
                        "sNext": "<%=Literal25.Text %>",
                        "sFirst": "<%=Literal23.Text %>",
                        "sLast": "<%=Literal26.Text %>",
                        "sPrevious": "<%=Literal24.Text %>"
                    },
                    "sSearch": "<%=Literal9.Text %>",
                    "sEmptyTable": "<%=Literal21.Text %>",
                    "sInfo": "<%=Literal22.Text %>",
                    "sInfoFiltered": "<%=Literal43.Text%>",
                    "sZeroRecords":"<%=Literal44.Text%>",
                    "sInfoEmpty": "<%=Literal45.Text%>",
                    "sLoadingRecords":"<%=Literal46.Text%>",
                    "sProcessing": "<%=Literal47.Text%>",
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',		
                "aoColumnDefs": [

                          
                             { "bVisible": true, "sWidth": "80px", "bSortable": true, "aTargets": [0] ,
                              "fnRender": function (oData, sVal) {
                              return parseInt(oData.aData[0]);
                              }
                              },
                              { "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [1] ,
                              "fnRender": function (oData, sVal) {
                              return oData.aData[1];
                              }
                    },

                    {
                        "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [2],
                        "fnRender": function (oData, sVal) {
                            return oData.aData[2];
                        }
                    }, 

                               { "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [3] ,
                              "fnRender": function (oData, sVal) {
                              return oData.aData[3];
                              }
                              },  

                               { "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [4] ,
                              "fnRender": function (oData, sVal) {
                              return oData.aData[4];
                              }
                              },  
                               { "bVisible": true, "sWidth": "150px", "bSortable": true, "aTargets": [5] ,
                              "fnRender": function (oData, sVal) {
                              return oData.aData[5];
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
                        $(this).dialog("close");
                    },
                    "<%=Literal12.Text%>": function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }
        function openUpdatePopup(plateno,driverid,firstdriver,secdriver) {
            $("#txtplateno").text(plateno);
            GetDriverList(plateno, driverid, firstdriver, secdriver);
            GetOwnerList(plateno, driverid);

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



        function UpdateData(poiid) {
            var res = validate();
            if (res == "0") {
                $.ajax({
                    type: "POST",
                    url: "DriverRoleAssignManagement.aspx/InsertupdateDriver",
                    data: '{userId: \"' + $('#ddluser1').val() + '\",plateno: \"' + $('#txtplateno').text() + '\",owner: \"' + $('#ddldriver').val() + '\",firstdriver: \"' + $('#ddlfdriver').val() + '\",secdriver: \"' + $('#ddlsdriver').val() + '\"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response.d == 2) {
                            alertbox("Update Successfully.");
                        }
                        else {
                            alertbox(response.d);
                        }
                        refreshTable();
                    },
                    failure: function (response) {
                        alertbox(response.d);
                    }
                });
            }
            else {
                alertbox(res);
            }
        }

        function GetDriverList(plateno, driverid, firstdriver, secdriver) {
                $.ajax({
                    type: "POST",
                    url: "DriverRoleAssignManagement.aspx/GetDriverList",
                    data: '{userid: \"' + $('#ddluser1').val() + '\",plateno: \"' + plateno + '\"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#ddlfdriver').select2();
                        $('#ddlsdriver').select2();
                        $('#ddlfdriver').empty();
                        $('#ddlsdriver').empty();
                        var json = JSON.parse(response.d);
                        $('#ddlfdriver').append("<option value=-1>No Driver</option>")
                        if (json.driver1.length > 0) {
                            for (var i = 0; i < json.driver1.length; i++) {
                                $('#ddlfdriver').append("<option value=" + json.driver1[i].driverid + ">" + json.driver1[i].drivername + "</option>");
                            }
                            $("#ddlfdriver").val(firstdriver).trigger('change');;
                        }
                        $('#ddlsdriver').append("<option value=-1>No Driver</option>")
                        if (json.driver1.length > 0) {
                            for (var i = 0; i < json.driver1.length; i++) {
                                $('#ddlsdriver').append("<option value=" + json.driver1[i].driverid + ">" + json.driver1[i].drivername + "</option>");
                            }
                            $("#ddlsdriver").val(secdriver).trigger('change');;
                        }
                    },
                    failure: function (response) {
                        alertbox(response.d);
                    }
                });           
        }


        function GetOwnerList(plateno, driverid) {
                $.ajax({
                    type: "POST",
                    url: "DriverRoleAssignManagement.aspx/GetOwnerList",
                    data: '{userid: \"' + $('#ddluser1').val() + '\",plateno: \"' + plateno + '\"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#ddldriver').select2();
                        $('#ddldriver').empty();
                        $('#ddldriver').append("<option value=-1>No Owner</option>")
                        var json = JSON.parse(response.d);
                        if (json.length > 0) {
                            for (var i = 0; i < json.length; i++) {
                                $('#ddldriver').append("<option value=" + json[i].driverid + ">" + json[i].drivername + "</option>");
                            }
                            $("#ddldriver").val(driverid).trigger('change');
                        }
                        else{
                            $("#ddldriver").val(0).trigger('change');
                        }
                    },
                    failure: function (response) {
                        alertbox(response.d);
                    }
                });            
        }

        function Clearfields() {
            if ($("#rle").val() != "User")
                $('#ddluser1').val("--Select User Name--");
            $("#txtpoiname").val("");
            $("#txtDOB").val("");
            $("#txtPhone").val("");
            $("#txtPwd").val("");
            $("#txtAddress").val("");
            $("#txtLicenceno").val("");
             $("#txtIssuingdate").val("");
            $("#txtExpiryDate").val("");
            $("#txtFuelCardNo").val("");
            $("#txtNoStaffId").val("");
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

        function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();
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
            &nbsp;Driver Role Assign Management</div>
        <br />
        <div >
        </div>
        <table style="font-family: Verdana; font-size: 11px; width: 1000px">
            <tr>
                <td align="left">
<%--                    <asp:DropDownList ID="ddluserid" runat="server" Width="182px" Height="20px" TabIndex="10" EnableViewState="false">
                    </asp:DropDownList>--%>
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right: 0px;">
                    <a class="button" title="Download Excel" style="width: 59px;"><span class="ui-button-text"
                        onclick="ExcelReport()">Excel </span></a>
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
                                                <th style="width: 50px;">
                                                    SNo
                                                </th>
                                                <th>
                                                    Plate No
                                                </th>
                                                <th>
                                                    Owner Name
                                                </th>
                                               <th>
                                                   First Driver
                                               </th>
                                                <th>
                                                    Second Driver
                                                </th>
                                                <th>
                                                    Action
                                                </th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                        <tfoot align="left">
                                            <tr>
                                                 <th style="width: 50px;">
                                                     SNo
                                                 </th>
                                                 <th>
                                                     Plate No
                                                 </th>
                                                 <th>
                                                     Owner Name
                                                 </th>
                                                <th>
                                                    First Driver
                                                </th>
                                                 <th>
                                                     Second Driver
                                                 </th>
                                                 <th>
                                                     Action
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
         <div id="div3" title="<%=Literal35.Text%>">
        <p id="P3">
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
                        Plate No
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <p id="txtplateno" class="form-control">
                    </p>
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Owner Name </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <select id="ddldriver" class="form-control" style="width:200px;">
                    </select>
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">First Driver </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <select id="ddlfdriver" class="form-control" style="width:200px;">
                    </select>
                </td>
            </tr>
            <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">Second Driver </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <select id="ddlsdriver" class="form-control" style="width:200px;">
                    </select>
                </td>
            </tr>
        </table>
    </div>
    </form>
     <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Drivers Role Assign List" />
    </form>
</body>
</html>