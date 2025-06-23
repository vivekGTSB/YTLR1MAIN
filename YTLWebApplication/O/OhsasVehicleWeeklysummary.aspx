<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.OhsasVehicleWeeklysummary" Codebehind="OhsasVehicleWeeklysummary.aspx.vb" %>

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
    <link type="text/css" href="cssfiles/ui-lightness/jquery-ui-1.8.24.custom.css" rel="Stylesheet" />
    <script type="text/javascript" src="jsfiles/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui-1.8.24.custom.min.js"></script>
 
<link href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/css/bootstrap.min.css"
    rel="stylesheet" type="text/css" />
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/js/bootstrap.min.js"></script>
    <link href="https://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css"
        rel="stylesheet" type="text/css" />
    <script src="https://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js"
        type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
    var ec=<%=ec %>;
        function ExcelReport() {
            if (ec == true) {
                var plateno = document.getElementById("ddlweek").value;

                document.getElementById("plateno").value = plateno;

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
         $(function() {
         $( ".datepick" ).datepicker({ dateFormat: "yy/mm/dd", minDate: -365, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
	     $( ".datepick" ).datepicker( "option", "showAnim", "slide" );
	    $('[id*=ddlVehicle]').multiselect({
            includeSelectAllOption: true
        });
   
	});
    </script>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="Form1" method="post" runat="server">
    <%-- <div style="position: absolute; top: 0px; left: 0px; width: 15px; height: 15px; font-size: 5px;">
        <img id="fcimg" src="mapfiles/fullscreen1.gif" alt="Full Screen" title="Full Screen"
            onclick="javascript:fullscreen();" />--%>
    </div>
    <center>
        <div>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Driver Behavior Report
                (Weekly)</b>
            <br />
            <br />
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Driver Behavior Report (Daily) </b>
                                </td>
                            </tr>
                            <tr>
                                <td class="tableBorder">
                                    <table style="width: 420px;">
                                        <tbody>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">From Year</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                       <asp:DropDownList ID="DropDownList1" runat="server" Font-Names="verdana" Font-Size="12px"
                                                        Width="200px" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                              <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Week</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                       <asp:DropDownList ID="ddlweek" runat="server" Font-Names="verdana" Font-Size="12px"
                                                        Width="200px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                           <%-- <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc">Group By</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:RadioButton ID="radDate" runat="server" Checked="True" GroupName="GroupBy" Text="Date" />
                                                    <asp:RadioButton ID="radPlateno" runat="server" GroupName="GroupBy" Text="Plate No" />
                                                </td>
                                            </tr>--%>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc">User Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlUsername" runat="server" CssClass="cboAlign FontText" AutoPostBack="True">
                                                        <asp:ListItem>--Select User Name--</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                           <%-- <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Plate No</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="cboAlign FontText">
                                                        <asp:ListItem>--All Vehicle--</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>--%>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Plate No</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:ListBox ID='ddlVehicle' runat='server' SelectionMode='Multiple'>
                                                    </asp:ListBox>
                                                        
                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                </td>
                                                <td colspan="2" align="left">
                                                    <br />
                                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Submit_s.jpg"
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
                        <%-- <p style="font-family: Verdana; font-size: 11px; color: #5373a2;">
                            Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.
                        </p>--%>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <br />
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="" Font-Size="12px" Font-Names="verdana"
                            Visible="false"></asp:Label>
                        <asp:Label ID="Label2" runat="server" Text="" Font-Size="12px" Font-Names="verdana"
                            Visible="false"></asp:Label>
                        <asp:Label ID="Label4" runat="server" Text="" Font-Size="12px" Font-Names="verdana"
                            Visible="false"></asp:Label>
                        <asp:Label ID="Label3" runat="server" Text="" Font-Size="12px" Font-Names="verdana"
                            Visible="false"></asp:Label>
                        <table>
                            <tr>
                                <td align="center" colspan="2">
                                    <div style="font-family: Verdana; font-size: 10px;">
                                        <br />
                                        <asp:GridView ID="GridView1" runat="server" AllowPaging="false" Width="100%" AutoGenerateColumns="False"
                                            HeaderStyle-Font-Size="10px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                            HeaderStyle-Font-Bold="True" Font-Bold="False" Font-Overline="False" EnableViewState="False"
                                            HeaderStyle-Height="17px" HeaderStyle-HorizontalAlign="Left" BorderColor="#F0F0F0"
                                            CssClass="GridText">
                                            <PagerSettings PageButtonCount="5" />
                                            <PagerStyle Font-Bold="True" Font-Names="Verdana" Font-Size="Small" HorizontalAlign="Center"
                                                VerticalAlign="Middle" BackColor="White" Font-Italic="False" Font-Overline="False"
                                                Font-Strikeout="False" />
                                            <Columns>
                                                <asp:BoundField DataField="No" HeaderText="No">
                                                    <ItemStyle Width="2px" HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Vehicle No" HeaderText="Vehicle No" HtmlEncode="False">
                                                    <ItemStyle Width="30px" HorizontalAlign="left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Overspeeding" HeaderText="Speeding Frequency" HtmlEncode="False">
                                                    <ItemStyle Width="77px" HorizontalAlign="right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="maxSpeed" HeaderText="Maximum Speed" HtmlEncode="False">
                                                    <ItemStyle Width="70px" HorizontalAlign="right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="longOverspeedDuration" HeaderText="Longest Overspeed Duration"
                                                    HtmlEncode="False">
                                                    <ItemStyle Width="100px" HorizontalAlign="right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Harsh Brake" HeaderText="Harsh Braking Frequency" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Harsh acc" HeaderText="Harsh Acc" HtmlEncode="False" Visible="false">
                                                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Unauth Stop" HeaderText="Unauth Stop" HtmlEncode="False"
                                                    Visible="false">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Unauth Road" HeaderText="Unauth Road" HtmlEncode="False"
                                                    Visible="false">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Banned Hour" HeaderText="Banned Hours" HtmlEncode="False"
                                                    Visible="false">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Idling" HeaderText="Excessive Idle" HtmlEncode="False"
                                                    Visible="false">
                                                    <ItemStyle Width="80px" HorizontalAlign="right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Cont Drive" HeaderText="Continuous Driving > 2 Hrs" HtmlEncode="False"
                                                    Visible="false">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Drive Hour" HeaderText="Continuous Driving > 4Hrs" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Work Hour" HeaderText="Total Continuous Driving Hours"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Violation" HeaderText="Total Violations" HtmlEncode="False"
                                                    Visible="false">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Drive Hour" HeaderText="Total Driving Hour" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total Work Hour" HeaderText="Total Work Hour" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="more14Work" HeaderText="Frequency of > 14 Hrs Work" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="more10Drive" HeaderText="Frequency of > 10 Hrs Driving"
                                                    HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Distance Travel" HeaderText="Distance Travel" HtmlEncode="False"
                                                    Visible="true">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="MidNightCount" HeaderText="Mid-Night Count" HtmlEncode="False"
                                                    Visible="true">
                                                    <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                </asp:BoundField>
                                            </Columns>
                                            <AlternatingRowStyle BackColor="white" />
                                            <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="10px" ForeColor="White"
                                                Height="17px" HorizontalAlign="center" />
                                        </asp:GridView>
                                        <% If show = True Then%>
                                        <center>
                                            <label id="pages" style="font-family: Verdana; font-size: 10px; font-weight: bold;">
                                                Pages</label></center>
                                        <%End If%>
                                        <br />
                                    </div>
                                </td>
                            </tr>
                        </table>
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
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label>
                        <br />
                        <asp:Label ID="Label90" runat="server" Text="9. Mid Night Count - Vehicle travelling duration between 2AM to 5AM."
                            Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label>
                        <!-- <asp:Label ID="Label9" runat="server" Text="4. Idling Hours counted in driving hours- Idling which is less than 5 minutes. The Idling hours will be included in working hours" Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="false" ></asp:Label><br/>
                        <asp:Label ID="Label12" runat="server" Text="6. Stopping Hours- The time counted the moment the engine is turned off until it is turn on again. The system should be able to give the duration of each stop regardless stoppage during the off working hours or stoppages during the working hours." Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label>
                                <asp:Label ID="Label20" runat="server" Text="6. Stopping Hours- The time counted the moment the engine is turned off until it is turn on again. The system should be able to give the duration of each stop regardless stoppage during the off working hours or stoppages during the working hours." Font-Size="10px" Font-Names="verdana" ForeColor="Red" Enabled="false" Visible="true"></asp:Label><br/>-->
                    </td>
                </tr>
            </table>
        </div>
    </center>
    <input id="values" type="hidden" runat="server" value="" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Vehicles Violation Weekly Summary Report" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
<style type="text/css">
    .tableAlign
    {
        text-align: center;
        background-color: #4D90FE;
        font-size: 14px;
        font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif;
        color: White;
        width: 500px;
    }
    
    .tableBorder
    {
        width: 420px;
        border: solid 1px #465ae8;
    }
    
    .FontText
    {
        font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif;
        color: #5f7afc;
        font-size: 13px;
    }
    
    .GridText
    {
        font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif;
        color: Black;
        font-size: 11px;
    }
    
    .cboAlign
    {
        width: 230px;
    }
    
    .cboHourMinAlign
    {
        width: 45px;
    }
    .buttonAlign
    {
        width: 65px;
    }
    
    .ui-widget-header
    {
        border: 1px solid #465ae8;
        background: #4D90FE;
    }
    
    .ui-widget
    {
        font-size: 12px;
    }
    
    .help
    {
        display: none;
    }
</style>
