<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.SMSOutboxReportNew" Codebehind="SMSOutboxReportNew.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>SMS Outbox Report</title>
    <style type="text/css" media="screen">
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
   <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script src="jsfiles/jquery-ui.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script src="jsfiles/DatePickerConv.js" type="text/javascript"></script>
    <script type="text/javascript">


        $(function () {
            $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: new Date(2014, 6, 01), maxDate: 0
            });

            $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', changeMonth: true, changeYear: true, numberOfMonths: 2, minDate: new Date(2014, 6, 01), maxDate: 0
            });

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: 1000,
                minHeight: 529
            });
        });

        function DisplayMap(intime, outtime, plateno) {
            document.getElementById("mappage").src = "GMap.aspx?bdt=" + intime + "&edt=" + outtime + "&plateno=" + plateno + "&scode=1&sf=0&r=" + Math.random();
            document.getElementById("mappage").style.visibility = "visible";
            $("#dialog-message").dialog("open");
        }


        function mysubmit() {
            var username = document.getElementById("DropDownList1").value;
            if (username == "--Select User Name--") {
                alert("Select User");
                return false;
            }
            else if (document.getElementById("ddlplate").value == "--Select Plate No--") {
                alert("Select Plate");
                return false;
            }



            var bigindatetime = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value;
            var enddatetime = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value;

            var fdate = Date.parse(bigindatetime);
            var sdate = Date.parse(enddatetime);

            var diff = (sdate - fdate) * (1 / (1000 * 60 * 60 * 24));
            var days = parseInt(diff) + 1;
            if (days > 5) {
                return confirm("5 days ok ? ");
            }
            return true;

        }

        function ExcelReport() {

           // var plateno = document.getElementById("ddlplate").value;
            var plateno = $("#ddlplate option:selected").text();


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
                "sPaginationType": "full_numbers",
                "iDisplayLength":500,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "aaSorting": [[2, "asc"]],
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
                    "sLengthMenu": "Display _MENU_ records",
                    "sSearch": "Search",
                    "sEmptyTable": "No data available in table",
                    "sInfo": "Got a total of _TOTAL_ entries to show (_START_ to _END_)",
                    "sInfoEmpty": "No entries to show",
                    "sInfoFiltered": "#NAME?",
                    "sZeroRecords":"No records to display",
                    "sInfoEmpty": "No entries to show",
                    "sLoadingRecords":"Please wait - loading...",
                    "sProcessing": "DataTables is currently busy"
                },
                "aoColumnDefs": [
                          { "bVisible": true, "bSortable": false, "sWidth": "45px", "aTargets": [0] },
                            { "bVisible": true, "bSortable": true, "sWidth": "125px", "aTargets": [1] },
                              { "bVisible": true, "bSortable": true, "sWidth": "150px", "aTargets": [2] },
                                { "bVisible": true, "bSortable": true, "aTargets": [3] },
                                  { "bVisible": true, "bSortable": true, "sWidth": "75px", "aTargets": [4] },
                                   { "bVisible": true, "bSortable": true, "sWidth": "65px",
                                       "fnRender": function (oData, sVal) {

                                           var server = oData.aData[5];
                                           switch (server) {
                                               case "0":
                                                   server = "Old";

                                                   break;
                                               case "1":
                                                   server = "New";

                                                   break;
                                               case "2":
                                                   server = "Bulk";

                                                   break;
                                           }
                                           return server;
                                       },


                                       "aTargets": [5]
                                   },
                                    { "bVisible": false, "bSortable": false, "sClass": "right", "sWidth": "40px", "aTargets": [6] }
                                       
                              ]
            });

        });

        function LoadPlates() {
            var uid = document.getElementById("DropDownList1").value;
            var plateno = document.getElementById("ddlplate").value;

            $("#ddlplate").attr("disabled", "disabled");
            $.getJSON('loadplatejson.aspx?userId=' + uid + '&luid=' + $('#luid').val() + '&role=' + $('#rle').val() + '&userslist=' + $('#ulist').val(), null, function (json) {
                var control = $('#ddlplate');
                $('#ddlplate option').remove();
                $('#ddlplate').removeAttr("disabled");
                $('#ddlplate').empty();
                var list = json;

                if (list.length > 0) {
                    control.empty().append('<option selected="selected" value="--All Plate No--">All plates</option>');
                    for (var i = 0; i < list.length; i++) {
                        control.append($("<option></option>").val(list[i][0]).html(list[i][0]));
                    }
                }
                else {
                    $('#ddlplate').empty().append('<option selected="selected" value="0">No Plates<option>');
                }

            });
        }
 

        function refreshTable() {
            var totalcost = 0;
            table = oTable.dataTable();
            table._fnProcessingDisplay(true);
            oSettings = table.fnSettings();
            $.getJSON('smsoutboxjson.aspx?u=' + $("#DropDownList1").val() + '&ddlp=' + $('#ddlplate').val() + '&bdt=' + $('#txtBeginDate').val() + " " + $('#ddlbh').val() + ":" + $('#ddlbm').val() + ":00" + '&edt=' + $('#txtEndDate').val() + " " + $('#ddleh').val() + ":" + $('#ddlem').val() + ":59" + '&luid=' + $('#luid').val() + '&role=' + $('#rle').val() + '&userslist=' + $('#ulist').val() + '&nu=' + document.getElementById("nu").value + '&pno=' + $('#pno').val() + '&gna=' + $('#gna').val() + '&una=' + $('#una').val() + '&mdt=' + $('#mdt').val() + '&lit=' + $('#lit').val() + '&dur=' + $('#dur').val() + '&gru=' + $('#gru').val(), null, function (json) {
                table.fnClearTable(this);
                for (var i = 0; i < json.length - 1; i++) {
                    table.oApi._fnAddData(oSettings, json[i]);
                    totalcost = totalcost + parseFloat(json[i][6]);
                }

                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                table._fnProcessingDisplay(false);
                table.fnDraw();
                if (json.length > 0) {
                    $("#thtotal").text(totalcost.toFixed(2)); 
                }
                else { 
                    $("#thtotal").text("0.00");
                }

            });

          return false;
          
        } 
          

    </script>
    <style type="text/css">
        .paging_full_numbers
        {
            width: 430px !important;
        }
        table.display thead th div.DataTables_sort_wrapper
        {
            font-weight: bold;
            font-family: verdana;
            font-size: 11px;
        }
        table.display tfoot th
        {
            font-family: Verdana;
            font-size: 11px;
        }
        table.display thead th div.DataTables_sort_wrapper
        {
            position: relative;
            padding-right: 0px;
        }
        
        .hor-minimalist-b
        {
            font-family: "Verdana";
            font-size: 11px;
            background: #fff;
            margin: 5px;
            width: 99%;
            border-collapse: collapse;
            text-align: left;
        }
        .hor-minimalist-b th
        {
            font-size: 14px;
            font-weight: normal;
            color: #039;
            padding: 5px 3px;
            border-bottom: 2px solid #6678b1;
        }
        .hor-minimalist-b td
        {
            border-bottom: 1px solid #ccc;
            color: #669;
            padding: 6px 8px;
        }
        .hor-minimalist-b tbody tr:hover td
        {
            color: #009;
        }
    </style>
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif"">
    <form id="Form1" method="post" runat="server">
  
    <center>
        <div>
            <br />
            <div class="c1">
                SMS Oubox Report</div>
            <br />
        </div>
        <table>
            <tr>
                <td align="center" style="width: 100%;">
                    <table style="font-family: Verdana; font-size: 11px;">
                        <tr>
                            <td colspan="2" class="t1">
                                <div class="h1">
                                    &nbsp; SMS Oubox Report&nbsp;:</div>
                            </td>
                        </tr>
                        <tr>
                            <td class="t2" style="width: 480px;">
                                <table style="width: 480px;">
                                    <tbody>
                                        <tr>
                                            <td align="left" style="width: 133px;">
                                                <b style="color: #465AE8;">
                                                    Begin Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                    id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" /><b
                                                        style="color: #465AE8;">&nbsp;HH&nbsp;:&nbsp;
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
                                                    </b><b style="color: #465AE8;">&nbsp;MM&nbsp;:&nbsp;
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
                                                <b style="color: #465AE8;">
                                                  End Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                    id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #465AE8;">&nbsp;HH&nbsp;:&nbsp;
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
                                                    </b><b style="color: #465AE8;">&nbsp;MM&nbsp;:&nbsp;
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
                                                <b style="color: #465AE8;">
                                                    UserName</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <asp:DropDownList ID="DropDownList1" runat="server" Width="251px"
                                                 onchange="LoadPlates()"   >
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">
                                                   Plate No
                                                </b>
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
                                                <a href="javascript:refreshTable();" class="button" style="width: 75px;"><span class="ui-button-text"
                                                    title="Submit">
                                                 Submit</span></a> <a href="javascript:ExcelReport();" title="Save Excel"
                                                        class="button" style="vertical-align: top; width: auto;"><span class="ui-button-text ">
                                                           Save Excel
                                                        </span></a><a href="javascript:print();" class="button" style="width: 74px;"><span
                                                            class="ui-button-text" title="Print">
                                                           Print</a>
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
        <table cellpadding="0" width="100%" cellspacing="0" border="0" class="display"  style="font-size: 11px;
                            font-weight: normal;  font-family: verdana">
            <tr>
                <td colspan="3" align="left">
                    <div>
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px;
                            font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
                         <thead>
                                <tr align="left">
                                    <th>
                                       S No
                                    </th>
                                    <th>
                                      Plate No
                                    </th>
                                    <th>
                                        Date Time
                                    </th>
                                    <th>
                                       Message
                                    </th>
                                    <th>
                                        Mobile No
                                    </th>
                                    <th>
                                    Server
                                    </th>
                                    <th style="display:none;">
                                        Cost
                                    </th>
                                    
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            <tfoot>
                            <tr align="left">
                                       <th>
                                     
                                    </th>
                                    <th>
                                     
                                    </th>
                                    <th>
                                   
                                    </th>
                                    <th>
                                     
                                    </th>
                                    <th>
                                     
                                    </th>
                                    <th >
                                 
                                    </th>
                                    <th id="thtotal" style="display:none;">
                                     
                                    </th>
                                </tr>
                                <tr align="left" >
                                       <th>
                                       S No
                                    </th>
                                    <th>
                                      Plate No
                                    </th>
                                    <th>
                                        Date Time
                                    </th>
                                    <th>
                                       Message
                                    </th>
                                    <th>
                                        Mobile No
                                    </th>
                                    <th>
                                    Server
                                    </th>
                                    <th style="display:none;">
                                        Cost
                                    </th>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <div id="dailog-details" style="padding-top: 0px; padding-right: 0px; padding-bottom: 0px;
            font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
            <div style="width: 960px;" id="exceldiv">
                <table id="hor-minimalist-b" class="hor-minimalist-b">
                </table>
            </div>
        </div>
    </center>
    <input type="hidden" name="luid" value="" id="luid" runat="server" />
    <input type="hidden" name="rle" value="" id="rle" runat="server" />
    <input type="hidden" name="ulist" value="" id="ulist" runat="server" />
     <input type="hidden" id="nu" value="No" />
    <input type="hidden" id="pno" value="Plate No" />
    <input type="hidden" id="gna" value="Geofence Name" />
    <input type="hidden" id="una" value="Username" />
    <input type="hidden" id="mdt" value="In Time" />
    <input type="hidden" id="lit" value="Out Time" />
    <input type="hidden" id="dur" value="Duration" />
    <input type="hidden" id="gru" value="Group Name" />
    <input type="hidden" value="en" id="lanc" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
   <input type="hidden" id="title" name="title" value="SMS Alert Summary Report" />
    <input type="hidden" id="titl1" name="titl1" value="Geofence Report" />
    <input type="hidden" id="plno" name="plno" value="vehicle plate number" />
    <input type="hidden" id="rd" name="rd" value="User Name" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
    <div class="demo">
        <div id="dialog-message" title="Mini Map" style="padding-top: 1px; padding-right: 0px;
            padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <iframe id="mappage" name="mappage" src="" frameborder="0" scrolling="no" height="500"
                width="998px" style="visibility: hidden; border: solid 1px #aac6ff;" />
        </div>
    </div>
</body>
</html>
