<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.OSSReportYTL" Codebehind="OSSReportYTL.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Fleet Monitoring Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        .dataTables_info
        {
            width: 25%;
            float: left;
        }
.textdecor{
    text-decoration: underline;
    color: blue;
cursor: pointer;
}
    </style>
    <style>
        .hideen {
            visibility: hidden;
            /* margin-top:-80px;*/
        }

        .opacity {
            /* IE 8 */
            -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=30)";
            /* IE 5-7 */
            filter: alpha(opacity=30);
            /* Netscape */
            -moz-opacity: 0.3;
            /* Safari 1.x */
            -khtml-opacity: 0.3;
            opacity: 0.3;
            pointer-events: none;
        }

        #floatingBarsG {
            position: fixed;
            top: 50%;
            left: 50%;
            /*width: 100%;
            height: 100%;

            left: 25%;*/
        }
    </style>



    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">

        function getWindowWidth() {
            if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement &&

                document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth;
        }
        function getWindowHeight() {
            if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement &&

                document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight;
        }

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
            document.getElementById("mappage").src = "GMap.aspx?bdt=" + intime + "&edt=" + outtime + "&plateno=" + plateno + "&scode=1&sf=0&r=" +

                Math.random();
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
        var ec =<%=ec %>;
        function ExcelReport() {
            var bigindatetime = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById

                ("ddlbm").value;
            var enddatetime = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById

                ("ddlem").value;
            $("#rperoid").val(bigindatetime + " - " + enddatetime);
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();

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
        var oTable;
        $(document).ready(function () {

            fnFeaturesInit()
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 25,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
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
                    { "bVisible": true, "bSortable": false, "aTargets": [0] },
                    { "bVisible": true, "sWidth": "300px", "aTargets": [13] } 



                ]
            });
            $("#Div1").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                buttons: {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
            });
            $("#dialog-form").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 500,
                minHeight: 200,
                height: 200,
                buttons: {
                    Update: function () {
                        UpdateData();
                        $(this).dialog("close");
                    },
                    Close: function () {
                        $(this).dialog("close");
                    }
                }
            });

        });

        function UpdateData() {

            var str = "updatevehiclestatus.aspx?s=OSS&pno=" + $("#plate_no").text() + "&u=" + $("#uid").val() + "&re=" + $("#txtRemarks").val();
            $.get(str, function (data) {
                alertbox("Updated Successfully");
                $("#dialog-message").dialog("close");
                Submit();
            });

        }
        function alertbox(message) {
            document.getElementById("displayp").innerHTML = message;
            $("#Div1").dialog("open");
        }
        function openPopup(plateno, remarks) {
            $("#plate_no").text(plateno);
            var nremarks = unescape(remarks);
            var str = nremarks.split("@");
            var lremark = "";
            if (str.length > 2) {
                for (let i = 0; i < str.length - 1; i++) {
                    if (lremark == "") {
                        lremark = str[i];
                    }
                    else {
                        lremark += "@" + str[i];
                    }
                }
            }
            else {
                lremark = str[0];
            }
            $.ajax({
                type: "POST",
                url: "OSSReportYTL.aspx/GetRecentRemarks",
                data: '{plateno: \"' + plateno  + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response.aaData;
                    $("#recentcmts tbody").html("");
                    var innterHrml = "";
                    for (var i = 0; i < json.length; i++) {
                        innterHrml += "<tr><td>" + json[i][0] + "</td><td>" + json[i][1] + "</td><td>" + json[i][2] + "</td><td>" + json[i][3] + "</td></tr>";
                    }
                    $("#recentcmts tbody").html(innterHrml);
                },
                failure: function (response) {
                    alert("Error");
                }
            });
           
            $('#txtRemarks').val(lremark);
            $("#dialog-form").dialog("open");

        }
    </script>
    
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
        <input type ="hidden" id="uid" runat ="server" />
    <center>
        <div>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Fleet Monitoring Report</b>
            <br />
            <br />
        </div>
        <table>
            <tr>
                <td align="center">
                    <table style="font-family: Verdana; font-size: 11px;">
                        <tr>
                            <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                <b style="color: White;">&nbsp;Fleet Monitoring Report&nbsp;:</b>
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
                                                <b style="color: #465AE8;">End Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
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
                                                <b style="color: #465AE8;">Type</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <select id="ddltransporter" class="form-control" name="ddltransporter" onchange="RefreshUser()" style 

="width :255px">
                                    <option value="ALL">ALL</option>
                                    <option value="1" selected ="selected" >Internal</option>
                                    <option value="2">External</option>
                                </select>
                                            </td>
                                        </tr>
                                          <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Transporter</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <select id="ddltransportername" class="form-control" name="ddltransportername" style ="width :255px" runat 

