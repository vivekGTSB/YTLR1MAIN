<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OssDayReport.aspx.vb" Inherits="YTLWebApplication.OssDayReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Daily Truck Performance Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";

        .dataTables_info {
            width: 25%;
            float: left;
        }
    </style>

    <style>
        .rightalign {
            text-align: right;
        }
      .display td{
    border: #b0e1d1 1px solid;
      }
      .ui-state-default{
          white-space :nowrap ;
      }
    </style>


    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">

        function mysubmit() {

            return true;

        }
        function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            document.getElementById("title").value = "Daily Truck Performance Report : " + document.getElementById("ddlPlants").options[document.getElementById("ddlPlants").selectedIndex].text;

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

        var isd =<%=isD  %>;

        $(document).ready(function () {
            fnFeaturesInit()
            var DefaultSortRow = 4;

            if (isd == true) {
                DefaultSortRow = 8;
            }
            //$('#examples').dataTable({
            //    "bJQueryUI": true,
            //    "sPaginationType": "full_numbers",
            //    "fnDrawCallback": function (oSettings) {
            //    },
            //    "aoColumnDefs": [
            //        { "bVisible": true, "bSortable": false, "aTargets": [0] },
            //        { "bVisible": true, "bSortable": false, "aTargets": [1] },
            //        { "bVisible": true, "bSortable": false, "sClass": "rightalign", "aTargets": [2] },
            //        { "bVisible": true, "bSortable": false, "sClass": "rightalign", "aTargets": [3] },
            //        { "bVisible": true, "bSortable": false, "sClass": "rightalign", "aTargets": [4] },
            //        { "bVisible": true, "bSortable": false, "sClass": "rightalign", "aTargets": [5] }
            //    ]
            //});
            //$('#examples1').dataTable({
            //    "bJQueryUI": true,
            //    "sPaginationType": "full_numbers",
            //    "fnDrawCallback": function (oSettings) {
            //    }
            //});
            //$('#examples2').dataTable({
            //    "bJQueryUI": true,
            //    "sPaginationType": "full_numbers",
            //    "fnDrawCallback": function (oSettings) {
            //    }
            //});
            $("#txtBeginDate").datepicker({
                dateFormat: 'MM', minDate: new Date(2022, 00, 01), maxDate: +0, changeMonth: true, changeYear: true
            });
            $("#txtEndDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2022, 00, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
            });
        });


    </script>

</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
        <center>
            <div>
                <br />
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Daily Truck Performance Report</b>
                <br />
                <br />
            </div>
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Daily Truck Performance Report&nbsp;:</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 450px; border: solid 1px #5B7C97;">
                                    <table style="width: 450px;">
                                        <tbody>
                                                                                         <tr>
     <td align="left">
         <b style="color: #465AE8;">Year</b>
     </td>
     <td>
         <b style="color: #465AE8;">:</b>
     </td>
     <td align="left" style="width: 326px">
         <asp:DropDownList ID="ddlyear" runat="server" Width="248px">
             
         </asp:DropDownList>

     </td>
 </tr>
                                                                                                                                    <tr>
    <td align="left">
        <b style="color: #465AE8;">Month</b>
    </td>
    <td>
        <b style="color: #465AE8;">:</b>
    </td>
    <td align="left" style="width: 326px">
        <asp:DropDownList ID="ddlmonth" runat="server" Width="248px">
            <asp:ListItem Value ="1">January</asp:ListItem>
            <asp:ListItem Value ="2">February</asp:ListItem>
            <asp:ListItem Value ="3">March</asp:ListItem>
            <asp:ListItem Value ="4">April</asp:ListItem>
            <asp:ListItem Value ="5">May</asp:ListItem>
            <asp:ListItem Value ="6">June</asp:ListItem>
            <asp:ListItem Value ="7">July</asp:ListItem>
            <asp:ListItem Value ="8">August</asp:ListItem>
            <asp:ListItem Value ="9">September</asp:ListItem>
            <asp:ListItem Value ="10">October</asp:ListItem>
            <asp:ListItem Value ="11">November</asp:ListItem>
            <asp:ListItem Value ="12">December</asp:ListItem>
        </asp:DropDownList>

    </td>
</tr>


                                            <tr style ="display :none ">
                                                <td align="left">
                                                    <b style="color: #5f7afc;">From Date</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <input style="width: 80px;" type="text" value="<%=strBeginDate%>" id="txtBeginDate"
                                                        runat="server" name="txtBeginDate" enableviewstate="false" readonly="readonly" />
                                                    <asp:DropDownList ID="ddlbh" runat="server" Width="50px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False" >
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
                                                    <asp:DropDownList ID="ddlbm" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False" >
                                                        <asp:ListItem Value="00" Selected ="True">00</asp:ListItem>
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
                                            <tr style="display :none">
    <td align="left">
        <b style="color: #5f7afc;">To Date</b>
    </td>
    <td>
        <b style="color: #5f7afc;">:</b>
    </td>
    <td align="left">
        <input style="width: 80px;" type="text" value="<%=strBeginDate%>" id="txtEndDate"
            runat="server" name="txtEndDate" enableviewstate="false" readonly="readonly" />
        <asp:DropDownList ID="ddlbh1" runat="server" Width="50px" Font-Size="12px" Font-Names="verdana"
            EnableViewState="False" >
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
        <asp:DropDownList ID="ddlbm1" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
            EnableViewState="False" >
            <asp:ListItem Value="00" Selected ="True">00</asp:ListItem>
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
                                                    <b style="color: #465AE8;">Username</b>
                                                </td>
                                                <td>
                                                    <b style="color: #465AE8;">:</b>
                                                </td>
                                                <td align="left" style="width: 326px">
                                                    <asp:DropDownList ID="ddlPlants" runat="server" Width="248px">
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
                                                    <a href="javascript:ExcelReport();" class="button">
                                                        <span class="ui-button-text" title="Download">Download</span></a>
                                                     <a href="javascript:print();" class="button"><span class="ui-button-text" title="Print">Print</span></a>
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

        </center>

        <center>
            <center><b>Report Generated On : <label runat ="server" id="reportonlbl"></label></b></center>
            <br />
            <br />

            <div>
                <%=sb1.ToString()%>
            </div>

            <br />
           

        </center>

    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Daily Truck Performance Report" />
        <input type="hidden" id="rperoid" name="rperoid" value="" />
    </form>


</body>
</html>
