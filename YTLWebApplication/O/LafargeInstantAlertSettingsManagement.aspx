<%@ Page Language="VB" AutoEventWireup="false"
    EnableEventValidation="false" Inherits="YTLWebApplication.LafargeInstantAlertSettingsManagement" Codebehind="LafargeInstantAlertSettingsManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Instant Alert Settings Management</title>
    <script type="text/javascript" language="javascript">
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
                var result = confirm("Are you delete checked information ?");
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
    	
    </script>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="InstantAlertSettingsManagementform" runat="server">
    <center>
        <br />
       
          <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Instant Alert Settings Management</b>
        <br />
        <br />
        <table border="0" cellpadding="0" cellspacing="0" style="font-family: Verdana; font-size: 11px;
            width: 710px;">
            <tr>
                <td align="left" style="width: 100px;">
                    <asp:ImageButton ID="delete1" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Information" />
                </td>
                <td align="center">
                    <b style="color: #5f7afc;">Select User Name :&nbsp;</b>
                    <asp:DropDownList ID="ddluser" runat="server" Width="300" AutoPostBack="True" EnableViewState="False">
                    </asp:DropDownList>
                </td>
                <td align="right" style="width: 100px;">
                    <a href="LafargeAddInstantAlertSettings.aspx">
                        <img src="images/Add.jpg" alt="Add Instant Alert Settings" style="border: 0px; vertical-align: top;
                            cursor: pointer" title="Add Instant Alert Settings" /></a>
                </td>
            </tr>
            <tr>
                <td align="left" colspan="3">
                    <div style="font-family: Verdana; font-size: 11px;">
                        <br />
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Size="12px"
                            HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                            HeaderStyle-Height="22px" EnableViewState="False" HeaderStyle-HorizontalAlign="Center"
                            Width="900px" RowStyle-ForeColor="black" BorderColor="#F0F0F0">
                            <Columns>
                                <asp:BoundField DataField="chk" HeaderText="<input type='checkbox' onclick='javascript:checkall(this);' />"
                                    HtmlEncode="False">
                                    <ItemStyle Width="20" />
                                </asp:BoundField>
                                <asp:BoundField DataField="S No" HeaderText="S No">
                                    <ItemStyle HorizontalAlign="center" Width="40" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Plate No" HeaderText="Plate No" HtmlEncode="false" HeaderStyle-Width="100">
                                </asp:BoundField>
                                <asp:BoundField DataField="Mobiles List" HeaderText="Mobiles List" HtmlEncode="false">
                                </asp:BoundField>
                                <asp:BoundField DataField="EMails List" HeaderText="EMails List" HtmlEncode="false">
                                </asp:BoundField>
                                <asp:BoundField DataField="EMail" HeaderText="EMail" HtmlEncode="false"></asp:BoundField>
                                <asp:BoundField DataField="Sms" HeaderText="Sms" HtmlEncode="false"></asp:BoundField>
                                <asp:BoundField DataField="Popup" HeaderText="Web" HtmlEncode="false"></asp:BoundField>
                            </Columns>
                            <AlternatingRowStyle BackColor="Lavender" />
                        </asp:GridView>
                        <br />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:ImageButton ID="delete2" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Information" />
                </td>
                <td align="center">
                </td>
                <td align="right">
                    <a href="AddInstantAlertSettings.aspx">
                        <img src="images/Add.jpg" alt="Add Instant Alert Settings" style="border: 0px; vertical-align: top;
                            cursor: pointer" title="Add Instant Alert Settings" /></a>
                </td>
            </tr>
        </table>
    </center>
    </form>
</body>
</html>
