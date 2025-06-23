<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.OssEta" Codebehind="OssEta.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OSS ETA</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/ColVis.css";
        .dataTables_info
        {
            width: 16%;
            float: left;
        }
        
        table.display td
        {
            padding: 2px 2px;
            border: 1px solid lightblue;
            nowrap: nowrap;
            white-space: nowrap;
        }
        table.display thead th div.DataTables_sort_wrapper
        {
            position: relative;
            padding-right: 15px;
        }
         #refreshbtn
        {
                width: 64px;
    text-align: center;
        }
    </style>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link type="text/css" href="cssfiles/common1.css" rel="stylesheet" />
    <link href="cssfiles/style.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/jquery.ui.dialog.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/jquery_ui.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">
        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
        function resize() {
            function resize() {
                //            $(".dataTables_scrollBody").height = getWindowHeight() - 110 + "px";
                table = oTable.dataTable();
                table.fnSettings().oScroll.sY = getWindowHeight() - 168 + "px";
                table.fnDraw();

            }

          
        }



        function LoadJobs() {
            $.ajax({
                type: "GET",
                url: "GetOssEta.aspx",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnLoadJobs,
                failure: function (response) {
                    alert(response);
                }
            });
        }


        function OnLoadJobs(response) {
            var json = response;
                                table = oTable.dataTable();
                                table._fnProcessingDisplay(true);
                                oSettings = table.fnSettings();
                                table.fnClearTable(this);
                                for (var i = 0; i < response.length-1; i++) {
                                    table.oApi._fnAddData(oSettings, response[i]);
                                }
                                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                                table._fnProcessingDisplay(false);
                                table.fnDraw();
                               
                                if (response.length == 0)
                                    alertbox("No Data Found..");
                                return false;

        }



        function refreshTable() {
            LoadJobs();
//            $.ajax({
//                type: "POST",
//                url: "OssInprocess.aspx/FillGrid",
//                contentType: "application/json; charset=utf-8",
//                dataType: "json",
//                success: function (response) {
//                    var json = response.d;
//                    table = oTable.dataTable();
//                    table._fnProcessingDisplay(true);
//                    oSettings = table.fnSettings();
//                    table.fnClearTable(this);
//                    for (var i = 0; i < response.length - 1; i++) {
//                        table.oApi._fnAddData(oSettings, response[i]);
//                    }
//                    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
//                    table._fnProcessingDisplay(false);
//                    table.fnDraw();
//                   
//                    if (response.length == 0)
//                        alertbox("No Data Found..");
//                    return false;
//                },
//                failure: function (response) {
//                    alert("Failed");
//                }
//            });
            
        }
        $(document).ready(function () {
            var h = getWindowHeight() - 168 + "px";
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "sScrollY": h,
                "sScrollX": "100%",
                "bScrollCollapse": true,
                //                "sScrollX": "100%",
                "bScrollCollapse": true,
                "bLengthChange": false,
                "iDisplayLength": -1,
                "bAutoWidth": false,
                "aaSorting": [[1, "asc"]],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {

                       
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);

                               

                            }
                           
                        }
                    }
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iT>',
                "aoColumnDefs": [
                           { "sClass": "left", "sWidth": "80px", "bVisible": true, "bSortable": false, "aTargets": [0] },
                           { "sClass": "left", "sWidth": "170px", "bVisible": true, "bSortable": true, "aTargets": [1] },
                            { "sWidth": "100px", "bVisible": true, "bSortable": true, "aTargets": [2] },
                                           { "sWidth": "230px", "bVisible": true, "bSortable": true, "aTargets": [3] },
//                                {"sClass": "left", "aTargets": [3], "bSortable": true, "fnRender": function (oData, sVal) {
//                                    if (oData.aData[3] != "") {
//                                        return '<span title="' + oData.aData[4] + '">' + oData.aData[3] + '</span>';
//                                    }
//                                    else {
//                                        return "--";
//                                    }

//                                }
//                            },
                               {"sWidth": "120px", "bVisible": true, "bSortable": true, "aTargets": [4] },

                         { "sWidth": "250px", "bVisible": true, "bSortable": true, "aTargets": [5] },
                         { "sWidth": "250px", "bVisible": true, "bSortable": true, "aTargets": [6] },
                         { "sWidth": "250px", "bVisible": true, "bSortable": true, "aTargets": [7] },
                         { "sWidth": "250px", "bVisible": true, "bSortable": true, "aTargets": [8] },
                                                         { "sClass": "right", "aTargets": [9], "bSortable": true, "fnRender": function (oData, sVal) {

                                                             return oData.aData[9] + " KM";
                                                         }
                                                         },

                         { "sWidth": "250px", "bVisible": true, "sClass": "right", "bSortable": true, "aTargets": [10] },




                         { "sWidth": "250px", "bVisible": true, "bSortable": true, "aTargets": [11] },
                         { "sWidth": "250px", "bVisible": true, "sClass": "right", "bSortable": true, "aTargets": [12] }

                                ]
            });
                                                 $("div.MyButton").html('<div> <button class="ColVis_Button TableTools_Button ui-button ui-state-default ColVis_MasterButton" id="refreshbtn" onclick="javascript:refreshTable()";><span>Refresh </span></button></div>'); 
                                                 oTable.fnClearTable(this);


            //oTable.fnAdjustColumnSizing();
            //       $(".dataTables_length").width(150);
        });

        
    </script>
    <style type="text/css">
        element.style
        {
        }
        body
        {
            overflow: hidden;
        }
        table.display tfoot th
        {
            padding: 0px 0px 0px 0px;
            padding-left: 6px;
            height: 25px;
        }
        table.display thead th
        {
            padding: 0px 0px 0px 0px;
            padding-left: 6px;
            height: 25px;
        }
        table.display td
        {
            padding: 6px 8px;
        }
    </style>
