<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" Inherits="YTLWebApplication.AVLS.AddUser" Codebehind="AddUser.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add User</title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />


    <script src="jsfiles/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="jsfiles/balloontip.js"></script>

    <script type="text/javascript">
    function mysubmit()
    {
        if(document.getElementById("userid").value=="")
        {
            alert("Please enter user ID");
            return false;
        }
        else if(document.getElementById("username").value=="")
        {
            alert("Please enter user name");
            return false;
        }
        else if(document.getElementById("password").value=="") 
        {
            alert("Please enter password");
            return false;   
        }
        else if(document.getElementById("password").value!=document.getElementById("confirmpassword").value) 
        {
            alert("password,confirm password are not equal");
            return false;   
        }
        else if(document.getElementById("companyname").value=="") 
        {
            alert("Please enter company name");
            return false;   
        }
        else if(document.getElementById("mobilenumber").value=="") 
        {
            alert("Please enter mobile number");
            return false;   
        }
        else if(document.getElementById("phonenumber").value=="") 
        {
            alert("Please enter phone number");
            return false;   
        }
        else if(document.getElementById("faxnumber").value=="") 
        {
            alert("Please enter fax number");
            return false;   
        }
         else if(document.getElementById("emailid").value=="") 
        {
            alert("Please enter email id");
            return false;   
        }
        else if(document.getElementById("streetname").value=="") 
        {
            alert("Please enter street name");
            return false;   
        }
        else if(document.getElementById("postalcode").value=="") 
        {
            alert("Please enter postal code number");
            return false;   
        }
        else if(document.getElementById("ddlstate").value=="--Select State--") 
        {
            alert("Please select state name");
            return false;   
        }
        else if(document.getElementById("ddlrole").value=="--Select Role--") 
        {
            alert("Please select user role");
            return false;   
        }
//        else if(document.getElementById("emailid").value!="")
//        {
//            var emailvalue=document.getElementById("emailid").value;
//            var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
//            if(emailvalue.match(emailRegEx))
//            {
//                return true;
//            }
//            else
//            {
//                alert('Please enter a valid email address.');
//                return false;
//            }
//        }
    }
    function cancel()
    {
        var formobj=document.getElementById("adduserform");
        formobj.reset();
    }
    </script>
    
    <script type="text/javascript">
        $(document).ready(function() {
            if ($('#HiddenField1').val() == "SVWONG") {
                $(".trhideSvwong").hide();
            };
        });
    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="adduserform" method="post" runat="server">
        <center>
            <div>
                <br />
                <img alt="Add User" src="images/AddUser.jpg" />
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add New User Details :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 350px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 350px;">
                                            <tr align="left" class="trhideSvwong">
                                                <td style="width: 130px;">
                                                    <b>User ID</b></td>
                                                <td style="width: 10px;">
                                                    <b>:</b></td>
                                                <td style="width: 210px;">
                                                    <a rel="balloon1">
                                                        <asp:TextBox ID="userid" runat="Server" Style="border-right: #cbd6e4 1px solid; border-top: #cbd6e4 1px solid;
                                                            font-size: 10pt; border-left: #cbd6e4 1px solid; color: #0b3d62; border-bottom: #cbd6e4 1px solid;
                                                            font-family: Verdana" Width="180px" ReadOnly="True" /></a>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td style="width: 130px;">
                                                    <b>User Name</b></td>
                                                <td style="width: 10px;">
                                                    <b>:</b></td>
                                                <td style="width: 210px;">
                                                    <asp:TextBox ID="username" runat="Server" Style="border-right: #cbd6e4 1px solid;
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
                                                    <asp:TextBox ID="password" runat="Server" Style="border-right: #cbd6e4 1px solid;
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
                                            <tr align="left" class="trhideSvwong">
                                                <td>
                                                    <b>Company Name</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="companyname" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Mobile Number</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="mobilenumber" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Phone Number</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="phonenumber" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Fax Number</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="faxnumber" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Email ID</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="emailid" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Street Name</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="streetname" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />&nbsp;
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Postal Code</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="postalcode" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                        border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                        color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>State Name</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlstate" runat="server" Width="186px" ForeColor="#0B3D62">
                                                        <asp:ListItem Value="--Select State--">--Select State--</asp:ListItem>
                                                        <asp:ListItem Value="Johor">Johor</asp:ListItem>
                                                        <asp:ListItem Value="Kedah">Kedah</asp:ListItem>
                                                        <asp:ListItem Value="Kelantan">Kelantan</asp:ListItem>
                                                        <asp:ListItem Value="Kuala Lumpur">Kuala Lumpur</asp:ListItem>
                                                        <asp:ListItem Value="Melaka">Melaka</asp:ListItem>
                                                        <asp:ListItem Value="N.Sembilan">N.Sembilan</asp:ListItem>
                                                        <asp:ListItem Value="Pahang">Pahang</asp:ListItem>
                                                        <asp:ListItem Value="Penang">Penang</asp:ListItem>
                                                        <asp:ListItem Value="Perak">Perak</asp:ListItem>
                                                        <asp:ListItem Value="Perlis">Perlis</asp:ListItem>
                                                        <asp:ListItem Value="Sabah">Sabah</asp:ListItem>
                                                        <asp:ListItem Value="Sarawak">Sarawak</asp:ListItem>
                                                        <asp:ListItem Value="Selangor">Selangor</asp:ListItem>
                                                        <asp:ListItem Value="Terengganu">Terengganu</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left" class="trhideSvwong">
                                                <td>
                                                    <b>User Role</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlrole" runat="server" Width="186px" ForeColor="#0B3D62">
                                                        <asp:ListItem Value="--Select Role--">--Select Role--</asp:ListItem>
                                                        <asp:ListItem Value="Admin">Admin</asp:ListItem>
                                                        <asp:ListItem Value="SuperUser">Super User</asp:ListItem>
                                                        <asp:ListItem Value="User">User</asp:ListItem>
                                                        <asp:ListItem Value="Operator">Operator</asp:ListItem>
                                                        <asp:ListItem Value="AdminViewer1">AdminViewer1</asp:ListItem>
                                                        <asp:ListItem Value="AdminViewer2">AdminViewer2</asp:ListItem>
                                                        <asp:ListItem Value="ChinaOperator">ChinaOperator</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left" class="trhideSvwong">
                                                <td>
                                                    <b>Site Access</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlaccess" runat="server" Width="186px" ForeColor="#0B3D62">
                                                        <asp:ListItem Value="0">Allow</asp:ListItem>
                                                        <asp:ListItem Value="1">Instant Deny</asp:ListItem>
                                                        <asp:ListItem Value="2">Deny After 1 Week </asp:ListItem>
                                                        <asp:ListItem Value="3">Deny After 2 Weeks </asp:ListItem>
                                                        <asp:ListItem Value="4">Deny After 1 Month </asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr align="left" class="trhideSvwong">
                                                <td><b>User Level</b>
                                                </td>
                                                <td><b>:</b>
                                                </td>
                                                <td><asp:DropDownList ID="ddllevel" runat="server" Width="186px" ForeColor="#0B3D62">
                                                    <asp:ListItem Value="0">0 - All Fuel</asp:ListItem>
                                                    <asp:ListItem Value="1">1 - Fuel Reports &amp; Charts</asp:ListItem>
                                                    <asp:ListItem Value="2">2 - Fuel Charts Only</asp:ListItem>
                                                    <asp:ListItem Value="3">3 - ALL Temperature</asp:ListItem>
                                                    <asp:ListItem Value="4">4 -  ALL Fuel &amp; Temperature</asp:ListItem>
                                                    <asp:ListItem Value="5">5 -  Viewer</asp:ListItem>
                                                    <asp:ListItem Value="6">6 -  Lite</asp:ListItem>
                                                    <asp:ListItem Value="7">7 -  User OP</asp:ListItem>
                                                </asp:DropDownList></td>
                                            </tr>
                                            <tr class="trhideSvwong">
                                                <td align="left">
                                                    <b>Server</b></td>
                                                <td align="left">
                                                    <b>:</b></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlServer" runat="server" ForeColor="#0B3D62" Width="186px">
                                                        <asp:ListItem Value="192.168.1.21">Server YTL</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr class="trhideSvwong">
                                                <td align="left">
                                                    <b>ERP Module</b></td>
                                                <td align="left">
                                                    <b>:</b></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlERP" runat="server" ForeColor="#0B3D62" Width="186px">
                                                        <asp:ListItem Value="0">No</asp:ListItem>
                                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <a href="UserManagement.aspx">
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
                                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.
                            </p>
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
        <div id="balloon1" class="balloonstyle">
            Last User ID is :
            <%=lastuserid %>
        </div>
        
        <asp:HiddenField ID="HiddenField1" runat="server" />
        
    </form>
</body>
</html>
