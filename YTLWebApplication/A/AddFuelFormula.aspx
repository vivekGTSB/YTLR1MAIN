<%@ Page Language="vb" AutoEventWireup="false" Inherits="YTLWebApplication.AVLS.AddFuelFormula" Codebehind="AddFuelFormula.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add Fuel Formula</title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />

    <script type="text/javascript" src="jsfiles/balloontip.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script>  

    <script type="text/javascript">
    function mysubmit()
    {
    if(document.getElementById("cbUpdateOld").checked)
    {
         if(document.getElementById("ddluserid").value=="-- select username --")
         {alert("please select username.");return false;}
         else if(document.getElementById("ddlplateno").value=="-- select plateno --")
        {alert("please select plateno.");return false;}
         else if(document.getElementById("textvolumename").value=="")
        {alert("please key in FV id.");return false;}
        else if(document.getElementById("textcalibrationname").value=="")
        {alert("please key in FL id.");return false;}
        else
        {
            return true;
        }
        alert();   }
    else
    { 
        document.getElementById("textRemarks").disabled='disabled';  
         if(document.getElementById("ddluserid").value=="-- select username --")
         {alert("please select username.");return false;}
         else if(document.getElementById("ddlplateno").value=="-- select plateno --")
        {alert("please select plateno.");return false;}
         else if(document.getElementById("textvolumename").value=="")
        {alert("please key in FV id.");return false;}
        else if(document.getElementById("textvolumeoffset").value=="")
        {alert("please key in offset value.");return false;}
        else if(document.getElementById("textvolumeformula").value=="")
        {alert("please key in formula.");return false;}
        else if(document.getElementById("textcalibrationname").value=="")
        {alert("please key in FL id.");return false;}
        else
        {
            return true;
        }
        alert();   } 
         
    }
    
    function cancel()
    {
        var formobj=document.getElementById("form1");
        formobj.reset();
    }
   
   function calcCalibration()
{
   vInitial = new Number(); 
   vInitial = this.form1.tInitial.value;
   vIncrement = new Number();
   vIncrement = this.form1.tIncrement.value;
   vHeight = new Number();
   vHeight = this.form1.tHeight.value;
   if (vHeight>=0)
   {
   this.form1.txtN0b.value = vInitial;
   this.form1.txtX0.value = Number(vInitial) + Number(vIncrement);
   }else {this.form1.txtN0b.value = ""; this.form1.txtX0.value= "";}
   if (vHeight>=1)
   {
   this.form1.txtN1b.value = Number(this.form1.txtX0.value) + Number("1");
   this.form1.txtX1.value = Number(this.form1.txtN1b.value) + Number(vIncrement);
   }else {this.form1.txtN1b.value = ""; this.form1.txtX1.value= "";}
   if (vHeight>=2)
   { 
   this.form1.txtN2b.value = Number(this.form1.txtX1.value) + Number("1");
   this.form1.txtX2.value = Number(this.form1.txtN2b.value) + Number(vIncrement);
   }else {this.form1.txtN2b.value = ""; this.form1.txtX2.value= "";}
   if (vHeight>=3)
   {
   this.form1.txtN3b.value = Number(this.form1.txtX2.value) + Number("1");
   this.form1.txtX3.value = Number(this.form1.txtN3b.value) + Number(vIncrement);
   }else {this.form1.txtN3b.value = ""; this.form1.txtX3.value= "";}
   if (vHeight>=4)
   {
   this.form1.txtN4b.value = Number(this.form1.txtX3.value) + Number("1");
   this.form1.txtX4.value = Number(this.form1.txtN4b.value) + Number(vIncrement);
   }else {this.form1.txtN4b.value = ""; this.form1.txtX4.value= "";}
   if (vHeight>=5)
   {
   this.form1.txtN5b.value = Number(this.form1.txtX4.value) + Number("1");
   this.form1.txtX5.value = Number(this.form1.txtN5b.value) + Number(vIncrement);
   }else {this.form1.txtN5b.value = ""; this.form1.txtX5.value= "";}
   if (vHeight>=6)
   {
   this.form1.txtN6b.value = Number(this.form1.txtX5.value) + Number("1");
   this.form1.txtX6.value = Number(this.form1.txtN6b.value) + Number(vIncrement);
   }else {this.form1.txtN6b.value = ""; this.form1.txtX6.value= "";}
   if (vHeight>=7)
   {
   this.form1.txtN7b.value = Number(this.form1.txtX6.value) + Number("1");
   this.form1.txtX7.value = Number(this.form1.txtN7b.value) + Number(vIncrement);
   }else {this.form1.txtN7b.value = ""; this.form1.txtX7.value= "";}
   if (vHeight>=8)
   {
   this.form1.txtN8b.value = Number(this.form1.txtX7.value) + Number("1");
   this.form1.txtX8.value = Number(this.form1.txtN8b.value) + Number(vIncrement);
   }else {this.form1.txtN8b.value = ""; this.form1.txtX8.value= "";}
   if (vHeight>=9)
   {
   this.form1.txtN9b.value = Number(this.form1.txtX8.value) + Number("1");
   this.form1.txtX9.value = Number(this.form1.txtN9b.value) + Number(vIncrement);
   }else {this.form1.txtN9b.value = ""; this.form1.txtX9.value= "";}
} 
    </script>

