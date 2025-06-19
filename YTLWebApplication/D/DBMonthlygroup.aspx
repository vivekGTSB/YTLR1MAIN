<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.DBMonthlygroup" Codebehind="DBMonthlygroup.aspx.vb" %>

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
    <form id="Form1" method="post" runat="server">
   
    <center>
        <div>
            <br />
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
                                                        Width="200px" OnSelectedIndexChanged ="ddlUsername_SelectedIndexChanged" AutoPostBack ="true"  >
                                                        <asp:ListItem>--ALL User--</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc">Group Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlGroupName" runat="server" Font-Names="verdana" Font-Size="12px"
                                                        Width="200px">
                                                        <asp:ListItem Value ="0">--ALL Groups--</asp:ListItem>
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
                                                <asp:BoundField DataField="Speeding Freequency" HeaderText="Speeding Frequency" HtmlEncode="False">
                                                    <ItemStyle Width="100px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Harsh Braking Frequency" HeaderText="Harsh Braking Frequency" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Excessive Idle" HeaderText="Excessive Idle" HtmlEncode="False">
                                                    <ItemStyle Width="80px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                                
                                                <asp:BoundField DataField="Continuous Driving > 4 Hrs" HeaderText="Continuous Driving &gt; 4 Hrs"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Continuous Driving Hours" HeaderText="Total Continuous Driving Hours"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Driving Hour" HeaderText="Total Driving Hour" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Working Hour" HeaderText="Total Working Hour" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Frequency of > 14 Hrs Work" HeaderText="Frequency of &gt; 14 Hrs Work"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Frequency of > 10 Hrs Drive" HeaderText="Frequency of &gt; 10 Hrs Drive"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Distance Travel" HeaderText="Distance Travel" HtmlEncode="False"
                                                    Visible="true">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                 <asp:BoundField DataField="Mid-Night Count" HeaderText="Mid-Night Count" HtmlEncode="False">
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
                         <asp:Label ID="Label5" runat="server" Text="For the System to generate the report, the following will be the definition"
                            Font-Size="11px" Font-Names="verdana" ForeColor="Blue" Enabled="false"></asp:Label><br />
                        <asp:Label ID="Label6" runat="server" Text="1. Working Hours- The time counted the moment the engine started (regardless there is speed or not) until it is turned off."
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />
                        <asp:Label ID="Label7" runat="server" Text="2. Driving Hours- The time counted the moment the vehicles started to move (regardless at what speed)"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />
                        <asp:Label ID="Label8" runat="server" Text="3. Idling Hours- The time counted the moment the engine started and no speed detected"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />
                        <asp:Label ID="Label10" runat="server" Text="4. Overspeeding- The vehicle speed more than 80 km/h"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />
                        <asp:Label ID="Label11" runat="server" Text="5. Harsh braking- Sudden drop of speed 15 km/h in 1 second"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />
                        <asp:Label ID="Label13" runat="server" Text="6. Speeding Frequency- Number of data occurance with speed more than 80KM/h"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label><br />
                        <asp:Label ID="Label14" runat="server" Text="7. Maximum Speed- Highest Vehicles speed register within the time frame"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label><br />
                        <asp:Label ID="Label15" runat="server" Text="8. Longest Overspeed Duration- The longest continuous period vehicles speed > 80KM/h"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label><br />
                              <asp:Label ID="Label59" runat="server" Text="9. Mid Night Count - Vehicle travelling duration between 2AM to 5AM."
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label></br>
                            <asp:Label ID="Label94" runat="server" Text="10. Continuous Driving > 4 Hrs - Number of counts that vehicles is in driving continuous more than 4 Hrs"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />

                            <asp:Label ID="Label92" runat="server" Text="11. Frequency of > 14 Hrs Work - Number of counts from the moment engine started (regardless there is speed or not) until it is turned off is more than 14 Hrs"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />

                            <asp:Label ID="Label93" runat="server" Text="12. Frequency of > 10 Hrs Work - Number of counts from the moment engine started (regardless there is speed or not) until it is turned off is more than 10 Hrs"
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false"></asp:Label><br />
                        <br />
                    </td>
                </tr>
            </table>
        </div>
    </center>
   
    <input id="values" type="hidden" runat="server" value="" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Driver Behavior Report (Monthly)" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
