<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.SuperUserManagementL" Codebehind="SuperUserManagementL.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Super Users</title>

    <script type="text/javascript">
function mysubmit()
    {
        if(document.getElementById("ddluser").value=="--Select Super User--")
        {
            alert("Please select super user name");
            return false;
        }
        var listbox2obj=document.getElementById("ListBox2");
        var i=0;
        var userslist="";
        for(i=0;i<listbox2obj.length;i++)
        {
            userslist+=listbox2obj.options[i].value+",";
        }
        userslist=userslist.substr(0, userslist.length-1);
        document.getElementById("userslist").value=userslist;
        return true;
     
    }
    function cancel()
    {
        var formobj=document.getElementById("superusersform");
        formobj.reset();
    }
var NS4 = (navigator.appName == "Netscape" && parseInt(navigator.appVersion) < 5);
function addOption(theSel, theText, theValue){var newOpt = new Option(theText, theValue);var selLength = theSel.length;theSel.options[selLength] = newOpt;}function deleteOption(theSel, theIndex){var selLength = theSel.length;if(selLength>0){theSel.options[theIndex] = null;}}function moveOptions(theSelFrom, theSelTo){var selLength = theSelFrom.length;var selectedText = new Array();var selectedValues = new Array();var selectedCount = 0;var i;for(i=selLength-1; i>=0; i--){if(theSelFrom.options[i].selected){selectedText[selectedCount] = theSelFrom.options[i].text;selectedValues[selectedCount] = theSelFrom.options[i].value;deleteOption(theSelFrom, i);selectedCount++;}}for(i=selectedCount-1; i>=0; i--){addOption(theSelTo, selectedText[i], selectedValues[i]);}if(NS4) history.go(0);}
    </script>

</head>
<body style="margin: 0px;">
    <form id="superusersform" runat="server">
        <center>
            <div>
                <br />
                <img alt="Super User Management" src="images/SuperUserManagement.jpg" />
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Super User Management :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 600px; border: solid 1px #3952F9;">
                                        <table style="width: 600px;">
                                            <tr>
                                                <td colspan="3">
                                                    <br />
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">User :</b>
                                                </td>
                                                <td align="left" colspan="2">
                                                    <asp:DropDownList ID="ddluser" runat="server" Width="250px" AutoPostBack="True">
                                                        <asp:ListItem>--Select User--</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3">
                                                    <br />
                                                   <b>YTL Super Users : </b>
                                                </td>
                                            </tr>
                                            <tr >

                                                 <td colspan="3">
                                                    <asp:Button ID="btnselectall" runat ="server" Text="Select All" Visible ="false"  />
                                                    </td>
                                                
                                            </tr>
                                            
                                            <tr>
                                               
                                                <td colspan="3">
                                                    <br />
                                                    <center>
                                                        <asp:CheckBoxList ID="lstsuperusers" runat="server" CssClass="cbl" RepeatColumns ="4"></asp:CheckBoxList>
                                                    </center>
                                                    
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    
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
        <input type="hidden" value="" name='userslist' id='userslist' />
    </form>
    <%  If message <> "" Then%>

    <script type="text/javascript">
        alert('<%=message %>');
    </script>

    <%  End If%>
</body>
</html>
