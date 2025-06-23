<%@ Page Language="VB" AutoEventWireup="false" MaintainScrollPositionOnPostback="true"
    Inherits="YTLWebApplication.JournalReport2" EnableEventValidation="false" Codebehind="JournalReport2.aspx.vb" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Journal Report</title>
    <script type="text/javascript" src="jsfiles/jquery-1.8.2.min.js"></script>
    <link type="text/css" href="../StyleSheet.css" rel="stylesheet" />
    <style media="print" type="text/css">
        body
        {
            color: #000000;
            background: #ffffff;
            font-family: verdana,arial,sans-serif;
            font-size: 12pt;
        }
        #fcimg
        {
            display: none;
        }
      
    </style>
    <style type="text/css">
        .TOP10Style
        {
            position: fixed;
            left: 25%;
            right: 25%;
            top: 10%;
            border: 10px solid rgba(82, 82, 82, .7);
            -webkit-border-radius: 8px;
        }
    </style>
    <style type="text/css">
        .Hover:hover
        {
            text-decoration: underline;
        }
        .Hover
        {
            text-decoration: none;
        }
          .g1
{
    background-image: url(images/g.png);
    background-repeat: no-repeat;
    width: 16px;
    height: 16px;
    display:inline-table;
}
.p1
{
    background-image: url(images/p.png);
    background-repeat: no-repeat;
    width: 16px;
    height: 16px;
    display:inline-table;
}
.r1
{
    background-image: url(images/r.png);
    background-repeat: no-repeat;
    width: 13px;
    height: 13px;
    display:inline-table;
}
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script>
    <script type="text/javascript">
        function Sent() {
            alert('Email Sent.');
        }
        $(function () {
            $("#ImageButton1").click(function () {
                $(".img1").show();
            });
            $("#btnEmail").click(function () {
                $(".img1").hide();
            });
            $("#btnSendEmail").click(function () {
                $(".img2").show();
            });
        });
    </script>
    <script type="text/javascript" language="javascript">

        function mysubmit() {

            var bigindatetime = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value;
            var enddatetime = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value;

            var fdate = Date.parse(bigindatetime);
            var sdate = Date.parse(enddatetime);

            var diff = (sdate - fdate) * (1 / (1000 * 60 * 60 * 24));
            var days = parseInt(diff) + 1;
            if (days > 31) {
                alert("Sorry, you've selected " + days + " days.\nMaximum 7 days of report can only be shown.");
                return false;
            }
            else if (diff <= 0) {
                alert("Sorry, Date selection Error. Try again");
                return false;
            }
            return true;

        }
        function ShowCalendar(strTargetDateField, intLeft, intTop) {
            txtTargetDateField = strTargetDateField;

            var divTWCalendarobj = document.getElementById("divTWCalendar");
            divTWCalendarobj.style.visibility = 'visible';
            divTWCalendarobj.style.left = intLeft + "px";
            divTWCalendarobj.style.top = intTop + "px";
        }
        function fullscreen() {
            var colvalues = window.parent.document.getElementById("left").cols.split(",");
            var rowvalues = window.parent.document.getElementById("mainpage").rows.split(",");
            if (colvalues[0] > 0) {
                window.parent.document.getElementById("left").cols = "0,*";
                window.parent.document.getElementById("mainpage").rows = "0,*";
                document.getElementById("fcimg").src = "mapfiles/fullscreen2.gif";
            }
            else {
                window.parent.document.getElementById("left").cols = "230,*";
                window.parent.document.getElementById("mainpage").rows = "75,*";
                document.getElementById("fcimg").src = "mapfiles/fullscreen1.gif";
            }
        }
        function SHVehicleMovement() {
            rows = document.getElementById("GridView1").rows;

            if (document.getElementById("cbVehicleMovement").checked.toString() == "false") {
                for (i = 0; i < rows.length; i++) {
                    rows[i].cells[5].style.display = "none";
                    rows[i].cells[6].style.display = "none";
                    rows[i].cells[7].style.display = "none";
                }
            }
            else {
                for (i = 0; i < rows.length; i++) {
                    rows[i].cells[5].style.display = "";
                    rows[i].cells[6].style.display = "";
                    rows[i].cells[7].style.display = "";
                }

            }
        }
        function ShowGoogleMaps() {
            var plateno = document.forms(0).ddlpleate.value;
            if (plateno == "--Select Plate No--") {
                alert("Please select vehicle plate number");
                return false;
            }
            else {
                var bdt = document.getElementById("txtBeginDate").value + "%2520" + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value + ":00"
                var edt = document.getElementById("txtEndDate").value + "%2520" + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value + ":59"

                var googlemapsformobj = document.getElementById("googlemapsform");
                googlemapsformobj.action = "https://www.google.com/maps?q=http%3A%2F%2F202.71.100.82%2FAVLS3.2%2FShowVehicelHistoryInGoogleMaps.aspx%3Fplateno%3D" + plateno + "%26bdt%3D" + bdt + "%26edt%3D" + edt
                googlemapsformobj.submit();
            }

        }
        function ShowGoogleEarth() {
            var plateno = document.forms(0).ddlpleate.value;
            if (plateno == "--Select Plate No--") {
                alert("Please select vehicle plate number");
                return false;
            }
            else {
                var bdt = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value + ":00"
                var edt = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value + ":59"

                var googleearthformobj = document.getElementById("googleearthform");
                googleearthformobj.action = "ShowVehicleHistoryInGoogleEarth.aspx?plateno=" + plateno + "&bdt=" + bdt + "&edt=" + edt
                googleearthformobj.submit();
            }

        }
        function mouseover(x, y) {
            document.getElementById("mapimage").src = "images/maploading.gif";
            document.getElementById("mapimage").src = "GussmannMap.aspx?x=" + x + "&y=" + y;
        }
        function googlemouseover(x, y) {
            document.getElementById("mapimage").src = "images/maploading.gif";
            document.getElementById("mapimage").src = "https://mt0.google.com/mt?x=" + x + "&y=" + y + "&zoom=10";
            alert(document.getElementById("mapimage").src);
        }

    </script>
   
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <center>
        <form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
      
        <script type="text/javascript" language="javascript">            DrawCalendarLayout();
        </script>
        <div>
           <br />
             <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Journal Report</b>
            <br />
            <br />
            <table width="97%">
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White; font-size: 15px">&nbsp;Journal Report &nbsp;:</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 500px; border: solid 1px #3952F9;">
                                    <table style="width: 500px;">
                                        <tbody>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Begin Date</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                        id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />&nbsp;<a
                                                            href="javascript:ShowCalendar('txtBeginDate', 250, 260);" style="text-decoration: none;">
                                                            <img alt="Show calendar control" title="Show calendar control" height="14" src="images/Calendar.jpg"
                                                                width="19" style="border: solid 1px blue;" />
                                                        </a><b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                    <asp:DropDownList ID="ddlbh" runat="server" Width="40px" Font-Size="12px" Font-Names="verdana"
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
                                                    <asp:DropDownList ID="ddlbm" runat="server" Width="40px" Font-Size="12px" Font-Names="verdana"
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
                                                        id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" />&nbsp;<a
                                                            href="javascript:javascript:ShowCalendar('txtEndDate', 250, 260);" style="text-decoration: none;">
                                                            <img alt="Show calendar control" title="Show calendar control" height="14" src="images/Calendar.jpg"
                                                                width="19" style="border: solid 1px blue;" />
                                                        </a><b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                    <asp:DropDownList ID="ddleh" runat="server" Width="40px" Font-Size="12px" Font-Names="verdana"
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
                                                    <asp:DropDownList ID="ddlem" runat="server" Width="40px" Font-Size="12px" Font-Names="verdana"
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
                                                    <b style="color: #5f7afc">User Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlUsername" runat="server" Font-Names="verdana" Font-Size="12px"
                                                        Width="250px" AutoPostBack="True">
                                                        <asp:ListItem>--Select User Name--</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlUsername"
                                                        ErrorMessage="*" InitialValue="--Select User Name--" ToolTip="Please Select User Name"
                                                        ValidationGroup="s">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc">Group&nbsp;Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlGroupName" runat="server" Font-Names="verdana" Font-Size="12px"
                                                        Width="250px">
                                                        <asp:ListItem>--Group Name--</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlGroupName"
                                                        ErrorMessage="*" InitialValue="--Group Name--" ToolTip="Please Select Group Name"
                                                        ValidationGroup="s">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Records/Page</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="noofrecords" runat="server" Width="50px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False">
                                                        <asp:ListItem Selected="True">300</asp:ListItem>
                                                        <asp:ListItem>400</asp:ListItem>
                                                        <asp:ListItem>500</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <asp:Label ID="lblWatch" runat="server"></asp:Label>
                                                    <asp:Label ID="lblWatch0" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Show/Hide Columns</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:CheckBox ID="cbVehicleMovement" runat="server" Text="Vehicle Movement" AutoPostBack="True"
                                                        Checked="True" />
                                                    <asp:CheckBox ID="cbLocation" runat="server" AutoPostBack="True" Checked="True" Text="Location" />
                                                    <asp:CheckBox ID="cbSpeeding" runat="server" Text="Speeding" AutoPostBack="True" />
                                                    <br />
                                                    <asp:CheckBox ID="cbFuel" runat="server" Text="Fuel" AutoPostBack="True" Checked="True" />
                                                    <asp:CheckBox ID="cbRoute" runat="server" Text="Route" AutoPostBack="True" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                   
                                                </td>
                                                <td colspan="2" align="center">
                                                    <br />
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Submit_s.jpg"
                                                        ToolTip="Submit" ValidationGroup="s"></asp:ImageButton>&nbsp;&nbsp;
                                                    <asp:ImageButton ID="btnExport" runat="server" ImageUrl="images/saveEXCEL.jpg" />
                                                    <%--                                  <a href="javascript:ExcelReport();">
                                                            <img alt="Save to excel file" src="images/saveEXCEL.jpg" style="border: solid 0px blue;" /></a>--%>
                                                    <a href="javascript:print();">
                                                        <img alt="Print" src="images/print.jpg" style="border: solid 0px blue;" /></a>
                                                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="../images/email.png" Visible=false /><div>
                                                        <img class='img1' src="images/loading_icon.gif" style="display: none" />
                                                        <span class='img1' style="display: none">Loading...</span>
                                                    </div>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>
                       
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <table width="100%" style="font: verdana">
                            <tr>
                                <td align="center">
                                    <div id="ColorMessage" runat="server" style="margin-left: 5%" visible="false">
                                        <b>Route :</b>
                                        <%--onclick="SHVehicleMovement()"--%>
                                        <font color="#FA5858">Red - Bintang POI</font>, <font color="#04B404">Green - Public
                                            Geofence</font>, <font color="#8181F7">Blue - Private Geofence</font>, <font color="#000000">
                                                Grey - Normal Address</font>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <div style="font-family: Verdana; font-size: 11px;">
                                        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" PageSize="20" AutoGenerateColumns="False"
                                            HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                            HeaderStyle-Font-Bold="True" Font-Bold="False" Font-Overline="False" HeaderStyle-Height="22px"
                                            HeaderStyle-HorizontalAlign="Center" BorderColor="#F0F0F0" EnableModelValidation="True"
                                            ShowFooter="True">
                                            <HeaderStyle HorizontalAlign="Center" BackColor="#465AE8" Font-Bold="True" Font-Size="12px"
                                                ForeColor="White" Height="22px"></HeaderStyle>
                                            <PagerSettings PageButtonCount="5" />
                                            <PagerStyle Font-Bold="True" Font-Names="Verdana" Font-Size="Small" HorizontalAlign="Center"
                                                VerticalAlign="Middle" BackColor="White" Font-Italic="False" Font-Overline="False"
                                                Font-Strikeout="False" />
                                            <Columns>
                                                <asp:BoundField DataField="No" HeaderText="No">
                                                    <ItemStyle Width="5px" HorizontalAlign="center" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Plate No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("[Plate No]") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" SkinID="Plate No"
                                                            ToolTip="Descending"><span style="color:White">Plate No</span></asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <ItemStyle Width="80px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Group Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Groupname") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_groupname" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="Groupname" ToolTip="Descending">Group Name</asp:LinkButton>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Start">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("Start") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_Start" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="Start" ToolTip="Descending">Start</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <ItemStyle Width="130px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="End">
                                                    <FooterTemplate>
                                                        <asp:Label ID="Label13" runat="server" Font-Bold="True" Text="Total :"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_End" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="End" ToolTip="Descending">End</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("End") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="130px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Stop">
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblStop" runat="server" Font-Bold="True"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_Stop" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="Stop" ToolTip="Descending">Stop</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("Stop") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Idling">
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblIdling" runat="server" Font-Bold="True"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_Idling" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="Idling" ToolTip="Descending">Idling</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label6" runat="server" Text='<%# Bind("Idling") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Travelling">
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblTravelling" runat="server" Font-Bold="True"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_Travelling" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="Travelling" ToolTip="Descending">Travelling</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label7" runat="server" Text='<%# Bind("Travelling") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Idling Liter">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label16" runat="server" Text='<%# String.Format("{0:0.00}", Eval("IdlingLiter")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_IdlingLiter" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="IdlingLiter" ToolTip="Descending">Idling Liter</asp:LinkButton>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Idling Cost">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label17" runat="server" Text="RM "></asp:Label>
                                                        <asp:Label ID="Label13" runat="server" Text='<%# String.Format("{0:0.00}", Eval("idlingCost")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_IdlingCost" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="idlingCost" ToolTip="Descending">Idling Cost</asp:LinkButton>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Speed &gt; 90">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label8" runat="server" Text='<%# Bind("speed90") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblSpeed90" runat="server"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_speed90" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="speed90" ToolTip="Descending">Speed &gt; 90</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Speed &gt; 95">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label9" runat="server" Text='<%# Bind("speed95") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblSpeed95" runat="server"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_speed95" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="speed95" ToolTip="Descending">Speed &gt; 95</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Start Location1" HeaderText="Start Location" HtmlEncode="False">
                                                    <HeaderStyle Width="360px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="End Location1" HeaderText="End Location" HtmlEncode="False">
                                                    <HeaderStyle Width="360px" />
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Refuel">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label10" runat="server" Text='<%# String.Format("{0:0.00}", Eval("LiterRefuel")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblReFuel" runat="server"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_Refuel" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="LiterRefuel" ToolTip="Descending">Refuel</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" Width="133px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Fuel Consumption (L)">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label11" runat="server" Text='<%# String.Format("{0:0.00}", Eval("Fuel")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblFuel" runat="server"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_FuelConsumption" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="Fuel" ToolTip="Descending">Fuel Consumption (L)</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Odometer">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label12" runat="server" Text='<%# String.Format("{0:0.00}", Eval("Mileage")) %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblOdomerter" runat="server"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_Odometer" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="Mileage" ToolTip="Descending">Odometer</asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Odometer" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPTO" runat="server" Text='<%# Eval("PTO") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="Link_PTO" runat="server" ForeColor="White" OnClick="LinkButton1_Click"
                                                            SkinID="PTO" ToolTip="Descending">PTO</asp:LinkButton>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <tr style="width: 100%; background-color: #F4FA58; opacity: 0.9; filter: alpha(opacity=90);">
                                                            <td colspan="100%">
                                                                <div>
                                                                    <div style="width: 500px; float: left">
                                                                        <asp:Label ID="Label15" runat="server" Text='<%# "Route for " + Eval("[Plate No]") + " vehicle" %>'
                                                                            Font-Underline="False" Font-Size="14px" Font-Bold="True"></asp:Label></div>
                                                                </div>
                                                                <div style="clear: both">
                                                                    <asp:Label ID="Label14" runat="server" Text='<%# Bind("route") %>'></asp:Label>
                                                                    <asp:GridView ID="GridView2" runat="server" EnableModelValidation="True" Width="100%"
                                                                        AutoGenerateColumns="False" EmptyDataText="No Record...">
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="Route">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("address") %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="600px" />
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Location">
                                                                                <ItemTemplate>
                                                                                    <a class="Hover" style="color: Black" href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=<%# Eval("x") %>,<%# Eval("y") %>&amp;om=1&amp;t=k"'
                                                                                        target="_blank">(Google Map)</a> <a class="Hover" style="color: Black" href='https://lafargev2.avls.com.my/GoogleEarthMaps.aspx?x=<%# Eval("y") %> &amp;y=<%# Eval("x") %>'
                                                                                            target="_self">(Google Earth)</a>
                                                                                </ItemTemplate>
                                                                                <HeaderStyle Width="250px" />
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                            </asp:TemplateField>
                                                                            <asp:BoundField DataField="IdlingTime" HeaderText="Idling Min" HtmlEncode="False">
                                                                                <HeaderStyle Width="250px" />
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                            </asp:BoundField>
                                                                            <asp:BoundField DataField="StartAndEndDate" HeaderText="Idling Time" HtmlEncode="False">
                                                                                <HeaderStyle Width="250px" />
                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                            </asp:BoundField>
                                                                        </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle BackColor="darkseagreen" Font-Bold="True" ForeColor="Black" />
                                            <AlternatingRowStyle BackColor="Lavender" />
                                        </asp:GridView>
                                        <br />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="Top10Table" class="TOP10Style" runat="server" visible="false" style="width: 750px">
            <%--display: none;--%>
            <table width="750px" bgcolor="White">
                <tr>
                    <td>
                        <div class="floatleft">
                            <div class="floatleft">
                                <asp:Label ID="Label18" runat="server" Font-Names="verdana" Font-Size="14px" Text="Top 10"
                                    Font-Bold="True"></asp:Label>
                                &nbsp;&nbsp;
                                <asp:Label ID="Label19" runat="server" Text="Email to:" Font-Names="verdana" Font-Size="11px"></asp:Label>
                                <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail"
                                    ErrorMessage="*" ToolTip="Please Enter Email" ValidationGroup="a"></asp:RequiredFieldValidator>
                                <asp:Label ID="Label20" runat="server" Text="CC to:" Font-Names="verdana" Font-Size="11px"></asp:Label>
                                <asp:TextBox ID="txtCC" runat="server"></asp:TextBox>
                            </div>
                            <div class="floatleft" style="padding-left: 10px">
                                <img class='img2' src="images/loading_icon.gif" style="display: none" />
                                <%--             <span class='img2' style="display: none">Sending Email...</span>--%>
                            </div>
                        </div>
                        <div class="floatright">
                            <asp:ImageButton ID="btnSaveExcel" runat="server" ImageUrl="images/saveEXCEL.jpg" />
                            <asp:ImageButton ID="btnSendEmail" runat="server" ImageUrl="~/images/email.png" Style="height: 18px"
                                ValidationGroup="a" />
                            <asp:Button ID="btnClose" runat="server" Text="x" CssClass="button-red" Height="20px"
                                Width="20px" />
                            &nbsp;</div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="GridView4" runat="server" HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF"
                            HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True" Font-Bold="False"
                            Font-Overline="False" HeaderStyle-Height="22px" HeaderStyle-HorizontalAlign="Center"
                            BorderColor="#F0F0F0" AutoGenerateColumns="False" EnableModelValidation="True"
                            Font-Names="verdana" Font-Size="11px">
                            <HeaderStyle HorizontalAlign="Center" BackColor="#465AE8" Font-Bold="True" Font-Size="12px"
                                ForeColor="White" Height="22px"></HeaderStyle>
                            <Columns>
                                <asp:BoundField DataField="no" HeaderText="No" />
                                <asp:BoundField DataField="Plate No" HeaderText="Plate No">
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Idlling" HeaderText="Idling">
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Plate No2" HeaderText="Plate No">
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="speed90" HeaderText="Speed &gt; 90">
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Plate No3" HeaderText="Plate No">
                                    <FooterStyle Width="70px" />
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="speed95" HeaderText="Speed &gt; 95">
                                    <HeaderStyle Width="80px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Plate No4" HeaderText="Plate No">
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Travelling" HeaderText="Travelling">
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Plate No5" HeaderText="Plate No">
                                    <HeaderStyle Width="70px" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Odometer">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# String.Format("{0:0.00}", Eval("Mileage")) %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="70px" />
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="darkseagreen" Font-Bold="True" ForeColor="Black" />
                            <AlternatingRowStyle BackColor="Lavender" />
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
        </form>
    </center>
</body>
</html>
