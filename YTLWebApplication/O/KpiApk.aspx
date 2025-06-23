<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="KpiApk.aspx.vb" Inherits="YTLWebApplication.KpiApk" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>KPI Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/ColVis.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/ColVis.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">
        $(function () {
            $("#dialog:ui-dialog").dialog("destroy");
            $("#ossdiv").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: 1100,
                minHeight: 450
            });
            $('.modal-content').resizable({
                //alsoResize: ".modal-dialog",
                //minHeight: 150
            });
            $('.modal-dialog').draggable();

            $('#ossdiv').on('show.bs.modal', function () {
                $(this).find('.modal-body').css({
                    'max-height': '100%'
                });
            });
        });


        function load() {
            var isInternal =<%=isInternal %>;
            if (isInternal == "false" || isInternal == false) {
                document.getElementById("GridView1").childNodes[1].innerHTML = "<tr align=\"center\" style=\"color:White;background-color:#465AE8;font-size:12px;font-weight:bold;height:22px;\"><th></th><th></th><th></th><th colspan=\"5\">OSS</th></tr>" + document.getElementById("GridView1").childNodes[1].innerHTML
            }
            else {
                document.getElementById("GridView1").childNodes[1].innerHTML = "<tr align=\"center\" style=\"color:White;background-color:#465AE8;font-size:12px;font-weight:bold;height:22px;\"><th></th><th></th><th></th><th colspan=\"8\">Whole System</th><th colspan=\"5\">OSS</th></tr>" + document.getElementById("GridView1").childNodes[1].innerHTML
            }
        }

        function openDetails(itype,tid, typ) {
            document.getElementById("ossframe").style.visibility = "visible";
            document.getElementById("ossframe").src = "GetKPIDetailsAPK.aspx?itype=" + itype +"&u=" + tid + "&tp=" + typ;
            $("#ossdiv").dialog("open");
        }
        google.load("visualization", "1", { packages: ["corechart"] });
        google.setOnLoadCallback(drawChart);
        function drawChart() {
            var data = google.visualization.arrayToDataTable([
                ['status', 'count'],
                ['Active', <%=tactive %>],
                ['Inactive', <%=tinactive %>]

            ]);
            var data2 = google.visualization.arrayToDataTable([
                ['status', 'count'],
                ['Active', <%=e_tactive %>],
                ['Inactive', <%=e_tinactive %>]

            ]);

            var options1 = {
                title: 'Internal Transporters'
            };
            var options2 = {
                title: 'External Transporters'
            };

            var chart = new google.visualization.PieChart(document.getElementById('chart_div'));
            chart.draw(data, options1);

            var chart2 = new google.visualization.PieChart(document.getElementById('chart_div2'));
            chart2.draw(data2, options2);
        }
    </script>
</head>
<body onload="load();">
    <form id="form1" runat="server">
        <center>
            <table>
                <tr>
                    <td valign="top">
                        <center>
                            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">KPI Chart - Vehicles Performence</b>
                            <table>
                                <tr>
        <td> <div id="chart_div" style="width: 600px; height: 300px;">
                            </div></td>
          <td>  <div id="chart_div2" style="width: 600px; height: 300px;">
                            </div></td>
    </tr>
