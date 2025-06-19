<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.AddShipToCode" Codebehind="AddShipToCode.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
   <title>Add Ship To Code</title> 
  <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen"/>
     <link type="text/css"  href="cssfiles/jquery-ui.css" rel="stylesheet" />
     <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
     <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
     <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
     <script type="text/javascript" src="jsfiles/balloontip.js"></script>

  

     <script type="text/javascript"  language ="javascript">
         $(function () {
             $("#tbxdatetime").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
             });
         });



         //        }
         //        function mysubmit() {
         //            if (document.getElementById("ddluser").value == "--Select User Name--") {
         //                alert("Please select user name");
         //                return false;
         //            }
         //            if (document.getElementById("ddlplatenumber").value == "--Select Plate Number--") {
         //                alert("Please select Plate Number");
         //                return false;
         //            }
         //            if (document.getElementById("tbxdatetime").value == "") {
         //                alert("Please enter  date");
         //                return false;
         //            }
         //            if (document.getElementById("ddlbh").value == "") {
         //                alert("Please enter  hours");
         //                return false;
         //            }
         //            if (document.getElementById("ddlbm").value == "") {
         //                alert("Please enter  mintues");
         //                return false;
         //            }
         //            if (document.getElementById("ddloil").value == "--Select Oil--") {
         //                alert("Please select fuel type");
         //                return false;
         //            }
         //            else if (document.getElementById("tbxlitters").value == "") {
         //                alert("Please enter liters");
         //                return false;
         //            }
         //            else if (document.getElementById("tbxcost").value == "") {
         //                alert("Please enter cost");
         //                return false;
         //            }
         //            else {
         //                return true;
         //            }

         //        }
         function cancel() {
             var formobj = document.getElementById("addfuelform");
             formobj.reset();
         }
      
    </script>
</head>
<body style="margin: 0px;">
    <form id="addfuelform" runat="server">
        <center>
            <div>
                <br />
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Add Ship To 
                Code</b><br />
                <br />

                

                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add Ship To Code&nbsp; Details :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 350px; border: solid 1px #3952F9; color: #5f7afc;">
                                       <table style="font-family: Verdana; font-size: 11px; width: 433px;">
                                          <tr align="left">
                                                <td>
                                                   <b style="color: #5f7afc;"> Ship To Code</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtShipToCode" runat="Server" Style="border: #cbd6e4 1px solid;"
                                                        Width="180px" />
                                                </td>
                                            </tr>
                                           <tr align="left">
                                                <td>
                                                   <b style="color: #5f7afc;"> Name</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtName" runat="Server" Style="border: #cbd6e4 1px solid;"
                                                        Width="180px" />
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b style="color: #5f7afc;">Address1</b>
                                                </td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtaddress1" runat="Server" Style="border: #cbd6e4 1px solid;"
                                                        Width="180px" />
                                                </td>
                                            </tr>
                                               <tr align="left">
                                                <td>
                                                    <b style="color: #5f7afc;">Address2</b>
                                                </td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtaddress2" runat="Server" Style="border: #cbd6e4 1px solid;"
                                                        Width="180px" />
                                                </td>
                                            </tr> 
                                             <tr align="left">
                                                <td>
                                                    <b style="color: #5f7afc;">Address3</b>
                                                </td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtaddress3" runat="Server" Style="border: #cbd6e4 1px solid;"
                                                        Width="180px" />
                                                </td>
                                            </tr>  
                                             <tr align="left">
                                                <td>
                                                    <b style="color: #5f7afc;">Address4</b>
                                                </td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:TextBox ID="txtaddress4" runat="Server" Style="border: #cbd6e4 1px solid;"
                                                        Width="180px" />
                                                </td>
                                            </tr> 
                                           
                                     
                                            
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                    <a href="ShipToCodeManagement.aspx" class="button" style="width:60px;" ><span class="ui-button-text ">Back</span>  </a>
                                                </td>
                                                <td colspan="2" align="center" valign="middle">
                                                    <br />
                                                    <asp:Button ID="ImageButton2" class="action blue" runat="server" Text="Submit"  
                                                            ToolTip="Submit"/>
                                                             <asp:Button ID="ImageButton3" class="action blue" runat="server" Text="Cancel"  
                                                            ToolTip="Cancel" OnClientClick="javascript:cancel();"/>
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

