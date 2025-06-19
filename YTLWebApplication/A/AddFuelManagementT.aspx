<%@ Page Language="vb" AutoEventWireup="false" Inherits="YTLWebApplication.AVLS.AddFuelManagementT" Codebehind="AddFuelManagementT.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add Fuel Management</title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />

    <script type="text/javascript" src="jsfiles/balloontip.js"></script>

        <script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script>

    <script type="text/javascript"> 
   function CloseWin()
	{
		window.close();
		opener.location.reload(false);
	}
    function entercost()
    {
     document.getElementById ('cbxliters').checked=false;
     document.getElementById ('cbxcost').checked=true;
     document.getElementById('tbxlitters').readOnly = true;
       document.getElementById('tbxlitters').style.backgroundColor = "ActiveBorder";
     document.getElementById('tbxcost').readOnly = false;
      document.getElementById('tbxcost').style.backgroundColor = "";
    
    }
    function enterliters()
    {
      document.getElementById ('cbxcost').checked=false;   
       document.getElementById ('cbxliters').checked=true;
      document.getElementById('tbxcost').readOnly = true;
      document.getElementById('tbxcost').style.backgroundColor = "ActiveBorder";
      document.getElementById('tbxlitters').readOnly = false;
      document.getElementById("tbxlitters").style.backgroundColor = "" ;
    }
    function changetotalcost()
    {
        var liters =document.getElementById ("tbxlitters");
        var totcost=document.getElementById ("tbxcost");
        var onelitercost=document.getElementById ("txtonelitercost");
        if(isNaN(totcost.value))
        {
            alert('enter numeric values');              
             var len = totcost.value.length;
             if (len > 1){
             var ans = totcost.value.substring(0,len-1);
             totcost.value=ans;
             }
             else{
             totcost.value="";
             }
             return false; 
        }
         

         if(!isNaN(totcost.value)&&(totcost.value!="")&&!isNaN(onelitercost.value)&&(onelitercost.value!=""))
         liters.value=(totcost.value/onelitercost.value).toFixed(2);                  
    
   }
 
   function changeonelitercost()
   {
        var liters =document.getElementById ("tbxlitters");
        var totcost=document.getElementById ("tbxcost");
        var onelitercost=document.getElementById ("txtonelitercost");
        if(isNaN(onelitercost.value))
        {
            alert('enter numeric values');
            var len = onelitercost.value.length;
             if (len > 1){
             var ans = onelitercost.value.substring(0,len-1);
             onelitercost.value=ans;
             }
             else{
             onelitercost.value="";
             }                      
            return false;
        }
        var check=document.getElementById('tbxcost').readOnly;
        if (check == false)
        {if(!isNaN(totcost.value)&&(totcost.value!="")&&!isNaN(onelitercost.value)&&(onelitercost.value!=""))
         liters.value=(totcost.value/onelitercost.value).toFixed(2);
       }else{
          if(!isNaN(onelitercost.value)&&(onelitercost.value!="")&&!isNaN(liters.value)&&(liters.value!=""))
         totcost.value= (onelitercost.value*liters.value).toFixed(2);       
       }
}
function changeliters()
{
        var liters =document.getElementById ("tbxlitters");
        var totcost=document.getElementById ("tbxcost");
        var onelitercost=document.getElementById ("txtonelitercost");
        if(isNaN(liters.value))
        {
             alert('enter numeric values');
             var len = liters.value.length;
             if (len > 1){
             var ans = liters.value.substring(0,len-1);
             liters.value=ans;
             }
             else{
             liters.value="";
             }         
             return false;    
        }
         if(!isNaN(onelitercost.value)&&(onelitercost.value!="")&&!isNaN(liters.value)&&(liters.value!=""))
         totcost.value= (onelitercost.value*liters.value).toFixed(2);  
}
    