</table>
                           
                            <br />
                           
                            
                        </center>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <center>
                            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">KPI Report - Vehicles Performence</b></center>
                        <br />
                        <div style="font-family: Verdana; font-size: 13px;">
                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Size="12px"
                                AllowPaging="False" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                HeaderStyle-Font-Bold="True" HeaderStyle-Height="22px" EnableViewState="False"
                                HeaderStyle-HorizontalAlign="Center" PageSize="50" Width="100%" BorderColor="#F0F0F0">
                                <Columns>
                                    <asp:BoundField DataField="S No" HeaderText="S No"></asp:BoundField>
                                    <asp:BoundField DataField="Username" HeaderText="Username"></asp:BoundField>
                                    <asp:BoundField DataField="Transporter" ItemStyle-Width="415" HeaderText="Internal Transporter"></asp:BoundField>
                                    <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                  <%--  <asp:BoundField DataField="Active" HeaderText="Active" ItemStyle-HorizontalAlign="right"></asp:BoundField>--%>
                                    <asp:BoundField DataField="Bag" HeaderText="Bag" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Bulk" HeaderText="Bulk" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Tipper" HeaderText="Tipper" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Inactive" HtmlEncode="false" HeaderText="Inactive" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Workshop" HtmlEncode="false" HeaderText="Maintenance" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="ServiceScheduled" HtmlEncode="false" HeaderText="Service Scheduled" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                     <asp:BoundField DataField="Pending" HeaderText="Pending" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Active %" HeaderText="Uptime %" ItemStyle-HorizontalAlign="right"></asp:BoundField>                                   
                                    <asp:BoundField DataField="Oss Total" HeaderText="Total" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Active" HeaderText="Active" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Inactive" HtmlEncode="false" HeaderText="Inactive" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Active %" HeaderText="Active %" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Inactive %" HeaderText="Inactive %" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                </Columns>
                                <AlternatingRowStyle BackColor="Lavender" />
                                <HeaderStyle HorizontalAlign="Center" BackColor="#465AE8" Font-Bold="True" Font-Size="12px"
                                    ForeColor="White" Height="22px"></HeaderStyle>
                            </asp:GridView>
                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Size="12px"
                                AllowPaging="False" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                HeaderStyle-Font-Bold="True" HeaderStyle-Height="22px" EnableViewState="False"
                                HeaderStyle-HorizontalAlign="Center" PageSize="50" Width="100%" BorderColor="#F0F0F0">
                                <Columns>
                                    <asp:BoundField DataField="S No" HeaderText="S No"></asp:BoundField>
                                    <asp:BoundField DataField="Username" HeaderText="Username"></asp:BoundField>
                                    <asp:BoundField DataField="Transporter" ItemStyle-Width="415" HeaderText="External Transporter"></asp:BoundField>
                                    <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                   <%-- <asp:BoundField DataField="Active" HeaderText="Active" ItemStyle-HorizontalAlign="right"></asp:BoundField>--%>
                                       <asp:BoundField DataField="Bag" HeaderText="Bag" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Bulk" HeaderText="Bulk" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Tipper" HeaderText="Tipper" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                 
                                    <asp:BoundField DataField="Inactive" HtmlEncode="false" HeaderText="Inactive" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Workshop" HtmlEncode="false" HeaderText="Maintenance" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="ServiceScheduled" HtmlEncode="false" HeaderText="Service Scheduled" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                     <asp:BoundField DataField="Pending" HeaderText="Pending" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Active %" HeaderText="Uptime %" ItemStyle-HorizontalAlign="right"></asp:BoundField>                                   
                                    <asp:BoundField DataField="Oss Total" HeaderText="Total" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Active" HeaderText="Active" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Inactive" HtmlEncode="false" HeaderText="Inactive" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Active %" HeaderText="Active %" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                    <asp:BoundField DataField="Oss Inactive %" HeaderText="Inactive %" ItemStyle-HorizontalAlign="right"></asp:BoundField>
                                </Columns>
                                <AlternatingRowStyle BackColor="Lavender" />
                                <HeaderStyle HorizontalAlign="Center" BackColor="#465AE8" Font-Bold="True" Font-Size="12px"
                                    ForeColor="White" Height="22px"></HeaderStyle>
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
            </table>
        </center>
    </form>

    <div id="ossdiv" title="KPI Information" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="ossframe" name="ossframe" src="" frameborder="0" scrolling="auto" height="450"
            width="1100" style="visibility: hidden; border: solid 1px #aac6ff;"></iframe>
    </div>

</body>
</html>
