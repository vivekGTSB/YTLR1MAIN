<%@ Page Language="vb" AutoEventWireup="false" Inherits="YTLWebApplication.AVLS.PasswordManagement" Codebehind="PasswordManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Password Management</title>

    <script type="text/javascript">
    function mysubmit()
    {
        var password=document.getElementById("passwordtextbox").value
        var newpassword=document.getElementById("newpasswordtextbox").value
        var confirmpassword=document.getElementById("confirmpasswordtextbox").value
        if(password=="")
        {
            alert("Please enter old password");
            return false;
        }
        else if(newpassword=="")  
        {
            alert("Please enter new password");
            return false;   
        }
        else if(confirmpassword=="")  
        {
            alert("Please enter confirm password");
            return false;   
        }
        else if(newpassword!=confirmpassword)
        {
            alert("New password, Confirm password are different")
            return false;
        }
        else
        {
            return true;
        }
    }
    function cancel()
    {
        var formobj=document.getElementById("updatepasswordform");
        formobj.reset();
    }
    </script>

<script type="text/javascript">

  var _gaq = _gaq || [];
  _gaq.push(['_setAccount', 'UA-32500429-1']);
  _gaq.push(['_setDomainName', 'avls.com.my']);
  _gaq.push(['_trackPageview']);

  (function() {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'https://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
  })();

</script>

</head>
<body style="margin: 0px;">
    <form id="updatepasswordform" method="post" runat="server">
        <center>
            <div>
               <br />
        <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Password Management</b>
   
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Update User Name & Password :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 450px; border: solid 1px #3952F9;">
                                        <table style="width: 450px;">
                                            <tr>
                                                <td colspan="3">
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">User Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="usernametextbox" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana"
                                                        Width="250px" ReadOnly="True"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Password</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="passwordtextbox" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="250px"
                                                        TextMode="Password"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">New Password</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="newpasswordtextbox" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="250px"
                                                        TextMode="Password"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Confirm Password</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="confirmpasswordtextbox" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="250px"
                                                        TextMode="Password"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <br />
                                                    <%--<a href="Management.aspx">
                                                        <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                            title="Back" /></a>--%>
                                                </td>
                                                <td colspan="2" align="center">
                                                    <br />
                                                    <br />
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/submit_s.jpg"
                                                        ToolTip="Submit"></asp:ImageButton>
                                                    &nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg" alt="Cancel" title="Cancel"
                                                        onclick="javascript:cancel();" style="cursor: pointer;" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <%--<p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                                Copyright © 2008 Global Telematics Sdn Bhd. All rights reserved.</p>--%>
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
