<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GetJobData.aspx.vb" Inherits="YTLWebApplication.GetJobData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <title>Polling</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/ColVis.css";
    </style>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #fw_container {
            width: 996px;
        }

        table.display td {
            padding: 2px 2px;
        }

        .fg-toolbar {
            font-size: 10px;
        }

        .MyButton {
            text-align: left;
            float: left;
            width: 350px;
        }

        .ColVis {
            display: none;
        }
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/ColVis.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">
        var oTable;

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
        
        $(document).ready(function () {
            fnFeaturesInit();

            // SECURITY FIX: Validate plateno before using
            var plateno = '<%=HttpUtility.JavaScriptStringEncode(plateno)%>';
            if (!plateno || plateno.length === 0) {
                alert('Invalid plate number');
                return;
            }

            oTable = $('#examples').dataTable({
                "bProcessing": false,
                "bJQueryUI": true,
                "bServerSide": false,
                "sAjaxSource": "GetOssData.aspx?plateno=" + encodeURIComponent(plateno),
                "sScrollY": "250px",
                "bPaginate": false,
                "bScrollCollapse": true,
                "bInfo": true,
                "bAutoWidth": false,
                "aaSorting": [[1, "desc"]],
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
                    { "bVisible": true,"sWidth": "50px", "sClass": "left", "bSortable": false, "aTargets": [0] },
                    { "sClass": "left","sWidth": "50px", "bSortable": true,"bVisible": true, "aTargets": [1], "asSorting": ["desc", "asc"] },
                    { "sClass": "left","sWidth": "60px", "bSortable": true, "bVisible": true, "aTargets": [2], "asSorting": ["desc", "asc"] },
                    { "sClass": "left","sWidth": "95px", "bSortable": true,"aTargets": [3], "bVisible": true, "asSorting": ["desc", "asc"] },
                    { "sClass": "left","sWidth": "90px", "bSortable": true,"aTargets": [4], "bVisible": true, "asSorting": ["desc", "asc"] },
                    { "sClass": "left","sWidth": "95px", "bSortable": true,"aTargets": [5], "bVisible": true, "asSorting": ["desc", "asc"] },
                    { "sClass": "left", "bSortable": true,"aTargets": [6], "bVisible": true, "asSorting": ["desc", "asc"] },
                    { "sClass": "left", "sWidth": "135px", "bSortable": true, "aTargets": [7], "bVisible": true, "asSorting": ["desc", "asc"] },
                    { "sClass": "left", "sWidth": "135px", "bSortable": true, "aTargets": [8], "bVisible": true, "asSorting": ["desc", "asc"] }
                ],
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"ipT>',
            });
            
            // SECURITY FIX: Encode plateno for display
            $("div.MyButton").html('<div style=\"Color:White;font-size:13px;\">Plate No: ' + $('<div>').text(plateno).html() + '</div>');
        });
    </script>
</head>
<body id="dt_example" style="margin: 0px;">
    <form id="form1" runat="server">
        <table>
            <br />
            <tr>
                <td align="left">
                    <div id="fw_container">
                        <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 11px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
                            <thead style="text-align: left;">
                                <tr>
                                    <th width="35px">S No</th>
                                    <th width="40px">Source</th>
                                    <th width="50px">Dn ID</th>
                                    <th width="70px">Product Type</th>
                                    <th width="70px">Area Info</th>
                                    <th width="70px">Ship to Code</th>
                                    <th>Ship to Name</th>
                                    <th width="130px">Weight Out</th>
                                    <th width="130px">ATA</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            <tfoot style="text-align: left; font-weight: bold;">
                                <tr>
                                    <th width="35px">S No</th>
                                    <th width="40px">Source</th>
                                    <th width="50px">Dn ID</th>
                                    <th width="70px">Product Type</th>
                                    <th width="70px">Area Info</th>
                                    <th width="70px">Ship to Code</th>
                                    <th>Ship to Name</th>
                                    <th width="130px">Weight Out</th>
                                    <th width="130px">ATA</th>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
        <input type="hidden" name="hdnPlate" value="" id="hdnPlate" runat="server" />
    </form>
</body>
</html>