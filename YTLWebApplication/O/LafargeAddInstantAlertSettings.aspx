<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.LafargeAddInstantAlertSettings" Codebehind="LafargeAddInstantAlertSettings.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Instant Alert Settings</title>
    <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
    <style type="text/css">
        .t
        {
            width: 200px;
        }
        
        .ui-tooltip
        {
            width: 140px;
        }
        #txtIdlingTime
        {
            width: 42px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            var tooltips = $("[title]").tooltip();

        });
        function trimString(s) {
            return s.replace(/^[\s|\xA0]+|[\s|\xA0]+$/g, '');
        }
        function mysubmit() {

            var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            var mobileRegEx = /^(0)(\d{9})$/;

            if (document.getElementById("ddluser").value == "--Select User Name--") {
                alert("Please select user name");
                return false;
            }
            if (document.getElementById("mobile1").value != "")
                if (!trimString(document.getElementById("mobile1").value).match(mobileRegEx)) {
                    alert("Please enter valid mobile 1");
                    return false;
                }
            if (document.getElementById("mobile2").value != "")
                if (!trimString(document.getElementById("mobile2").value).match(mobileRegEx)) {
                    alert("Please enter valid mobile 2");
                    return false;
                }
            if (document.getElementById("mobile3").value != "")
                if (!trimString(document.getElementById("mobile3").value).match(mobileRegEx)) {
                    alert("Please enter valid mobile 3");
                    return false;
                }
            if (document.getElementById("mobile4").value != "")
                if (!trimString(document.getElementById("mobile4").value).match(mobileRegEx)) {
                    alert("Please enter valid mobile 4");
                    return false;
                }
            if (document.getElementById("mobile5").value != "")
                if (!trimString(document.getElementById("mobile5").value).match(mobileRegEx)) {
                    alert("Please enter valid mobile 5");
                    return false;
                }

            if (document.getElementById("email1").value != "")
                if (!trimString(document.getElementById("email1").value).match(emailRegEx)) {
                    alert("Please enter valid email 1");
                    return false;
                }
            if (document.getElementById("email2").value != "")
                if (!trimString(document.getElementById("email2").value).match(emailRegEx)) {
                    alert("Please enter valid email 2");
                    return false;
                }
            if (document.getElementById("email3").value != "")
                if (!trimString(document.getElementById("email3").value).match(emailRegEx)) {
                    alert("Please enter valid email 3");
                    return false;
                }
            if (document.getElementById("email4").value != "")
                if (!trimString(document.getElementById("email4").value).match(emailRegEx)) {
                    alert("Please enter valid email 4");
                    return false;
                }
            if (document.getElementById("email5").value != "")
                if (!trimString(document.getElementById("email5").value).match(emailRegEx)) {
                    alert("Please enter valid email 5");
                    return false;
                }
                else {
                    return true;
                }
        }

        function cancel() {
            var formobj = document.getElementById("AddInstantAlertSettingsform");
            formobj.reset();
        }


        function checkallplate(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm.id.substring(0, 14) == "CheckBoxList1_") {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
        }


        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i];
                if (elm.type == 'checkbox' && (document.forms[0].elements[i].value % 3) == chkobj.value) {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
        }
    </script>