</head>
<body style="margin: 0px;">
    <form id="form1" method="post" runat="server">
        <center>
            <div>
                <img alt="Add Fuel Formula" src="images/AddFuelFormula.gif" />&nbsp;<br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add Fuel Formula :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 450px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 450px;">
                                            <tr align="left">
                                                <td width="110">
                                                    <b>User Name</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="ddlUserid" runat="server" Width="185px" Font-Names="Verdana" AutoPostBack="True">
                                                        <asp:ListItem>-- select username --</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>Plate No.</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3"><asp:DropDownList ID="ddlPlateNo" runat="server" Width="185px" Font-Names="Verdana">
                                                    <asp:ListItem>-- select username --</asp:ListItem>
                                                </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>Remarks</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="textRemarks" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="90%" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>Tank No.</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="2">
                                                    <asp:DropDownList ID="ddlTankNo" runat="server" Width="185px" Font-Names="Verdana">
                                                        <asp:ListItem>1</asp:ListItem>
                                                        <asp:ListItem>2</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td bgcolor="lavender" colspan="4" style="font-weight: bold; color: black">
                                                    &nbsp;Tank Volume Information</td>
                                                <td bgcolor="lavender" colspan="1" style="font-weight: bold; color: black" width="25%">
                                                    
                                                    <asp:CheckBox ID="cbUpdateOld" runat="server" Font-Bold="False" Font-Names="Verdana"
                                                        Font-Size="9px" Text="existing level" />
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <strong>Formula Name</strong></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="textVolumeName" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>Offset</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="textVolumeOffset" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>Formula</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="textVolumeFormula" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="90%" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>Formula Type</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3"><asp:DropDownList ID="ddlVolumeType" runat="server" Width="185px" Font-Names="Verdana">
                                                    <asp:ListItem>Tank Volume</asp:ListItem>
                                                    <asp:ListItem>Tank Cylinder</asp:ListItem>
                                                </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td bgcolor="lavender" colspan="4" style="font-weight: bold; color: black">
                                                    &nbsp;Tank Calibration Information</td>
                                                <td bgcolor="lavender" colspan="1" style="font-weight: bold; color: black">
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>Formula Name</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="textCalibrationName" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td style="font-weight: bold" width="110">
                                                    Formula Type</td>
                                                <td style="font-weight: bold" width="2">
                                                    :</td>
                                                <td colspan="3" style="font-weight: bold">
                                                    <asp:DropDownList ID="ddlCalibrationType" runat="server" Width="185px" Font-Names="Verdana">
                                                        <asp:ListItem>Calibration</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td bgcolor="#ffcc66" colspan="5">
                                                    &nbsp;Initial :
                                                    <asp:TextBox ID="tInitial" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" />
                                                    &nbsp;Increment :
                                                    <asp:TextBox ID="tIncrement" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" />
                                                    &nbsp;Height :
                                                    <asp:TextBox ID="tHeight" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="30px" />
                                                    &nbsp;&nbsp;
                                                    <input id="calSubmit" type="button" value="calculate" onclick="calcCalibration();" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td bgcolor="lavender" colspan="5" style="font-weight: bold; color: black">
                                                    &nbsp;Calibration Details</td>
                                            </tr>
                                            <tr align="left">
                                                <td bgcolor="lavender" style="color: black" width="110">
                                                    Height</td>
                                                <td bgcolor="lavender" style="color: black" width="2">
                                                </td>
                                                <td bgcolor="lavender" style="color: black" width="35%">
                                                    Mininum Value</td>
                                                <td bgcolor="lavender" style="color: black" width="35%" colspan="2">
                                                    Maxminum Value</td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>0</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td width="35%">
                                                    <asp:TextBox ID="txtN0b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX0" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>100</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td width="35%"><asp:TextBox ID="txtN1b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX1" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>200</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td width="35%"><asp:TextBox ID="txtN2b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX2" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td width="110">
                                                    <b>300</b></td>
                                                <td width="2">
                                                    <b>:</b></td>
                                                <td width="35%"><asp:TextBox ID="txtN3b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX3" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold" width="110">
                                                    400</td>
                                                <td align="left" style="font-weight: bold" width="2">
                                                    :</td>
                                                <td align="left" style="font-weight: bold" width="35%">
                                                    <asp:TextBox ID="txtN4b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td align="left" style="font-weight: bold" width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX4" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold" width="110">
                                                    500</td>
                                                <td align="left" style="font-weight: bold" width="2">
                                                    :</td>
                                                <td align="left" style="font-weight: bold" width="35%">
                                                    <asp:TextBox ID="txtN5b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td align="left" style="font-weight: bold" width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX5" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold" width="110">
                                                    600</td>
                                                <td align="left" style="font-weight: bold" width="2">
                                                    :</td>
                                                <td align="left" style="font-weight: bold" width="35%">
                                                    <asp:TextBox ID="txtN6b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td align="left" style="font-weight: bold" width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX6" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold" width="110">
                                                    700</td>
                                                <td align="left" style="font-weight: bold" width="2">
                                                    :</td>
                                                <td align="left" style="font-weight: bold" width="35%">
                                                    <asp:TextBox ID="txtN7b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td align="left" style="font-weight: bold" width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX7" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold" width="110">
                                                    800</td>
                                                <td align="left" style="font-weight: bold" width="2">
                                                    :</td>
                                                <td align="left" style="font-weight: bold" width="35%">
                                                    <asp:TextBox ID="txtN8b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td align="left" style="font-weight: bold" width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX8" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold" width="110">
                                                    900</td>
                                                <td align="left" style="font-weight: bold" width="2">
                                                    :</td>
                                                <td align="left" style="font-weight: bold" width="35%">
                                                    <asp:TextBox ID="txtN9b" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                                <td align="left" style="font-weight: bold" width="35%" colspan="2">
                                                    <asp:TextBox ID="txtX9" runat="server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="60px" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="110">
                                                    <br />
                                                    <a href="fuelformulamanagement.aspx">
                                                        <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                            title="Back" /></a>
                                                </td>
                                                <td colspan="4" align="right" valign="middle">
                                                    <br />
                                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/submit_s.jpg"
                                                        ToolTip="Submit"></asp:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg"
                                                            alt="Cancel" style="border: 0px; vertical-align: top; cursor: pointer" title="Cancel"
                                                            onclick="javascript:cancel();" /></td>
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
        <div id="balloon1" class="balloonstyle" style="width: 52px; vertical-align: middle;">
            <img id="smallimagepreview" src="vehiclesmallimages/smallvehicle.gif" alt="smallimage"
                style="border: 1px solid silver; width: 20px; height: 20px; vertical-align: middle;" />
        </div>
        <div id="balloon2" class="balloonstyle" style="width: 102px; vertical-align: middle;">
            <img id="bigimagepreview" src="vehiclebigimages/bigvehicle.gif" alt="bigimage" style="border: 1px solid silver;
                width: 100px; height: 100px; vertical-align: middle;" />
        </div>
    </form>
</body>
</html>