function fuelchange(obj)
{
    var fueltype=obj.value;
    switch(obj.value)
    {
     case "--Select Fuel Type--":
        document.getElementById("txtonelitercost").value="";
        break;
     case "Diesel":
        document.getElementById("txtonelitercost").value="1.75";
        break;
     case "LPG":
        document.getElementById("txtonelitercost").value="";
        break;
     case "Petrol":
        document.getElementById("txtonelitercost").value="1.80";
        break;  
     case "Petrol V-Power":
        document.getElementById("txtonelitercost").value="1.90";
        break;
//     default:
//        document.getElementById("txtonelitercost").value="";
//        break;
//     
    } 
   

}
    function mysubmit()
    {
        if(document.getElementById("ddluser").value=="--Select User Name--") 
        {
            alert("Please select user name");
            return false;   
        }
        if(document.getElementById("ddlplatenumber").value=="--Select Plate Number--") 
        {
            alert("Please select Plate Number");
            return false;   
        }
        if(document.getElementById("tbxdatetime").value=="") 
        {
            alert("Please enter  date");
            return false;   
        }
        if(document.getElementById("ddlbh").value=="") 
        {
            alert("Please enter  hours");
            return false;   
        }
        if(document.getElementById("ddlbm").value=="") 
        {
            alert("Please enter  mintues");
            return false;   
        }
         if(document.getElementById("ddloil").value=="--Select Oil--") 
        {
            alert("Please select fuel type");
            return false;   
        }
        else if(document.getElementById("tbxlitters").value=="") 
        {
            alert("Please enter liters");
            return false;                  
        }
        else if(document.getElementById("tbxcost").value=="") 
        {
            alert("Please enter cost");
            return false;   
        }
        else
        {
            return true;
        }
           
    }
    function cancel()
    {
        var formobj=document.getElementById("addfuelform");
        formobj.reset();
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
    <form id="addfuelform" runat="server">
        <center>
            <div>
                <br />
                            <b id="imgHead" runat="server"  style="font-family: Verdana; font-size: 20px; color: #38678B;">Add Refuel Receipt</b>
                            <%--<label id="imgHead" runat="server"  style="font-family: Verdana; font-weight:bold ; font-size: 20px; color: #38678B;">Add Refuel Receipt</label> 
                          --%>
                
                <br />
                <br />

                <script type="text/javascript">javascript:DrawCalendarLayout();</script>

                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add Refuel Receipt Details :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 450px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 450px;">
                                            <tr align="left">
                                                <td>
                                                    <b>User Name</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddluser" runat="server" Width="185px">
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Plate Number</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlplatenumber" runat="server" Width="185px">
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b>Date Time</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td align="left">
                                                    <input type="text" id="tbxdatetime" runat="server" style="width: 85px;" readonly="readonly" 
                                                        enableviewstate="false" />&nbsp;<a href="javascript:javascript:ShowCalendar('tbxdatetime', 790, 160);" style="text-decoration: none;">
                                                            <img alt="Show calendar control" title="Show calendar control" src="images/Calendar.jpg"
                                                                style="border: solid 1px #5f7afc;" /></a>&nbsp; <strong>Hour :</strong>&nbsp;<asp:DropDownList ID="ddlbh" runat="server" CssClass="dclass"
                                                            EnableViewState="False" Width="40px">
                                                            <asp:ListItem Value="00">00</asp:ListItem>
                                                            <asp:ListItem Value="01">01</asp:ListItem>
                                                            <asp:ListItem Value="02">02</asp:ListItem>
                                                            <asp:ListItem Value="03">03</asp:ListItem>
                                                            <asp:ListItem Value="04">04</asp:ListItem>
                                                            <asp:ListItem Value="05">05</asp:ListItem>
                                                            <asp:ListItem Value="06">06</asp:ListItem>
                                                            <asp:ListItem Value="07">07</asp:ListItem>
                                                            <asp:ListItem Value="08">08</asp:ListItem>
                                                            <asp:ListItem Value="09">09</asp:ListItem>
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
                                                            <asp:ListItem Value="21">21</asp:ListItem>
                                                            <asp:ListItem Value="22">22</asp:ListItem>
                                                            <asp:ListItem Value="23">23</asp:ListItem>
                                                        </asp:DropDownList>&nbsp;<b style="color: #5f7afc">Min : </b>
                                                    <asp:DropDownList ID="ddlbm" runat="server" EnableViewState="False" Width="40px">
                                                        <asp:ListItem Value="00">00</asp:ListItem>
                                                        <asp:ListItem Value="01">01</asp:ListItem>
                                                        <asp:ListItem Value="02">02</asp:ListItem>
                                                        <asp:ListItem Value="03">03</asp:ListItem>
                                                        <asp:ListItem Value="04">04</asp:ListItem>
                                                        <asp:ListItem Value="05">05</asp:ListItem>
                                                        <asp:ListItem Value="06">06</asp:ListItem>
                                                        <asp:ListItem Value="07">07</asp:ListItem>
                                                        <asp:ListItem Value="08">08</asp:ListItem>
                                                        <asp:ListItem Value="09">09</asp:ListItem>
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="11">11</asp:ListItem>
                                                        <asp:ListItem Value="12">12</asp:ListItem>
                                                        <asp:ListItem Value="13">13</asp:ListItem>
                                                        <asp:ListItem Value="14">14</asp:ListItem>
                                                        <asp:ListItem Value="15">15</asp:ListItem>
                                                        <asp:ListItem Value="16">16</asp:ListItem>
                                                        <asp:ListItem Value="16">17</asp:ListItem>
                                                        <asp:ListItem Value="18">18</asp:ListItem>
                                                        <asp:ListItem Value="19">19</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                        <asp:ListItem Value="21">21</asp:ListItem>
                                                        <asp:ListItem Value="22">22</asp:ListItem>
                                                        <asp:ListItem Value="23">23</asp:ListItem>
                                                        <asp:ListItem Value="24">24</asp:ListItem>
                                                        <asp:ListItem Value="25">25</asp:ListItem>
                                                        <asp:ListItem Value="26">26</asp:ListItem>
                                                        <asp:ListItem Value="27">27</asp:ListItem>
                                                        <asp:ListItem Value="28">28</asp:ListItem>
                                                        <asp:ListItem Value="29">29</asp:ListItem>
                                                        <asp:ListItem Value="30">30</asp:ListItem>
                                                        <asp:ListItem Value="31">31</asp:ListItem>
                                                        <asp:ListItem Value="32">32</asp:ListItem>
                                                        <asp:ListItem Value="33">33</asp:ListItem>
                                                        <asp:ListItem Value="34">34</asp:ListItem>
                                                        <asp:ListItem Value="35">35</asp:ListItem>
                                                        <asp:ListItem Value="36">36</asp:ListItem>
                                                        <asp:ListItem Value="37">37</asp:ListItem>
                                                        <asp:ListItem Value="38">38</asp:ListItem>
                                                        <asp:ListItem Value="39">39</asp:ListItem>
                                                        <asp:ListItem Value="40">40</asp:ListItem>
                                                        <asp:ListItem Value="41">41</asp:ListItem>
                                                        <asp:ListItem Value="42">42</asp:ListItem>
                                                        <asp:ListItem Value="43">43</asp:ListItem>
                                                        <asp:ListItem Value="44">44</asp:ListItem>
                                                        <asp:ListItem Value="45">45</asp:ListItem>
                                                        <asp:ListItem Value="46">46</asp:ListItem>
                                                        <asp:ListItem Value="47">47</asp:ListItem>
                                                        <asp:ListItem Value="48">48</asp:ListItem>
                                                        <asp:ListItem Value="49">49</asp:ListItem>
                                                        <asp:ListItem Value="50">50</asp:ListItem>
                                                        <asp:ListItem Value="51">51</asp:ListItem>
                                                        <asp:ListItem Value="52">52</asp:ListItem>
                                                        <asp:ListItem Value="53">53</asp:ListItem>
                                                        <asp:ListItem Value="54">54</asp:ListItem>
                                                        <asp:ListItem Value="55">55</asp:ListItem>
                                                        <asp:ListItem Value="56">56</asp:ListItem>
                                                        <asp:ListItem Value="57">57</asp:ListItem>
                                                        <asp:ListItem Value="58">58</asp:ListItem>
                                                        <asp:ListItem Value="59">59</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Fuel Type</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b></td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddloil" runat="server" Width="185px" EnableViewState="False">
                                                        <asp:ListItem>--Select Fuel Type--</asp:ListItem>
                                                        <asp:ListItem Selected="True">Diesel</asp:ListItem>
                                                        <asp:ListItem>LPG</asp:ListItem>
                                                        <asp:ListItem>Petrol</asp:ListItem>
                                                        <asp:ListItem>Petrol V-Power</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Total Cost</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <input id="tbxcost" readonly="readonly"  style="border: #cbd6e4 1px solid; background-color: ActiveBorder; width: 180px;" type="text" runat="Server" onkeyup="return changetotalcost(); " />
                                                    <input id="cbxcost" type="radio" onclick="javascript:entercost();"
                                                        title="click to activate total cost" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Total Liters</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <input id="tbxlitters" type="text" runat="Server" style="border: #cbd6e4 1px solid; width: 180px;" onkeyup="return changeliters();" />
                                                    <input id="cbxliters" type="radio" onclick="javascript:enterliters();" title="click to activate total liters" checked="CHECKED" /></td>
                                            </tr>
                                            <tr align="left">
                                                <td>
                                                    <b>Cost per 1 Liter</b></td>
                                                <td>
                                                    <b>:</b></td>
                                                <td>
                                                    <input id="txtonelitercost" style="border: #cbd6e4 1px solid; width: 180px;" type="text"
                                                        runat="Server" onkeyup="return changeonelitercost();" value="1.75" /></td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <br />
                                                    <a href="FuelManagement.aspx">
                                                        </a></td>
                                                <td colspan="2" align="right" valign="middle">
                                                    <br />
                                                    &nbsp;<asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="images/submit_s.jpg"
                                                        ToolTip="Submit"></asp:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<img src="images/cancel_s.jpg"
                                                            alt="Cancel" style="border: 0px; vertical-align: top; cursor: pointer" title="Cancel"
                                                            onclick="javascript:cancel();" /></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                                Copyright © 2009 Global Telematics Sdn Bhd. All rights reserved.<%--  *This is a beta page.--%></p>
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
