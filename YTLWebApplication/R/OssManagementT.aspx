<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false"
    Inherits="YTLWebApplication.OssManagementT" CodeBehind="OssManagementT.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>OSS Management</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/ColVis.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #fw_container {
            width: 100%;
        }

        table.display td {
            padding: 2px 2px;
        }

        .redtext {
            color: red;
        }

        .red {
            background-color: #dd4b39 !important;
            font-weight: bold;
            font-style: oblique;
            color: aliceblue;
        }
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/ColVis.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">


        $(function () {
            $("#idlingpage").height(getWindowHeight() - 100);
            $("#idlingpage").width(getWindowWidth() - 110);
            $("#txtBeginDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2022, 00, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2,
                onSelect: function (dateText) {
                    $(this).change();
                    console.log("Selected date BDT: " + dateText + "; input's current value: " + this.value);
                    changeDDL();

                }
            });
            $("#txtEndDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2022, 00, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2,
                onSelect: function (dateText) {
                    $(this).change();
                    console.log("Selected date EDT: " + dateText + "; input's current value: " + this.value);
                    changeDDL();
                }
            });
            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: getWindowWidth() - 110,
                minHeight: getWindowHeight() - 100
            });
            $("#ddlJobType").change(function () {
                changeDDL();
            });
        });
        function closePopup() {
            $("#dialog-message").dialog("close");
        }

        function closePopupOndivert(dnno) {
            $("#examples tr td:nth-child(7)").each(function () {
                if ($(this).html() == dnno) {
                    $(this).parent().children('td').eq(16).html("Diversion (Manual)");
                    $(this).parent().children('td').eq(16).attr("class", "red");
                }

            });
            $("#dialog-message").dialog("close");
        }

        function closePopuponCancelDivert(dnno) {
            $("#dialog-message").dialog("close");
            $("#examples tr td:nth-child(7)").each(function () {
                if ($(this).html() == dnno) {
                    $(this).parent().children('td').eq(16).html("Submit Page again");
                    $(this).parent().children('td').eq(16).removeClass("red");

                }

            });
        }

        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }

        var ec =<%=ec %>;
        //     function openpage(plateno,bdt,shiptocode,shiptoname) {
        //     openMap("ShowVehicleIdlings.aspx?pno=" + plateno + "&bdt=" + bdt +"&shiptocode="+ shiptocode +"&shiptoname=" + shiptoname);
        //        }

        function openpage(id) {
            openMap("ShowInformation.aspx?id=" + id);
        }
        function OpenDiversion(id) {
            openMap("ShowDiversionList.aspx?id=" + id);
        }
        function OpenGeoInfo(plateno, weightout) {
            openMap("buildcongeos.aspx?p=" + plateno + "&w=" + weightout);
        }
        function changeDDL() {
            var begindatetime = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value;
            var enddatetime = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value;
            var type = $("#ddlJobType").val();
            $.get("GetShipToData.aspx?bdt=" + begindatetime + "&edt=" + enddatetime + "&type=" + type + "", function (response) {
                $("#ddlShipToCode").html("<option value='ALL'>ALL</option>");
                for (var i = 0; i < response.length; i++) {
                    if (response[i][2] == "1") {
                        $("#ddlShipToCode").append($("<option class='redtext' />").val(response[i][0]).text(response[i][1]));
                    }
                    else {
                        $("#ddlShipToCode").append($("<option  />").val(response[i][0]).text(response[i][1]));
                    }

                }
                $("#ddlShipToCode").val($("#ddlshipto").val());
            });
        }
        function ReprocessJob(patchno) {
            $.get("ReprocessOSSJob.aspx?bdt=" + document.getElementById("txtBeginDate").value + "&edt=" + document.getElementById("txtEndDate").value + "&pno=" + patchno + "", function (response) {
                var d = new Date();
                $("#txtmessage").html(d.toLocaleString() + " Reprocess Request Submitted Successfully");
            });
        }
        function openMap(message) {
            document.getElementById("idlingpage").style.visibility = "visible";
            document.getElementById("idlingpage").src = message;
            $("#dialog-message").dialog("open");
        }
        function openvehiclepath(plateno, bdt, edt, shiptocode, shiptoname) {
            document.getElementById("idlingpage").style.visibility = "visible";
            document.getElementById("idlingpage").src = "ShowVehicleIdlings2.aspx?pno=" + plateno + "&bdt=" + bdt + "&edt=" + edt + "&shiptocode=" + shiptocode + "&shiptoname=" + shiptoname + "&scode=1";
            // document.getElementById("idlingpage").src ="GMap.aspx?plateno=" + plateno + "&bdt=" + bdt +"&edt="+ edt +"&scode=1" ;
            $("#dialog-message").dialog("open");
        }

        function openvehiclepathwithmarker(plateno, bdt, edt, lat, lon, uid, tr, sr, dn, wo, sc, sn, ata, pos, shiptocode, shiptoname) {
            document.getElementById("idlingpage").style.visibility = "visible";
            var qs = lat + "," + lon + "," + uid + "," + tr + "," + sr + "," + dn + "," + wo + "," + sc + "," + sn + "," + ata + "," + pos;
            document.getElementById("idlingpage").src = "ShowVehicleIdlings3.aspx?qs=" + qs + "&pno=" + plateno + "&bdt=" + bdt + "&edt=" + edt + "&shiptocode=" + shiptocode + "&shiptoname=" + shiptoname + "&scode=2";
            // document.getElementById("idlingpage").src ="GMap.aspx?markerlat="+ lat + "&markerlon=" + lon +"&plateno=" + plateno + "&bdt=" + bdt +"&edt="+ edt +"&uid=" + uid +"&tr=" + tr + "&sr=" + sr +"&dn="+ dn + "&wo=" + wo +"&sc="+ sc +"&sn=" + sn +"&ata=" + ata + "&pos=" + pos +"&scode=2" ;
            $("#dialog-message").dialog("open");
        }

        function openvehiclepathWM(plateno, bdt, edt, uid, tr, sr, dn, wo, sc, sn, shiptocode, shiptoname) {
            document.getElementById("idlingpage").style.visibility = "visible";
            var qs = uid + "," + tr + "," + sr + "," + dn + "," + wo + "," + sc + "," + sn;
            document.getElementById("idlingpage").src = "ShowVehicleIdlings3.aspx?qs=" + qs + "&pno=" + plateno + "&bdt=" + bdt + "&edt=" + edt + "&shiptocode=" + shiptocode + "&shiptoname=" + shiptoname + "&scode=3";
            //document.getElementById("idlingpage").src ="GMap.aspx?plateno=" + plateno + "&bdt=" + bdt +"&edt="+ edt +"&uid=" + uid +"&tr=" + tr + "&sr=" + sr +"&dn="+ dn + "&wo=" + wo +"&sc="+ sc +"&sn=" + sn +"&scode=3" ;
            $("#dialog-message").dialog("open");
        }
        function mysubmit() {

            var username = document.getElementById("ddluser").value;
            if (username == "--Select User Name--") {
                alert("Please select user name");
                return false;
            }
            var plateno = document.getElementById("ddlpleate").value;
            if (plateno == "--Select Plate No--") {
                alert("Please select vehicle plate number");
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
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    if (elm.checked == true) {
                        checked = true;
                        break;
                    }
                }
            }
            if (checked) {
                var result = confirm("Are you delete checked OssPatch information ?");
                if (result) {
                    return true;
                }
                return false;
            }
            else {
                alert("Please select checkboxes");
                return false;
            }
        }
        function ExcelReport() {
            var ec =<%= ec %>;
            if (ec == true) {
                var excelformobj = document.getElementById("excelform");
                excelformobj.submit();
            }
            else {
                alert("First click submit button");
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
        var oTable;
        $(document).ready(function () {
            fnFeaturesInit();
            if ($("#ddlShipToCode option").length == 0) {
                changeDDL();
            }

            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 25,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "aaSorting": [[7, "asc"]],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                                if ($('td:eq(18)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html().includes("Diversion")) {
                                    $('td:eq(19)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).attr("class", "red")
                                }
                            }
                        }
                    }
                },
                "aoColumnDefs": [
                    { "bVisible": true, "bSortable": false, "aTargets": [0] },
                    { "bVisible": false, "bSortable": false, "aTargets": [2] },
                    {
                        "sClass": "center", "sWidth": "10px", "aTargets": [21], "bSortable": false, "bVisible": true,
                        "fnRender": function (oData, sVal) {
                            if (oData.aData[21] == "--") {
                                return "";
                            }
                            else {
                                return "" + oData.aData[21] + " <input type='checkbox' name='chk' id='chk' value='" + oData.aData[22] + "' class=\"group1\"  />";
                            }

                        }

                    },
                    { "bVisible": false, "bSortable": false, "aTargets": [22] }
                ],

                "fnFooterCallback": function (nRow, aaData, iStart, iEnd, aiDisplay) {
                    /*
                     * Calculate the total market share for all browsers in this table (ie inc. outside
                     * the pagination)
                     */
                   

                    var iPageMarket = 0;
                    for (var i = 0; i < aaData.length; i++) {
                        if (parseFloat(aaData[i][10]) > 100) {
                            if (aaData[[i]][8] == "SILICA FUME(BAG)") {
                                iPageMarket += parseFloat(aaData[i][10]) * 0.02;
                            }
                            else {
                                iPageMarket += parseFloat(aaData[i][10]) * 0.05;
                            }

                        }
                        else {
                            iPageMarket += parseFloat(aaData[i][10]);
                        }

                    }
                    /* Calculate the market share for browsers on this page */
                    var iPageMarket1 = 0;
                    for (var i = 0; i < aiDisplay.length; i++) {
                        if (parseFloat(aaData[aiDisplay[i]][10]) > 100) {
                            if (aaData[aiDisplay[i]][8] == "SILICA FUME(BAG)") {
                                iPageMarket1 += parseFloat(aaData[aiDisplay[i]][10]) * 0.02;
                            }
                            else {
                                iPageMarket1 += parseFloat(aaData[aiDisplay[i]][10]) * 0.05;
                            }
                        }
                        else {
                            iPageMarket1 += parseFloat(aaData[aiDisplay[i]][10]);
                        }

                    }



                    /* Modify the footer row to match what we want */
                    var nCells = nRow.getElementsByTagName('th');
                    nCells[9].innerHTML = 'Total ' + iPageMarket.toFixed(2) + 'Ton ';

                   // nCells[9].innerHTML = 'Total ' + iPageMarket.toFixed(2) + 'Ton [' + iPageMarket1.toFixed(2) + ' Ton]';
                }
            });
            //oTable.fnSetColumnVis(4, false);
        });




        function check() {
            var tpt =<%= tpton %>;
            if (tpt == "0" || tpt == 0) {
                $("#tpt1").hide();
            }
        }
        var matches = [];
        function Reprocessjob() {
            $(".group1:checked").each(function () {
                matches.push(this.value);
            });
            if (matches.length == 0) {
                alertbox("Please select at least one Job to reprocess.");
            }
            else {
                var ids = "";//JSON.stringify(matches);
                for (var i = 0; i < matches.length; i++) {
                    if (ids == "") {
                        ids = matches[i];
                    }
                    else {
                        ids += "," + matches[i];
                    }
                }
                $.get("ReprocessOSSJobAll.aspx?pno=" + ids + "", function (response) {
                    for (var i = 0; i < matches.length; i++) {
                        $("#" + matches[i] + "").attr("checked", false);
                    }
                    var d = new Date();
                    $("#txtmessage").html(d.toLocaleString() + " Reprocess Request Submitted Successfully");

                });
            }

        }



    </script>
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif"
    onload="javascript:check()">
    <form id="fuelform" runat="server">
        <center style="margin-bottom: 34px;">
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">OSS Management</b>
            <br />
            <br />
            <div style="position: absolute; left: 10px; top: 10px; text-align: left;">
                <span style="color: navy; font-family: Verdana; font-size: 10px; font-weight: normal;">
                    <b>Delivery Completed (E)</b>
                    <br />
                    Stay Time < 15mins Inside Geofence<br />
                    <br />
                    <b>Delivery Completed (D)</b>
                    <br />
                    Diversion<br />
                    <br />
                    <b>Delivery Completed (P)</b>
                    <br />
                    No Geofence with PTO ON<br />
                    <br />
                    <b>Inside Geofence</b>
                    <br />
                    Inside Geofence Currently<br />
                    <br />
                    <b>Delivery Completed</b>
                    <br />
                    Delivered<br />
                    <br />
                    <b>No GPS device</b>
                    <br />
                    Vehicle Not Installed With GPS device yet<br />
                    <br />
                    <b>Reprocess Job</b>
                    <br />
                    Reprocess of Missing Delivery<br />
                    <br />
                    <b>Pending Destination Set Up</b>
                    <br />
                    Delivery Destination Not Set In System Yet<br />
                    <br />
                    <b>Timeout</b>
                    <br />
                    1. Delivery Time > 48Hrs<br />
                    2. Vehicle Return To Plant
                    <br />
                    3. Vehicle Having New DN<br />
                </span>
            </div>
            <table style="font-family: Verdana; font-size: 11px;">
                <tr>
                    <td style="height: 20px; background-color: #465ae8;" align="left">
                        <b style="color: White;">&nbsp;OSS Management &nbsp;:</b>
                    </td>
                </tr>
                <tr>
                    <td style="width: 420px; border: solid 1px #3952F9; height: 184px;">
                        <center>
                            <table style="width: 420px;">
                                <tbody>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">Begin Date</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <input style="width: 70px;" type="text" value="<%=strBeginDate%>" id="txtBeginDate"
                                                runat="server" name="txtBeginDate" enableviewstate="false" readonly="readonly" />&nbsp;<b
                                                    style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                            <asp:DropDownList ID="ddlbh" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                EnableViewState="False">
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
                                            <b style="color: #5f7afc;">&nbsp;Min&nbsp;:&nbsp;</b>
                                            <asp:DropDownList ID="ddlbm" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                EnableViewState="False">
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
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">End Date</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #5f7afc;">
                                                    &nbsp;Hour&nbsp;:&nbsp;</b>
                                            <asp:DropDownList ID="ddleh" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                EnableViewState="False">
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
                                            <b style="color: #5f7afc;">&nbsp;Min&nbsp;:&nbsp;</b>
                                            <asp:DropDownList ID="ddlem" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                EnableViewState="False">
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
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">Source</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlSource" runat="server" Font-Size="12px" Width="260px" Font-Names="verdana"
                                                EnableViewState="False">
                                                <%--  <asp:ListItem Value="ALL SOURCES" Selected="True"> ALL SOURCES </asp:ListItem>
                                               <asp:ListItem Value="BC">BC - BATU CAVES</asp:ListItem>
                                               <asp:ListItem Value="BS">BS - BUKIT SAGU</asp:ListItem>
                                               <asp:ListItem Value="GPK">GPK - GELANG PATAH</asp:ListItem>
                                               <asp:ListItem Value="LM">LM - LUMUT</asp:ListItem>
                                               <asp:ListItem Value="PG">PG - PASIR GUDANG</asp:ListItem>
                                               <asp:ListItem Value="PR">PR - PADANG RENGAS</asp:ListItem>
                                               <asp:ListItem Value="WP">WP - WESTPORT, PULAU INDAH</asp:ListItem>
                                               <asp:ListItem Value="PG1">PG1 - PG1</asp:ListItem>
                                               <asp:ListItem Value="PG2">PG2 - PG2</asp:ListItem>
                                               <asp:ListItem Value="KT">KT - KANTHAN</asp:ListItem>
                                               <asp:ListItem Value="RW">RAWANG</asp:ListItem>
                                               <asp:ListItem Value="LK">LANGKAWI</asp:ListItem>
                                               <asp:ListItem Value="WP2">WESTPORT2</asp:ListItem>
                                               <asp:ListItem Value="TB">TANJUNG BIN</asp:ListItem>
                                               <asp:ListItem Value="KP">KAPAR</asp:ListItem>--%>
                                                <%--
                                            <asp:ListItem Value="SA">DEPOT - SHAH ALAM, SELANGOR</asp:ListItem>
                                            <asp:ListItem Value="KW">KANTHAN WORKS</asp:ListItem>
                                            <asp:ListItem Value="LW">LANGKAWI WORKS</asp:ListItem>
                                            <asp:ListItem Value="RW">RAWANG WORKS</asp:ListItem>
                                            <asp:ListItem Value="PGT">TERMINAL - PASIR GUDANG, JOHOR</asp:ListItem>
                                            <asp:ListItem Value="WPT">TERMINAL - WEST PORT, SELANGOR</asp:ListItem>--%>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">Job Type</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlJobType" runat="server" Font-Size="12px" Width="260px" Font-Names="verdana"
                                                EnableViewState="False">
                                                <asp:ListItem Value="0">ALL</asp:ListItem>
                                                <asp:ListItem Value="1">BULK</asp:ListItem>
                                                <asp:ListItem Value="2">BAG</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">Ship To Code</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlShipToCode" runat="server" Font-Size="12px" Width="260px" Font-Names="verdana"
                                                EnableViewState="False">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="tpt1">
                                        <td align="left">
                                            <b style="color: #5f7afc;">Filter With</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlfiltertype" runat="server" Font-Size="12px" Width="260px"
                                                Font-Names="verdana" OnSelectedIndexChanged="ddlfiltertype_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Value="0">TRANSPORTER</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">USERNAME</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">Type</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddltranstype" runat="server" Font-Size="12px" Width="260px"
                                                Font-Names="verdana" OnSelectedIndexChanged="ddltranstype_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Value="0" Selected="True">ALL</asp:ListItem>
                                                <asp:ListItem Value="1">INTERNAL</asp:ListItem>
                                                <asp:ListItem Value="2">EXTERNAL</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr id="tpt1">
                                        <td align="left">
                                            <b style="color: #5f7afc;" id="idlbl" runat="server">Transporter</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlTransport" runat="server" Font-Size="12px" Width="260px"
                                                Font-Names="verdana">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">Status</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="ddlstatus" runat="server" Font-Size="12px" Width="260px" Font-Names="verdana"
                                                EnableViewState="False">
                                                <asp:ListItem Value="--ALL STATUS--">--ALL STATUS--</asp:ListItem>
                                                <asp:ListItem Value="3">In Progress</asp:ListItem>
                                                <asp:ListItem Value="5">Inside Geofence</asp:ListItem>
                                                <asp:ListItem Value="7">Delivery Completed</asp:ListItem>
                                                <asp:ListItem Value="8">Delivery Completed (E)</asp:ListItem>
                                                <asp:ListItem Value="12">Delivery Completed (D)</asp:ListItem>
                                                <asp:ListItem Value="13">Delivery Completed (P)</asp:ListItem>
                                                <asp:ListItem Value="1">No GPS Device</asp:ListItem>
                                                <asp:ListItem Value="11">Reprocess Job</asp:ListItem>
                                                <asp:ListItem Value="2">Pending Destination Set Up</asp:ListItem>
                                                <asp:ListItem Value="10">Timeout</asp:ListItem>
                                                <asp:ListItem Value="14">No GPS Data</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <b style="color: #5f7afc;">Based on</b>
                                        </td>
                                        <td>
                                            <b style="color: #5f7afc;">:</b>
                                        </td>
                                        <td align="left" style="color: #5f7afc; font-weight: bold;">
                                            <asp:RadioButton ID="RadioButton1" Checked="true" GroupName="basedon" Text="Weight Out Time"
                                                runat="server" />
                                            <asp:RadioButton ID="RadioButton2" GroupName="basedon" Text="ATA" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <br />
                                            <br />
                                            <%--<a href="Management.aspx" class="button" style="width: 60px;" title="Back"><span
                                            class="ui-button-text ">Back</span> </a>--%>
                                        </td>
                                        <td colspan="2" align="left">
                                            <br />
                                            <br />
                                            <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />
                                            <a href="javascript:ExcelReport();" class="button"><span class="ui-button-text" title="Download">Download</a>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </center>
                    </td>
                </tr>
            </table>

        </center>
        <br />
        <center>
            <%--  <asp:Label ID="reprocesslbl" runat="server" Text="" style="color: blue;font-size: 15px;"></asp:Label>--%>
            <span id="txtmessage" style="color: blue; font-size: 15px;"></span>
        </center>

        <br />
        <div id="fw_container">
            <input type="button" class="btn btn-primary" value="Reprocess" style="float: right; margin-bottom: 5px; background-color: #86b5d9; color: navy; font-weight: 500; cursor: pointer" title="Reprocess Job" onclick="Reprocessjob()" />
            <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
                <%=sb1.ToString()%>
            </table>
        </div>
        <input type="hidden" id="ddlshipto" runat="server" value="ALL" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="OSS Report" />
        <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
    <div id="dialog-message" title="Vehicle's Information" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="idlingpage" name="idlingpage" src="" frameborder="0" scrolling="auto"
            height="512" width="1198px" style="visibility: hidden; border: solid 1px #aac6ff;"
            onclick="return idlingpage_onclick()" />
    </div>
</body>
</html>
