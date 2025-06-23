<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.RefuelDetailscust" Codebehind="RefuelDetailscust.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" media="screen">
        @import "css/g1/jquery-ui-1.8.21.custom.css";
        @import "css/vehiclelogchartmap.css";
    </style>
    <script type="text/javascript" src="js/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="js/jquery-ui-1.8.20.custom.min.js"></script>
    <script type='text/javascript' src='https://www.google.com/jsapi'></script>
    <script type="text/javascript" src="js/infobubble-compiled.js"></script>
    <script type="text/javascript" src="js/highstock.src.js"></script>
    <script type="text/javascript" src="js/highcharts-more.js"></script>
    <script type="text/javascript" src="js/exporting.js"></script>
    <script type="text/javascript" src="js/Date.js"></script>
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?sensor=false"></script>
    <style type="text/css">
        .hor-minimalist-b
        {
            font-family: "Verdana";
            font-size: 11px;
            background: #fff;
            margin: 0px;
            width: 920px;
            border-collapse: collapse;
            text-align: left;
        }
        .hor-minimalist-b th
        {
            font-size: 14px;
            font-weight: normal;
            color: #039;
            padding: 3px 2px;
            border-bottom: 1px solid #6678b1;
        }
        .hor-minimalist-b td
        {
            border-bottom: 1px solid #ccc;
            color: #669;
            padding: 4px 6px;
        }
        .hor-minimalist-b tbody tr:hover td
        {
            color: #009;
        }
        .info_graph
        {
            width: 920px;
        }
        .info_box
        {
            width: 920px;
        }
        .footer
        {
            width: 920px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Literal ID="Literal12" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sNo%>" />
    <asp:Literal ID="Literal13" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sDateTime%>" />
    <asp:Literal ID="Literal14" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, ex85%>" />
    <asp:Literal ID="Literal15" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, ex87%>" />
    <asp:Literal ID="Literal16" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, fr49%>" />
    <asp:Literal ID="Literal17" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, Fuel%>" />
    <asp:Literal ID="Literal18" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, fr43%>" />
    <asp:Literal ID="Literal9" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, fname%>" />
    <asp:Literal ID="Literal10" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sLocation%>" />
    <asp:Literal ID="Literal49" runat="server" Visible="false" Text="<%$ Resources:chienvh.language, fp5%>" />
    <asp:Literal ID="Literal20" runat="server" Visible="false" Text="<%$ Resources:chienvh.language, fp9%>" />
    <asp:Literal ID="Literal21" runat="server" Visible="false" Text="<%$ Resources:chienvh.language, fp8%>" />
    <asp:Literal ID="Literal19" runat="server" Visible="false" ></asp:Literal>
    <center>
        <div id="container" style="width: 920px;">
            <div id="report">
                <div class="info_graph">
                    <table width="920px;">
                        <tr>
                            <td colspan="2">
                                <fieldset class="table info_box" id="Div2">
                                    <legend>
                                        <h3 id="hed" runat="server"  style="color: #039">
                                           <%=Literal19.Text%>
</h3>
                                    </legend>
                                    <table class="hor-minimalist-b" style="font-size: 11px;">
                                        <tr>
                                            <td style="width: 100px;">
                                                <b> <asp:Literal ID="Literal1"  runat="server" Text="<%$ Resources:chienvh.language, tit1%>" /></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblusername" runat="server"></asp:Label>
                                            </td>
                                            <td style="width: 120px;">
                                                <b><asp:Literal ID="Literal2"  runat="server" Text="<%$ Resources:chienvh.language, atp1%>" /></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblplate" runat="server"></asp:Label>
                                            </td>
                                            <td colspan="3">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 160px;">
                                                <b><asp:Literal ID="Literal3"  runat="server" Text="<%$ Resources:chienvh.language, fr32%>" /></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td style="width: 150px;">
                                                <asp:Label ID="lblfrom" runat="server"></asp:Label>
                                            </td>
                                            <td style="width: 80px;">
                                                <b><asp:Literal ID="Literal4"  runat="server" Text="<%$ Resources:chienvh.language, fr33%>" /></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td style="width: 160px;">
                                                <asp:Label ID="lblto" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <b><asp:Literal ID="Literal5"  runat="server" Text="<%$ Resources:chienvh.language, gr17%>" /></b>
                                            </td>
                                            <td>
                                                :
                                            </td>
                                            <td>
                                                <asp:Label ID="lblduration" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b><asp:Literal ID="Literal6"  runat="server" Text="<%$ Resources:chienvh.language, fr23%>" /></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblvol1" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <b><asp:Literal ID="Literal7"  runat="server" Text="<%$ Resources:chienvh.language, fr24%>" /></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblvol2" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <b><asp:Literal ID="Literal8"  runat="server" Text="<%$ Resources:chienvh.language, fm16%>" /></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td>
                                                <asp:Label ID="lbltot" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <b>
                                                    <asp:Label ID="lblst" runat="server"></asp:Label></b>
                                            </td>
                                            <td>
                                                <b>:</b>
                                            </td>
                                            <td colspan="7">
                                                <asp:Label ID="lblfuelst" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                           <%-- <td>
                                <fieldset title="<%=Literal18.Text%>1" class="chart info_box" id="chart_box" style="clear: both;
                                    width: 95%;">
                                    <legend>
                                        <h3 style="color: #039">
                                            <%=Literal18.Text%>1</h3>
                                    </legend>
                                    <img alt="<%=Literal18.Text%>" title="<%=Literal18.Text%>" src="FuelChart.aspx?<%=sb2.ToString()%>" style="width: 420px;
                                        height: 300px;" />
                                </fieldset>
                            </td>--%>
                            <td colspan ="2">
                                <fieldset title="<%=Literal18.Text%>1" class="chart info_box" id="Fieldset1" style="clear: both;
                                    width: 95%;">
                                    <legend>
                                        <h3 style="color: #039">
                                            <%=Literal18.Text%>2</h3>
                                    </legend>
                                    <img alt="<%=Literal18.Text%>" title="<%=Literal18.Text%>" src="FuelChart3.aspx?<%=sb2.ToString()%>" style="width: 900px;
                                        height: 400px;" />
                                </fieldset>
                            </td>
                        </tr>
                        <%--   <tr><td colspan="2" align="center">
            <div>
				<div class="map info_box" id="map_box" style="clear: both;width:450px;height:200px;"> 
                 <img alt="Map" title="Map" src="http://maps.avls.com.my/GussmannMap.aspx?<%=sb3.ToString()%>" style="width:450px;height:200px;"/>
                </div>
				</div>
            </td>
            </tr>--%>
                        <tr>
                            <td colspan="2">
                                <div>
                                    <fieldset class="table info_box" id="data_log">
                                        <legend>
                                            <h3 style="color: #039">
                                                <asp:Literal ID="Literal11"  runat="server" Text="<%$ Resources:chienvh.language, fp7%>" /></h3>
                                        </legend>
                                        <%=sb6.ToString()%>
                                    </fieldset>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div class="footer">
            </div>
        </div>
    </center>
    </form>
</body>
</html>
