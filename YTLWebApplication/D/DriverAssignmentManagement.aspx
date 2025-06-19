<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.DriverAssignmentManagement" Codebehind="DriverAssignmentManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Driver Assignment Management</title>
    <link rel="shortcut icon" type="image/ico" href="images/car.ico">
    <style type="text/css" title="currentStyle">
        @import "css/demo_table_jui.css";
        @import "css/jquery-ui.css";
        @import "css/TableTools.css";
        @import "css/ColVis.css";
        @import "css/common1.css";
        @import "css/chosen.css";
    </style>
    <script type="text/javascript">
        function mysubmit() {
            if (document.getElementById("ddlplate").value == "--Select Plate No--") {
                alert("Please select plate number");
                return false;
            }
            var listbox1obj = document.getElementById("ListBox1");
            var i = 0;
            var newdisablegeofenceids = "";
            for (i = 0; i < listbox1obj.length; i++) {
                newdisablegeofenceids += listbox1obj.options[i].value + ",";
            }
            newdisablegeofenceids = newdisablegeofenceids.substr(0, newdisablegeofenceids.length - 1);
            document.getElementById("newdisablegeofenceids").value = newdisablegeofenceids;

            var listbox2obj = document.getElementById("ListBox2");
            var i = 0;
            var newenablegeofenceids = "";
            for (i = 0; i < listbox2obj.length; i++) {
                newenablegeofenceids += listbox2obj.options[i].value + ",";
            }
            newenablegeofenceids = newenablegeofenceids.substr(0, newenablegeofenceids.length - 1);
            document.getElementById("newenablegeofenceids").value = newenablegeofenceids;
            return true;

        }
        function cancel() {
            var formobj = document.getElementById("operatorform");
            formobj.reset();
        }
        var NS4 = (navigator.appName == "Netscape" && parseInt(navigator.appVersion) < 5);
        function addOption(theSel, theText, theValue) { var newOpt = new Option(theText, theValue); var selLength = theSel.length; theSel.options[selLength] = newOpt; } function deleteOption(theSel, theIndex) { var selLength = theSel.length; if (selLength > 0) { theSel.options[theIndex] = null; } } function moveOptions(theSelFrom, theSelTo) { var selLength = theSelFrom.length; var selectedText = new Array(); var selectedValues = new Array(); var selectedCount = 0; var i; for (i = selLength - 1; i >= 0; i--) { if (theSelFrom.options[i].selected) { selectedText[selectedCount] = theSelFrom.options[i].text; selectedValues[selectedCount] = theSelFrom.options[i].value; deleteOption(theSelFrom, i); selectedCount++; } } for (i = selectedCount - 1; i >= 0; i--) { addOption(theSelTo, selectedText[i], selectedValues[i]); } if (NS4) history.go(0); }
    </script>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="operatorform" runat="server">
    <center>
        <div>
            <br>
            <div class="c1">
                Driver Assignment Management</div>
            <br>
            <table>
                <tr>
                    <td colspan="3" align="left" style="color: navy; font-family: Verdana; font-size: 14px;
                        font-weight: normal;">
                        <span><a href="DriverManagement.aspx">Step1: Driver Management Registration</a></span><br/>
                        <span>Step2: Driver Assginment for Vehicle</span><br />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Driver Assignment Management :</b>
                                </td>
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
                                                <b style="color: #5f7afc;">Plate No :</b>
                                            </td>
                                            <td align="left" colspan="2">
                                                <asp:DropDownList ID="ddlplate" runat="server" Width="250px" AutoPostBack="True">
                                                    <asp:ListItem>--Select Plate No--</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <br />
                                                <table border="0">
                                                    <tr>
                                                        <td>
                                                            Disable Drivers
                                                            <br />
                                                            <br />
                                                            <asp:ListBox ID="ListBox1" runat="server" Rows="10" SelectionMode="Multiple" Width="275px"
                                                                EnableViewState="False"></asp:ListBox>
                                                        </td>
                                                        <td align="center" valign="middle">
                                                            <input type="button" value="&gt;&gt;" onclick="moveOptions(this.form.ListBox1, this.form.ListBox2);" /><br />
                                                            <input type="button" value="&lt;&lt;" onclick="moveOptions(this.form.ListBox2, this.form.ListBox1);" />
                                                        </td>
                                                        <td>
                                                            Enable Drivers
                                                            <br />
                                                            <br />
                                                            <asp:ListBox ID="ListBox2" runat="server" Rows="10" SelectionMode="Multiple" Width="275px"
                                                                EnableViewState="False"></asp:ListBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <br />
                                            </td>
                                            <td colspan="2" align="center" valign="middle">
                                                <br />
                                                <asp:Button Text="Submit" ID="ImageButton1" class="action blue" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <br />
            <div style="font-family: Verdana; font-size: 13px;">
                <asp:GridView runat="server" ID="Gv1" PagerStyle-CssClass="pgr" CssClass="Grid" BackColor="White"
                    BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical">
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                    <PagerStyle CssClass="pgr" BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <%--<SortedAscendingCellStyle BackColor="#F1F1F1" />
                    <SortedAscendingHeaderStyle BackColor="#0000A9" />
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                    <SortedDescendingHeaderStyle BackColor="#000065" />--%>
                </asp:GridView>
            </div>
        </div>
    </center>
    <input type="hidden" value="" name='newdisablegeofenceids' id='newdisablegeofenceids' />
    <input type="hidden" value="" name='newenablegeofenceids' id='newenablegeofenceids' />
    </form>
    <%  If message <> "" Then%>
    <script type="text/javascript">
        alert('<%=message %>');
    </script>
    <%  End If%>
</body>
</html>
