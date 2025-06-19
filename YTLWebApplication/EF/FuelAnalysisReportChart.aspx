<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.FuelAnalysisReportChart" Codebehind="FuelAnalysisReportChart.aspx.vb" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fuel Analysis Report</title>
    <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen"/>
     <link type="text/css"  href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
     <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
     <script type="text/javascript" src="jsfiles/balloontip.js"></script>
      <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />

    <script type="text/javascript" src="jsfiles/balloontip.js"></script>

    <script type="text/javascript" language="javascript">

     $(function () {
          $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
          });

          $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

          });
      });

   

   
var ec=<%=ec %>;
function mysubmit()
{
    var plateno=document.getElementById("ddlpleate").value;
    
      document.getElementById("platenotemp").value=plateno;
    if (plateno=="--Select User Name--")
    {
        alert("Please select user name and vehicle plate number");
        return false;
     }   
    if (plateno=="--Select Plate No--")
    {
         alert("Please select vehicle plate number");
         return false;         
    }
    var bigindatetime=document.getElementById("txtBeginDate").value+" "+document.getElementById("ddlbh").value+":"+document.getElementById("ddlbm").value;
    var enddatetime=document.getElementById("txtEndDate").value+" "+document.getElementById("ddleh").value+":"+document.getElementById("ddlem").value;
    
    var fdate=Date.parse(bigindatetime);
    var sdate=Date.parse(enddatetime);
    
    var diff=(sdate-fdate)*(1/(1000*60*60*24));
    var days=parseInt(diff)+1;
    if(days>7)
    {
        return confirm("You selected "+days+" days of data.So it will take more time to execute.\nAre you sure you want to proceed ? ");
    }
    return true;
     
}
function download()
{
    if(ec==true)
    {
	ec=false;
        var plateno=document.getElementById("ddlpleate").value;
       
        document.getElementById("plateno").value=plateno;

        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("Please click Submit button to obtain latest result.");
    }
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

function mouseover(path)
{
    document.getElementById("bigimage").src="vehiclebigimages/"+path;
}
function pmouseover(unitid,versionid,simno)
{
    document.getElementById("balloon2").innerHTML="Unit ID : "+unitid+"<br/>Version ID : "+versionid+"<br/>SIM No : "+simno;
}
function gmapmouseover(x,y)
{
    document.getElementById("mapimage").src="images/maploading.gif";
    document.getElementById("mapimage").src="GussmannMap.aspx?x="+x+"&y="+y;
}
 
    </script>

<script type="text/javascript">

  var _gaq = _gaq || [];
  _gaq.push(['_setAccount', 'UA-32500429-1']);
  _gaq.push(['_setDomainName', 'avls.com.my']);
  _gaq.push(['_trackPageview']);

  (function() {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'https://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
  })();

</script>

</head>
<body>
    <form id="form1" runat="server">


        <center>
            <div>
                
<br />
             <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Fuel Analysis Report And Chart</b>

         <br />
                <br />
                <table width="80%">
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Fuel Analysis Report &amp; Chart &nbsp;:</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 420px; border: solid 1px #3952F9;">
                                        <table style="width: 420px;">
                                            <tbody>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Begin Date</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                            id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />&nbsp;<b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlbh" runat="server" Width="42px" EnableViewState="False"
                                                            Font-Size="12px" Font-Names="verdana">
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
                                                        </asp:DropDownList><b style="color: #5f7afc;">&nbsp;Min&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlbm" runat="server" Width="42px" EnableViewState="False"
                                                            Font-Size="12px" Font-Names="verdana">
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
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">End Date</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                            id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" />&nbsp;<b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddleh" runat="server" Width="42px" 
                                                            EnableViewState="False" Font-Size="12px" Font-Names="verdana">
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
                                                            <asp:ListItem Value="23" Selected="True">23</asp:ListItem>
                                                        </asp:DropDownList><b style="color: #5f7afc;">&nbsp;Min&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlem" runat="server" Width="42px" 
                                                            EnableViewState="False">
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
                                                            <asp:ListItem Value="59" Selected="True">59</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">User Name</b></td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlUsername" runat="server" Width="255px" 
                                                            Font-Size="12px" Font-Names="verdana" AutoPostBack="True">
                                                            <asp:ListItem>--Select User Name--</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Plate No </b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlpleate" runat="server" Width="255px" Font-Size="12px" 
                                                            Font-Names="verdana">
                                                            <asp:ListItem>--Select User Name--</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                              
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Records/Page</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="noofrecords" runat="server" Width="70px" Font-Size="12px" Font-Names="verdana"
                                                            EnableViewState="False">
                                                            <asp:ListItem>10</asp:ListItem>
                                                            <asp:ListItem>20</asp:ListItem>
                                                            <asp:ListItem>30</asp:ListItem>
                                                            <asp:ListItem>40</asp:ListItem>
                                                            <asp:ListItem>50</asp:ListItem>
                                                            <asp:ListItem>75</asp:ListItem>
                                                            <asp:ListItem>100</asp:ListItem>
                                                            <asp:ListItem>500</asp:ListItem>
                                                             <asp:ListItem>1000</asp:ListItem>
                                                            <asp:ListItem Selected="True">5000</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <br />
                                                         </td>
                                                  <td colspan="2" align="left" >
                                                        <br />
                                                          <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit"  ToolTip="Submit" />
                                                       <a href="javascript:download();" class="button"><span class="ui-button-text ">Download</span>  </a>    
                                                    <a href="javascript:print();" class="button" style="vertical-align:top; width:55px;"><span class="ui-button-text"
                                                        title="Print">Print</a>
                                                          </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td align="center">
                                        <chart:webchartviewer id="WebChartViewer1" runat="server" visible="False"></chart:webchartviewer>
                                        <asp:Image ID="Image1" runat="server" Visible="False" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr align="left">
                        <td>
                            <table align="center">
                                <tr>
                                    <td align="left" colspan="2">
                                        <asp:Label ID="Label3" runat="server" Font-Names="Arial" Font-Size="X-Small" ForeColor="Red"
                                            Text="*  RM/KM will be higher if idling time is too long for the selected period. "></asp:Label><br />
                                        <asp:Label ID="Label2" runat="server" Font-Names="Arial" Font-Size="X-Small" ForeColor="Red"
                                            Text="** Minimum distance of 50KM is required for effective calculation on RM/KM."></asp:Label></td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                        <div style="font-family: Verdana; font-size: 11px;">
                                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" EnableViewState="False"
                                                Font-Bold="False" Font-Overline="False" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                                HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-Height="22px"
                                                HeaderStyle-HorizontalAlign="Center"
                                                PageSize="1" Width="930px">
                                                <PagerSettings PageButtonCount="5" />
                                                <PagerStyle BackColor="White" Font-Bold="True" Font-Italic="False" Font-Names="Verdana"
                                                    Font-Overline="False" Font-Size="Small" Font-Strikeout="False" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" />
                                                <Columns>
                                                    <asp:BoundField DataField="Begin Date Time" HeaderText="Begin Date Time" HtmlEncode="False">
                                                        <ItemStyle Width="130px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="End Date Time" HeaderText="End Date Time" HtmlEncode="False">
                                                        <ItemStyle Width="130px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Max Speed" HeaderText="Average Speed" HtmlEncode="False"
                                                        Visible="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Mileage" HeaderText="Distance Travelled (KM) **" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Fuel" HeaderText="Fuel Consumption (L)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Fuel Cost" HeaderText="Fuel Consumption Cost (RM)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Liter/KM" HeaderText="Litre/KM" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="KM/Liter" HeaderText="KM/Litre" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Cost/liter" HeaderText="RM/KM *" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" Width="70px" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <AlternatingRowStyle BackColor="Lavender" />
                                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="12px" ForeColor="White"
                                                    Height="22px" HorizontalAlign="Center" />
                                            </asp:GridView>
                                            <asp:Label ID="lblSubsidy" runat="server" Font-Names="Arial" Font-Size="X-Small"
                                                ForeColor="Red" Text="*** RM in bracket is calculated by subsidy price." Visible="False"></asp:Label>
                                            <br />
                                            <br />
                                            <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" EnableViewState="False"
                                                Font-Bold="False" Font-Overline="False" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                                HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-Height="22px"
                                                HeaderStyle-HorizontalAlign="Center" PageSize="1" Width="930px">
                                                <PagerSettings PageButtonCount="5" />
                                                <PagerStyle BackColor="White" Font-Bold="True" Font-Italic="False" Font-Names="Verdana"
                                                    Font-Overline="False" Font-Size="Small" Font-Strikeout="False" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" />
                                                <Columns>
                                                    <asp:BoundField DataField="Idling Time" HeaderText="Total Idling Time" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Idling Fuel" HeaderText="Idling Fuel Consumption (L)"
                                                        HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Hour Idling Fuel" HeaderText="Idling Fuel/Hr" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Idling Cost" HeaderText="Idling Cost/Hr" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Total Idling Cost" HeaderText="Total Idling Cost (RM)"
                                                        HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <AlternatingRowStyle BackColor="Lavender" />
                                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="12px" ForeColor="White"
                                                    Height="22px" HorizontalAlign="Center" />
                                            </asp:GridView>
                                            &nbsp;<br />
                                            <br />
                                            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False" OnRowDataBound="GridView1_RowDataBound"
                                                EnableViewState="False" Font-Bold="False" Font-Overline="False" HeaderStyle-BackColor="#465AE8"
                                                HeaderStyle-Font-Bold="True" HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF"
                                                HeaderStyle-Height="22px" HeaderStyle-HorizontalAlign="Center" PageSize="20"
                                                Width="930px">
                                                <PagerSettings PageButtonCount="5" />
                                                <PagerStyle BackColor="White" Font-Bold="True" Font-Italic="False" Font-Names="Verdana"
                                                    Font-Overline="False" Font-Size="Small" Font-Strikeout="False" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" />
                                                <Columns>
                                                    <asp:BoundField DataField="S No" HeaderText="No">
                                                        <ItemStyle HorizontalAlign="Center" Width="5px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Date Time" HeaderText="Date Time" HtmlEncode="False">
                                                        <ItemStyle Width="130px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="GPS AV" HeaderText="GPS AV" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Speed" HeaderText="Speed (KM/H)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Ignition" HeaderText="Ignition" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Odometer" HeaderText="Odometer<br/>(KM)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="OdometerIncrement" HeaderText="Odometer Increment<br/>(KM)"
                                                        HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Tank Level" HeaderText="Tank Level (MM)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" Width="100px"  />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Tank Volume" HeaderText="Tank Volume (L)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" Width="100px"  />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Tank Level 2" HeaderText="Tank Level 2 (MM)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" Width="100px"  />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Tank Volume 2" HeaderText="Tank Volume 2 (L)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" Width="100px"  />
                                                    </asp:BoundField>
                                                   <asp:BoundField DataField="Total Volume" HeaderText="Total Volume" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" Width="100px"  />
                                                    </asp:BoundField>  
                                                </Columns>
                                                <AlternatingRowStyle BackColor="Lavender" />
                                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="12px" ForeColor="White"
                                                    Height="22px" HorizontalAlign="Center" />
                                            </asp:GridView>
                                            &nbsp;&nbsp;
                                           <div align="right">&nbsp;</div> 
                                            <% If show = True Then%> 
                                            <label id="pages" style="font-family: Verdana; font-size: 11px; font-weight: bold;"> Pages</label>
                                            <%End If%><br />
                                            <br />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </center>
        <div id="balloon3" class="balloonstyle" style="width: 258px; vertical-align: middle;">
            <img id="mapimage" src="images/maploading.gif" alt="" style="border: 1px solid silver;
                width: 256px; height: 256px; vertical-align: middle;" />
        </div>
        <input id="values" type="hidden" runat="server" value="" />
         <input type="hidden" id="platenotemp" runat="server"  name="title" value="" />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Fuel Analysis Report" />
        <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
