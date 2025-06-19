<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.ClientLoginReport" Codebehind="ClientLoginReport.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Delivery Apps Login Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="X-Content-Type-Options" content="nosniff">
    <meta http-equiv="X-Frame-Options" content="DENY">
    <meta http-equiv="X-XSS-Protection" content="1; mode=block">
    
    <style media="print" type="text/css">
        body {
            color: #000000;
            background: #ffffff;
            font-family: verdana,arial,sans-serif;
            font-size: 12pt;
        }
        #fcimg {
            display: none;
        }
    </style>
    
    <style type="text/css">
        .dataTables_length {
            width: 18%;
            float: left;
        }
        div.dataTables_wrapper .ui-widget-header {
            font-weight: normal;
        }
        .dataTables_wrapper .ui-toolbar {
            padding: 5px;
            width: 788px;
        }
        table.display tfoot th {
            padding: 0px 0px 0px 0px;
            font-weight: bold;
        }
        table.display thead th {
            padding: 0px 0px 0px 0px;
            font-weight: bold;
        }
        table.display thead th div.DataTables_sort_wrapper {
            position: relative;
            padding-right: 0px;
            padding-left: 0px;
        }
        tfoot input {
            margin: 0.2em 0;
            width: 100%;
            color: #444;
        } 
    </style>
    
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
    <script type="text/javascript" src="jsfiles/balloontip.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/calendar.js"></script>

    <script type="text/javascript" language="javascript">
        var ec = <%=SafeJavaScript(ec)%>;
      
        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for(i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
        }
        
        function deleteconfirmation() {
            var checked = false;
            for(i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    if(elm.checked == true) {
                        checked = true;
                        break;
                    }
                }
            }
            if(checked) {
                var result = confirm("Are you sure you want to delete the selected items?");
                if(result) {
                    return true;
                }
                return false;
            } else {
                alert("Please select checkboxes");
                return false;
            }
        }
        
        function ShowCalendar(strTargetDateField, intLeft, intTop) {
            txtTargetDateField = strTargetDateField;
            var divTWCalendarobj = document.getElementById("divTWCalendar");
            divTWCalendarobj.style.visibility = 'visible';
            divTWCalendarobj.style.left = intLeft + "px";
            divTWCalendarobj.style.top = intTop + "px";
            selecteddate(txtTargetDateField);  
        }
    </script>
    
    <script type="text/javascript">
        function fnFeaturesInit() {
            $('ul.limit_length>li').each(function (i) {
                if (i > 10) {
                    this.style.display = 'none';
                }
            });

            $('ul.limit_length').append('<li class="css_link">Show more<\/li>');
            $('ul.limit_length li.css_link').click(function () {
                $('ul.limit_length li').each(function (i) {
                    if (i > 5) {
                        this.style.display = 'list-item';
                    }
                });
                $('ul.limit_length li.css_link').css('display', 'none');
            });
        }
        
        jQuery.extend(jQuery.fn.dataTableExt.oSort, {
            "num-html-pre": function (a) {
                var x = parseFloat(a.toString().replace(/<.*?>/g, ""));
                if (isNaN(x)) {
                    if (a.toString().indexOf("fuel sensor problem") > 0) {
                        return -1;
                    }
                    else if (a.toString().indexOf("no fuel sensor") > 0) {
                        return -0.1;
                    }
                    return 0.0;
                }
                else {
                    return x;
                }
            },
            "num-html-asc": function (a, b) {
                return ((a < b) ? -1 : ((a > b) ? 1 : 0));
            },
            "num-html-desc": function (a, b) {
                return ((a < b) ? 1 : ((a > b) ? -1 : 0));
            }
        });

        $(document).ready(function () {
            fnFeaturesInit();
            $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 100,
                "aaSorting": [[2, "asc"]],
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "aoColumnDefs": [
                    { "bSortable": false, "aTargets": [0] }
                ]
            });
        });
        
        function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();
            return false;
        }
    </script> 
