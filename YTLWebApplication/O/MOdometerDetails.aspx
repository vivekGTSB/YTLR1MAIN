<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.MOdometerDetails" Codebehind="MOdometerDetails.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
    <center>
    <div>
   
    <table width ="600px" class ="hor-minimalist-b">
    <thead >
    <tr align="left" ><th>Recorded Odometer:</th><th ><%= Odometer%></th><th>Recorded Date:</th><th colspan ="3"><%=Odometerdate %></th></tr>
    <tr><th style ="width :180px;">Plate No</th><th style ="width :120px;" >Date Time</th><th style ="width :120px;text-align :right ;">Start Odometer</th><th style ="width :120px;text-align :right ;">End Odometer</th><th style ="width :90px;text-align :right ;">Mileage</th><th style ="width :90px;text-align :right ;">Absolute</th></tr></thead>
    <tbody>
    <%=sb.ToString () %>
    </tbody>
    </table>
    </div>
    </center>
    </form>
</body>
</html>
