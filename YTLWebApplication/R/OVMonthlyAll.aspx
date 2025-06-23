<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.Lafarge_OVMonthlyAll" Codebehind="OVMonthlyAll.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
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
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
    <script type="text/javascript" src="jsfiles/balloontip.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script>
    <script type="text/javascript" language="javascript">
    var ec=<%=ec %>;

        function ExcelReport() {
            if (ec == true) {
              

                var excelformobj = document.getElementById("excelform");
                excelformobj.submit();
            }
            else {
                alert("First click submit button");
            }
        }
        function mysubmit() {
            var plateno = document.getElementById("ddlVehicle").value;
            if (plateno == "--Select User Name--") {
                alert("Please select user name and vehicle plate number");
                return false;
            }
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
            if (days > 7) {
                return confirm("You selected " + days + " days of data.So it will take more time to execute.\nAre you sure you want to proceed ? ");
            }
            return true;

        }

        function pmouseover(sensordata) {
            document.getElementById("balloon2").innerHTML = "CP position being opened : " + sensordata;
            document.getElementById("balloon2").style.backgroundColor = "#FFE5EC";
        }


        function ShowCalendar(strTargetDateField, intLeft, intTop) {
            txtTargetDateField = strTargetDateField;

            var divTWCalendarobj = document.getElementById("divTWCalendar");
            divTWCalendarobj.style.visibility = 'visible';
            divTWCalendarobj.style.left = intLeft + "px";
            divTWCalendarobj.style.top = intTop + "px";
            selecteddate(txtTargetDateField);
        }
      
        function mouseover(x, y) {
            document.getElementById("mapimage").src = "images/maploading.gif";
            document.getElementById("mapimage").src = "GussmannMap.aspx?x=" + x + "&y=" + y;
        }
    </script>
    <script type="text/javascript">

        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-32500429-1']);
        _gaq.push(['_setDomainName', 'avls.com.my']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'https://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();

    </script>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <%--<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 15px;">--%>
    <form id="Form1" method="post" runat="server">
    <script type="text/javascript" language="javascript">        DrawCalendarLayout();</script>
    <center>
        <div>
            <br />
            <!--<img alt="Skid Tank Analysis Report & Chart" src="images/ar.jpg" />-->
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Driver Behavior Report
                (Monthly)</b>
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Driver Behavior Report(Monthly)</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 420px; border: solid 1px #3952F9;">
                                    <table style="width: 420px;">
                                        <tbody>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc">Month</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="DropDownList1" runat="server" Font-Names="verdana" Font-Size="12px"
                                                        Width="200px">
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
                                                        Width="200px">
                                                        <asp:ListItem>--ALL User--</asp:ListItem>
                                                    </asp:DropDownList>
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
                                                    <asp:DropDownList ID="noofrecords" runat="server" Width="80px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False">
                                                        <asp:ListItem Selected="True" Value="500">500</asp:ListItem>
                                                        <asp:ListItem Value="750">750</asp:ListItem>
                                                        <asp:ListItem Value="1000">1000</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                </td>
                                                <td colspan="2" align="left">
                                                    <br />
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Submit_s.jpg"
                                                        ToolTip="Submit"></asp:ImageButton>&nbsp;&nbsp; <a href="javascript:ExcelReport();">
                                                            <img alt="Save to Excel file" title="Save to Excel file" src="images/saveExcel.jpg"
                                                                style="border: solid 0px blue;" /></a>&nbsp;&nbsp;
                                                    <!-- <a href="javascript:print();"><img alt="Print" src="images/print.jpg" style="border: solid 0px blue;" /></a>-->
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr align="center">
                    <td>
                        <asp:Label ID="Label9" runat="server" Text="Data not being process, Please Contact Gussmann For Detail"
                            Font-Size="12px" Font-Names="verdana" Visible="false" ForeColor="Red"></asp:Label>
                        <br />
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Size="12px" Font-Names="verdana"></asp:Label>
                        <asp:Label ID="Label2" runat="server" Text="" Font-Size="12px" Font-Names="verdana"></asp:Label>
                        <asp:Label ID="Label4" runat="server" Text="" Font-Size="12px" Font-Names="verdana"></asp:Label>
                        <asp:Label ID="Label3" runat="server" Text="" Font-Size="12px" Font-Names="verdana"></asp:Label>
                        <table>
                            <tr>
                                <td align="center" colspan="2">
                                    <div style="font-family: Verdana; font-size: 11px;">
                                        <br />
                                        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" Width="100%" AutoGenerateColumns="False"
                                            HeaderStyle-Font-Size="10px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                            HeaderStyle-Font-Bold="True" Font-Bold="False" Font-Overline="False" EnableViewState="False"
                                            HeaderStyle-Height="17px" HeaderStyle-HorizontalAlign="Center" BorderColor="#F0F0F0">
                                            <PagerSettings PageButtonCount="5" />
                                            <PagerStyle Font-Bold="True" Font-Names="Verdana" Font-Size="Small" HorizontalAlign="Center"
                                                VerticalAlign="Middle" BackColor="White" Font-Italic="False" Font-Overline="False"
                                                Font-Strikeout="False" />
                                            <Columns>
                                                <asp:BoundField DataField="No" HeaderText="No">
                                                    <ItemStyle Width="2px" HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Vehicle No" HeaderText="Vehicle No" HtmlEncode="False">
                                                    <ItemStyle Width="30px" HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Overspeeding" HeaderText="Speeding Frequency" HtmlEncode="False">
                                                    <ItemStyle Width="100px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Harsh Brake" HeaderText="Harsh Braking Frequency" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Harsh acc" HeaderText="Harsh Acc" HtmlEncode="False" Visible="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Unauth Stop" HeaderText="Unauth Stop" HtmlEncode="False"
                                                    Visible="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Unauth Road" HeaderText="Unauth Road" HtmlEncode="False"
                                                    Visible="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Banned Hour" HeaderText="Banned Hours" HtmlEncode="False"
                                                    Visible="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Idling" HeaderText="Excessive Idle" HtmlEncode="False">
                                                    <ItemStyle Width="80px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                                
                                                <asp:BoundField DataField="Cont Drive" HeaderText="Continuous Driving &gt; 2 Hrs"
                                                    HtmlEncode="False" Visible="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Drive Hour" HeaderText="Continuous Driving &gt; 4 Hrs"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Work Hour" HeaderText="Total Continuous Driving Hours"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Violation" HeaderText="Total Violations" HtmlEncode="False"
                                                    Visible="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Drive Hour" HeaderText="Total Driving Hour" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Work Hour" HeaderText="Total Working Hour" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="more14Work" HeaderText="Frequency of &gt; 14 Hrs Work"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="more10Drive" HeaderText="Frequency of &gt; 10 Hrs Drive"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Distance Travel" HeaderText="Distance Travel" HtmlEncode="False"
                                                    Visible="true">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                 <asp:BoundField DataField="midnightcount" HeaderText="Mid-Night Count" HtmlEncode="False">
                                                    <ItemStyle Width="80px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                            </Columns>
                                            <AlternatingRowStyle BackColor="White" />
                                            <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="10px" ForeColor="White"
                                                Height="17px" HorizontalAlign="Center" />
                                        </asp:GridView>
                                        <% If show = True Then%>
                                        <center>
                                            <label id="pages" style="font-family: Verdana; font-size: 11px; font-weight: bold;">
                                                Pages</label></center>
                                        <%End If%>
                                        <br />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="Label7" runat="server" Text="For the System to generate the report, the following will be the definition"
                            Font-Size="11px" Font-Names="verdana" ForeColor="Blue" Visible="false"></asp:Label>
                        <br />
                        <asp:Label ID="Label5" runat="server" Text="*Harsh Braking- Sudden Drop of Speed 15KM/h In one Second"
                            Font-Size="11px" Font-Names="verdana" ForeColor="red" Visible="false"></asp:Label><br />
                        <asp:Label ID="Label6" runat="server" Text="*Continous Driving > 4Hrs- 1 Count = 4 Hours Continuous Driving without stop for 30minutes rest"
                            Font-Size="11px" Font-Names="verdana" ForeColor="red" Visible="false"></asp:Label>
                        <br />
                        <asp:Label ID="Label8" runat="server" Text="*Frequency of > 10Hrs Driving- " Font-Size="12px"
                            Font-Names="verdana" ForeColor="red" Visible="false"></asp:Label>
                        <br />
                    </td>
                </tr>
            </table>
        </div>
    </center>
    <div id="balloon1" class="balloonstyle" style="width: 258px; vertical-align: middle;">
        <img id="mapimage" src="images/maploading.gif" alt="" style="border: 1px solid silver;
            width: 256px; height: 256px; vertical-align: middle;" />
    </div>
    <div id="balloon2" class="balloonstyle" style="width: 500px; vertical-align: middle;">
    </div>
    <input id="values" type="hidden" runat="server" value="" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Vehicles Violation Summary Report" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
