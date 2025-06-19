<%@ Page Language="vb" AutoEventWireup="false" Inherits="YTLWebApplication.AVLS.AddVehicle" Codebehind="AddVehicle.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add Vehicle</title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />

    <script type="text/javascript" src="jsfiles/balloontip.js"></script>
   
   <script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script> 

    <script type="text/javascript">
    function mysubmit()
    {
        if(document.getElementById("ddluserid").value=="--Select User Name--")
        {
            alert("Please select user name");
            return false;
        }
        else if(document.getElementById("textplateno").value=="") 
        {
            alert("Please enter vehicle plate number");
            return false;   
        }
        else if(document.getElementById("texttype").value=="") 
        {
            alert("Please select vehicle type");
            return false;   
        }
        else if(document.getElementById("textcolor").value=="") 
        {
            alert("Please enter vehicle color");
            return false;   
        }
        else if(document.getElementById("textmodel").value=="") 
        {
            alert("Please enter vehicle model");
            return false;   
        }
        else if(document.getElementById("textbrand").value=="") 
        {
            alert("Please enter vehicle brand");
            return false;   
        }
        else if(document.getElementById("ddlgroupname").value=="--Select Group Name--") 
        {
            alert("Please select vehicle group name");
            return false;   
        }
        else if(document.getElementById("texttrailerid").value=="") 
        {
            alert("Please enter vehicle trailer ID");
            return false;   
        }
        else if (document.getElementById("txtprimermoverid").value == "") {
            alert("Please enter vehicle prime mover ID");
            return false;
        }
        else if(document.getElementById("textspeedlimit").value=="") 
        {
            alert("Please enter vehicle speed limit");
            return false;   
        }
        else if(document.getElementById("textunitid").value=="") 
        {
            alert("Please enter vehicle unit ID");
            return false;   
        }
        else if(document.getElementById("textversionid").value=="") 
        {
            alert("Please enter vehicle version ID");
            return false;   
        }
        else
        {
            return true;
        }
           
    }
    function cancel()
    {
        var formobj=document.getElementById("addvehicleform");
        formobj.reset();
    }
    function smallimagemouseover()
    {   
        var smallimagefileobj=document.getElementById("FileUpload1");
        if(smallimagefileobj.value=="")
        {
            //document.getElementById("smallimagepreview").src="";
        }
        else
        {
            document.getElementById("smallimagepreview").src=smallimagefileobj.value;    
        }    
    }
    function smallimagemouseout()
    {   
        
    }
    function bigimagemouseover()
    {   
        var bigimagefileobj=document.getElementById("FileUpload2");
        if(bigimagefileobj.value=="")
        {
            //document.getElementById("bigimagepreview").src="";   
        }
        else
        {
            document.getElementById("bigimagepreview").src=bigimagefileobj.value;    
        }    
    }
    function bigimagemouseout()
    {   
        
    }
   function ShowCalendar(strTargetDateField, intLeft, intTop)
    {
    txtTargetDateField = strTargetDateField;
    
    var divTWCalendarobj=document.getElementById("divTWCalendar");
    divTWCalendarobj.style.visibility = 'visible';
    divTWCalendarobj.style.left = intLeft+"px";
    divTWCalendarobj.style.top = intTop+"px";
         selecteddate(txtTargetDateField);  
    } 
    </script>