</head>
<body style="font-family: Verdana; font-size: 11px;">
    <form id="AddInstantAlertSettingsform" runat="server">
    <center>
        <br />
       
          <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Add Instant Alert Settings</b>
        <br />
        <br />
        <table width="600px;" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td>
                    <fieldset id="commentform" style="width: 600px;">
                        <legend>Select Username</legend>
                        <table>
                            <tr>
                                <td>
                                    <br />
                                    <asp:DropDownList ID="ddluser" runat="server" Width="580px" AutoPostBack="True">
                                    </asp:DropDownList>
                                    <br />
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="left">
                    <fieldset id="Fieldset1" style="width: 600px;">
                        <legend>Select Plate Numbers&nbsp;<input type="checkbox" onclick='javascript:checkallplate(this);'
                            title="Check to Select all plate numbers" /></legend>
                        <table>
                            <tr>
                                <td>
                                    <asp:Panel ID="panplate" runat="server">
                                        <asp:CheckBoxList ID="CheckBoxList1" runat="server" Width="580px" RepeatColumns="3"
                                            RepeatDirection="Horizontal">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    <fieldset>
                        <legend>Mobiles & Emails List</legend>
                        <table cellpadding="0" cellspacing="2" border="0">
                            <tr>
                                <td align="left">
                                    <fieldset id="Fieldset2" style="width: 275px;">
                                        <legend>Mobiles List</legend>
                                        <table cellpadding="0" cellspacing="4" border="0">
                                            <tr>
                                                <td style="width: 60px;">
                                                    Mobile 1:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="mobile1" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Mobile 2:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="mobile2" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Mobile 3:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="mobile3" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Mobile 4:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="mobile4" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Mobile 5:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="mobile5" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                                <td align="left">
                                    <fieldset id="Fieldset3" style="width: 275px;">
                                        <legend>Emails List</legend>
                                        <table cellpadding="0" cellspacing="4" border="0">
                                            <tr>
                                                <td style="width: 60px;">
                                                    Email 1:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="email1" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Email 2:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="email2" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Email 3:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="email3" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Email 4:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="email4" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Email 5:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="email5" runat="server" CssClass="t" />
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                  
                    <fieldset id="Fieldset4" style="width: 600px;">
                        <legend>Alert Settings</legend>
                        <table cellspacing="2px;">
                            <tr>
                                <td align="left" style="width: 286px">
                                </td>
                                <td>
                                </td>
                                <td align="left" valign="middle" style="width: 408px; height: 22px;">
                                    &nbsp;&nbsp;<span style="visibility: hidden;">Email :</span>
                                    <input name="chk1" type="checkbox" value="0" title="Select all email checkboxes"
                                        onclick="javascript:checkall(this);" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="visibility: hidden;">Web :</span>
                                    <input name="chk1" type="checkbox" value="1" title="Select all web checkboxes" onclick="javascript:checkall(this);" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="visibility: hidden;">SMS :</span>
                                    <input name="chk1" type="checkbox" value="2" title="Select all sms checkboxes" onclick="javascript:checkall(this);"
                                        disabled="disabled" style="visibility: hidden;" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Geofence In Alerts</b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="0" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="1" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input name="chk" type="checkbox" value="2" disabled="disabled" style="visibility: hidden;" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Geofence Out Alerts</b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="3" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="4" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input name="chk" type="checkbox" value="5" disabled="disabled" style="visibility: hidden;" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Idling Alerts&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="text" name="txtIdlingTime"
                                        value="45" id="txtIdlingTime" runat="server" title="Enter idling minutes" maxlength="3"
                                        style="text-align: right;" />&nbsp; Mins </b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="6" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="7" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input name="chk" type="checkbox" value="8" disabled="disabled" style="visibility: hidden;" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Immobilizer Alerts</b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="9" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="10" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input name="chk" type="checkbox" value="11" disabled="disabled" style="visibility: hidden;" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Overspeed Alerts</b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="12" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="13" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input name="chk" type="checkbox" value="14" disabled="disabled" style="visibility: hidden;" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Panic Alerts</b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="15" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="16" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SMS :
                                    <input name="chk" type="checkbox" value="17" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Powercut Alerts</b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="18" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="19" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SMS :
                                    <input name="chk" type="checkbox" value="20" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 400px; height: 22px;">
                                    <b>PTO On Alerts </b>
                                </td>
                                <td style="height: 22px">
                                    <b>:</b>
                                </td>
                                <td style="width: 407px; height: 22px;">
                                    &nbsp;&nbsp;Email :
                                    <input name="chk" type="checkbox" value="21" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input name="chk" type="checkbox" value="22" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <input name="chk" type="checkbox" value="23" disabled="disabled" style="visibility: hidden;" />
                                </td>
                            </tr>
                            <tr align="left">
                                <td style="width: 286px">
                                    <b>Jamming</b>
                                </td>
                                <td>
                                    <b>:</b>
                                </td>
                                <td style="width: 407px">
                                    &nbsp;&nbsp;Email :
                                    <input id="email9" name="chk" type="checkbox" value="24" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Web :
                                    <input id="pop9" name="chk" type="checkbox" value="25" />                                    
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SMS :
                                    <input id="sms9" name="chk" type="checkbox" value="26" />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    <fieldset style="width: 600px;">
                        <legend>Actions</legend>
                        <table>
                            <tr>
                                <td align="left" style="width: 400px">
                                    <br />
                                    <a href="LafargeInstantAlertSettingsManagement.aspx">
                                        <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer" /></a>
                                </td>
                                <td colspan="2" align="right" valign="middle">
                                    <br />
                                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="images/submit_s.jpg">
                                    </asp:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg"
                                        alt="Cancel" style="border: 0px; vertical-align: top; cursor: pointer" onclick="javascript:cancel();" />
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
        </table>
    </center>
    </form>
</body>
</html>
