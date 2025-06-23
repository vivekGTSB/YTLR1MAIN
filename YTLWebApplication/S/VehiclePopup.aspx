<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.VehiclePopup" Codebehind="VehiclePopup.aspx.vb" %>



<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vehicle Popup</title>
      <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen"/>
      <link type="text/css"  href="cssfiles/jquery-ui.css" rel="stylesheet" />
      <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
      <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
      <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
      <script type="text/javascript" src="jsfiles/balloontip.js"></script>
    <script type="text/javascript">
//        function Resetdata() {
//            document.forms[0].reset();
//         //   $(".ui-icon-closethick").click();
//        }
//        function UpdateData() {
//            var str = "UpdateVehicleInfo.aspx?plateno=" + $("#lblPlateno").text() + "&groupname=" + $("#ddlgroupname").val() + "&type=" + $("#txtType").val() + "&brand=" + $("#txtBrand").val() + "&model=" + $("#txtModel").val() + "&speedlimit=" + $("#txtSpeed").val() +"&r="+ Math.random();
//            $.get(str, "", function (data) {
//                alert(data);
//            });
//           // $(".ui-icon-closethick").click();
        //        }
        $(function () {
            var year = $("#year").val();
            var month = $("#month").val();
            var date = $("#dd").val();
            $("#txtRecDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: new Date(year, month, date), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 1
            }); 
        });
    
    </script>
</head>
<body style="width:300px; height: 148px;">
    <form id="form1" runat="server">
    <table  border="0" cellpadding="1" cellspacing="1" style="width: 296px ;font-size: 11px; font-weight: bold;
    font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif"">
   
        <tr>
            <td align="left"><b style="color: #4E6CA3;">Plate No</b>
            </td>
             <td style="color: #4E6CA3;">:
            </td>
             <td >
                 <asp:Label ID="lblPlateno" Text="" runat="server"  style="color:Gray;"/>
            </td>
        </tr>
         <tr>
             <td align="left"><b style="color: #4E6CA3;">Prime Mover ID</b>
            </td>
           <td style="color: #4E6CA3;">:
            </td>
             <td>
              <asp:TextBox runat="server"  ID="txtpmid"/>
            </td>
        </tr>
         <tr>
            <td align="left"><b style="color: #4E6CA3;">Group</b>
            </td>
            <td style="color: #4E6CA3;">:
            </td>
             <td>
                 <asp:DropDownList runat="server" ID="ddlgroupname" Width="158px">                    
                 </asp:DropDownList>
            </td>
        </tr>
         <tr>
            <td align="left"><b style="color: #4E6CA3;">Type</b>
            </td>
           <td style="color: #4E6CA3;">:
            </td>
             <td>
                 <asp:DropDownList runat="server" ID="txtType" Width="158px">                    
                 </asp:DropDownList>
                
            </td>
        </tr>
         <tr>
             <td align="left"><b style="color: #4E6CA3;">Brand</b>
            </td>
           <td style="color: #4E6CA3;">:
            </td>
             <td>
              <asp:TextBox runat="server"  ID="txtBrand"/>
            </td>
        </tr>
         <tr>
             <td align="left"><b style="color: #4E6CA3;">Model</b>
            </td>
            <td style="color: #4E6CA3;">:
            </td>
             <td>
              <asp:TextBox runat="server"  ID="txtModel"/>
            </td>
        </tr>
        <tr>
            <td align="left"><b style="color: #4E6CA3;">Speed</b>
            </td>
           <td style="color: #4E6CA3;">:
            </td>
             <td>
              <asp:TextBox runat="server"  ID="txtSpeed"/>
            </td>
        </tr>
       <tr>
            <td align="left"><b style="color: #4E6CA3;">Phone No</b>
            </td>
           <td style="color: #4E6CA3;">:
            </td>
             <td>
              <asp:TextBox runat="server"  ID="txtphone"/>
            </td>
        </tr>
          <tr>
            <td align="left">
                <b style="color: #4E6CA3;">
                   Odometer </b>
            </td>
            <td style="color: #4E6CA3;">
                :
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtodometer" CssClass="textbox1" Width="158px" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <b style="color: #4E6CA3;">
                   Recorded Date  </b>
            </td>
            <td style="color: #4E6CA3;">
                :
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtRecDate" CssClass="textbox1" Width="108px"   />
                   <asp:DropDownList ID="ddlbh" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
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
                                                    </asp:DropDownList>
                                                   <asp:DropDownList ID="ddlbm" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
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
                                                        <asp:ListItem Value="59">59</asp:ListItem>
                                                    </asp:DropDownList>
            </td>
        </tr>
         <tr>
            <td align="left"><b style="color: #4E6CA3;">Base Plant</b>
            </td>
           <td style="color: #4E6CA3;">:
            </td>
             <td>
                 <asp:DropDownList runat="server" ID="ddlbaseplant" Width="158px">                    
                 </asp:DropDownList>
                
            </td>
        </tr>
        <tr>
            <td align="left"><b style="color: #4E6CA3;">Permit</b>
            </td>
           <td style="color: #4E6CA3;">:
            </td>
             <td>
                 <asp:DropDownList runat="server" ID="ddlpermit" Width="200px">                    
                 </asp:DropDownList>
                
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