</head>
<body style="margin: 0px;">
    <form id="addvehicleform" method="post" runat="server" action="addVehicle.aspx">
        <center>
            <div>
                <br />
                <img alt="Add Vehicle" src="images/AddVehicle.jpg" />
                <br />
               <script type="text/javascript">javascript:DrawCalendarLayout();</script> 
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add Vehicle :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 450px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 450px;">
                                            <tr align="left">
                                                <td style="width: 150px;">
                                                    <b>User Name</b></td>
                                                <td style="width: 10px;">
                                                    <b>:</b></td>
                                                <td style="width: 220px;">
                                                    <asp:DropDownList ID="ddluserid" runat="server" Width="185px" AutoPostBack="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Plate No</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="textplateno" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Type</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="texttype" runat="server" Width="185px">
                                                        </asp:DropDownList>
                                                   <%-- <asp:TextBox ID="texttype" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />--%>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Color</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="textcolor" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Model</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="textmodel" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Brand</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="textbrand" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Group Name</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlgroupname" runat="server" Width="185px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Trailer ID</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="texttrailerid" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Prime Mover ID</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtprimermoverid" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Speed Limit</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="textspeedlimit" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Unit ID</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="textunitid" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Version ID</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="textversionid" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>PTO</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:RadioButtonList runat="server" ID="btnPTO" RepeatDirection="Horizontal">
                                                        <asp:ListItem Text="Yes" Value="1" />
                                                        <asp:ListItem Text="No" Selected="True" Value="0"  />
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold" align="left">
                                                    Port No.</td>
                                                <td style="font-weight: bold" align="left">
                                                    :</td>
                                                <td style="font-weight: bold" align="left">
                                                    <asp:TextBox ID="textPortNo" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Installation Date </b>
                                                </td>
                                                <td>
                                                    <b>:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="textInstallDate" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />&nbsp; 
                                                        <a href="javascript:ShowCalendar('textInstallDate', 655, 300);">
                                                        <img alt="Show calendar control" src="images/Calendar.jpg" style="border-right: #5f7afc 1px solid;border-top: #5f7afc 1px solid; border-left: #5f7afc 1px solid; border-bottom: #5f7afc 1px solid" title="Show calendar control" />
                                                        </a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold" align="left">
                                                    Immobilizer</td>
                                                <td style="font-weight: bold" align="left">
                                                    :</td>
                                                <td style="font-weight: bold" align="left"><asp:DropDownList ID="ddlImmobilizer" runat="server" Width="185px">
                                                    <asp:ListItem Value="False">No</asp:ListItem>
                                                    <asp:ListItem Value="True">Yes</asp:ListItem>
                                                </asp:DropDownList></td>
                                            </tr>
                                                <tr>
                                                <td style="font-weight: bold" align="left">
                                                    Roaming</td>
                                                <td style="font-weight: bold" align="left">
                                                    :</td>
                                                <td style="font-weight: bold" align="left"><asp:DropDownList ID="ddlroaming" runat="server" Width="185px">
                                                    <asp:ListItem Value="False">No</asp:ListItem>
                                                    <asp:ListItem Value="True">Yes</asp:ListItem>
                                                </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td style="font-weight: bold">
                                                    Tank 1 Size</td>
                                                <td style="font-weight: bold">
                                                    :</td>
                                                <td style="font-weight: bold">
                                                    <asp:TextBox ID="txttank1size" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td style="font-weight: bold">
                                                    Tank 2 Size</td>
                                                <td style="font-weight: bold">
                                                    :</td>
                                                <td style="font-weight: bold">
                                                    <asp:TextBox ID="txttank2size" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td style="font-weight: bold">
                                                    Tank 1 Shape</td>
                                                <td style="font-weight: bold">
                                                    :</td>
                                                <td style="font-weight: bold">
                                                    <asp:DropDownList ID="ddltank1shape" runat="server" Width="185px">
                                                        <asp:ListItem>-- select shape --</asp:ListItem>
                                                        <asp:ListItem>Circular Cylinder</asp:ListItem>
                                                        <asp:ListItem>Octagon</asp:ListItem>
                                                        <asp:ListItem>Rectangle</asp:ListItem>
                                                        <asp:ListItem>Custom</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold">
                                                    Tank 2 Shape</td>
                                                <td align="left" style="font-weight: bold">
                                                    :</td>
                                                <td align="left" style="font-weight: bold">
                                                    <asp:DropDownList ID="ddltank2shape" runat="server" Width="185px">
                                                        <asp:ListItem>-- select shape --</asp:ListItem>
                                                        <asp:ListItem>Circular Cylinder</asp:ListItem>
                                                        <asp:ListItem>Octagon</asp:ListItem>
                                                        <asp:ListItem>Rectangle</asp:ListItem>
                                                        <asp:ListItem>Custom</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td style="font-weight: bold">
                                                </td>
                                                <td style="font-weight: bold">
                                                </td>
                                                <td style="font-weight: bold">
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Small Image</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td valign="middle">
                                                    <asp:FileUpload ID="FileUpload1" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="267px" />&nbsp;
                                                    <a rel="balloon1">
                                                        <img src="images/preview.gif" width="20px" height="20px" alt="Preview" title="Preview"
                                                            onmouseover="smallimagemouseover();" onmouseout="smallimagemouseout();" style="vertical-align: top;" /></a></td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Big Image</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td valign="middle">
                                                    <asp:FileUpload ID="FileUpload2" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana;" Width="267px" />&nbsp;
                                                    <a rel="balloon2">
                                                        <img src="images/preview.gif" width="20px" height="20px" alt="Preview" title="Preview"
                                                            onmouseover="bigimagemouseover();" onmouseout="bigimagemouseout();" style="vertical-align: top;" /></a></td>
                                            </tr>
                                             <tr align="left">
                                                <td align="left" style="font-weight: bold">
                                                    Permit</td>
                                                <td align="left" style="font-weight: bold">
                                                    :</td>
                                                <td align="left" style="font-weight: bold">
                                                    <asp:DropDownList ID="ddlpermit" runat="server" Width="185px">
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <a href="VehicleManagement.aspx">
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
                            <p style="margin-bottom: 0px; font-family: Verdana; font-size: 11px; color: #5373a2;">
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
        <div id="balloon1" class="balloonstyle" style="width: 50px; vertical-align: middle;">
            <img id="smallimagepreview" src="vehiclesmallimages/smallvehicle.gif" alt="smallimage"
                style="border: 1px solid silver; width: 20px; height: 20px; vertical-align: middle;" />
        </div>
        <div id="balloon2" class="balloonstyle" style="width: 100px; vertical-align: middle;">
            <img id="bigimagepreview" src="vehiclebigimages/bigvehicle.gif" alt="bigimage" style="border: 1px solid silver;
                width: 100px; height: 100px; vertical-align: middle;" />
        </div>
    </form>
</body>
</html>
