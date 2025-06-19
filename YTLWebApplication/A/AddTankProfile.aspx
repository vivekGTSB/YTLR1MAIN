<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.AddVehToTankProfile" Codebehind="AddTankProfile.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Tank Profile </title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />

   <%-- <script type="text/javascript" src="jsfiles/balloontip.js"></script>--%>

    <%--<script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script>--%>

    <script type="text/javascript"> 
   
   function mysubmit()
    {
        
        if(document.getElementById("ddlplatenumber").value=="--Select Plate Number--") 
        {
            alert("Please select Plate Number");
            return false;   
        }
        if(document.getElementById("TankLevel").value=="") 
        {
            alert("Please enter TankLevel");
            return false;   
        }
        if(document.getElementById("TankVolume").value=="") 
        {
            alert("Please enter  TankVolume");
            return false;   
        }
          else
        {
            return true;
        }
           
    }
    function cancel()
    {
        var formobj=document.getElementById("addprofilegenerator");
        formobj.reset();
    }
    
    </script>

</head>
<body style="margin: 0px;">
    <form id="addprofilegenerator" runat="server">
        <center>
            <div>
                <br />
               <%-- <img alt="Add Fuel" src="images/AddFuel.jpg" />--%>
                <br />
                <br />

                 <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add Tank Profile :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 350px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 450px;">
                                            <tr align="left">
                                                <td>
                                                    <b>Plate Number</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td colspan="2">
                                                    <asp:DropDownList ID="ddlplatenumber" runat="server" Width="191px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Tank Level</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="TankLevel" runat="server" Width="183px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="TankLevel b/t (200-500)" ControlToValidate="TankLevel" MaximumValue="500" MinimumValue="200" SetFocusOnError="True"></asp:RangeValidator></td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Tank Volume</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td colspan="2">
                                                    <asp:TextBox ID="TankVolume" runat="server" Width="185px">
                                                    </asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <a href="AdminManagement.html">
                                                        <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                            title="Back" /></a>
                                                </td>
                                                <td colspan="2" align="center" valign="middle">
                                                    <br />
                                                    <asp:ImageButton ID="btn_Add" runat="server" ImageUrl="images/Add.jpg" ToolTip="Submit">
                                                    </asp:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg"
                                                        alt="Cancel" style="border: 0px; vertical-align: top; cursor: pointer" title="Cancel"
                                                        onclick="javascript:cancel();" />
                                                   
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.
                                <%--  *This is a beta page.--%>
                            </p>
                        </td>
                    </tr>
                </table>
                 <asp:Label ID="Label1" runat="server" Text="Label" Width="258px"></asp:Label>
            </div>
        </center>
       
            </form>
</body>
</html>
