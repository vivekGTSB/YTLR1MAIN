<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.SpeedSummaryMonthly" Codebehind="SpeedSummaryMonthly.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Speed Summary Report (Monthly)</title>
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
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Speed Summary Report
                (Monthly)</b>
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Speed Summary Report (Monthly)</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 420px; border: solid 1px #3952F9;">
                                    <table style="width: 420px;">
                                        <tbody>
                                            <tr style="display: none;">
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
                        <asp:Label ID="Label115" runat="server" Text="Note : Data available since 2014/02/26 00:00:00."
                            Font-Size="12px" Font-Names="verdana" ForeColor="Green"></asp:Label>
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
                                                <asp:BoundField DataField="No" HeaderText="No" HtmlEncode="False">
                                                    <ItemStyle Width="20px" HorizontalAlign="Center" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Month" HeaderText="Month" HtmlEncode="False">
                                                    <ItemStyle Width="150px" HorizontalAlign="Left" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Speed > 80KM" HeaderText="Speed &gt 80KM" HtmlEncode="False">
                                                    <ItemStyle Width="100px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Speed > 85KM" HeaderText="Speed &gt 85KM" HtmlEncode="False">
                                                    <ItemStyle HorizontalAlign="Right" Width="100px" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Speed > 90KM" HeaderText="Speed &gt 90KM" HtmlEncode="False">
                                                    <ItemStyle Width="100px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Speed > 100KM" HeaderText="Speed &gt 100KM" HtmlEncode="False">
                                                    <ItemStyle Width="100px" HorizontalAlign="Right" ForeColor="Black" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" HtmlEncode="False">
                                                    <ItemStyle Width="100px" HorizontalAlign="Right" ForeColor="Black" />
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
                    </td>
                </tr>
            </table>
        </div>
    </center>
    <input id="values" type="hidden" runat="server" value="" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Speed Summary Report (Monthly)" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
