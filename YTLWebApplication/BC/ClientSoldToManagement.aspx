<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.ClientSoldToManagement" Codebehind="ClientSoldToManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Sold To Management</title>
    <style type="text/css" media="screen">
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
    <script type="text/javascript" language="javascript" src="js/FixedColumns.js"></script>
    <style type="text/css">
        .dataTables_info {
            width: 32%;
            float: left;
        }


        .ui-widget-header {
            border: 0px solid #4297d7;
        }


        table.display thead th div.DataTables_sort_wrapper {
            position: relative;
            padding-right: 6px;
        }

        table.display tfoot th {
            padding: 0px 0px 0px 0px;
            padding-left: 3px;
        }

        table.display thead th {
            padding: 0px 0px 0px 0px;
            padding-left: 3px;
            height: 31px;
        }

        table.display td {
            padding: 2px 2px;
        }

        .dataTables_filter {
            width: 300px;
        }

        .MyButton {
            text-align: left;
            float: left;
            width: 350px;
        }




        .textbox1 {
            height: 18px;
            width: 180px;
            border-right: #cbd6e4 1px solid;
            border-top: #cbd6e4 1px solid;
            border-left: #cbd6e4 1px solid;
            color: #0b3d62;
            border-bottom: #cbd6e4 1px solid;
        }

        .button {
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

        button, .button {
            width: inherit;
            height: inherit;
        }

        .chzn-results {
            color: black;
        }
    </style>
    <script type="text/javascript" language="javascript">


        function validate() {
             var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            var result = $("#opr").val();
            if (document.getElementById("txtclientname").value == "") {
                alertbox("Please Enter Client Name");
                return false;
            }

            else if (document.getElementById("txtpwd").value == "") {
                alertbox("Please Enter Client Password");
                return false;
            }
                if (document.getElementById("txtmobile").value == "") {
                alertbox("Please Enter Enter Mobile(s)");
                return false;
            }

            else if (document.getElementById("txtemail").value == "") {
                alertbox("Please Enter Email(s)");
                return false;
                }
                    if (document.getElementById("ddlsoldto").value == "0") {
                alertbox("Please Select Customer Name");
                return false;
                    }

            else if ($('#txtemail').val().trim() != "") {

			    var emls = document.getElementById("txtemail").value;
			    if (emls.indexOf(";") > 0) {
			        arr = new Array();
			        var mar = new Array();
			        mar = emls.split(";");
			        for (var i = 0; i < mar.length; i++) {
			            if (!emailRegEx.test($.trim(mar[i]))) {
			                alertbox("Please  enter valid mail id at position " + (i + 1) + "");
			                return false;
			            }
			            else {
			                arr.push($.trim(mar[i]));
			            }

			        }
			    }
			    else {
			        if (!emailRegEx.test(emls)) {
			            alertbox("Please  enter valid mail id ");
			            return false;
			        }
			        else {
			            arr = new Array();
			            arr.push(emls);
			        }

			    }
			}


            
                return true;
           
        }

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


        function refreshTable() {
            $.get("GetClientSoldToManagement.aspx?op=0", function (response) {
                var json = JSON.parse(response);
                table = oTable.dataTable();
                table._fnProcessingDisplay(true);
                oSettings = table.fnSettings();
                table.fnClearTable(this);
                for (var i = 0; i < json.length; i++) {
                    	json[i][0] = "<input type='checkbox' name='chk' id='chk' value='" + response[i][8] + "' class=\"group1\"  />";
                     


                    json[i][2] = "<span style='cursor:pointer;text-decoration:underline;' onclick=javascript:openUpdatePopup('" + escape(json[i][2]) + "','" + escape(json[i][3]) + "','" + escape(json[i][4]) + "','" + escape( json[i][5]) + "','" + json[i][8] + "','" + json[i][9] + "')>" + json[i][2] + "</span>";
                    table.oApi._fnAddData(oSettings, json[i]);
                }
                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                table._fnProcessingDisplay(false);
                table.fnDraw();
                return false;
            });
        }

        var oTable;

        $(document).ready(function () {
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
                width: 350,
                minHeight: 250,
                height: 250

            });

            //  fnFeaturesInit();
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 500,
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


                "aoColumnDefs": [
                    { "bVisible": false, "bSortable": false, "sWidth": "40px", "sClass": "right", "aTargets": [0] },
                    { "bVisible": true, "bSortable": true,"sWidth": "40px", "aTargets": [1] },
                    { "bVisible": true, "sWidth": "100px", "bSortable": true, "aTargets": [2] },
                    { "bVisible": true, "bSortable": true, "aTargets": [3] },
                    { "bVisible": true, "bSortable": true, "aTargets": [4] },
                { "bVisible": true, "bSortable": true,"sWidth": "200px", "aTargets": [5] }]
            });

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
                title: "Add Cleint",
                buttons: {
                    "Add": function () {
                        UpdateData("0");
                    },
                    "Close": function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }
        function openUpdatePopup(cname, cpwd, mails,mobiles,cid,sid) {
            $("#opr").val(1);
            $("#txtclientname").val(unescape( cname));
            $("#txtpwd").val(unescape(cpwd));
            $("#txtmobile").val(unescape(mobiles));
            $("#txtemail").val(unescape(mails));
            $("#ddlsoldto").val(sid);

            $("#dialog-message").dialog({
                title: "Update Cleint",
                buttons: {
                    "Update Cleint Info": function () {
                        UpdateData(cid);
                    },
                    "Close": function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }



        function UpdateData(id) {
            var res = validate();
            var opr = $("#opr").val();
            if (res == true) {

                $.get("GetClientSoldToManagement.aspx?op=1&prevpid=" + id + "&cid=" + $('#txtclientname').val() + "&cpwd=" + $('#txtpwd').val() + "&email=" + $('#txtemail').val() + "&mobile=" + $('#txtmobile').val() + "&soldto=" + $('#ddlsoldto').val() + "", function (response) {
                    if (opr == 0 && response == "1") {
                        alertbox("Client Added Successfully");
                        $("#dialog-message").dialog("close");
                    }
                        
                    else if (opr == 1 && response == "1") {
                        alertbox("Client Updated Successfully");
                        $("#dialog-message").dialog("close");
                    }
                        
                    else if (response = "2") {
                        alertbox("The Username Already Registered.Please try with different one.");
                    }
                        
                    else {
                        alertbox("Something Went Wrong Cannot Process Your Request Now");
                        $("#dialog-message").dialog("close");
                    }
                    
                      
                    refreshTable();
                });


                


            }


        }
        function Clearfields() {
             $("#txtclientname").val("");
            $("#txtpwd").val("");
            $("#txtmobile").val("");
            $("#txtemail").val("");
            $("#ddlsoldto").val("0");
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

        <center>
            <br />
            <br />
            <div class="c1">
                &nbsp;Client Login Management For Sold To
            </div>
            <br />
            <table style="font-family: Verdana; font-size: 11px; width: 1000px">
                <tr>
                    <td align="left"></td>
                    <td align="center"></td>
                    <td align="right" valign="middle" style="margin-right: 0px;">
                        <a class="button" title="Add New Client" style="width: 79px;" onclick="openAddPopup()">
                            <span class="ui-button-text">Add </span></a>
                        <a href="javascript:ExcelReport();" class="button" style="margin-right: 3px; width: 75px;">Save Excel</a>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" style="width: 1000px;">
                        <div id="fw_container">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td colspan="3" align="center">
                                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-family: Verdana; font-size: 11px; width: 1000px">
                                            <thead align="left">
                                                <tr>
                                                    <th align="center" style="width: 20px;" class="btn-primary ">
                                                        <input type="checkbox" id="chkh" onclick=" checkall(this)" style="margin-top: -16px" />
                                                    </th>
                                                    <th style="width: 50px;">S No
                                                    </th>
                                                    <th>Client Name
                                                    </th>
                                                    <th>Password
                                                    </th>
                                                    <th>Email(s)
                                                    </th>
                                                     <th>Mobile(s)
                                                    </th>
                                                    <th>Sold To Customer</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                            </tbody>

                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="left"></td>
                    <td align="center"></td>
                    <td align="right" valign="middle" style="margin-right: 0px;"></td>
                </tr>
            </table>
            <input type="hidden" id="opr" runat="server" />
            <input type="hidden" name="uid" value="" runat="server" id="uid" />
            <input type="hidden" name="rle" value="" runat="server" id="rle" />
            <input type="hidden" name="ulist" value="" runat="server" id="ulist" />
            <input type="hidden" id="gid" value="" runat="server" />
            <input type="hidden" value="" id="ss" runat="server" />
        </center>

        <div id="dialog-msg" title="Alert!">
            <p id="displayp">
                <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;"></span>
            </p>
        </div>
        <div id="dialog-message" title="Add Unit" align="center" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 5px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <table border="0" cellpadding="1" cellspacing="1" style="width: 320px; font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif; vertical-align: middle;">
                <br />

                <tr align="left">
                    <td align="left">
                        <b style="color: #4E6CA3;">Client Name
                        </b>
                    </td>
                    <td style="color: #4E6CA3;">:
                    </td>
                    <td>
                        <asp:TextBox ID="txtclientname" runat="Server" CssClass="textbox1" TabIndex="11" />
                    </td>
                </tr>
                <tr align="left">
                    <td align="left">
                        <b style="color: #4E6CA3;">Password
                        </b>
                    </td>
                    <td style="color: #4E6CA3;">:
                    </td>
                    <td>
                        <asp:TextBox ID="txtpwd" runat="Server" CssClass="textbox1" TabIndex="11" />
                    </td>
                </tr>
                 <tr align="left">
                    <td align="left">
                        <b style="color: #4E6CA3;">Mobile(s)
                        </b>
                    </td>
                    <td style="color: #4E6CA3;">:
                    </td>
                    <td>
                        <asp:TextBox ID="txtmobile" runat="Server" CssClass="textbox1" TabIndex="11" />
                    </td>
                </tr>
                 <tr align="left">
                    <td align="left">
                        <b style="color: #4E6CA3;">Email(s)
                        </b>
                    </td>
                    <td style="color: #4E6CA3;">:
                    </td>
                    <td>
                        <asp:TextBox ID="txtemail" runat="Server" CssClass="textbox1" TabIndex="11" />
                    </td>
                </tr>
                 <tr align="left">
                    <td align="left">
                        <b style="color: #4E6CA3;">Customer Name
                        </b>
                    </td>
                    <td style="color: #4E6CA3;">:
                    </td>
                    <td>
                        <select id="ddlsoldto" class ="textbox1" runat ="server" ></select>
                    </td>
                </tr>

            </table>
        </div>
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Sold To Sustomer Management" />
        <input type="hidden" id="titl1" name="titl1" value="Sold To Sustomer Management" />
        <input type="hidden" id="rd" name="rd" value="Report Date" />


    </form>
</body>
</html>
