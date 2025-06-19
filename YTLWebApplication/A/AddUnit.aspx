<%@ Page Language="vb" AutoEventWireup="false" Inherits="YTLWebApplication.AVLS.AddUnit" Codebehind="AddUnit.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add Unit</title>
    
    <script type="text/javascript">
    function mysubmit()
    {
        if(document.getElementById("ddlusers").value=="--Select User Name--")
        {
            alert("Please select user name");
            return false;
        }
        else if(document.getElementById("unitid").value=="")
        {
            alert("Please enter unit ID");
            return false;
        }
        else if(document.getElementById("versionid").value=="")
        {
            alert("Please enter version ID");
            return false;
        }
        else if(document.getElementById("password").value=="") 
        {
            alert("Please enter password");
            return false;   
        }
        else if(document.getElementById("simno").value=="") 
        {
            alert("Please enter SIM number");
            return false;   
        }
        else
        {
            return true;
        }
           
    }
    function cancel()
    {
        var formobj=document.getElementById("addunitfrom");
        formobj.reset();
    }
    </script>

</head>
<body style="margin:0px;">
    <form id="addunitfrom" method="post" runat="server">
        <center>
            <div>
                <br />
                <img alt="Add Unit" src="images/AddUnit.jpg" />
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add New Unit Details :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 350px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 350px;">
                                            <tr align="left">
                                                <td style="width: 130px;">
                                                    <b>User ID</b></td>
                                                <td style="width: 10px;">
                                                    <b>:</b></td>
                                                <td style="width: 210px;">
                                                    <asp:DropDownList ID="ddlusers" runat="server" Width="186px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td style="width: 130px;">
                                                    <b>Unit ID</b></td>
                                                <td style="width: 10px;">
                                                    <b>:</b></td>
                                                <td style="width: 210px;">
                                                    <asp:TextBox ID="unitid" runat="Server" Style="border-right: #cbd6e4 1px solid; border-top: #cbd6e4 1px solid;
                                                        font-size: 10pt; border-left: #cbd6e4 1px solid; color: #0b3d62; border-bottom: #cbd6e4 1px solid;
                                                        font-family: Verdana" Width="180px" EnableViewState="False" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td style="width: 130px;">
                                                    <b>Version ID</b></td>
                                                <td style="width: 10px;">
                                                    <b>:</b></td>
                                                <td style="width: 210px;">
                                                    <asp:TextBox ID="versionid" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" EnableViewState="False" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Password</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="password" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px"
                                                        TextMode="Password" EnableViewState="False" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>SIM No</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="simno" runat="Server" Style="border-right: #cbd6e4 1px solid; border-top: #cbd6e4 1px solid;
                                                        font-size: 10pt; border-left: #cbd6e4 1px solid; color: #0b3d62; border-bottom: #cbd6e4 1px solid;
                                                        font-family: Verdana" Width="180px" EnableViewState="False" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <a href="UnitManagement.aspx">
                                                        <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                            title="Back" /></a>
                                                </td>
                                                <td colspan="2" align="center" valign="middle">
                                                    <br />
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/submit_s.jpg"
                                                        ToolTip="Submit"></asp:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg"
                                                            alt="Cancel" style="border: 0px; vertical-align: top; cursor: pointer" title="Cancel"
                                                            onclick="javascript:cancel();" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.</p>
                        </td>
                    </tr>
                </table>
            </div>
        </center>
        <%  If errormessage <> "" Then%>

        <script type="text/javascript">
        alert('<%=errormessage %>');
        </script>

        <%  End If%>
    </form>
</body>
</html>
