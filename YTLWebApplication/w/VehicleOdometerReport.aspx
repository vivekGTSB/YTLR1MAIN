<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.VehicleOdometerReport" Debug="true" Codebehind="VehicleOdometerReport.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vehicle Odometer Report</title>

 <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen"/>
      <link type="text/css"  href="cssfiles/jquery-ui.css" rel="stylesheet" />
      <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
      <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
      <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
      <script type="text/javascript" src="jsfiles/balloontip.js"></script>
      <script type="text/javascript" language="javascript">
          $(function () {
              $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -420, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
              });

              $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -420, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

              });
          });
   </script>
    <script type="text/javascript" language="javascript">  
var ec=<%=ec %>;
function mysubmit()
{
    var plateno=document.getElementById("ddlpleate").value;
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
function ExcelReport()
{
    if(ec==true)
    {
        var plateno=document.getElementById("ddlpleate").value;
       
        document.getElementById("plateno").value=plateno;

        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
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

</head>
<body>
    <form id="form1" runat="server">
              <center>
            <div>
                
<br />
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Vehicle Odometer Report</b>&nbsp;<br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;Vehicle Odometer Report &nbsp;:</b></td>
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
                                                        <b style="color: #5f7afc">User Name</b></td>
                                                    <td>
                                                        <b style="color: #5f7afc">:</b></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlUsername" runat="server" AutoPostBack="True" Font-Names="verdana"
                                                            Font-Size="12px" Width="256px">
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
                                                        <asp:DropDownList ID="ddlpleate" runat="server" Width="256px" Font-Size="12px" Font-Names="verdana">
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
                                                            <asp:ListItem Selected="True">1000</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <br />
                                                       
                                                    </td>
                                                    <td colspan="2" align="left">
                                                        <br />
                                                 <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit"  ToolTip="Submit" />
                                                      <a href="javascript:ExcelReport();" class="button"><span class="ui-button-text ">Save Excel</span>  </a>
                                                     
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
                        <td>
                            <table>
                                <tr>
                                    <td align="center" colspan="2">
                                        <div style="font-family: Verdana; font-size: 11px;">
                                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" EnableViewState="False" OnRowDataBound="GridView2_RowDataBound"
                                                Font-Bold="False" Font-Overline="False" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                                HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-Height="22px"
                                                HeaderStyle-HorizontalAlign="Center" PageSize="1" Width="850px" ShowFooter="True">
                                                <PagerSettings PageButtonCount="5" />
                                                <PagerStyle BackColor="White" Font-Bold="True" Font-Italic="False" Font-Names="Verdana"
                                                    Font-Overline="False" Font-Size="Small" Font-Strikeout="False" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" />
                                                <Columns>
                                                    <asp:BoundField DataField="No" HeaderText="No" HtmlEncode="False">
                                                        <ItemStyle Width="5px" Font-Bold="False" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="PlateNo" HeaderText="Plate No." HtmlEncode="False">
                                                        <ItemStyle Width="140px" HorizontalAlign="Center" Font-Bold="False" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="BeginDateTime" HeaderText="Begin Date Time" HtmlEncode="False">
                                                        <ItemStyle Width="130px" HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="EndDateTime" HeaderText="End Date Time" HtmlEncode="False">
                                                        <ItemStyle Width="130px" HorizontalAlign="Center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="startOdometer" HeaderText="Start Odometer (KM)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField> 
                                                   <asp:BoundField DataField="endOdometer" HeaderText="End Odometer (KM)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>  
                                                    <asp:BoundField DataField="Travelled" HeaderText="Distance Travelled (KM)" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <AlternatingRowStyle BackColor="Lavender" />
                                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="12px" ForeColor="White"
                                                    Height="22px" HorizontalAlign="Center" />
                                                <FooterStyle BackColor="DarkSeaGreen" Font-Bold="True" Font-Names="Tahoma" Font-Size="Small"
                                                    HorizontalAlign="Right" />
                                            </asp:GridView>
                                            &nbsp; &nbsp;&nbsp;
                                            <% If show = True Then%> 
                                            <%End If%>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </center>
       
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Vehicle Odometer Report" />
        <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
