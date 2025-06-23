<%@ Page Language="VB" AutoEventWireup="false" EnableViewState="false" EnableEventValidation="false" Inherits="YTLWebApplication.VehicleMileageChart" Codebehind="VehicleMileageChart.aspx.vb" %>
<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Vehicle Mileage Chart</title>
    <style media="print" type="text/css">
body {color : #000000;background : #ffffff;font-family : verdana,arial,sans-serif;font-size : 12pt;}
#fcimg
{display : none;}

</style>

    <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen"/>
 <link type="text/css"  href="cssfiles/jquery-ui.css" rel="stylesheet" />
     <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
     <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
     <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
     <script type="text/javascript" src="jsfiles/balloontip.js"></script>
      <script type="text/javascript" language="javascript">
          $(function () {
              $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
              });

              $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

              });
          });
         

 var ec=<%=ec %>;
function download()
{
    if(ec==true)
    {
        var downloadformobj=document.getElementById("downloadform");
        downloadformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
}
function mysubmit()
{
    var plateno=document.getElementById("ddlpleate").value;
    
      document.getElementById("platenotemp").value=plateno;

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
  
}

//function mysubmit()
//{
//    var plateno=document.getElementById("ddlpleate").value;
//    document.getElementById("plateno").value=plateno;
//    alert(plateno);
//    if (plateno=="--Select Plate No--")
//    {
//         alert("Please select vehicle plate number");
//         return false;         
//    }
//    var bigindatetime=document.getElementById("txtBeginDate").value+" "+document.getElementById("ddlbh").value+":"+document.getElementById("ddlbm").value;
//    var enddatetime=document.getElementById("txtEndDate").value+" "+document.getElementById("ddleh").value+":"+document.getElementById("ddlem").value;
//    
//    var fdate=Date.parse(bigindatetime);
//    var sdate=Date.parse(enddatetime);
//    
//    var diff=(sdate-fdate)*(1/(1000*60*60*24));
//    var days=parseInt(diff)+1;
//    if(days>7)
//    {
//        return confirm("You selected "+days+" days of data.So it will take more time to execute.\nAre you sure you want to proceed ? ");
//     
//    }
//  
//}


function getWindowWidth(){if(window.self && self.innerWidth){return self.innerWidth;}if (document.documentElement && document.documentElement.clientWidth){return document.documentElement.clientWidth;}return document.documentElement.offsetWidth;}
function getWindowHeight(){if(window.self && self.innerHeight){return self.innerHeight;}if (document.documentElement && document.documentElement.clientHeight){return document.documentElement.clientHeight;}return document.documentElement.offsetHeight;}

    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;
    font-family: Verdana; font-size: 5px;">
  
    <form id="Form1" method="post" runat="server" style="font-family: Verdana; font-size: 11px;">
       <%-- <div style="position: absolute; top: 0px; left: 0px; width: 15px; height: 15px; font-size: 5px;">
            <img id="fcimg" src="mapfiles/fullscreen1.gif" alt="Full Screen" title="Full Screen"
                onclick="javascript:fullscreen();" /></div>--%>

   

        <center>
            <div>
              
                <br />
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Vehicle Mileage Chart</b>
               
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Vehicle Mileage Chart&nbsp;:</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 450px; border: solid 1px #3952F9;">
                                        <table style="width: 450px;">
                                            <tbody>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Begin Date</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                            id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />&nbsp;<b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlbh" runat="server" Width="42px" EnableViewState="False" Font-Size="12px" Font-Names="verdana">
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
                                                        </asp:DropDownList><b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;Min&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlbm" runat="server" Width="42px" EnableViewState="False" Font-Size="12px" Font-Names="verdana">
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
                                                        <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">End Date</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                            id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" />&nbsp;<b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddleh" runat="server" Width="42px" EnableViewState="False" Font-Size="12px" Font-Names="verdana">
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
                                                        </asp:DropDownList><b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;Min&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlem" runat="server" Width="42px" EnableViewState="False" Font-Size="12px" Font-Names="verdana">
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
                                                        <b style="color: #5f7afc; font-size: 11px; font-family: Verdana;">User Name</b></td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left">
                                                      <asp:DropDownList ID="ddlUsername" runat="server" AutoPostBack="True" Font-Names="verdana"
                                                            Font-Size="12px" Width="256px" >
                                                            <asp:ListItem>--Select User Name--</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Plate No </b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left">
                                                
                                                     <asp:DropDownList ID="ddlpleate" runat="server" Width="257px" Font-Size="12px" 
                                                            Font-Names="verdana">
                                                            <asp:ListItem>--Select Plate No--</asp:ListItem>
                                                        </asp:DropDownList> 
                                                    </td>
                                                 
                                                </tr>
                                                <tr>
                                                    <td align="center">
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
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr align="left">
                        <td align="center" colspan="2">
                            &nbsp;<chart:WebChartViewer ID="WebChartViewer1" runat="server" Visible="False" ImageAlign="Middle"  />
                            <asp:Image ID="Image1" runat="server" Visible="False" />
                        </td>
                    </tr>
                </table>
            </div>
        </center>
      <input type="hidden" id="platenotemp" runat="server"  name="title" value="" />
    </form>
    <form id="googlemapsform" method="post" action="" target="_blank">
    </form>
    <form id="googlemapsform1" method="post" action="">
    </form>
     <form id="downloadform" name="downloadform" method="get" action="DownloadChart.aspx">
        <input type="hidden" id="title" name="title" value="Vehicle Mileage Chart" />

    </form>
</body>
</html>
