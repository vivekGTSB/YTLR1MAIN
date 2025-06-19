<%@ Page Language="VB" Debug ="true"  AutoEventWireup="false"  EnableEventValidation="false" Inherits="YTLWebApplication.AVLS.AddPOI" Codebehind="AddPOI.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Point</title>

    <script type="text/javascript">
    function mysubmit()
    {
   
        if(document.getElementById("RadioButton1").checked==true)
        {
         if(document.getElementById("ddlusers").value=="--Select User Name--")
            {
                alert("Please select user name");
                return false;
            }
            if(document.getElementById("poinametextbox").value=="")
            {
                alert("Please enter POI name");
                return false;
            }
            else if(document.getElementById("latitudetextbox").value=="") 
            {
                alert("Please enter latitude value");
                return false;   
            }
            else if(document.getElementById("longitudetextbox").value=="") 
            {
                alert("Please enter longitude value");
                return false;   
            }
            else if( document.getElementById("FileUpload1").value!="")
            {
            //---
                var file = document.getElementById("FileUpload1").value;
                var type = "";

                //create an array of acceptable files
                var validExtensions = new Array(".jpeg", ".jpg",".png",".gif");
                var allowSubmit = false;

                //if our control contains no file then alert the user
                if (file.indexOf("\\") == -1)
                {
                    alert("You must select a file before hitting the upload button");
                    return false;
                }
                else
                {
                    //get the file type
                    type = file.slice(file.indexOf("\\") + 1);
                    var ext = file.slice(file.lastIndexOf(".")).toLowerCase();
                    //loop through our array of extensions
                    for (var i = 0; i < validExtensions.length; i++) 
                    {
                        //check to see if it's the proper extension
                        if (validExtensions[i] == ext) 
                        { 
                        //it's the proper extension
                        allowSubmit = true; 
                        }
                    }
                }

                //now check the final bool value
                if (allowSubmit == false)
                {
                    //let the user know they selected a wrong file extension
                    alert("Only files with extensions " + (validExtensions.join("  ").toUpperCase()) + " are allowed");
                    return false;
                }
                else
                {
                    return true
                }     
    //--  
            }
            else
            {
                if(document.getElementById("RadioButton1").checked==true)
                {
                    alert("Public POIs will be added up on approvel");
                }
               
                return true;
            }
        }
        else
        {
            if(document.getElementById("ddlusers").value=="--Select User Name--")
            {
                alert("Please select user name");
                return false;
            }
            else if(document.getElementById("poinametextbox").value=="")
            {
                alert("Please enter POI name");
                return false;
            }
            else if(document.getElementById("latitudetextbox").value=="") 
            {
                alert("Please enter latitude value");
                return false;   
            }
            else if(document.getElementById("longitudetextbox").value=="") 
            {
                alert("Please enter longitude value");
                return false;   
            }
            else if( document.getElementById("FileUpload1").value!="")
            {
            //---
                var file = document.getElementById("FileUpload1").value;
                var type = "";

                //create an array of acceptable files
                var validExtensions = new Array(".jpeg", ".jpg",".png",".gif");
                var allowSubmit = false;

                //if our control contains no file then alert the user
                if (file.indexOf("\\") == -1)
                {
                    alert("You must select a file before hitting the upload button");
                    return false;
                }
                else
                {
                    //get the file type
                    type = file.slice(file.indexOf("\\") + 1);
                    var ext = file.slice(file.lastIndexOf(".")).toLowerCase();
                    //loop through our array of extensions
                    for (var i = 0; i < validExtensions.length; i++) 
                    {
                        //check to see if it's the proper extension
                        if (validExtensions[i] == ext) 
                        { 
                        //it's the proper extension
                        allowSubmit = true; 
                        }
                    }
                }

                //now check the final bool value
                if (allowSubmit == false)
                {
                    //let the user know they selected a wrong file extension
                    alert("Only files with extensions " + (validExtensions.join("  ").toUpperCase()) + " are allowed");
                    return false;
                }
                else
                {         
                               
//                    var heavyImage = new Image(); 
//                    heavyImage.src = document.getElementById("FileUpload1").value;                              
//                    var himage= heavyImage.width; 
//                    var wimage=heavyImage.height;
//                    if ((himage>256))// && (wimage >256)
//                    {
//                    alert("Only image files with width and height less then 256*256 dimentions are allowed")
//                        return false;
//                    }
//                    else
//                    {
                        return true;
                    //}
                }     
                //--  
            }
            else
            {
                if(document.getElementById("RadioButton1").checked==true)
                {
                    alert("Public POIs will be added up on approvel");
                }
                return true;
            }
        }
     
    }
    function radioclick()
    {
//        if(document.getElementById("RadioButton1").checked==true)
//        { var userid= document.getElementById("ddlusers")
//        document.getElementById("ddlusers").name=userid.name
//            document.getElementById("ddlusers").disabled="disabled";
//        }
//        else
//        {
//            document.getElementById("ddlusers").disabled=false;   
//        }
    }
    function cancel()
    {
        var formobj=document.getElementById("addpointform");
        formobj.reset();
    }
    function change(obj)
    {
       document.getElementById("iconimage").src="images/"+obj.value+".gif";
       document.getElementById("poitypevalue").value=obj.value;
       
    }
    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="addpointform" runat="server">
        <center>
            <div>
                <br />
                <img alt="Add POI" src="images/add_poi.jpg" />&nbsp;<br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add New POI&nbsp;:</b></td>
                                </tr>
                                <tr>
                                    <td style="padding-right: 5px; padding-top: 5px; padding-bottom: 5px; border: solid 1px #3952F9;">
                                        <table style="width: 470px;">
                                            <tbody>
                                                <tr>
                                                    <td align="left" style="width: 1304px;">
                                                        <b style="color: #5f7afc;">User Name</b>
                                                    </td>
                                                    <td style="width: 5px;">
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left" style="width: 986px;">
                                                        <asp:DropDownList ID="ddlusers" runat="server" Width="304px">
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td align="left" style="width: 1304px">
                                                        <b style="color: #5f7afc;">POI Name</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="poinametextbox" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                            border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                            color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="300px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" style="width: 1304px">
                                                        <b style="color: #5f7afc;">Select POI type </b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <select id="poitypecombobox" name="poitypecombobox" style="width: 304px;" onchange="change(this);">
                                                            <optgroup label="Common">
                                                                <option value="1">&nbsp;&nbsp;Private house, residence</option>
                                                                <option value="2">&nbsp;&nbsp;Office building or complex</option>
                                                                <option value="3">&nbsp;&nbsp;Factory, production, farm</option>
                                                                <option value="4">&nbsp;&nbsp;Other</option>
                                                            </optgroup>
                                                            <optgroup label="Government">
                                                                <option value="5">&nbsp;&nbsp;Administrative building</option>
                                                                <option value="6">&nbsp;&nbsp;Police</option>
                                                                <option value="7">&nbsp;&nbsp;Fire depot</option>
                                                                <option value="8">&nbsp;&nbsp;Post office</option>
                                                                <option value="9">&nbsp;&nbsp;Military building or area</option>
                                                                <option value="10">&nbsp;&nbsp;Cemetery</option>
                                                            </optgroup>
                                                            <optgroup label="Geography">
                                                                <option value="11">&nbsp;&nbsp;Town, city</option>
                                                                <option value="12">&nbsp;&nbsp;Region, neighborhood</option>
                                                                <option value="13">&nbsp;&nbsp;Village</option>
                                                                <option value="14">&nbsp;&nbsp;Island</option>
                                                                <option value="15">&nbsp;&nbsp;Hill, mountain</option>
                                                                <option value="16">&nbsp;&nbsp;Lake</option>
                                                                <option value="17">&nbsp;&nbsp;Beach</option>
                                                                <option value="18">&nbsp;&nbsp;Park, garden</option>
                                                                <option value="19">&nbsp;&nbsp;Bridge</option>
                                                                <option value="56">&nbsp;&nbsp;Mile Marker</option>
                                                            </optgroup>
                                                            <optgroup label="Dining and leisure">
                                                                <option value="20">&nbsp;&nbsp;Restaurant, pizza, food</option>
                                                                <option value="21">&nbsp;&nbsp;Cafe, coffee or tea</option>
                                                                <option value="22">&nbsp;&nbsp;Bar, pub</option>
                                                                <option value="23">&nbsp;&nbsp;Night club</option>
                                                                <option value="24">&nbsp;&nbsp;Theatre</option>
                                                                <option value="25">&nbsp;&nbsp;Cinema</option>
                                                                <option value="26">&nbsp;&nbsp;Other leisure (casino, bowling...)</option>
                                                            </optgroup>
                                                            <optgroup label="Tourism">
                                                                <option value="27">&nbsp;&nbsp;Hotel, hostel</option>
                                                                <option value="28">&nbsp;&nbsp;Museum</option>
                                                                <option value="29">&nbsp;&nbsp;Monument, memorial</option>
                                                                <option value="30">&nbsp;&nbsp;Church, temple, mosque</option>
                                                                <option value="31">&nbsp;&nbsp;Interesting place</option>
                                                                <option value="32">&nbsp;&nbsp;Other for tourists</option>
                                                            </optgroup>
                                                            <optgroup label="Transportation">
                                                                <option value="33">&nbsp;&nbsp;Airport</option>
                                                                <option value="34">&nbsp;&nbsp;Railroad station</option>
                                                                <option value="35">&nbsp;&nbsp;Metro station</option>
                                                                <option value="36">&nbsp;&nbsp;Station (bus, tram, ferry...)</option>
                                                                <option value="37">&nbsp;&nbsp;Gas station, petrol</option>
                                                                <option value="38">&nbsp;&nbsp;Other transportation</option>
                                                            </optgroup>
                                                            <optgroup label="Shopping and services">
                                                                <option value="39">&nbsp;&nbsp;Shopping</option>
                                                                <option value="40">&nbsp;&nbsp;Market</option>
                                                                <option value="41">&nbsp;&nbsp;Bank, ATM, exchange</option>
                                                                <option value="42">&nbsp;&nbsp;Services (rent, repair, ...)</option>
                                                                <option value="43">&nbsp;&nbsp;Other commercial</option>
                                                            </optgroup>
                                                            <optgroup label="Health and care">
                                                                <option value="44">&nbsp;&nbsp;Hospitals</option>
                                                                <option value="45">&nbsp;&nbsp;Drugstore, pharmacy</option>
                                                                <option value="46">&nbsp;&nbsp;Other health and care (dentist, beauty, SPA...)</option>
                                                            </optgroup>
                                                            <optgroup label="Sport">
                                                                <option value="47">&nbsp;&nbsp;Golf</option>
                                                                <option value="48">&nbsp;&nbsp;Stadium</option>
                                                                <option value="49">&nbsp;&nbsp;Tennis</option>
                                                                <option value="50">&nbsp;&nbsp;Swimming pool</option>
                                                                <option value="51">&nbsp;&nbsp;Other sports (gym, fitness...)</option>
                                                            </optgroup>
                                                            <optgroup label="Educational">
                                                                <option value="52">&nbsp;&nbsp;School, college</option>
                                                                <option value="53">&nbsp;&nbsp;University</option>
                                                                <option value="54">&nbsp;&nbsp;Library</option>
                                                                <option value="55">&nbsp;&nbsp;Other educational (campus, lab...)</option>
                                                            </optgroup>
                                                        </select>
                                                        &nbsp;<img id="iconimage" src="images/1.gif" style="width: 20px; height: 20px;" alt="Icon" />
                                                    </td>
                                                </tr>
                                                <tr style="display:none;">
                                                    <td align="left" style="width: 1304px;">
                                                        <b style="color: #5f7afc;">Public or Private</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:RadioButton ID="RadioButton1" runat="server" GroupName="POI" Text="Public POI" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="RadioButton2" runat="server"
                                                            GroupName="POI" Text="Private POI" Checked="true" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" style="width: 1304px">
                                                        <b style="color: #5f7afc;">Latitude</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="latitudetextbox" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                            border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                            color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="300px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" style="width: 1304px">
                                                        <b style="color: #5f7afc;">Longitude </b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:TextBox ID="longitudetextbox" runat="Server" Style="border-right: #cbd6e4 1px solid;
                                                            border-top: #cbd6e4 1px solid; font-size: 10pt; border-left: #cbd6e4 1px solid;
                                                            color: #0b3d62; border-bottom: #cbd6e4 1px solid; font-family: Verdana" Width="300px" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" style="width: 1304px;">
                                                        <b style="color: #5f7afc;">Zoom Level</b>
                                                    </td>
                                                    <td style="width: 5px;">
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left" style="width: 986px;">
                                                        &nbsp;&nbsp;&nbsp; <b style="color: #5f7afc;">Min </b>&nbsp;
                                                        <select id="ddlmin" name="ddlmin" style="width: 100px;" runat="server">
                                                            <option value="8" selected="selected">8</option>
                                                            <option value="9">9</option>
                                                            <option value="10">10</option>
                                                            <option value="11">11</option>
                                                            <option value="12">12</option>
                                                            <option value="13">13</option>
                                                            <option value="14">14</option>
                                                            <option value="15">15</option>
                                                            <option value="16">16</option>
                                                            <option value="17">17</option>
                                                            <option value="18">18</option>
                                                            <option value="19">19</option>
                                                            <option value="20">20</option>
                                                        </select>
                                                        &nbsp;&nbsp; <b style="color: #5f7afc;">Max </b>&nbsp;
                                                        <select id="ddlmax" name="ddlmax" style="width: 100px;" runat="server">
                                                            <option value="8">8</option>
                                                            <option value="9">9</option>
                                                            <option value="10">10</option>
                                                            <option value="11">11</option>
                                                            <option value="12">12</option>
                                                            <option value="13">13</option>
                                                            <option value="14">14</option>
                                                            <option value="15">15</option>
                                                            <option value="16">16</option>
                                                            <option value="17">17</option>
                                                            <option value="18">18</option>
                                                            <option value="19">19</option>
                                                            <option value="20" selected="selected">20</option>
                                                        </select>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" style="width: 1304px;">
                                                        <b style="color: #5f7afc;">Upload Image</b>
                                                    </td>
                                                    <td style="width: 5px;">
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <asp:FileUpload ID="FileUpload1" runat="server" Width="304px"  />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style="width: 1304px">
                                                        <br />
                                                        <a href="<%=backpage %>">
                                                            <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                                title="Back" /></a>
                                                    </td>
                                                    <td colspan="2" align="center" valign="middle">
                                                        <br />
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/submit_s.jpg"
                                                            ToolTip="Submit"></asp:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img
                                                                src="images/cancel_s.jpg" alt="Cancel" style="border: 0px; vertical-align: top;
                                                                cursor: pointer" title="Cancel" onclick="javascript:cancel();" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                
            </div>
        </center>
        <asp:HiddenField ID="poitypevalue" runat="server" Value="1" />       
        
    </form>
   </body>
   <%  If errormessage <> "" Then%>
   <script type="text/javascript" language="javascript">
   alert('<%= errormessage%>');
   </script>
   <%  End If%>
</html>
