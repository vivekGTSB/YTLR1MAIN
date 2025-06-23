<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.Polling" Codebehind="Polling.aspx.vb" %>

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
        #fw_container
        {
            width: 996px;
        }
        table.display td
        {
            padding: 2px 2px;
        }
        .fg-toolbar
        {
            font-size: 10px;
        }
        .MyButton
        {
            text-align: left;
            float: left;
            width: 350px;
        }
       .ColVis 
       {
           display:none;
       }
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/ColVis.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">
    var oTable;
       
         function refreshPoll() {
            table = oTable.dataTable();
            oSettings = table.fnSettings();
            $.getJSON('GetPollData.aspx?plateno=' + document.getElementById("hdnPlate").value, null, function (json) {
                table.fnClearTable(this);

                for (var i = 0; i < json.aaData.length; i++) {
                    table.oApi._fnAddData(oSettings, json.aaData[i]);
                }

                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                table.fnDraw();
            });
            return false;
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
        $(document).ready(function () {
            fnFeaturesInit();

            oTable = $('#examples').dataTable({
                "bProcessing": false,
                "bJQueryUI": true,
                "bServerSide": false,                
                "sAjaxSource": "GetPollData.aspx?plateno=<%=plateno %>",
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
                          { "bVisible": true,"sClass": "left", "bSortable": false, "aTargets": [0] },
                             { "sClass": "left", "bSortable": true, "aTargets": [1] },
                     { "sClass": "left", "bSortable": true, "aTargets": [3] },
                
                  {"sClass": "left", "aTargets": [3], "asSorting": ["desc", "asc"],
                   "fnRender": function (oData, sVal) {
                   var status = oData.aData[4];
                  
                    if (status == "0") {
                        return "<span style='Color:Green;'><b>" + oData.aData[3] + "</b><span>";
                      }
                  else {
                      return "<span style='Color:Blue;'><b>" + oData.aData[3] + "</b><span>";
                    }
              } 
          },
                    { "aTargets": [3], "bVisible": true, "asSorting": ["desc", "asc"] }

                              ],

                             "sDom": '<"H"Cl<"MyButton">f>rt<"F"ipT>',

            });
            $("div.MyButton").html('<div style=\"Color:White;font-size:13px;\">Plate No :  <%=plateno %> </div>');


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
                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 11px;
                        font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
                        <thead style="text-align: left;">
                            <tr>
                                <th width="50px">
                                    S No
                                </th>
                                <th width="110px">
                                    Date Time
                                </th>
                                <th width="125px">
                                    From/To Mobile No
                                </th>
                                <th>
                                    Message
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                        <tfoot style="text-align: left; font-weight: bold;">
                            <tr>
                                <th>
                                    S No
                                </th>
                                <th >
                                    Date Time
                                </th>
                                <th >
                                    From/To Mobile No
                                </th>
                                <th >
                                    Message
                                </th>
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