="server" ></select>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Merge Trucks</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input type ="checkbox" id="chkmerge" />
                                            </td>
                                        </tr>

                                         
                                        <tr>
                                            <td align="center">
                                                <br />
                                            </td>
                                            <td colspan="2" align="center">
                                                <br />
                                                 <a href="javascript:Submit();" class="button"><span class="ui-button-text"
                                                        title="Submit">Submit</a>
                                                 <a href="javascript:ExcelReport();" class="button"><span class="ui-button-text" title="Download">
                                                    Download</a> 
                                               
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
       <table>
            <tr>
                <td colspan="3">
                    <div style="width: 1190px;">
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
                                <thead>
                                    <tr>
                                        <th>SNo</th>
                                         <th style="width: 80px">Plateno</th>
                                        <th style="width: 50px">Code</th>
                                        <th style="width: 50px">Bases</th>
                                        <th style="width: 50px">Trips</th>
                                        <th>Last Trip</th>
                                        <th>Day</th>
                                        <th>Last Customer</th>
                                        <th>Location</th>
                                        <th>Current Lattest Loc</th>
                                        <th>Dist From Town</th>
                                        <th>Dist From Plant</th>
                                        <th>Dist From Base Plant</th>
                                        <th>Remarks</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                    </div>
                </td>
            </tr>
        </table>
    </center>
     <div id="floatingBarsG" class="hideen" style="z-index: 999; opacity: 2;">
            <center>
            <div class="row">

            <div class ="col-md-12">
                  <div class="box box-primary">
                <div class="box-header">
                  <h3 class="box-title">Loading</h3>
                </div>
               <!-- /.box-body -->
                <!-- Loading (remove the following to stop the loading)-->
               <%-- <div class="overlay">
                  <i class="fa fa-refresh fa-spin"></i>
                </div>--%>
                <!-- end loading -->
              </div><!-- /.box -->
            </div>
                   </div>
                </center>
        </div>
        <div id="dialog-form" title="Update Vehicle's Status" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
            <table border="0" cellpadding="1" cellspacing="1" style="width: 500px; font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida 

Grande,Helvetica,Arial,sans-serif">
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Plate No</b> &nbsp;:&nbsp;&nbsp;
                    </td>
                    <td>
                        <span id="plate_no"></span>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Latest Remarks</b>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtRemarks" TextMode="MultiLine" Width="150px" Height="30px" />
                    </td>
                </tr>
                <tr>
                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="recentcmts" style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
                         <thead><tr><th class="ui-state-default">Sno</th><th class="ui-state-default">Timestamp</th><th class="ui-state-default">Username</th><th class="ui-state-default">Remarks</th></tr></thead>
                        <tbody>
                            <tr>
                               <td colspan="4">No Recenet Remarks</td> 
                            </tr>
                        </tbody>
                    </table>
                </tr>
            </table>
        </div>
    </form>
     <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Fleet Monitoring Report" />
    <input type="hidden" id="plateno" name="plateno" value="" />
         <input type="hidden" id="reporttype" name="reporttype" value="1" />
         <input type="hidden" id="rperoid" name="rperoid" value="" />
    </form>
    <div id="Div1" title="Alert">
                <p id="displayp">
                    <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;"></span>
                </p>
            </div>
    

    <script type="text/javascript" >
        var ec =<%=ec %>;
        $(function () {
            $("#floatingBarsG").addClass("hideen");
            $("body").removeClass("opacity");

        });
        function Submit() {
            $("#floatingBarsG").removeClass("hideen");
            $("body").addClass("opacity");
            var bdt = $("#txtBeginDate").val() + " " + $("#ddlbh").val() + ":" + $("#ddlbm").val() + ":00"
            var edt = $("#txtEndDate").val() + " " + $("#ddleh").val() + ":" + $("#ddlem").val() + ":00"
            $.ajax({
                type: "POST",
                url: "OSSReportYTL.aspx/GetData",
                data: '{fromd: \"' + bdt + '\",tod:\"' + edt + '\",type:\"' + $("#ddltransporter").val() + '\",username:\"' + $("#ddltransportername").val() + '\",merge:\"' + $("#chkmerge").prop("checked") + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response.aaData;
                    table = oTable.dataTable();
                    table._fnProcessingDisplay(true);
                    oSettings = table.fnSettings();
                    table.fnClearTable(this);
                    for (var i = 0; i < json.length; i++) {
                        json[i][1] = "<span class='textdecor' onclick=openPopup(\'" + escape(json[i][1]) + "\',\'" + escape(json[i][12]) + "\')>" + json[i][1] + "</span>";
                        table.oApi._fnAddData(oSettings, json[i]);
                    }
                    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                    table._fnProcessingDisplay(false);
                    table.fnDraw();
                    ec = true;
                    $("#floatingBarsG").addClass("hideen");
                    $("body").removeClass("opacity");
                },
                failure: function (response) {
                    alert("Error");
                }
            });
        }

        function RefreshUser() {
            $.ajax({
                type: "POST",
                url: "OSSReportYTL.aspx/GetUsers",
                data: '{type:\"' + $("#ddltransporter").val() + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var json = response.aaData;
                    $("#ddltransportername").html("");
                    $("#ddltransportername").html("<option value='ALL'>ALL</option>");
                    for (var i = 0; i < json.length - 1; i++) {
                        $("#ddltransportername").append("<option value=" + json[i][0] + ">" + json[i][1] + "</option>");
                    }
                },
                failure: function (response) {
                    alert("Error");
                }
            });
        }
    </script>
   
</body>
</html>