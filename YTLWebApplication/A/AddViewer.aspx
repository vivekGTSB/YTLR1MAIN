<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" Inherits="YTLWebApplication.AVLS.AddViewer" Codebehind="AddViewer.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add Viewer</title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />

    <script type="text/javascript" src="jsfiles/balloontip.js"></script>

    <script type="text/javascript">
    function mysubmit()
    {
       if(document.getElementById("viewername").value=="")
        {
            alert("Please enter viewer name");
            return false;
        }
        else if(document.getElementById("viewerpwd").value=="") 
        {
            alert("Please enter password");
            return false;   
        }
        else if(document.getElementById("confirmpassword").value=="") 
        {
            alert("Please confirm password");
            return false;   
        }
        else if(document.getElementById("viewerpwd").value!=document.getElementById("confirmpassword").value) 
        {
            alert("Sorry, passwords are not the same.\nPlease try again.");
            return false;
        }
    }
    function cancel()
    {
        var formobj=document.getElementById("adduserform");
        formobj.reset();
    }
    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="adduserform" method="post" runat="server">
        <center>
            <div>
                <br />
                <img alt="Add Viewer" src="images/AddNewViewerDetails.jpg" />
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add New Viewer Details :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 350px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 350px;">
                                            <tr align="left">
                                                <td style="width: 130px;">
                                                    <b>Viewer Name</b></td>
                                                <td style="width: 10px;">
                                                    <b>:</b></td>
                                                <td style="width: 210px;">
                                                    <asp:TextBox ID="viewername" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Password</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="viewerpwd" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px"
                                                        TextMode="Password" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Confirm Password</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="confirmpassword" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px"
                                                        TextMode="Password" />
                                                </td>
                                            </tr>
                                             <tr align="left">
                                                <td>
                                                    <b>Viewer Type</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                   <asp:DropDownList ID="ddlviewertype" runat ="server" Enabled ="false"  >
                                                    <asp:ListItem Text ="--Select Viewer Type--" Value ="--Select Viewer Type--"></asp:ListItem>
                                                   <asp:ListItem Text ="Basic" Value ="0" Selected ="true" ></asp:ListItem>
                                                   <asp:ListItem Text ="Advance" Value ="1"></asp:ListItem>
                                                   </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <a href="ViewerManagement.aspx">
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
                                Copyright © 2009 Global Telematics Sdn Bhd. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
                <br />
                <asp:HiddenField ID="hfViewerID" runat="server" />
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
