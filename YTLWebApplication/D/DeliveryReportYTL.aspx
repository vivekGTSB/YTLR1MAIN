<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DeliveryReportYTL.aspx.vb" Inherits="YTLWebApplication.DeliveryReportYTL" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Transporter Delivery Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        
        @import "cssfiles/TableTools.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";

        .dataTables_info {
            width: 25%;
            float: left;
        }

        .whitespace {
            white-space: nowrap;
        }
        .loader {
            border: 16px solid #f3f3f3;
            border-radius: 50%;
            border-top: 16px solid #3498db;
            width: 120px;
            height: 120px;
            -webkit-animation: spin 2s linear infinite; /* Safari */
            animation: spin 2s linear infinite;
        }

/* Safari */
@-webkit-keyframes spin {
  0% { -webkit-transform: rotate(0deg); }
  100% { -webkit-transform: rotate(360deg); }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
    </style>



    	<link rel="stylesheet" type="text/css" href="https://cdn.datatables.net/buttons/1.6.1/css/buttons.dataTables.min.css">
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
<script type="text/javascript" language="javascript" src="https://cdn.datatables.net/buttons/1.6.1/js/dataTables.buttons.min.js"></script>
<script type="text/javascript" language="javascript" src="https://cdn.datatables.net/buttons/1.6.1/js/buttons.flash.min.js"></script>
<script type="text/javascript" language="javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
<script type="text/javascript" language="javascript" src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/pdfmake.min.js"></script>
<script type="text/javascript" language="javascript" src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.53/vfs_fonts.js"></script>
<script type="text/javascript" language="javascript" src="https://cdn.datatables.net/buttons/1.6.1/js/buttons.html5.min.js"></script>
<script type="text/javascript" language="javascript" src="https://cdn.datatables.net/buttons/1.6.1/js/buttons.print.min.js"></script>
    
    <script type="text/javascript">

        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }

        $(function () {
            $("#txtBeginDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2013, 0, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txtEndDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2013, 0, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

            });

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: getWindowWidth() - 60,
                minHeight: getWindowHeight() - 200
            });

            $("#idlingpage").width(getWindowWidth() - 65);
            $("#idlingpage").height(getWindowHeight() - 200);
        });

        function DisplayMap(intime, outtime, plateno) {
            document.getElementById("mappage").src = "GMap.aspx?bdt=" + intime + "&edt=" + outtime + "&plateno=" + plateno + "&scode=1&sf=0&r=" + Math.random();
            document.getElementById("mappage").style.visibility = "visible";

            $("#dialog-message").dialog("open");
        }


        function mysubmit() {

            var username = document.getElementById("ddltransporter").value;
            if (username == "0") {
                alert("Please select transporter name");
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
        var ec = "<%=ec %>";
        var dtrange = "";
        function ExcelReport() {
            if (ec == "True") {
                var excelformobj = document.getElementById("excelform");
                document.getElementById("rperoid").value = dtrange;
                document.getElementById("reporttype").value = "1";
                if (document.getElementById("title").value == "") {
                    document.getElementById("title").value = "Delivery Report";
                }
                excelformobj.submit();
            }
            else {
                alert("First click submit button");
            }
        }
        function openpage(id, bdt, edt, tid) {
            openMap("ShowCompetitorInformation.aspx?geoid=" + id + "&transid=" + tid + "&bdt=" + bdt + "&edt=" + edt);
        }

        function openMap(message) {
            document.getElementById("idlingpage").style.visibility = "visible";
            document.getElementById("idlingpage").src = message;
            $("#dialog-message").dialog("open");
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
            $("#rbplant").attr("checked", true);
            $("#plantsummary").show();
            var bigindatetime = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value;
            var enddatetime = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value;
            dtrange = bigindatetime + "-" + enddatetime;
            $("#transportersummary").hide();
            $("#vehiclesummary").hide();
            fnFeaturesInit()
            $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 25,
                "aaSorting": [],
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                //    "fnDrawCallback": function (oSettings) {
                //       if (oSettings.bSorted || oSettings.bFiltered) {
                //        if (oSettings.aoColumns[0].bVisible == true) {
                //            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                //                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                //            }
                //        }
                //    }
                //},
                "aoColumnDefs": [
                    { "bVisible": true, "bSortable": false, "aTargets": [0] },
                    { "bVisible": true, "bSortable": false, "aTargets": [1] },
                    { "bVisible": true, "bSortable": false, "aTargets": [2] },
                    { "bVisible": true, "bSortable": false, "aTargets": [3] },
                    { "bVisible": true, "bSortable": false, "aTargets": [4] },
                    { "bVisible": true, "bSortable": false, "aTargets": [5] },
                    { "bVisible": true, "bSortable": false, "aTargets": [6] },
                    { "bVisible": true, "bSortable": false, "aTargets": [7] },
                    { "bVisible": true, "bSortable": false, "aTargets": [8] },
                    { "bVisible": true, "bSortable": false, "aTargets": [9] },



                ]
            });



        });
        var Bplant, Btype, Byear, Bmonth, Btype, BVtype;
        function BacktoTrans() {
            if (Bplant != null) {
                DrilldownToTransporter(Bplant, Byear, Bmonth, Btype, BVtype);
            }
        }
        var displayname;
        function DrilldownToTransporter(plant, year, month, type, vtype, protype) {
            $("#plantsummary").hide();
            $("#transportersummary").show();
            $("#vehiclesummary").hide();
            var d = new Date($("#txtBeginDate").val())
            dtrange = (d.getMonth() + 1) + "/" + d.getFullYear();
            document.getElementById("rperoid").value = dtrange;
            document.getElementById("reporttype").value = "1";
            displayname= plant;
            if (plant == "LAFARGEE") {
                displayname = "Ex-Lafarge"
            }
            else if (plant == "YTLALL") {
                displayname = "Total YTL plant"
            }
            else if (plant == "LAFARGEE") {
                displayname = "Total ex-Lafarge plant"
            }
            else if (plant == "ASHALL") {
                displayname = "Total Power Plant"
            }
            else if (plant == "ALL") {
                displayname = "Total All Plant"
            }
           
            document.getElementById("title").value = displayname + " - Summary By Truck Type";
            $(".summarytransporterhead").html("<center><b>" + displayname + " - Summary By Truck Type</b></center>")
            Bplant = plant;
            Btype = type;
            Byear = year;
            Bmonth = month;
            Btype = type;
            BVtype = vtype;
            $.ajax({
                type: "POST",
                url: "DeliveryReportYTL.aspx/GetData",
                data: '{Plantid: \"' + plant + '\",year:\"' + year + '\",month:\"' + month + '\",type:\"' + type + '\",vehicletype:\"' + vtype + '\",protype:\"' + protype + '\",bh:\"' + $("#ddlbh").val() + '\",bm:\"' + $("#ddlbm").val() + '\",eh:\"' + $("#ddleh").val() + '\",em:\"' + $("#ddlem").val() + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $(".transpotersummIn").html(unescape(response.d.split("APK")[0]));
                    $(".transpotersummEx").html(unescape(response.d.split("APK")[1]));
                    var h = getWindowHeight() - 180 + "px";
                    $('#examples1').dataTable({
                        "bProcessing": true,
                        "sPaginationType": "full_numbers",
                        "sScrollY": h,
                        "sScrollX": "100%",
                        "bScrollCollapse": true,
                        "bLengthChange": false,
                        "iDisplayLength": 2000,
                        "bStateSave": true,
                        "bInfo": true,
                        "bAutoWidth": false,
                        "ordering": false,
                        "bJQueryUI": true,
                        "aaSorting": [],
                        "sDom": '<"H"Cl<"MyButton">f>tr<"F"iT>',
                        "aoColumnDefs": [
                            { "bVisible": true, "bSortable": false, "aTargets": [0] },
                            { "bVisible": true, "bSortable": false, "aTargets": [2] },
                            { "bVisible": true, "bSortable": false, "sClass": "whitespace", "aTargets": [1] }
                        ],
                        "oTableTools": { "aButtons": ["xls", "copy", "print"], "sSwfPath": "cssfiles/copy_csv_xls_pdf.swf" }
                        
                    });
                    var t1 = $('#examples2').dataTable({
                        "bProcessing": true,
                        "sPaginationType": "full_numbers",
                        "sScrollY": h,
                        "sScrollX": "100%",
                        "bScrollCollapse": true,
                        "bLengthChange": false,
                        "iDisplayLength": 2000,
                        "bStateSave": true,
                        "bInfo": true,
                        "bAutoWidth": false,
                        "ordering": false,
                        "bJQueryUI": true,
                        "sDom": '<"H"Cl<"MyButton">f>tr<"F"iT>',
                        "aaSorting": [],
                        "aoColumnDefs": [
                            { "bVisible": true, "bSortable": false, "aTargets": [0] },
                            { "bVisible": true, "bSortable": false, "aTargets": [2] },
                            { "bVisible": true, "bSortable": false, "sClass": "whitespace", "aTargets": [1] }
                        ],
                        "oTableTools": { "aButtons": ["xls", "copy", "print"], "sSwfPath": "cssfiles/copy_csv_xls_pdf.swf" }

                    });
                    //t.on('order.dt search.dt', function () {
                    //    t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                    //        cell.innerHTML = i + 1;
                    //    });
                    //}).draw();
                },
                failure: function (response) {
                    alert("Error");
                }
            });
        }

        function DrilldownToVehicle(transporter, transportername) {
            $("#plantsummary").hide();
            $("#transportersummary").hide();
            $("#vehiclesummary").show();
            $("#loaderDiv").addClass("loader");
            var d = new Date($("#txtBeginDate").val())
            dtrange = (d.getMonth() + 1) + "/" + d.getFullYear();
            document.getElementById("rperoid").value = dtrange;
            document.getElementById("reporttype").value = "1";
            document.getElementById("title").value = displayname+ " - " + transportername + " - Summary By Vehicle Type";
            $(".summaryvehhead").html("<center><b>" + displayname+" - " + transportername + " - Summary By Vehicle Type</b></center>")
            $.ajax({
                type: "POST",
                url: "DeliveryReportYTL.aspx/GetDataTruck",
                data: '{Plantid: \"' + Bplant + '\",year:\"' + Byear + '\",month:\"' + Bmonth + '\",type:\"' + Btype + '\",vehicletype:\"' + BVtype + '\",Transporterid:\"' + transportername + '\",bh:\"' + $("#ddlbh").val() + '\",bm:\"' + $("#ddlbm").val() + '\",eh:\"' + $("#ddleh").val() + '\",em:\"' + $("#ddlem").val() + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $(".trucksumm").html(unescape(response.d));
                    var h = getWindowHeight() - 180 + "px";
                    var t3 = $('#examples3').dataTable({
                        "bProcessing": true,
                        "sPaginationType": "full_numbers",
                        "sScrollY": h,
                        "sScrollX": "100%",
                        "bScrollCollapse": true,
                        "bLengthChange": false,
                        "iDisplayLength": 2000,
                        "bStateSave": true,
                        "bInfo": true,
                        "aaSorting": [[2, "asc"]],
                        dom: 'Bfrtip',
                        buttons: [
                            'copy', 'csv', 'excel', 'pdf', 'print'
                        ],
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
                        "bJQueryUI": true,
                        "aoColumnDefs": [
                            { "bVisible": true, "bSortable": false, "aTargets": [0] },
                            { "bVisible": true, "bSortable": true, "sClass": "whitespace", "aTargets": [1] },
                            { "bVisible": true, "bSortable": true, "sClass": "whitespace", "aTargets": [2] },
                            { "bVisible": true, "bSortable": true, "aTargets": [3] },
                            { "bVisible": true, "bSortable": true, "aTargets": [4] },
                            { "bVisible": true, "bSortable": true, "aTargets": [5] }
                        ]
                      //  "oTableTools": { "aButtons": ["xls", "copy", "print"], "sSwfPath": "cssfiles/copy_csv_xls_pdf.swf" }

                    });
                    //t3.on('order.dt search.dt', function () {
                    //    t3.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
                    //        cell.innerHTML = i + 1;
                    //    });
                    //}).draw();
                    $("#loaderDiv").removeClass("loader");

                },
                failure: function (response) {
                    alert("Error");
                    $("#loaderDiv").removeClass("loader");
                }
            });

        }


    </script>

