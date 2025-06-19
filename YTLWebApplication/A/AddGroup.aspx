<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false" Inherits="YTLWebApplication.AddNewGroup" Codebehind="AddGroup.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Group</title>

    <script type="text/javascript">
    function mysubmit()
    {
        if(document.getElementById("ddlusers").value=="-- Select User Name --")
        {  
            alert("Please select user name");
            return false;
        }
        else if(document.getElementById("groupnametextbox").value=="")
        {
            alert("Please enter group name");
            return false;
        }
        else
        {
            return true;
        }
        
    }
    function cancel()
    {
        var formobj=document.getElementById("form1");
        formobj.reset();
    }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <center>
            <div>
                <br />
                <img alt="Add Vehicle Groups" src="images/AddGroup.jpg" />
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add Group &nbsp;:</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 450px; border: solid 1px #3952F9;">
                                        <table style="width: 450px;">
                                            <tbody>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">User Name</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlusers" runat="server" Width="205px">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Group Name</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="groupnametextbox" runat="server" MaxLength="50" Width="200px"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Description</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="descriptiontextbox" runat="server" MaxLength="100" Width="200px"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <br />
                                                        <a href="GroupManagement.aspx">
                                                            <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                                title="Back" /></a>
                                                    </td>
                                                    <td colspan="2" align="center">
                                                        <br />
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Submit_s.jpg" ToolTip="Submit"></asp:ImageButton>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg"
                                                                alt="Cancel" style="border: 0px; vertical-align: top; cursor: pointer" title="Cancel"
                                                                onclick="javascript:cancel();" />
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
                <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                    Copyright © 2008 Global Telematics Sdn Bhd. All rights reserved.</p>
            </div>
        </center>
    </form>
</body>
</html>
