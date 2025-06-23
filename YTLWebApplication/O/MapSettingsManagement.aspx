<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.MapSettingsManagement" Codebehind="MapSettingsManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Map Settings</title>

    <script type="text/javascript">
    function mysubmit()
    {
  
        if(document.getElementById("ddluser").value=="--Select User Name--") 
        {
            alert("Please select user name");
            return false;   
        }
        if(document.getElementById("ddlzoomlevel").value=="--Select Zoom Level--") 
        {
            alert("Please select map zoom level");
            return false;   
        }
        else if(document.getElementById("Latitude").value=="") 
        {
            alert("Please enter Latitude value");
            return false;                  
        }
        else if(document.getElementById("Longitude").value=="") 
        {
            alert("Please enter Longitude value");
            return false;   
        }
        else
        {
            return true;
        }
           
    }
    function cancel()
    {
        var formobj=document.getElementById("mapsettingform");
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
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="mapsettingform" runat="server">
        <center>
            <div>
                <br />
                  <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Map Settings Management</b>
                
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Map Settings Management :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 420px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 420px;">
                                            <tr align="left">
                                                <td>
                                                    <b>User Name</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddluser" runat="server" Width="255px" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Zoom Level</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlzoomlevel" runat="server" Width="255px">
                                                        <asp:ListItem Value="--Select Zoom Level--">--Select Zoom Level--</asp:ListItem>
                                                        <asp:ListItem Value="1">1</asp:ListItem>
                                                        <asp:ListItem Value="2">2</asp:ListItem>
                                                        <asp:ListItem Value="3">3</asp:ListItem>
                                                        <asp:ListItem Value="4">4</asp:ListItem>
                                                        <asp:ListItem Value="5">5</asp:ListItem>
                                                        <asp:ListItem Value="6" Selected="True">6</asp:ListItem>
                                                        <asp:ListItem Value="7">7</asp:ListItem>
                                                        <asp:ListItem Value="8">8</asp:ListItem>
                                                        <asp:ListItem Value="9">9</asp:ListItem>
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="11">11</asp:ListItem>
                                                        <asp:ListItem Value="12">12</asp:ListItem>
                                                        <asp:ListItem Value="13">13</asp:ListItem>
                                                        <asp:ListItem Value="14">14</asp:ListItem>
                                                        <asp:ListItem Value="15">15</asp:ListItem>
                                                        <asp:ListItem Value="16">16</asp:ListItem>
                                                        <asp:ListItem Value="17">17</asp:ListItem>
                                                        <asp:ListItem Value="18">18</asp:ListItem>
                                                        <asp:ListItem Value="19">19</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Latitude</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="latitude" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="250px"
                                                        Text="2.504883" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Longitude</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="longitude" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="250px"
                                                        Text="107.841797" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <%--<a href="<%=backpage %>">
                                                        <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                            title="Back" /></a>--%>
                                                </td>
                                                <td colspan="2" align="center" valign="middle">
                                                    <br />
                                                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="images/submit_s.jpg"
                                                        ToolTip="Submit"></asp:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg"
                                                            alt="Cancel" style="border: 0px; vertical-align: top; cursor: pointer" title="Cancel"
                                                            onclick="javascript:cancel();" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                           <%-- <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.</p>--%>
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