</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
        <center>
            <div>

                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Delivery Report</b>
            </div>
            <input type="radio" id="rbplant" runat="server" style="visibility: hidden" />
            <input type="radio" id="rbtransporter" runat="server" style="visibility: hidden" />
            <input type="radio" id="rbplateno" runat="server" style="visibility: hidden" />
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Delivery Report&nbsp;:</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 450px; border: solid 1px #5B7C97;">
                                    <table style="width: 450px;">
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
                                                        id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" /><b
                                                            style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlbh" runat="server" Width="40px" EnableViewState="False">
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
                                                        </asp:DropDownList>
                                                        </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlbm" runat="server" Width="40px" EnableViewState="False">
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
                                                    <b style="color: #465AE8;">Vehicle Type</b>
                                                </td>
                                                <td>
                                                    <b style="color: #465AE8;">:</b>
                                                </td>
                                                <td align="left" style="width: 326px">
                                                    <asp:DropDownList ID="ddlvehicletype" runat="server" Width="248px" AutoPostBack="true">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                </td>
                                                <td colspan="2" align="center">
                                                    <br />
                                                    <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />
                                                    <a href="javascript:ExcelReport();" class="button"><span class="ui-button-text" title="Download">Download</a>

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
            <table runat="server" id="plantsummary">
                <tr>
                    <td colspan="3">
                        <div style="width: 1100px;">

                            <%=sb2.ToString()%>
                        </div>
                    </td>
                </tr>
            </table>
            <table runat="server" id="transportersummary">
                <tr>
                    <td><a href="<%=qureystr %>">Back To Plant</a></td>
                    <td class="summarytransporterhead">
                        <center><b>Summary By Transporter</b></center>
                    </td>
                    <td style="float: right"><a href="<%=qureystr %>">Back To Plant</a></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <h3>External Transporters</h3>
                        <div style="width: 1190px;" class="transpotersummEx">
                        </div>
                        <br />
                        <h3>Internal Transporters</h3>
                        <div style="width: 1190px;" class="transpotersummIn">
                        </div>
                    </td>
                </tr>
            </table>

            <table runat="server" id="vehiclesummary">
                <tr>
                    <td><a href="<%=qureystr %>">Back To Plant</a> / <a href="#" onclick="javascript:BacktoTrans()">Back To Truck Type</a> </td>
                    <td class="summaryvehhead">
                        <center><b>Summary By Vehicle</b></center>
                    </td>
                    <td style="float: right"><a href="<%=qureystr %>">Back To Plant</a> / <a href="#" onclick="javascript:BacktoTrans()">Back To Truck Type</a></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <center>
                            <div id="loaderDiv" class=""></div>
                        </center>
                        
                        <div style="width: 1190px;" class="trucksumm">
                        </div>
                    </td>
                </tr>
            </table>
        </center>
       
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="" />
        <input type="hidden" id="rperoid" name="rperoid" value="" />
        <input type="hidden" id="reporttype" name="reporttype" value="" />
    </form>


    <div id="dialog-message" title="Trips's Information" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="idlingpage" name="idlingpage" src="" frameborder="0" scrolling="auto"
            height="512" width="1198px" style="visibility: hidden; border: solid 1px #aac6ff;" />
    </div>

</body>
</html>
