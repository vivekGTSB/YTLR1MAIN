<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="OssGeofenceDiffReportNew.aspx.vb" Inherits="YTLWebApplication.OssGeofenceDiffReportNew" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Waiting Inside Plant Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";

        .dataTables_info {
            width: 25%;
            float: left;
        }
    </style>




    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">

        function mysubmit() {

            return true;

        }
        function ExcelReport() {
          
        }
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

        var isd =<%=isD  %>;

        $(document).ready(function () {
            fnFeaturesInit()
            var DefaultSortRow = 4;
          
            if (isd==true) {
                DefaultSortRow = 8;
            }
            $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 25,
                "aaSorting": [[DefaultSortRow, "asc"]],
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "aoColumnDefs": [
                    { "bVisible": true, "bSortable": false, "aTargets": [0] }
                    //{ "bVisible": true, "bSortable": true, "aTargets": [1] },
                    //{ "bVisible": true, "bSortable": true, "aTargets": [2] },
                    //{ "bVisible": true, "bSortable": true, "aTargets": [3] },
                    //{ "bVisible": true, "bSortable": true, "aTargets": [4] }
                ]
            });
        });


    </script>

</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
        <center>
            <div>
                <br />
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Waiting Inside Plant Report</b>
                <br />
                <br />
            </div>
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Waiting Inside Plant Report&nbsp;:</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 450px; border: solid 1px #5B7C97;">
                                    <table style="width: 450px;">
                                        <tbody>
                                           
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #465AE8;">Geofence Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #465AE8;">:</b>
                                                </td>
                                                <td align="left" style="width: 326px">
                                                    <asp:DropDownList ID="ddlPlants" runat="server" Width="248px">
                                                       
                                                    </asp:DropDownList>

                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                </td>
                                                <td colspan="2" align="center">
                                                    <br />
                                                    <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
               <%If ec = "True" Then%>
           <span style="color:blue;font-size:14px;font-style:italic;"> <%=reportDateTime %></span>
            <br /><br />
             <span style="color:red;font-size:15px;font-style:italic;"> <%=noDataText %></span>
             <%End If%>
            <table>
                <tr>
                    <td colspan="3">
                        <div style="width: 1190px;">
                            <%If ec = "True" Then%>
                            <%=sb1.ToString()%>
                            <%End If%>
                        </div>
                    </td>
                </tr>
            </table>
        </center>

    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Waiting Inside Plant Report" />
        <input type="hidden" id="plateno" name="plateno" value="" />
    </form>


</body>
</html>
