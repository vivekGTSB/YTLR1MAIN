<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.RefuelSummaryReportT" EnableViewState="false" EnableEventValidation="false" Codebehind="RefuelSummaryReportT.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Refuel Summary Report</title>
    <style type="text/css" media="screen">
        @import "css/demo_page.css";
        @import "css/demo_table_jui.css";
        @import "css/common1.css";
        @import "css/jquery-ui.css";
        .dataTables_info
        {
            width: 25%;
            float: left;
        }
        .style1
        {
            height: 12px;
        }
        .pt1
        {
            background-image: url(images/pst.png);
            background-repeat: no-repeat;
            width: 16px;
            height: 16px;
            display:inline-table;
            vertical-align:middle;
        }
    </style>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script src="js/jquery_ui.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.dataTables.js"></script>
     <script type="text/javascript">


         $(function () {
             $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -210, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2
             });
             $("#tbxdatetime").datepicker({ dateFormat: 'yy/mm/dd', minDate: -210, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2
             });

             $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -210, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2

             });

             $("#dialog:ui-dialog").dialog("destroy");

             $("#dialog-message").dialog({
                 resizable: false,
                 draggable: false,
                 modal: true,
                 autoOpen: false,
                 minWidth: 1000,
                 minHeight: 529,
                 buttons: {
                     "Close": function () {
                         $("#dialog-message").dialog("close");
                     }
             }
         });

         $("#div-add").dialog({
             resizable: false,
             draggable: false,
             modal: true,
             autoOpen: false,
             minWidth: 400,
             minHeight: 250,
             buttons: {
                 "save": function () {
                     insertnewrefuel($("#ddluserid").val(), $("#ddlplateno").val(), $("#tbxdatetime").val() + " " + $("#ddlhr").val() + ":" + $("#ddlmin").val(), '0');

                 },

                 "Cancel": function () {
                     $("#div-add").dialog("close");
                 }
             }
         });

         document.getElementById('cbxcost').checked = false;
         document.getElementById('cbxliters').checked = true;
         document.getElementById('tbxcost').readOnly = true;
         document.getElementById('tbxcost').style.backgroundColor = "ActiveBorder";
         document.getElementById('tbxlitters').readOnly = false;
         document.getElementById("tbxlitters").style.backgroundColor = "";
     });

        function DisplayMap(intime, outtime, plateno) {
            document.getElementById("mappage").src = "GMap.aspx?bdt=" + intime + "&edt=" + outtime + "&plateno=" + plateno + "&scode=1&sf=0&r=" + Math.random();
            document.getElementById("mappage").style.visibility = "visible";
           
        }

        function openPopup() {
            $("#div-add").dialog("open");
            var date= $("#txtBeginDate").val();
            $("#tbxdatetime").val(date);
            LoadPlates1();
        }
        function mysubmit() {
            var username = document.getElementById("DropDownList1").value;
            if (username == "--Select User Name--") {
                alert("Please select user name");
                return false;
            }
            else if (document.getElementById("ddlplate").value == "--Select Plate No--") {
                alert("Please select plate no");
                return false;
            }



            var bigindatetime = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value;
            var enddatetime = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value;

            var fdate = Date.parse(bigindatetime);
            var sdate = Date.parse(enddatetime);

            var diff = (sdate - fdate) * (1 / (1000 * 60 * 60 * 24));
            var days = parseInt(diff) + 1;
            if (days > 5) {
                return confirm("You selected " + days + " days of data.So it will take more time to execute.\nAre you sure you want to proceed ? ");
            }
            return true;

        }

        function ExcelReport() {

            var plateno = document.getElementById("ddlplate").value;

            document.getElementById("plateno").value = plateno;

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
        jQuery.extend(jQuery.fn.dataTableExt.oSort, {
            "num-html-pre": function (a) {
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
        $(document).ready(function () {
            fnFeaturesInit()

          


            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 25,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "aaSorting": [[4, "desc"]],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },

                "sProcessing": "System is loading data, please wait....",
                "aoColumnDefs": [
                          { "bVisible": true, "bSortable": false, "sWidth": "45px", "aTargets": [0] },
                           { "bVisible": true,  "sWidth": "80px", "aTargets": [1], "sClass": "left" },
                            { "bVisible": true, "bSortable": true, "sWidth": "120px", "aTargets": [2] },
                              { "bVisible": true, "bSortable": true, "sWidth": "120px", "aTargets": [3] },
                                { "bVisible": true, "bSortable": true, "sWidth": "120px", "aTargets": [4] },
                                { "bVisible": true, "bSortable": true, "aTargets": [5] },
                                  { "bVisible": true, "bSortable": true, "sWidth": "80px", "aTargets": [6], "sClass": "right" },
                                    { "bVisible": true, "bSortable": true, "sWidth": "80px", "aTargets": [7], "sClass": "right" },
                                    { "bVisible": true, "bSortable": true, "sWidth": "80px", "aTargets": [8], "sClass": "right" },
                                      { "sClass": "right", "sType": "num-html", "aTargets": [8], "bSortable": true, "sWidth": "100px", "fnRender": function (oData, sVal) {
                                          if (oData.aData[8] != "--") {
                                              return " <span style='cursor:pointer;color:blue;text-decoration:underline;' id='i" + oData.iDataRow + "'  title='Click to see the details' onclick='javascript: DisplayDetails(\"" + oData.aData[1] + "\",\"" + oData.aData[2] + "\",\"" + oData.aData[3] + "\",\"" + oData.aData[4] + "\",\"" + oData.aData[6] + "\",\"" + oData.aData[7] + "\",\"" + oData.aData[8] + "\",\"" + oData.aData[17] + "\",\"" + oData.aData[16] + "\",\"" + oData.aData[15] + "\",\"" + oData.aData[18] + "\",\"" + oData.aData[16] + "\")'>" + oData.aData[8] + "</span>";
                                          }
                                          else {
                                              return "--"
                                          }
                                      }
                                      },
                                    
                                        { "bVisible": true, "bSortable": true, "sWidth": "100px", "aTargets": [9], "sType": "num-html", "sClass": "right" },
                                           { "bVisible": true, "sWidth": "80px", "aTargets": [10], "sClass": "left" },
                                             { "bVisible": true, "sWidth": "80px", "aTargets": [11], "sClass": "left" },
                                             { "bVisible": true, "sWidth": "60px", "aTargets": [12], "sClass": "right" },
                                              { "bVisible": false, "sWidth": "100px", "aTargets": [13], "sClass": "left" },
                                             { "bVisible": true, "sWidth": "100px", "aTargets": [14], "sClass": "right" }
                                           

                              ]
            });

                            

                          });


                          var oTable;
                          var aPos;
                          $("#examples  tbody td").live("click", function (event) {
                              oTable = $('#examples').dataTable();
                              aPos = oTable.fnGetPosition(this)[0]; // getting the clicked row position
                           
                          });

                        


                          function DisplayDetails( username, Vehicleid, fromdate, todate, bofore, after, totltrs,userid, lat, lon, fuelst, time) {
                              document.getElementById("RefuelDetails").src = "RefuelDetailscust.aspx?un=" + username + "&plno=" + Vehicleid + "&fd=" + fromdate + "&td=" + todate + "&vol1=" + bofore + "&vol2=" + after + "&totltrs=" + totltrs + "&uid=" + userid + "&lat=" + lat + "&lon=" + lon + "&fuelst=" + fuelst + "&due=" + time + "";
                              document.getElementById("RefuelDetails").style.visibility = "visible";
                               $("#dialog-message").dialog("open");
                          }

        function LoadPlates() {
            var uid = document.getElementById("DropDownList1").value;
            var plateno = document.getElementById("ddlplate").value;
            $("#ddlplate").attr("disabled", "disabled");
            $.ajax({
                type: "POST",
                url: "RefuelSummaryReportT.aspx/LoadVehicles",
                data: '{userId: \"' + uid + '\",luid: \"' + $('#luid').val() + '\",role: \"' + $('#rle').val() + '\",userslist: \"' + $('#ulist').val() + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var control = $('#ddlplate');
                    $('#ddlplate option').remove();
                    $('#ddlplate').removeAttr("disabled");
                    $('#ddlplate').empty();
                    var list = response;

                    if (list.length > 0) {
                        control.empty().append('<option selected="selected" value="--All Plate No--">--ALL PLATE NO--</option>');
                        for (var i = 0; i < list.length; i++) {
                            control.append($("<option></option>").val(list[i][0]).html(list[i][0]));
                        }
                    }
                    else {
                        $('#ddlplate').empty().append('<option selected="selected" value="0">NO VEHICLES<option>');
                    }
                },
                failure: function (response) {
                    $('#ddlplate').empty().append('<option selected="selected" value="0">NO VEHICLES<option>');
                }
            });
        }

        function LoadPlates1() {
            var uid = document.getElementById("ddluserid").value;
            var plateno = document.getElementById("ddlplateno").value;
            $("#ddlplateno").attr("disabled", "disabled");

             var str = "GetPlateNo.aspx?userId=" + uid;
           $.getJSON(str, function (json) {
                 var control = $('#ddlplateno');
                    $('#ddlplateno option').remove();
                    $('#ddlplateno').removeAttr("disabled");
                    $('#ddlplateno').empty();
                    var list = json;

                    if (list.length > 0) {
                        
                        for (var i = 0; i < list.length; i++) {
                            control.append($("<option></option>").val(list[i][0]).html(list[i][0]));
                        }
                    }
                    else {
                        $('#ddlplateno').empty().append('<option selected="selected" value="0">NO VEHICLES<option>');
                    }
            });







            ////$.ajax({
            ////    type: "POST",
            ////    url: "RefuelSummaryReportT.aspx/LoadVehicles",
            ////    data: '{userId: \"' + uid + '\",luid: \"' + $('#luid').val() + '\",role: \"' + $('#rle').val() + '\",userslist: \"' + $('#ulist').val() + '\"}',
            ////    contentType: "application/json; charset=utf-8",
            ////    dataType: "json",
            ////    success: function (response) {
            ////        var control = $('#ddlplateno');
            ////        $('#ddlplateno option').remove();
            ////        $('#ddlplateno').removeAttr("disabled");
            ////        $('#ddlplateno').empty();
            ////        var list = response;

            ////        if (list.length > 0) {
                        
            ////            for (var i = 0; i < list.length; i++) {
            ////                control.append($("<option></option>").val(list[i][0]).html(list[i][0]));
            ////            }
            ////        }
            ////        else {
            ////            $('#ddlplateno').empty().append('<option selected="selected" value="0">NO VEHICLES<option>');
            ////        }
            ////    },
            ////    failure: function (response) {
            ////        $('#ddlplateno').empty().append('<option selected="selected" value="0">NO VEHICLES<option>');
            ////    }
            ////});
        }

        function insertrefuel(userid, plateno, timestamp, value, type, cpl) {
            var receiptno = "";
            $.ajax({
                type: "POST",
                url: "RefuelSummaryReportT.aspx/insertactrefuel",
                data: '{userid: \"' + userid + '\",plateno: \"' + plateno + '\",timestamp: \"' + timestamp + '\",ltrs: \"' + value + '\",fc: \"' + cpl + '\",receiptno: \"' + receiptno + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (type == 1) {
                        table.fnUpdate(response.d, aPos, 12);
                    }
                    else {
                        $("#div-add").dialog("close");
                    }
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
           
        }
        function insertnewrefuel(userid, plateno, timestamp, type) { 
            var receiptno = $("#txtReceipt").val();
            var value = $("#tbxlitters").val();
            var cpl = $("#txtltrCost").val();
             var str = "AddNewRefuel.aspx?userid=" + userid + "&plateno=" +  plateno + "&timestamp=" + timestamp + "&ltrs=" + value + " &fc=" + cpl + " &receiptno=" + receiptno  ;
            $.get(str, function (data) {
                 if (type == 1) {
                        table.fnUpdate(response.d, aPos, 12);
                    }
                    else {
                        $("#div-add").dialog("close");
                    }
            });


            ////$.ajax({
            ////    type: "POST",
            ////    url: "RefuelSummaryReportT.aspx/insertactrefuel",
            ////    data: '{userid: \"' + userid + '\",plateno: \"' + plateno + '\",timestamp: \"' + timestamp + '\",ltrs: \"' + value + '\",fc: \"' + cpl + '\",receiptno: \"' + receiptno + '\"}',
            ////    contentType: "application/json; charset=utf-8",
            ////    dataType: "json",
            ////    success: function (response) {
            ////        if (type == 1) {
            ////            table.fnUpdate(response.d, aPos, 12);
            ////        }
            ////        else {
            ////            $("#div-add").dialog("close");
            ////        }
            ////    },
            ////    failure: function (response) {
            ////        alert(response.d);
            ////    }
            ////});

        }
        function insertrefuelcpl(userid, plateno, timestamp, cpl, type,id) {
            var value = $("#txt_" + id).val();
            var receiptno = "";
             var str = "AddNewRefuel.aspx?userid=" + userid + "&plateno=" +  plateno + "&timestamp=" + timestamp + "&ltrs=" + value + " &fc=" + cpl + " &receiptno=" + receiptno  ;
            $.get(str, function (data) {
                 if (type == 1) {
                        table.fnUpdate(response.d, aPos, 12);
                    }
                    else {
                        $("#div-add").dialog("close");
                    }
            });


            ////$.ajax({
            ////    type: "POST",
            ////    url: "RefuelSummaryReportT.aspx/insertactrefuel",
            ////    data: '{userid: \"' + userid + '\",plateno: \"' + plateno + '\",timestamp: \"' + timestamp + '\",ltrs: \"' + value + '\",fc: \"' + cpl + '\",receiptno: \"' + receiptno + '\"}',
            ////    contentType: "application/json; charset=utf-8",
            ////    dataType: "json",
            ////    success: function (response) {
            ////        if (type == 1) {
            ////            table.fnUpdate(response.d, aPos, 12);
            ////        }
            ////        else {
            ////            $("#div-add").dialog("close");
            ////        }
            ////    },
            ////    failure: function (response) {
            ////        alert(response.d);
            ////    }
            ////});

         }



        function refreshTable() {
            $("#examples_processing").css("visibility", "visible");
            $("#rptinfo").text("Report Generated From " + $('#txtBeginDate').val() + " " + $('#ddlbh').val() + ":" + $('#ddlbm').val() + ":00 To " + $('#txtEndDate').val() + " " + $('#ddleh').val() + ":" + $('#ddlem').val() + ":59");

              var str = "GetRefuelSummary.aspx?ddlu=" + $("#DropDownList1").val() + "&ddlp=" +  $('#ddlplate').val() + "&bdt=" + $('#txtBeginDate').val() + " " + $('#ddlbh').val() + ":" + $('#ddlbm').val() + ":00" + "&edt=" + $('#txtEndDate').val() + " " + $('#ddleh').val() + ":" + $('#ddlem').val() + ":59" ;
             $.getJSON(str, function (json) {

           //  $.getJSON('GetSmartfleetRT.aspx?ddlu=' + $("#DropDownList1").val(), null, function (json) {
               // table.fnClearTable(this);

                 for (var i = 0; i < json.length; i++) {

                      table = oTable.dataTable();
                    table._fnProcessingDisplay(true);
                    oSettings = table.fnSettings();
                    table.fnClearTable(this);
                    for (var i = 0; i < json.length - 1; i++) {
                        table.oApi._fnAddData(oSettings, json[i]);
                    }
                    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                    table._fnProcessingDisplay(false);
                    table.fnDraw();
                    $('#totrefuel').text(json[json.length - 1][8]);
                    $('#totcost').text(json[json.length - 1][9]);
                   // table.oApi._fnAddData(oSettings, json.aaData[i]);
                }

               
            });

             
            //$.ajax({
            //    type: "POST",
            //    url: "RefuelSummaryReportT.aspx/DisplayLogInformation",
            //    data: '{ddlu: \"' + $('#DropDownList1').val() + '\",ddlp: \"' + $('#ddlplate').val() + '\",bdt: \"' + $('#txtBeginDate').val() + " " + $('#ddlbh').val() + ":" + $('#ddlbm').val() + ":00" + '\",edt: \"' + $('#txtEndDate').val() + " " + $('#ddleh').val() + ":" + $('#ddlem').val() + ":59" + '\",luid: \"' + $('#luid').val() + '\",role: \"' + $('#rle').val() + '\",userslist: \"' + $('#ulist').val() + '\"}',
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (response) {
            //        var json = response.d;
            //        table = oTable.dataTable();
            //        table._fnProcessingDisplay(true);
            //        oSettings = table.fnSettings();
            //        table.fnClearTable(this);
            //        for (var i = 0; i < response.length - 1; i++) {
            //            table.oApi._fnAddData(oSettings, response[i]);
            //        }
            //        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
            //        table._fnProcessingDisplay(false);
            //        table.fnDraw();
            //        $('#totrefuel').text(response[response.length - 1][8]);
            //        $('#totcost').text(response[response.length - 1][9]);
            //        return false;
            //    },
            //    failure: function (response) {
            //        alert("Failed");
            //    }
            //});
        }

        function enterliters(value) {
            if (value == 0) {
                document.getElementById('cbxcost').checked = false;
                document.getElementById('cbxliters').checked = true;
                document.getElementById('tbxcost').readOnly = true;
                document.getElementById('tbxcost').style.backgroundColor = "ActiveBorder";
                document.getElementById('tbxlitters').readOnly = false;
                document.getElementById("tbxlitters").style.backgroundColor = "";
            }
            else {
                document.getElementById('cbxliters').checked = false;
                document.getElementById('cbxcost').checked = true;
                document.getElementById('tbxlitters').readOnly = true;
                document.getElementById('tbxlitters').style.backgroundColor = "ActiveBorder";
                document.getElementById('tbxcost').readOnly = false;
                document.getElementById("tbxcost").style.backgroundColor = "";
            }

        }

        function changeliters(type) {
            var totltrs = $("#tbxlitters").val();
            var totcost = $("#tbxcost").val();
            var ltrcost = $("#txtltrCost").val(); 
            if (type == "0") {
                $("#tbxcost").val((totltrs * ltrcost).toFixed(2));
            }
            else if (type == "1") {
                $("#tbxlitters").val((totcost / ltrcost).toFixed(2));
            }
            else if (type == "2") {
                $("#tbxcost").val((totltrs * ltrcost).toFixed(2));
            }

            //// var str = "Changeltrs.aspx?userid=" +  $("#ddluserid").val() + "&timestamp=" +  $("#tbxdatetime").val() + "&cost=" + value;
            ////$.get(str, function (response) {
            ////      $("#tbxlitters").val(response.d);
            ////});

            //$.ajax({
            //    type: "POST",
            //    url: "RefuelSummaryReportT.aspx/Changeltrs",
            //    data: '{userid: \"' + $("#ddluserid").val() + '\",timestamp: \"' + $("#tbxdatetime").val() + '\",cost: \"' + value + '\"}',
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (response) {
            //            $("#tbxlitters").val(response.d);
                    
            //    },
            //    failure: function (response) {
            //        alert(response.d);
            //    }
            //});
        }

      

    </script>
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif"
    >
    <form id="Form1" method="post" runat="server">
    <center>
        <div>
            <br />
            <div class="c1">
                Refuel Summary Report</div>
            <br />
        </div>
        <table>
            <tr>
                <td align="center">
                    <table style="font-family: Verdana; font-size: 11px;">
                        <tr>
                            <td colspan="2" class="t1">
                                <div class="h1">
                                    &nbsp;Refuel Summary Report&nbsp;:</div>
                            </td>
                        </tr>
                        <tr>
                            <td class="t2" style="width: 420px;">
                                <table style="width: 420px;">
                                    <tbody>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Begin Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                    id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false"  /><b
                                                        style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlbh" runat="server" Width="40px" EnableViewState="False">
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
                                                <b style="color: #465AE8;">End Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                    id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddleh" runat="server" Width="40px" EnableViewState="False">
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
                                                            <asp:ListItem Value="23" Selected="True">23</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlem" runat="server" Width="40px" EnableViewState="False">
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
                                                            <asp:ListItem Value="59" Selected="True">59</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">User Name</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                 <asp:DropDownList ID="DropDownList1" runat="server" Width="251px" AutoPostBack="true" >
                                                </asp:DropDownList>
                                               <%-- <asp:DropDownList ID="DropDownList1" runat="server" Width="251px" AutoPostBack="false"
                                                    onchange="LoadPlates()">
                                                </asp:DropDownList>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Plate No </b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <asp:DropDownList ID="ddlplate" runat="server" Width="251px" onchange="refreshTable()">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                       
                                        <tr>
                                            <td align="center">
                                                <br />
                                            </td>
                                            <td colspan="2" align="left">
                                                <br />
                                                <a href="javascript:refreshTable();" class="button"><span class="ui-button-text"
                                                    title="Submit">Submit</span></a> 
                                                
                                                 <%--<asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />--%>
                                                <a href="javascript:ExcelReport();" class="button"
                                                        style="vertical-align: top; width:74px;"><span title="Export to Excel" class="ui-button-text ">Save
                                                            Excel</span> </a><a href="javascript:print();" class="button"><span class="ui-button-text"
                                                                title="Print">Print</a>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
         
        <table style="width :100%;">
        <tr><td colspan="3" align="right">
         <a onclick="openPopup()" target ="_blank"  class="button"><span class="ui-button-text"
                                                    title="Submit">Add New</span></a>
        </td></tr>
            <tr>
                <td colspan="3" align="left">
                 <div style="background-color: #465AE8; color: White; text-align: center; height: 20px;
        padding-top: 7px;">
        <label id="rptinfo" ></label>
      </div>
                    <div>
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px;
                            font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;width:100%;" >
                            <thead>
                                <tr align="left">
                                    <th class="style1">
                                        S No
                                    </th>
                                    <th class="style1">
                                        User Id
                                    </th>
                                    <th class="style1">
                                        Plate No
                                    </th>
                                   
                                    <th class="style1">
                                        Refuel Start Time
                                    </th>
                                    <th class="style1">
                                        Refuel End Time
                                    </th>
                                    <th class="style1">
                                       Address
                                    </th>
                                    <th class="style1">
                                        Refuel Start
                                    </th>
                                    <th class="style1">
                                          Refuel End
                                    </th>
                                    <th class="style1">
                                      Refuel (Ltr)
                                    </th>
                                     <th>
                                      Refuel Cost (RM)
                                    </th>
                                    <th>
                                    Actual Refuel
                                    </th>
                                     <th>
                                    Cost/Ltr
                                    </th>
                                      <th>
                                    Actual Cost
                                    </th>
                                     <th>
                                    Accuracy Against Refuel
                                    </th>
                                    <th>
                                    Accuracy Against Tank Volume	
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            <tfoot>
                                <tr align="left">
                                      <th>
                                        S No
                                    </th>
                                       <th class="style1">
                                        User Id
                                    </th>
                                    <th>
                                        Plate No
                                    </th>
                                   
                                    <th class="style1">
                                        Refuel Start Time
                                    </th>
                                    <th class="style1">
                                        Refuel End Time
                                    </th>
                                    <th>
                                       Address
                                    </th>
                                    <th>
                                        Refuel Start
                                    </th>
                                    <th>
                                          Refuel End
                                    </th>
                                    <th style="text-align :right ;">
                                   <label id="totrefuel"  ></label>
                                    </th>
                                    <th style="text-align :right ;">
                                      <label id="totcost" ></label>
                                    </th>
                                    <th></th>
                                     <th></th>
                                     <th></th>
                                       <th>
                                    Accuracy Against Refuel
                                    </th>
                                    <th>
                                    Accuracy Against Tank Volume	
                                    </th>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </center>
    <input type="hidden" name="luid" value="" id="luid" runat="server" />
    <input type="hidden" name="rle" value="" id="rle" runat="server" />
    <input type="hidden" name="ulist" value="" id="ulist" runat="server" />
      <div class="demo">
        <div id="div-add" title="Add New Receipt" style="padding-top: 1px; padding-right: 0px;
            padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <table border="0" cellpadding="1" cellspacing="1" style="width: 400px; font-size: 11px;
            font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
            <br />
             <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                        User Name
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                  
                    <asp:DropDownList runat="server" Width ="120px" ID="ddluserid"  onchange="LoadPlates1()" />
                </td>
            </tr>
             <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                        Plateno
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:DropDownList runat="server" Width ="120px" ID="ddlplateno" />
                </td>
            </tr>
                 <tr>
                                            <td align="left"  style="color: #4E6CA3;">
                                                <b>Refuel DateTime</b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td align="left"  style="color: #4E6CA3;">
                                                <input type="text" id="tbxdatetime" runat="server" style="width: 85px;" readonly="readonly"
                                                    enableviewstate="false" />&nbsp;&nbsp; <strong>HH :</strong>&nbsp;<asp:DropDownList
                                                        ID="ddlhr"  CssClass="dclass" EnableViewState="False" runat="server"  Width="40px">
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
                                                &nbsp;<b style="color: #5f7afc">mm : </b>
                                                <asp:DropDownList ID="ddlmin"  EnableViewState="False" Width="40px" runat ="server" >
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
                                                    <asp:ListItem Value="16">17</asp:ListItem>
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
                                         <tr align="left" style="color: #4E6CA3;">
                                            <td>
                                                <b>Refuel Litrs</b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <input id="tbxlitters" type="text" runat="Server" style="border: #cbd6e4 1px solid;
                                                    width: 180px;"   onkeyup="return changeliters(0);" />
                                                <input id="cbxliters"  type="radio" name ="rdlc" onclick="javascript:enterliters(this.value);" value ="0" title="click to activate total liters"
                                                    />
                                            </td>
                                        </tr>
                                         <tr align="left" style="color: #4E6CA3;">
                                            <td>
                                                <b>Refuel Price</b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <input id="tbxcost" type="text" runat="Server" style="border: #cbd6e4 1px solid; width: 180px;" onkeyup="return changeliters(1);" />
                                                   
                                                <input id="cbxcost" type="radio" name ="rdlc" onclick="javascript:enterliters(this.value);"  value ="1" title="click to activate total liters"
                                                     />
                                            </td>
                                        </tr>
                                         <tr align="left" style="color: #4E6CA3;">
                                            <td>
                                                <b>Cost/Ltr</b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <input id="txtltrCost" type="text" runat="Server" style="border: #cbd6e4 1px solid; width: 180px;" onkeyup="return changeliters(2);"
                                                     /> 
                                            </td>
                                        </tr>
                                         <tr align="left" style="color: #4E6CA3;">
                                            <td>
                                                <b>Receipt No</b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <input id="txtReceipt" type="text" runat="Server" style="border: #cbd6e4 1px solid; width: 180px;"
                                                     /> 
                                            </td>
                                        </tr>
            </table>
        </div>
    </div>
      <div class="demo">
       <%--<div id="dialog-alert" title="Information">
                <p id="displayp" style="color:Blue;">
                    <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
                    </span>
                </p>
            </div>
              <div id="dialog-confirm" title="Confirmation ">
            <p id="displayc" style="color:Blue;">
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            </p>
        </div>--%>
        <div id="dialog-message" title="Information" style="padding-top: 1px; padding-right: 0px;
            padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <iframe id="RefuelDetails" name="RefuelDetails" src="" frameborder="0" scrolling="yes" height="445px"
                width="998px" style="visibility: hidden; border: solid 1px #aac6ff;" ></iframe>
        </div>
    </div>
   
</body>
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Refuel Summary Report" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
    

   
</html>