</head>
<body style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="fuelform" runat="server">
        <%=GetCSRFTokenHtml()%>
        <script type="text/javascript" language="javascript">DrawCalendarLayout();</script>
        <center>
            <br />
            <table style="font-family: Verdana; font-size: 11px; width:800px">
                <tr>
                    <table style="font-family: Verdana; font-size: 11px;">
                        <tr>
                            <td style="height: 20px; background-color: #465ae8;" align="left">
                                <b style="color: White;">&nbsp;Delivery Apps Login Report &nbsp;:</b>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 500px; border: solid 1px #3952F9;">
                                <table style="width: 500px;">
                                    <tbody>
                                        <tr>
                                            <td align="left" width="130">
                                                <b style="color: #5f7afc;">Begin Date</b>
                                            </td>
                                            <td width="2">
                                                <b style="color: #5f7afc;">:</b>
                                            </td>
                                            <td align="left" colspan="2" width="320">
                                                <input readonly="readonly" style="width: 70px;" type="text" value="<%=SafeOutput(strBeginDate)%>"
                                                    id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />
                                                <a href="javascript:ShowCalendar('txtBeginDate', 250, 250);" style="text-decoration: none;">
                                                    <img alt="Show calendar control" title="Show calendar control" height="14" src="images/Calendar.jpg"
                                                        width="19" style="border: solid 1px blue;" />
                                                </a>
                                                <b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                <asp:DropDownList ID="ddlbh" runat="server" Width="40px" EnableViewState="False"
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
                                                </asp:DropDownList>
                                                <b style="color: #5f7afc;">&nbsp;Min&nbsp;:&nbsp;</b>
                                                <asp:DropDownList ID="ddlbm" runat="server" Width="40px" EnableViewState="False"
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
                                            <td align="left" width="130">
                                                <b style="color: #5f7afc;">End Date</b>
                                            </td>
                                            <td width="2">
                                                <b style="color: #5f7afc;">:</b>
                                            </td>
                                            <td align="left" colspan="2" width="320">
                                                <input style="width: 70px;" readonly="readonly" type="text" value="<%=SafeOutput(strEndDate)%>"
                                                    id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" />
                                                <a href="javascript:javascript:ShowCalendar('txtEndDate', 250, 250);" style="text-decoration: none;">
                                                    <img alt="Show calendar control" title="Show calendar control" height="14" src="images/Calendar.jpg"
                                                        width="19" style="border: solid 1px blue;" />
                                                </a>
                                                <b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                <asp:DropDownList ID="ddleh" runat="server" Width="40px" EnableViewState="False">
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
                                                </asp:DropDownList>
                                                <b style="color: #5f7afc;">&nbsp;Min&nbsp;:&nbsp;</b>
                                                <asp:DropDownList ID="ddlem" runat="server" Width="40px" EnableViewState="False">
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
                                            <td align="left" width="130">
                                                <b style="color: #5f7afc">User Name</b>
                                            </td>
                                            <td width="2">
                                                <b style="color: #5f7afc">:</b>
                                            </td>
                                            <td align="left" colspan="2" width="320">
                                                <asp:DropDownList ID="ddlUsername" runat="server" AutoPostBack="True" Font-Names="verdana"
                                                    Font-Size="12px" Width="200px">
                                                    <asp:ListItem>--Select User Name--</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" width="130">
                                                <br />
                                            </td>
                                            <td colspan="3" align="center" width="320">
                                                <br />
                                                <asp:ImageButton ID="ImageButton" runat="server" ImageUrl="images/Submit_s.jpg" ToolTip="Submit"></asp:ImageButton>
                                                &nbsp;&nbsp;                                                     
                                                <a href="javascript:ExcelReport();">
                                                    <img alt="Save to Excel file" title="Save to Excel file" src="images/saveExcel.jpg" style="border: solid 0px blue;" />
                                                </a>&nbsp;&nbsp;
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </tr>
                <br />
                <tr>
                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 10px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif; width: 800px">
                        <%=SafeOutput(sb1.ToString())%>
                    </table> 
                </tr>
            </table>
        </center>
    </form>

    <form id="excelform" method="post" action="ClientLoginExcel.aspx">
        <%=GetCSRFTokenHtml()%>
    </form>
</body>
</html>