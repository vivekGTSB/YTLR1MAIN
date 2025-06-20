<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.GetLostData" Codebehind="GetLostData.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Lost and VData</title>
    <link type="text/css" href="cssfiles/css3-buttons.css" rel="stylesheet" />
    <style type="text/css">
        .g1
        {
            background-image: url(images/g.png);
            background-repeat: no-repeat;
            width: 16px;
            height: 16px;
            display: inline-table;
            vertical-align: middle;
        }
        .p1
        {
            background-image: url(images/p.png);
            background-repeat: no-repeat;
            width: 16px;
            height: 16px;
            display: inline-table;
            vertical-align: middle;
        }
        .r1
        {
            background-image: url(images/r.png);
            background-repeat: no-repeat;
            width: 13px;
            height: 13px;
            display: inline-table;
            vertical-align: middle;
        }
    </style>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;font-family:Verdana Sans-Serif;font-size:13px;">
    <form id="form1" runat="server">
    <div>
        <center>
           <br />
            
             <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Detailed Report</b>
            <br />
            <br />

            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" Width="100%" PageSize="20"
                AutoGenerateColumns="False" HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF"
                HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True" Font-Bold="False"
                Font-Overline="False" EnableViewState="False" HeaderStyle-Height="22px" HeaderStyle-HorizontalAlign="Center"
                BorderColor="#F0F0F0">
                <AlternatingRowStyle BackColor="Lavender" />
                <Columns>
                    <asp:BoundField DataField="Sl No" HeaderText="S No" Visible="false">
                        <ItemStyle HorizontalAlign="center" Width="35" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Plate No" HeaderText="Plate No" HtmlEncode="False"></asp:BoundField>
                    <asp:BoundField DataField="From Time" HeaderText="From Time" HtmlEncode="False">
                    </asp:BoundField>
                    <asp:BoundField DataField="To Time" HeaderText="To Time" HtmlEncode="False"></asp:BoundField>
                    <asp:BoundField DataField="From Location" HeaderText="From Location" HtmlEncode="False">
                    </asp:BoundField>
                    <asp:BoundField DataField="To Location" HeaderText="To Location" HtmlEncode="False">
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Duration" HeaderText="Mins" HtmlEncode="false">
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Type" HeaderText="Type" HtmlEncode="false">
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>
                </Columns>
            </asp:GridView>
        </center>
    </div>
    </form>
</body>
</html>
