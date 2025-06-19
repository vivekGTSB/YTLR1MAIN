<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" Inherits="YTLWebApplication.AVLS.AddViewerVehicle" Codebehind="AddViewerVehicle.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add Viewer Vehicle</title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />

    <script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script>

    <script type="text/javascript" src="jsfiles/balloontip.js"></script>

    <script type="text/javascript">
    function mysubmit()
    {
       if(document.getElementById("ddlviewername").value=="-- select viewer name --")
        {
            alert("Please select viewer name");
            return false;
        }
        else if(document.getElementById("ddlplateno").value=="-- select vehicle --") 
        {
            alert("Please select vehicle");
            return false;   
        }
    }
    function cancel()
    {
        var formobj=document.getElementById("adduserform");
        formobj.reset();
    }
   function ShowCalendar(strTargetDateField, intLeft, intTop)
    {
        txtTargetDateField = strTargetDateField;
        
        var divTWCalendarobj=document.getElementById("divTWCalendar");
        divTWCalendarobj.style.visibility = 'visible';
        divTWCalendarobj.style.left = intLeft+"px";
        divTWCalendarobj.style.top = intTop+"px"; selecteddate(txtTargetDateField);   
    }  
    function CheckBoxListSelect(cbControl, state)
    {    
       var chkBoxList = document.getElementById(cbControl);
        var chkBoxCount= chkBoxList.getElementsByTagName("input");
        for(var i=0;i<chkBoxCount.length;i++) 
        {
            chkBoxCount[i].checked = state;
        }
        
        return false; 
    }
    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="adduserform" method="post" runat="server">

        <script type="text/javascript" language="javascript">DrawCalendarLayout();</script>

        <center>
            <div>
                <br />
                <img alt="Add New Viewer Vehicle Details" src="images/AddNewViewerVehicleDetails.jpg" />&nbsp;<br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table width="750px" style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Add New Viewer Vehicle Details :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 750px; border: solid 1px #3952F9; color: #5f7afc;">
                                        <table style="width: 750px;">
                                            <tr align="left">
                                                <td align="left" width="16%">
                                                    <b>Viewer Name</b></td>
                                                <td align="left" width="5">
                                                    <b>:</b></td>
                                                <td align="left" width="80%">
                                                    <asp:DropDownList ID="ddlViewerName" runat="server" Width="340px">
                                                        <asp:ListItem>-- select viewer name --</asp:ListItem>
                                                    </asp:DropDownList></td>
                                            </tr>
                                            <tr align="left">
                                                <td align="left" width="16%">
                                                    <b>Vehicles<br />
                                            Select All</b>&nbsp;<input type="checkbox" onclick="javascript: CheckBoxListSelect ('<%= ddlPlateno.ClientID %>',this.checked)"
                                                title="Check to select all plate numbers" /></td>
                                                <td align="left" width="5">
                                                    <b>:</b></td>
                                                <td align="left" width="80%">                                                   
                                                    <div style="padding: 0px; margin: 0px; text-align: left; height: 230px; width: 580px;
                                                        overflow: scroll; overflow-x: hidden">
                                                        <asp:CheckBoxList ID="ddlPlateno" runat="server" Font-Names="Verdana" Font-Size="11px"
                                                            Width="100%" TabIndex="1" RepeatColumns="4" RepeatDirection="Horizontal">
                                                        </asp:CheckBoxList></div>
                                                </td>
                                            </tr>
                                            <tr align="left">
                                                <td align="left" width="16%">
                                                    <b>Start Time</b></td>
                                                <td align="left" width="5">
                                                    <b>:</b></td>
                                                <td align="left" width="80%">
                                                    <input id="txtBeginDate" runat="server" enableviewstate="false" name="txtBeginDate"
                                                        readonly="readonly" style="width: 70px" type="text" value="<%=strBeginDate%>" />&nbsp;<a
                                                            href="javascript:ShowCalendar('txtBeginDate', 250, 250);" style="text-decoration: none">
                                                            <img alt="Show calendar control" height="14" src="images/Calendar.jpg" style="border-right: blue 1px solid;
                                                                border-top: blue 1px solid; border-left: blue 1px solid; border-bottom: blue 1px solid"
                                                                title="Show calendar control" width="19" />
                                                        </a><b style="color: #5f7afc">&nbsp;&nbsp;&nbsp;&nbsp;Hour :&nbsp;</b>
                                                    <asp:DropDownList ID="ddlbh" runat="server" EnableViewState="False" Font-Names="verdana"
                                                        Font-Size="12px" Width="45px">
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
                                                    </asp:DropDownList><b style="color: #5f7afc"> &nbsp;&nbsp;&nbsp;&nbsp;Min :&nbsp;</b>
                                                    <asp:DropDownList ID="ddlbm" runat="server" EnableViewState="False" Font-Names="verdana"
                                                        Font-Size="12px" Width="45px">
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
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="16%">
                                                    <b>End Time</b></td>
                                                <td align="left" width="5">
                                                    <b>:</b></td>
                                                <td align="left" width="80%">
                                                    <input id="txtEndDate" runat="server" enableviewstate="false" name="txtEndDate" readonly="readonly"
                                                        style="width: 70px" type="text" value="<%=strEndDate%>" />&nbsp;<a href="javascript:javascript:ShowCalendar('txtEndDate', 250, 250);"
                                                            style="text-decoration: none">
                                                            <img alt="Show calendar control" height="14" src="images/Calendar.jpg" style="border-right: blue 1px solid;
                                                                border-top: blue 1px solid; border-left: blue 1px solid; border-bottom: blue 1px solid"
                                                                title="Show calendar control" width="19" />
                                                        </a><b style="color: #5f7afc">&nbsp;&nbsp;&nbsp;&nbsp;Hour :&nbsp;</b>
                                                    <asp:DropDownList ID="ddleh" runat="server" EnableViewState="False" Width="45px">
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
                                                        <asp:ListItem Selected="True" Value="23">23</asp:ListItem>
                                                    </asp:DropDownList><b style="color: #5f7afc">&nbsp;&nbsp;&nbsp;&nbsp; Min :&nbsp;</b>
                                                    <asp:DropDownList ID="ddlem" runat="server" EnableViewState="False" Width="45px">
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
                                                        <asp:ListItem Selected="True" Value="59">59</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="16%">
                                                    <b>Consign Note</b></td>
                                                <td align="left" width="5">
                                                    <b>:</b></td>
                                                <td align="left" width="80%">
                                                    <asp:TextBox ID="txtConsignNote" runat="server" Width="335px"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td align="left" width="16%">
                                                    <b>Remarks</b></td>
                                                <td align="left" width="5">
                                                    <b>:</b></td>
                                                <td align="left" width="80%">
                                                    <asp:TextBox ID="txtRemarks" runat="server" Rows="4" TextMode="MultiLine" Width="335px"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td align="center" width="16%">
                                                    <br />
                                                    <a href="ViewerVehicleManagement.aspx">
                                                        <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                            title="Back" /></a>
                                                </td>
                                                <td colspan="2" align="center" valign="middle" width="80%">
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
                                Copyright © 2009 Global Telematics Sdn Bhd. All rights reserved.
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