</head>
<body id="index" style="margin: 0px; font-size: 11px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif"
    onload="refreshTable()">
    <form runat="server">
    <center>
    <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 11px;
        width: 100%; font-family: verdana">
        <thead style="text-align: left; width: 100%;">
            <tr>
                <th rowspan="2" style="width: 50px;">
                    S No
                </th>
                <th rowspan="2" style="width: 150px;">
                    DN NO
                </th>
                <th rowspan="2"  style="width: 200px;">
                    Plate NO
                </th>
                <th rowspan="2" >
                   Unit ID
                </th>
                <th rowspan="2" >
                    Group Name
                </th>
                <th rowspan="2"  style="width: 60px;">
                    Source
                </th>
                <th rowspan="2"  style="width: 150px;">
                    Weight Out Time
                </th>
                <th rowspan="2"  style="width: 80px;">
                    Ship To Code
                </th>
                <th rowspan="2"  style="width: 80px;">
                     Ship To Name
                </th>
                <th colspan="2"   >
                    Google
                </th>
                <th colspan="2"  >
                    Previous
                </th>
                
            </tr>
            <tr>
            <th>Distance</th>
            <th>Duration</th>

            <th>ETA</th>
            <th>Duration</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
        <tfoot style="text-align: left; font-weight: bold;">
            <tr>
              
                <th rowspan="2"  style="width: 50px;">
                    S No
                </th>
                <th rowspan="2"  style="width: 150px;">
                    DN NO
                </th>
                <th rowspan="2"  style="width: 200px;">
                    Plate NO
                </th>
                <th rowspan="2" >
                   Unit ID
                </th>
                <th rowspan="2" >
                    Group Name
                </th>
                <th rowspan="2"  style="width: 60px;">
                    Source
                </th>
                <th rowspan="2"  style="width: 150px;">
                    Weight Out Time
                </th>
                <th rowspan="2"  style="width: 80px;">
                    Ship To Code
                </th>
                <th rowspan="2"  style="width: 80px;">
                     Ship To Name
                </th>
                <th>Distance</th>
            <th >Duration</th>

            <th>ETA</th>
            <th>Duration</th>
               
            </tr>
            <tr>
             <th colspan="2" >
                    Google
                </th>
                <th colspan="2" >
                    Previous
                </th>
            </tr>
        </tfoot>
    </table>
    </center>
    </form>
</body>
</html>
