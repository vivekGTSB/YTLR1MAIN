<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false"
    Inherits="YTLWebApplication.AVLS.hLogin" Codebehind="hLogin.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>AVLS - Automatic Vehicle Locating System</title>
</head>
<body style="margin: 0px;">
    <form id="loginform" runat="server">

            <table width="100%" style="vertical-align: middle;">
                <tr align="center">
                    <td>
                        <table>
                            <tr align="center">
                                <td>
                                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                                    <asp:TextBox ID="txtPwd" runat="server" TextMode="Password"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/empty.jpg" Width="1px" Height="1px" TabIndex="3" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

    </form>
</body>

<%  If errormessage <> "" Then%>

<script type="text/javascript" language="javascript">
	alert('<%= errormessage%>');
</script>

<%  End If%>
</html>
